using System.Collections.Generic;
using System.Threading.Tasks;

namespace Umoya.Core
{
    public interface IPackageDownloadsSource
    {
        Task<Dictionary<string, Dictionary<string, long>>> GetPackageDownloadsAsync();
    }
}
