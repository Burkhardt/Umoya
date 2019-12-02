using System;
using System.Threading.Tasks;
using Umoya.Core.Search;
using Umoya.Core.ServiceIndex;
using Umoya.Protocol;
using Microsoft.AspNetCore.Mvc;

namespace Umoya.Controllers
{
    public class SearchController : Controller
    {
        private readonly IUmoyaSearchService _searchService;
        private readonly IUmoyaUrlGenerator _url;

        public SearchController(IUmoyaSearchService searchService, IUmoyaUrlGenerator url)
        {
            _searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
            _url = url ?? throw new ArgumentNullException(nameof(url));
        }

        public async Task<ActionResult<SearchResponse>> SearchAsync(
            [FromQuery(Name = "q")] string query = null,
            [FromQuery]int skip = 0,
            [FromQuery]int take = 20,
            [FromQuery]bool prerelease = false,
            [FromQuery]string semVerLevel = null,

            // These are unofficial parameters
            [FromQuery]string packageType = null,
            [FromQuery]string framework = null)
        {
            var includeSemVer2 = semVerLevel == "2.0.0";

            return await _searchService.SearchAsync(new BaGetSearchRequest
            {
                Skip = skip,
                Take = take,
                IncludePrerelease = prerelease,
                IncludeSemVer2 = includeSemVer2,
                Query = query ?? string.Empty,

                PackageType = packageType,
                Framework = framework,
            });
        }

        public async Task<ActionResult<AutocompleteResponse>> AutocompleteAsync([FromQuery(Name = "q")] string query = null)
        {
            return await _searchService.AutocompleteAsync(new AutocompleteRequest
            {
                Skip = 0,
                Take = 20,
                Query = query
            });
        }

        public async Task<ActionResult<DependentsResponse>> DependentsAsync([FromQuery] string packageId)
        {
            // TODO: Add other dependents parameters.
            return await _searchService.FindDependentsAsync(new DependentsRequest
            {
                PackageId = packageId,
            });
        }
    }
}
