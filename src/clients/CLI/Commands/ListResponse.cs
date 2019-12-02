using System.Collections.Generic;

namespace Repo.Clients.CLI
{
    public class ListResponse
    {
        public int TotalHits { get; set; }
        public IEnumerable<Package> Data { get; set; }
    }
}