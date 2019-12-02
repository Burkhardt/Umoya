using System;
using System.Threading;
using System.Threading.Tasks;
using Umoya.Core.ServiceIndex;
using Umoya.Protocol;
using Microsoft.AspNetCore.Mvc;

namespace Umoya.Controllers
{
    /// <summary>
    /// The NuGet Service Index. This aids NuGet client to discover this server's services.
    /// </summary>
    public class ServiceIndexController : Controller
    {
        private readonly IUmoyaServiceIndex _serviceIndex;

        public ServiceIndexController(IUmoyaServiceIndex serviceIndex)
        {
            _serviceIndex = serviceIndex ?? throw new ArgumentNullException(nameof(serviceIndex));
        }

        // GET v3/index
        [HttpGet]
        public async Task<ServiceIndexResponse> GetAsync(CancellationToken cancellationToken)
        {
            return await _serviceIndex.GetAsync(cancellationToken);
        }
    }
}
