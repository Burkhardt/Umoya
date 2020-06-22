using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using Umoya.Core;
using Umoya.Database.Sqlite;
using Umoya.Hosting;
using Umoya.Protocol;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Umoya
{
    // TODO: Move this to Umoya.Core
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddBaGet(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.ConfigureAndValidate<UmoyaOptions>(configuration);
            services.ConfigureAndValidate<SearchOptions>(configuration.GetSection(nameof(UmoyaOptions.Search)));
            services.ConfigureAndValidate<MirrorOptions>(configuration.GetSection(nameof(UmoyaOptions.Mirror)));
            services.ConfigureAndValidate<StorageOptions>(configuration.GetSection(nameof(UmoyaOptions.Storage)));
            services.ConfigureAndValidate<DatabaseOptions>(configuration.GetSection(nameof(UmoyaOptions.Database)));
            services.ConfigureAndValidate<FileSystemStorageOptions>(configuration.GetSection(nameof(UmoyaOptions.Storage)));

            
            
            services.ConfigureIis(configuration);

            services.AddBaGetContext();

            services.AddTransient<IUrlGenerator, UmoyaUrlGenerator>();

            services.AddTransient<IPackageService>(provider =>
            {
                var databaseOptions = provider.GetRequiredService<IOptionsSnapshot<DatabaseOptions>>();

                switch (databaseOptions.Value.Type)
                {
                    case DatabaseType.Sqlite:                    
                        return new PackageService(provider.GetRequiredService<IContext>());

                  
                    default:
                        throw new InvalidOperationException(
                            $"Unsupported database provider: {databaseOptions.Value.Type}");
                }
            });

            services.AddTransient<IPackageIndexingService, PackageIndexingService>();
            services.AddTransient<IPackageDeletionService, PackageDeletionService>();
            services.AddTransient<ISymbolIndexingService, SymbolIndexingService>();
            services.AddTransient<IServiceIndexService, UmoyaServiceIndex>();
            services.AddTransient<IPackageContentService, DefaultPackageContentService>();
            services.AddTransient<IPackageMetadataService, DefaultPackageMetadataService>();
            services.AddSingleton<IFrameworkCompatibilityService, FrameworkCompatibilityService>();
            services.AddSingleton<RegistrationBuilder>();
            services.AddMirrorServices();

            services.AddStorageProviders();
            services.AddSearchProviders();
            services.AddAuthenticationProviders();

            return services;
        }

        public static IServiceCollection AddBaGetContext(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<IContext>(provider =>
            {
                var databaseOptions = provider.GetRequiredService<IOptionsSnapshot<DatabaseOptions>>();

                switch (databaseOptions.Value.Type)
                {
                    case DatabaseType.Sqlite:
                        return provider.GetRequiredService<SqliteContext>();

                    default:
                        throw new InvalidOperationException(
                            $"Unsupported database provider: {databaseOptions.Value.Type}");
                }
            });

            services.AddDbContext<SqliteContext>((provider, options) =>
            {
                var databaseOptions = provider.GetRequiredService<IOptionsSnapshot<DatabaseOptions>>();

                options.UseSqlite(databaseOptions.Value.ConnectionString);
            });
            

            return services;
        }

        

        
        
        
        public static IServiceCollection ConfigureIis(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<IISServerOptions>(iis =>
            {
                iis.MaxRequestBodySize = 262144000;
            });

            services.ConfigureAndValidate<IISServerOptions>(configuration.GetSection(nameof(IISServerOptions)));

            return services;
        }

        public static IServiceCollection AddStorageProviders(this IServiceCollection services)
        {
            services.AddSingleton<NullStorageService>();
            services.AddTransient<FileStorageService>();
            services.AddTransient<IPackageStorageService, PackageStorageService>();
            services.AddTransient<ISymbolStorageService, SymbolStorageService>();


            services.AddTransient<IStorageService>(provider =>
            {
                var options = provider.GetRequiredService<IOptionsSnapshot<UmoyaOptions>>();

                switch (options.Value.Storage.Type)
                {
                    case StorageType.FileSystem:
                        return provider.GetRequiredService<FileStorageService>();

                    
                    case StorageType.Null:
                        return provider.GetRequiredService<NullStorageService>();

                   
                    default:
                        throw new InvalidOperationException(
                            $"Unsupported storage service: {options.Value.Storage.Type}");
                }
            });

            return services;
        }

        public static IServiceCollection AddSearchProviders(this IServiceCollection services)
        {
            services.AddTransient<ISearchService>(provider =>
            {
                var searchOptions = provider.GetRequiredService<IOptionsSnapshot<SearchOptions>>();

                switch (searchOptions.Value.Type)
                {
                    case SearchType.Database:
                        var databaseOptions = provider.GetRequiredService<IOptionsSnapshot<DatabaseOptions>>();

                        switch (databaseOptions.Value.Type)
                        {
                            
                            case DatabaseType.Sqlite:                            
                                return provider.GetRequiredService<DatabaseSearchService>();

                        
                            default:
                                throw new InvalidOperationException(
                                    $"Database type '{databaseOptions.Value.Type}' cannot be used with " +
                                    $"search type '{searchOptions.Value.Type}'");
                        }

                    
                    case SearchType.Null:
                        return provider.GetRequiredService<NullSearchService>();

                    default:
                        throw new InvalidOperationException(
                            $"Unsupported search service: {searchOptions.Value.Type}");
                }
            });

            services.AddTransient<ISearchIndexer>(provider =>
            {
                var searchOptions = provider.GetRequiredService<IOptionsSnapshot<SearchOptions>>();

                switch (searchOptions.Value.Type)
                {
                    case SearchType.Null:
                    case SearchType.Database:
                        return provider.GetRequiredService<NullSearchIndexer>();

                 
                    default:
                        throw new InvalidOperationException(
                            $"Unsupported search service: {searchOptions.Value.Type}");
                }
            });

            services.AddTransient<DatabaseSearchService>();
            services.AddSingleton<NullSearchService>();
            services.AddSingleton<NullSearchIndexer>();
          
            return services;
        }

        /// <summary>
        /// Add the services that mirror an upstream package source.
        /// </summary>
        /// <param name="services">The defined services.</param>
        public static IServiceCollection AddMirrorServices(this IServiceCollection services)
        {
            services.AddTransient<NullMirrorService>();
            services.AddTransient<MirrorService>();

            services.AddTransient<IMirrorService>(provider =>
            {
                var options = provider.GetRequiredService<IOptionsSnapshot<MirrorOptions>>();

                if (!options.Value.Enabled)
                {
                    return provider.GetRequiredService<NullMirrorService>();
                }
                else
                {
                    return provider.GetRequiredService<MirrorService>();
                }
            });

            services.AddSingleton<NuGetClient>();
            services.AddSingleton(provider =>
            {
                var httpClient = provider.GetRequiredService<HttpClient>();
                var options = provider.GetRequiredService<IOptions<MirrorOptions>>();

                return new NuGetClientFactory(
                    httpClient,
                    options.Value.PackageSource.ToString());
            });

            services.AddSingleton(provider =>
            {
                var options = provider.GetRequiredService<IOptions<UmoyaOptions>>().Value;

                var assembly = Assembly.GetEntryAssembly();
                var assemblyName = assembly.GetName().Name;
                var assemblyVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "0.0.0";

                var client = new HttpClient(new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                });

                client.DefaultRequestHeaders.Add("User-Agent", $"{assemblyName}/{assemblyVersion}");
                client.Timeout = TimeSpan.FromSeconds(options.Mirror.PackageDownloadTimeoutSeconds);

                return client;
            });

            services.AddScoped<DownloadsImporter>();
            services.AddScoped<IPackageDownloadsSource, PackageDownloadsJsonSource>();

            return services;
        }

        public static IServiceCollection AddAuthenticationProviders(this IServiceCollection services)
        {
            services.AddTransient<IAuthenticationService, ApiKeyAuthenticationService>();

            return services;
        }
    }
}
