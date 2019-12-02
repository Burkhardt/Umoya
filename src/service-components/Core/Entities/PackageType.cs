namespace Umoya.Core.Entities
{
    public class PackageType
    {
        public int Key { get; set; }

        public string Name { get; set; }
        public string Version { get; set; }

        public Package Package { get; set; }
    }
}
