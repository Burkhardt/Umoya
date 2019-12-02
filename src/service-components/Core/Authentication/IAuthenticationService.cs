using System.Threading.Tasks;

namespace Umoya.Core.Authentication
{
    public interface IAuthenticationService
    {
        Task<bool> AuthenticateAsync(string apiKey);
    }
}
