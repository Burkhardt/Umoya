using System;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Repo.Clients.CLI.Commands;
using System.Linq;

namespace Repo.Clients.CLI
{
    [HelpOption]
    [VersionOptionFromMember(MemberName = nameof(Console.GetVersion))]
    class Program
    {

        public static string[] allAgrs = { };
        static int Main(string[] args)
        {
            int commandResult = 0;
            System.Console.CancelKeyPress += new ConsoleCancelEventHandler(CancelEventHandler);
            if (args.Count() > 0) commandResult = RunApp(args).GetAwaiter().GetResult();
            else commandResult = Console.PrintListOfActions();
            return commandResult;
        }

        protected static void CancelEventHandler(object sender, ConsoleCancelEventArgs args)
        {
            System.Console.WriteLine("\nThe operation has been interrupted.");
            PSOps.UserRequestToStop = true;
        }

        private static async Task<int> RunApp(string[] args)
        {
            Logger.Do(args[0] + "   " + Environment.CurrentDirectory);
            string CommandName = args[0].ToLower();
            string[] newArgs = args.Skip(1).ToArray();
            allAgrs = args;
            //if --json is present
            string NextOf = string.Empty;
            if (newArgs.ToList().Contains("-j") || newArgs.ToList().Contains("--json"))
            {
                if (newArgs.ToList().Contains("-j"))
                    NextOf = "-j";
                else
                    NextOf = "--json";
                Console.Init(args);
            }
            string fileName;
            fileName = args.SkipWhile(x => x != NextOf).Skip(1).DefaultIfEmpty(args[0]).FirstOrDefault();
            int ActionExitCode = 0;
            if (CommandName.Equals("publish")) ActionExitCode = await CommandLineApplication.ExecuteAsync<PublishCommand>(newArgs);
            else if (CommandName.Equals("add")) ActionExitCode = await CommandLineApplication.ExecuteAsync<AddCommand>(newArgs);
            else if (CommandName.Equals("init")) ActionExitCode = await CommandLineApplication.ExecuteAsync<InitCommand>(newArgs);
            else if (CommandName.Equals("delete")) ActionExitCode = await CommandLineApplication.ExecuteAsync<DeleteCommand>(newArgs);
            else if (CommandName.Equals("list") || CommandName.Equals("ls")) ActionExitCode = await CommandLineApplication.ExecuteAsync<ListCommand>(newArgs);
            else if (CommandName.Equals("info")) ActionExitCode = await CommandLineApplication.ExecuteAsync<InfoCommand>(newArgs);
            else if (CommandName.Equals("backup")) ActionExitCode = await CommandLineApplication.ExecuteAsync<BackupCommand>(newArgs);
            else if (CommandName.Equals("compress")) ActionExitCode = await CommandLineApplication.ExecuteAsync<CompressCommand>(newArgs);
            else if (CommandName.Equals("deploy")) ActionExitCode = await CommandLineApplication.ExecuteAsync<DeployCommand>(newArgs);
            else if (CommandName.Equals("-v") || CommandName.Equals("--version") || CommandName.Equals("version")) ActionExitCode = Console.ShowVersion();
            else ActionExitCode = Console.PrintListOfActions();
            Console.Close(fileName);
            return ActionExitCode;
        }
    }
}