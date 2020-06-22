using System.Threading;
using System.Threading.Tasks;
using Umoya.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Umoya.Hosting
{
    public static class IHostExtensions
    {
        public static async Task RunMigrationsAsync(this IHost host, CancellationToken cancellationToken)
        {
            // Run migrations if necessary.
            var options = host.Services.GetRequiredService<IOptions<UmoyaOptions>>();

            if (options.Value.RunMigrationsAtStartup && options.Value.Database.Type != DatabaseType.AzureTable)
            {
                using (var scope = host.Services.CreateScope())
                {
                    var ctx = scope.ServiceProvider.GetRequiredService<IContext>();

                    await ctx.RunMigrationsAsync(cancellationToken);
                }
            }
        }
    }
}
