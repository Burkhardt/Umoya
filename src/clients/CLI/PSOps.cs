using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace Repo.Clients.CLI
{
    public static class PSOps
    {

        public static bool UserRequestToStop = false;
        public class PSOpsException : System.Exception {
            public PSOpsException(string message)
            : base(message)
            {
            }
        }
        
        public static List<string> StartAndWaitForFinish(string CommandName, string CommandArgs, out List<string> OutputInfo, string WorkingDir = "")
        {
            List<string> ErrorInfo = new List<string>();
            OutputInfo = new List<string>();
            try
            {
                ProcessStartInfo procStartInfo = new ProcessStartInfo(CommandName, CommandArgs);
                if(!Info.Instance.ISDebugging) procStartInfo.RedirectStandardOutput = true;
                Process proc = new Process();
                if(!WorkingDir.Equals("")) procStartInfo.WorkingDirectory = WorkingDir;
                proc.StartInfo = procStartInfo;                            
                proc.Start();
                string TempOutput = string.Empty;
                if(!Info.Instance.ISDebugging)
                {
                    while (!proc.StandardOutput.EndOfStream)
                    {
                        TempOutput = proc.StandardOutput.ReadLine();
                        OutputInfo.Add(TempOutput);
                        if(TempOutput.ToLower().Contains("error") && !TempOutput.ToLower().Contains("0 error")) 
                        {
                            ErrorInfo.Add(TempOutput);
                        }
                    }
                }
                proc.WaitForExit();
            }
            catch(Exception ex)
            {
                ErrorInfo.Add(ex.Message);
                Logger.Do(ex.StackTrace);
            }
            return ErrorInfo;
        }
    }
   
}