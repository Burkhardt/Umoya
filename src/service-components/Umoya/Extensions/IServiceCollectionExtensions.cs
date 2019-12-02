using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using Umoya.Core.Authentication;
using Umoya.Core.Configuration;
using Umoya.Core.Content;
using Umoya.Core.Entities;
using Umoya.Core.Extensions;
using Umoya.Core.Indexing;
using Umoya.Core.Metadata;
using Umoya.Core.Mirror;
using Umoya.Core.Search;
using Umoya.Core.Server.Extensions;
using Umoya.Core.ServiceIndex;
using Umoya.Core.Storage;
using Umoya.Database.Sqlite;
using Umoya.Protocol;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Umoya.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureBaGet(
            this IServiceCollection services,
            IConfiguration configuration,
            bool httpServices = false)
        {
            services.ConfigureAndValidate<UmoyaOptions>(configuration);
            services.ConfigureAndValidate<SearchOptions>(configuration.GetSection(nameof(UmoyaOptions.Search)));
            services.ConfigureAndValidate<MirrorOptions>(configuration.GetSection(nameof(UmoyaOptions.Mirror)));
            services.ConfigureAndValidate<StorageOptions>(configuration.GetSection(nameof(UmoyaOptions.Storage)));
            services.ConfigureAndValidate<DatabaseOptions>(configuration.GetSection(nameof(UmoyaOptions.Database)));
            services.ConfigureAndValidate<FileSystemStorageOptions>(configuration.GetSection(nameof(UmoyaOptions.Storage)));
            

            if (httpServices)
            {
                services.ConfigureHttpServices();
            }

            services.AddBaGetContext();

            services.AddTransient<IPackageService, PackageService>();
            services.AddTransient<IPackageIndexingService, PackageIndexingService>();
            services.AddTransient<IPackageDeletionService, PackageDeletionService>();
            services.AddTransient<ISymbolIndexingService, SymbolIndexingService>();
            services.AddTransient<IUmoyaServiceIndex, UmoyaServiceIndex>();
            services.AddTransient<IUmoyaPackageContentService, DatabasePackageContentService>();
            services.AddTransient<IUmoyaPackageMetadataService, DatabasePackageMetadataService>();
            services.AddTransient<IUmoyaUrlGenerator, BaGetUrlGenerator>();
            services.AddSingleton<IFrameworkCompatibilityService, FrameworkCompatibilityService>();
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
            services.AddTransient<IUmoyaSearchService>(provider =>
            {
                var options = provider.GetRequiredService<IOptionsSnapshot<SearchOptions>>();

                switch (options.Value.Type)
                {
                    case SearchType.Database:
                        return provider.GetRequiredService<DatabaseSearchService>();


                    case SearchType.Null:
                        return provider.GetRequiredService<NullSearchService>();

                    default:
                        throw new InvalidOperationException(
                            $"Unsupported search service: {options.Value.Type}");
                }
            });

            services.AddTransient<DatabaseSearchService>();
            services.AddSingleton<NullSearchService>();

            return services;
        }

        /// <summary>
        /// Add the services that mirror an upstream package source.
        /// </summary>
        /// <param name="services">The defined services.</param>
        public static IServiceCollection AddMirrorServices(this IServiceCollection services)
        {
            services.AddTransient<FakeMirrorService>();
            services.AddTransient<MirrorService>();

            services.AddTransient<IMirrorService>(provider =>
            {
                var options = provider.GetRequiredService<IOptionsSnapshot<MirrorOptions>>();

                if (!options.Value.Enabled)
                {
                    return provider.GetRequiredService<FakeMirrorService>();
                }
                else
                {
                    return provider.GetRequiredService<MirrorService>();
                }
            });

            services.AddTransient<IPackageContentService, PackageContentClient>();
            services.AddTransient<IPackageMetadataService, PackageMetadataClient>();
            services.AddTransient<IUrlGeneratorFactory, UrlGeneratorClientFactory>();

            services.AddSingleton<IServiceIndex>(provider =>
            {
                var httpClient = provider.GetRequiredService<HttpClient>();
                var options = provider.GetRequiredService<IOptions<MirrorOptions>>();

                return new ServiceIndexClient(
                    httpClient,
                    options.Value.PackageSource.ToString());
            });

            services.AddTransient<IPackageDownloader, PackageDownloader>();

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

            services.AddSingleton<DownloadsImporter>();
            services.AddSingleton<IPackageDownloadsSource, PackageDownloadsJsonSource>();

            return services;
        }

        public static IServiceCollection AddAuthenticationProviders(this IServiceCollection services)
        {
            services.AddTransient<IAuthenticationService, ApiKeyAuthenticationService>();

            return services;
        }
    }
}
