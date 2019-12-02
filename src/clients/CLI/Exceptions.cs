using System;

namespace Repo.Clients.CLI
{
    public static class Exceptions
    {
        public class ActionNotSuccessfullyPerformException : System.Exception
        {

            public ActionNotSuccessfullyPerformException(string actionName, string message)
            : base(message)
            {
                Console.LogError(message);
                message = "UMOYA action <" + actionName + "> is not performed successfully.";
                Console.LogError(message);
            }

            public ActionNotSuccessfullyPerformException(string message)
            : base(message)
            {
                Console.LogError(message);
                message = "UMOYA action is not performed successfully.";
                Console.LogError(message);
            }
        }

        public class ResourceInfoInvalidFormatException : System.Exception
        {
            private static string message = "ResourceInfo resourcename@version format is not correct. i.e. HelloWorld.pmml@1.0.0";
            public ResourceInfoInvalidFormatException(string actionName)
            : base(message)
            {
                throw new ActionNotSuccessfullyPerformException(actionName, message);
            }
        }

        public class ResourceNotFoundException : System.Exception
        {
            private static string message = "ResourceInfo is not found.";
            public ResourceNotFoundException(string actionName) : base(message)
            {
                throw new ActionNotSuccessfullyPerformException(actionName, message);
            }
        }

        public class ConfigurationNotSuccessfullyDoneException : System.Exception
        {
            private static string message = "ZMOD configuration is not performed successfully.";
            public ConfigurationNotSuccessfullyDoneException(string actionName) : base(message)
            {
                throw new ActionNotSuccessfullyPerformException(actionName, message);
            }

            public ConfigurationNotSuccessfullyDoneException() : base(message)
            {
                throw new ActionNotSuccessfullyPerformException(message);
            }
        }

        public class ConfigurationNotFoundException : System.Exception
        {
            private static string message = "ZMOD configuration is not found, Use init action to configure and resolve this error.";

            public ConfigurationNotFoundException() : base(message)
            {
                throw new ActionNotSuccessfullyPerformException(message);
            }

            public ConfigurationNotFoundException(string actionName) : base(message)
            {
                throw new ActionNotSuccessfullyPerformException(actionName, message);
            }
        }

        public class ResourceTypeException : System.Exception
        {
            private static string message = "Resource name has no extension";
            public ResourceTypeException() : base(message)
            {
                throw new ActionNotSuccessfullyPerformException(message);
            }
        }

        public class ResourceNotFoundInRepoException : System.Exception
        {
            private static string message = "Resource is not found in Repo. Please, check resource name and version is present.";
            public ResourceNotFoundInRepoException(string actionName) : base(message)
            {
                throw new ActionNotSuccessfullyPerformException(message);
            }
        }

        public class ResourceTypeInvalidException : System.Exception 
        {
            private static string message = "Resource Type is not correct; expecting Type as Model, Data, Code or Other";            
            public ResourceTypeInvalidException()
            : base(message)
            {                
                Console.LogError(message); 
            }
        }

        public class OutputJsonFileException : System.Exception
        {
            private static string message = "Please enter correct path for output file to generate. ";
            
            public OutputJsonFileException(string actionName)
            : base(message)
            {
                throw new ActionNotSuccessfullyPerformException(actionName, message);
            }
        }
    }
}
