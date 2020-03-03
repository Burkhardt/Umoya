using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Repo.Clients.CLI
{
    public class Console
    {
        private static bool IsJSONOutput = false;
        private static bool IsListAction = false;
        private static string[] ActionInput = new string[]{};
        private static System.ConsoleColor WarningColor = System.ConsoleColor.Yellow;
        private static System.ConsoleColor ErrorColor = System.ConsoleColor.Red;
        private static System.ConsoleColor TableHeaderColor = System.ConsoleColor.Yellow;
        private static System.ConsoleColor TableFrameColor = System.ConsoleColor.White;
        private static System.ConsoleColor QuestionColor = System.ConsoleColor.White;
        private static System.ConsoleColor TaskPassColor = System.ConsoleColor.Green;
        private static Dictionary<string, string> listOfCommandsDescription = new Dictionary<string, string> {
            { Constants.InitCommandName, Constants.InitCommandDescription},
            { Constants.InfoCommandName, Constants.InfoCommandDescription},
            { Constants.ListCommandName, Constants.ListCommandDescription},
            { Constants.AddCommandName, Constants.AddCommandDescription},
            { Constants.DeleteCommandName, Constants.DeleteCommandDescription},
            { Constants.PublishCommandName, Constants.PublishCommandDescription},
            /*{ Constants.UpgradeCommandName, Constants.UpgradeCommandDescription},*/
            { Constants.BackupCommandName, Constants.BackupCommandDescription},
            { Constants.CompressCommandName, Constants.CompressCommandDescription},
            { Constants.DeployCommandName, Constants.DeployCommandDescription},
        };
        public static bool IsZMODConfigured()
        {
            bool Status = FSOps.HasNecessaryDirs() && FSOps.ConfigFileExists();
            return Status;
        }
        public static int PrintListOfActions()
        {
            bool IsConfigured = Console.IsZMODConfigured();
            System.Console.WriteLine();
            Console.WriteLine("Usage:", System.ConsoleColor.Green);
            System.Console.WriteLine(" umoya [Name of the action] [options for the action]");
            System.Console.WriteLine();
            Console.WriteLine("Actions:", System.ConsoleColor.DarkYellow);
            foreach (KeyValuePair<string, string> descriptionKVPair in listOfCommandsDescription)
            {
                string actionName = descriptionKVPair.Key;
                string description = descriptionKVPair.Value;
                System.Console.WriteLine(FormatActionNameToPringInConsole(actionName) + "\t" + description);
            }
            System.Console.WriteLine();
            Console.WriteLine("Options:", System.ConsoleColor.DarkYellow);
            System.Console.WriteLine(" " + "-v|--version" + "\t" + "Show version information (" + GetVersion() + ")");
            System.Console.WriteLine(" " + "-h|--help" + "\t" + "Show list of actions");

            System.Console.WriteLine();
            Console.WriteLine("Configuration:", System.ConsoleColor.Blue);
            System.Console.WriteLine(FormatActionNameToPringInConsole("Repo", 10) + "\t" +
            Info.Instance.Source.Url + ", You can update this with CLI umoya info -u <repo url>.");
            System.Console.WriteLine(FormatActionNameToPringInConsole("AccessKey", 10) + "\t" + Info.Instance.Source.Accesskey);
            System.Console.WriteLine();            
            string ZMODInitString = ZMODInitializeString();
            if(IsConfigured) Console.LogLine(ZMODInitString);
            else 
            {
                Console.LogError(ZMODInitString);
                System.Console.WriteLine("  >>  To initialize ZMOD here, use command : umoya init");
                System.Console.WriteLine("  >>  To configure, use command : umoya info <options>");
            }
            Console.LogWarning("* Get available options for specific action");
            System.Console.WriteLine("  >>  umoya [Name of the action] --help");
            System.Console.WriteLine();
            return 0;
        }

        private static string ZMODInitializeString()
        {
            string InitializedString = "is initialized here (" + Environment.CurrentDirectory + ")";
            if (!Console.IsZMODConfigured()) InitializedString = "is not initialized here (" + Environment.CurrentDirectory + ")";
            return "* ZMOD " + InitializedString;
        }
        public static void WriteLine(string str, System.ConsoleColor InterestedColor)
        {
            //
            int returnCount = str.Count(ch => ch == '\n');
            System.Console.ResetColor();
            System.ConsoleColor currentColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = InterestedColor;
            for (int i = 0; i < str.Length; i++)
            {
                System.Console.Write(str[i]);
            }
            System.Console.ResetColor();
            System.Console.ForegroundColor = currentColor;
            System.Console.Write("\n");
        }

        public static void PrintActionPerformSuccessfully(string ActionName)
        {
            Console.LogLine("> UMOYA action <" + ActionName + "> successfully performed.");
        }

        public static void LogLine(string LogMessage)
        {
            WriteLine(LogMessage, TaskPassColor);
        }

        public static void LogError(string LogMessage)
        {
            //If with json or not
            WriteLine(LogMessage, ErrorColor);
        }

        public static void LogWarning(string LogMessage)
        {
            WriteLine(LogMessage, WarningColor);
        }

        public static void Init(string[] args)
        {
            //Find action and set IsListAction = true
            //Find JSON output option and if found then set IsJSONOutput = true;
            //
        }

        public static void Close()
        {
            if(IsJSONOutput) OutputJson();
        }

        public static void OutputJson()
        {}

        public static void UpdateInput(string InputString)
        {

        }

        public static bool AskYesOrNo(string question, bool acceptEnterAsYes = true)
        {
            bool? response = null;

            while (!response.HasValue)
            {
                System.Console.ResetColor();
                System.Console.ForegroundColor = QuestionColor;
                System.Console.Write(question + " [y/n] ");
                string input = System.Console.ReadLine();

                if (input.Trim().ToLower() == "y" || (acceptEnterAsYes && input.Trim() == ""))
                {
                    response = true;
                }
                else if (input.Trim().ToLower() == "n")
                {
                    response = false;
                }
                else
                {
                    WriteLine($"Invalid response \"{input.Trim()}\": type in \"y\" or \"n\"", ErrorColor);
                }
            }
            return response.Value;
        }

        public static string FormatActionNameToPringInConsole(string Name, int FixedWidth = 8)
        {
            string TempName = " " + Name;
            int EmptyCharsToAppend = FixedWidth - TempName.Length;
            for (int i = 0; i < EmptyCharsToAppend; i++) TempName += " ";
            return TempName;
        }

        public static string GetVersion()
        {
            return typeof(Console).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        }

        public static int SetInfoConfigurationValues(string RepoSourceURL, string Accesskey, string Owner, bool ISDebugging ,bool ShowProgress)
        {
            try
            {
                Logger.Do("Command : Info SetInfoConfigurationValues");
                if (IsZMODConfigured())
                {
                    // bool Status = bool.Parse(StatusInString);
                    Logger.Do("SetInfoConfigurationValues " +  RepoSourceURL + "  " + Accesskey + "  " + ISDebugging + " " +Owner  +"  " + ShowProgress);
                    FSOps.UpdateInfoValue(RepoSourceURL, Accesskey, Owner, ISDebugging ,ShowProgress);
                    return 1;
                }
                else throw new Exceptions.ConfigurationNotFoundException();
            }
            catch (Exceptions.ConfigurationNotFoundException ext) { Logger.Do(ext.Message); return 0; }
            catch (Exception) { return 0; }
        }
        public static void PrintInfo()
        {
            System.Console.WriteLine();
            Console.WriteLine("Configuration:", System.ConsoleColor.Blue);
            //ToDo Surbhi
            System.Console.WriteLine(Console.FormatActionNameToPringInConsole(" " + "ZMOD Home ") + "\t : " + Info.Instance.ZmodHome);
            System.Console.WriteLine(Console.FormatActionNameToPringInConsole(" " + "Umoya Home ") + "\t : " + Info.Instance.UmoyaHome);
            System.Console.WriteLine(Console.FormatActionNameToPringInConsole(" " + "Owner ") + "\t : " + Info.Instance.Owner);
            System.Console.WriteLine(Console.FormatActionNameToPringInConsole(" " + "Version ") + "\t : " + Info.Instance.Version);
            if (Info.Instance.Source.Url != null && Info.Instance.Source.Url != "") System.Console.WriteLine(Console.FormatActionNameToPringInConsole(" " + "Repo", 10) + "\t : " + Info.Instance.Source.Url + ", You can update this with CLI umoya info -u <repo url>.");
            else System.Console.WriteLine(Console.FormatActionNameToPringInConsole(" " + "Repo", 10) + "\t : " + "No Repo URL exists. You can update this with CLI umoya info -u <repo url>");
            if (Info.Instance.Source.Accesskey != null && Info.Instance.Source.Accesskey != "") System.Console.WriteLine(Console.FormatActionNameToPringInConsole(" " + "AccessKey", 10) + "\t : " + Info.Instance.Source.Accesskey);
            else System.Console.WriteLine(Console.FormatActionNameToPringInConsole(" " + "AccessKey", 10) + "\t : " + "No Key found; You need to set with CLI umoya info -k <your key>");
            System.Console.WriteLine(Console.FormatActionNameToPringInConsole(" " + "ShowProgress") + "\t : " + Info.Instance.ShowProgress);
            System.Console.WriteLine(Console.FormatActionNameToPringInConsole(" " + "IsDebugging") + "\t : " + Info.Instance.ISDebugging);
            System.Console.WriteLine();
        }
        public static int ShowVersion()
        {
            try
            {
                Repo.Clients.CLI.Console.WriteLine("Current version for umoya: " + GetVersion(), System.ConsoleColor.DarkYellow);
                return 1;
            }
            catch (Exception) { return 0; }
        }
    }
}
