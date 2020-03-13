using System.Collections.Generic;

namespace Repo.Clients.CLI
{
    public class RootJsonOutput
    {
        public string action { get; set; }
        public string input { get; set; }
        public List<string> output { get; set; }
        public List<string> errorMessage { get; set; }
        public bool status { get; set; }
    }
}
