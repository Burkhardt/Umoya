namespace Repo.Clients.CLI
{
    public class Logger
    {
        public static bool isDebugging = Info.Instance.ISDebugging;
        public static void Do(string logDetail)
        {
            if(isDebugging) System.Console.WriteLine(logDetail);
        }
    }
}
