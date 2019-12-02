using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Umoya.Protocol;
using NuGet.Versioning;

namespace Umoya.Core.Content
{
    /// <summary>
    /// Umoya's extensions to the NuGet Package Content resource. These additions
    /// are not part of the official protocol.
    /// </summary>
    public interface IUmoyaPackageContentService : IPackageContentService
    {
        /// <summary>
        /// Download a package's readme, or null if the package or readme does not exist.
        /// </summary>
        /// <param name="id">The package id.</param>
        /// <param name="version">The package's version.</param>
        /// <param name="cancellationToken">A token to cancel the task.</param>
        /// <returns>
        /// The package's readme stream, or null if the package or readme does not exist. The stream may not be seekable.
        /// </returns>
        Task<Stream> GetPackageReadmeStreamOrNullAsync(string id, NuGetVersion version, CancellationToken cancellationToken = default);
    }
}