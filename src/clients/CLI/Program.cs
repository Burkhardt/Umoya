﻿using System;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Repo.Clients.CLI.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using DotNetSearch;
using  System.Runtime.InteropServices;

namespace Repo.Clients.CLI
{
    [HelpOption]
    [VersionOptionFromMember(MemberName = nameof(Console.GetVersion))]
    class Program
    {
        static int Main(string[] args)
        {
            int commandResult = 0;
            if (args.Count() > 0) commandResult = RunApp(args).GetAwaiter().GetResult();
            else commandResult = Console.PrintListOfActions();
            return commandResult;
        }

    

        private static async Task<int> RunApp(string[] args)
        {
            Logger.Do(args[0] + "   " + Environment.CurrentDirectory);
            string CommandName = args[0].ToLower();
            string[] newArgs = args.Skip(1).ToArray();
            // foreach(var i in args)
            //System.Console.WriteLine(i);
            if (CommandName.Equals("publish")) return await CommandLineApplication.ExecuteAsync<PublishCommand>(newArgs);
            else if (CommandName.Equals("add")) return await CommandLineApplication.ExecuteAsync<AddCommand>(newArgs);
            else if (CommandName.Equals("init")) return await CommandLineApplication.ExecuteAsync<InitCommand>(newArgs);
            else if (CommandName.Equals("delete")) return await CommandLineApplication.ExecuteAsync<DeleteCommand>(newArgs);
            else if (CommandName.Equals("list")) return await CommandLineApplication.ExecuteAsync<ListCommand>(newArgs);
            else if (CommandName.Equals("info")) return await CommandLineApplication.ExecuteAsync<InfoCommand>(newArgs);
            else if (CommandName.Equals("-v") || CommandName.Equals("--version") || CommandName.Equals("version")) return Console.ShowVersion();
            else return Console.PrintListOfActions();
        }
    }
}