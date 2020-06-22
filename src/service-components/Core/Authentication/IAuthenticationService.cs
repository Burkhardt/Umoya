using System.Threading;
using System.Threading.Tasks;

namespace Umoya.Core
{
    public interface IAuthenticationService
    {
        Task<bool> AuthenticateAsync(string apiKey, CancellationToken cancellationToken);
    }
}
