using System.Collections.Generic;
using Newtonsoft.Json;

namespace Repo.Clients.CLI
{
    public class Package
    {
      

        
        public string Id { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public IEnumerable<string> Authors { get; set; }
        public int TotalDownloads { get; set; }
        //Later to be added
        //public string FileSize {get; set;}
    }
}