using System;
using static Repo.Clients.CLI.Resources;
using System.Collections.Generic;

namespace Repo.Clients.CLI
{
    public class ResourceIdentifier
    {
        public string ResourceName = string.Empty;
        public string Version;
        public ResourceType TypeOfResource;

        public string Description;

        public string Authors;

        public string Size;  
    
        public ResourceIdentifier()
        {}

        public Dictionary<string, ResourceIdentifier> Dependencies = new Dictionary<string, ResourceIdentifier>();

        public ResourceIdentifier(string ResourceInfo, bool EnforceResourceVersion=true)
        {            
            try
            {
                string[] ResourceNameAndVersion = ResourceInfo.Split('@');
                this.ResourceName = ResourceNameAndVersion[0];
                this.TypeOfResource = GetResourceType(this.ResourceName);
                this.Version = ResourceNameAndVersion[1];
            }
            catch(Exception ex)
            {
                this.ResourceName = ResourceInfo;
                this.TypeOfResource = GetResourceType(this.ResourceName);
                this.Version = Constants.DefaultNoVersionValue;
                Logger.Do("ResourceIdentifier " + ex.Message);
            }
        }

        public ResourceIdentifier(string ResourceName, string ResourceVersion, ResourceType TypeOfResource, string Size)
        {       
            this.ResourceName = ResourceName;
            this.TypeOfResource = TypeOfResource;
            this.Version = ResourceVersion; 
            this.Description = string.Empty;
            this.Authors = string.Empty;
            this.Size = Size;
        }

        public ResourceIdentifier(string ResourceName, string ResourceVersion, ResourceType TypeOfResource, string Description, string Authors)
        {       
            this.ResourceName = ResourceName;
            this.TypeOfResource = TypeOfResource;
            this.Version = ResourceVersion; 
            this.Description = Description;
            this.Authors = Authors;
        }

        public bool HasVersion()
        {
            bool Status = !this.Version.Equals(Constants.DefaultNoVersionValue);
            Logger.Do("Resource Has Version " + Status);
            return Status;
        }

        public string ToString()
        {
            string Info = "Name " + this.ResourceName + " , Version " + this.Version + " , Type " + this.TypeOfResource + " Authors " + this.Authors + " Description " + this.Description; 
            foreach(KeyValuePair<string, ResourceIdentifier> KeyValueItem in Dependencies)
            {
                Info += " Dependent " + KeyValueItem.Value.ToString();
            }
            return Info;
        }

        public bool IsEmpty()
        {
            return this.ResourceName.Equals(string.Empty);
        }

        public static ResourceIdentifier Empty
        {
            get{
                return new ResourceIdentifier();
            }
        }

        public bool ExistsLocally
        {
            get
            {
                return Resources.IsResourcePresentLocally(this.TypeOfResource, this.ResourceName);
            }
        }

    }
}
