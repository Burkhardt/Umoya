using System.Reflection;
using System.IO;
using System.Linq;

namespace Repo.Clients.CLI.Commands
{
    public static class EmbeddedResources
    {
        public static void Extract(string outDirectory,  string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var resNames =assembly.GetManifestResourceNames();
            using (Stream s = assembly.GetManifestResourceStream(resNames.ToList()[0]))
            {
                using (BinaryReader r = new BinaryReader(s))
                using (FileStream fs = new FileStream(outDirectory + "\\" + resourceName, FileMode.OpenOrCreate))
                using (BinaryWriter w = new BinaryWriter(fs))
                    w.Write(r.ReadBytes((int)s.Length));
            }
        }
    }
}
