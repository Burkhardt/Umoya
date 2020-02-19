using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;

namespace Repo.Clients.CLI
{
    public class Downloader
    {

        public bool Status = false;

        private WebClient Client;

        public string SourceEndPoint = string.Empty;

        public string DownloadFilePath = string.Empty;

        public bool ShowProgress = false;

        public long DownloadedBytes = 0;

        public long TotalBytes = 0;

        public int ProgressPercentage = 0;

        public string DownloadByteStatus = string.Empty;

        public string DownloadFileName = string.Empty;

        public bool IsCancelled = false;

        public Exception Error = null;

        public Downloader(string EndPoint, string DownloadFilePath, bool ShowProgressInfo=false)
        {
            this.SourceEndPoint = EndPoint;
            this.DownloadFilePath = DownloadFilePath;
            if(ShowProgressInfo) Logger.Do("Started downloading " + this.SourceEndPoint);
            this.DownloadFileName = this.DownloadFilePath.Substring(this.DownloadFilePath.LastIndexOfAny(new char[] {Constants.PathSeperator}) + 1);
            this.DownloadFileName = this.DownloadFileName.Replace(".nupkg", string.Empty);
            this.ShowProgress = ShowProgressInfo;
            this.Do();
        }
        
        public void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
        {            
            
            this.DownloadedBytes = e.BytesReceived;
            this.TotalBytes = e.TotalBytesToReceive;
            this.DownloadByteStatus = (string)e.UserState;
            this.ProgressPercentage = e.ProgressPercentage;
            if (ShowProgress)
            {
                System.Console.Write("\r  >> {0} {1} downloaded {2} of {3} bytes. [{4} %]",
                    this.DownloadFileName,
                    (string)e.UserState,
                    e.BytesReceived,
                    e.TotalBytesToReceive,
                    e.ProgressPercentage);
            }
        }

        private void Do()
        {
            try
            {
                this.Client = new WebClient();
                Uri uri = new Uri(this.SourceEndPoint);
                this.Client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCompleted);
                this.Client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallback);
                this.Client.DownloadFileAsync(uri, this.DownloadFilePath);
            }
            catch (Exception ex)
            {
                System.Console.Error.Write(ex.StackTrace);
            }
        }

        public void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.IsCancelled = e.Cancelled;
            if(e.Error!=null) 
            {
                this.Error = e.Error;
                Logger.Do("Download " + this.DownloadFileName + " error " + this.Error.StackTrace);
            }
            Status = true;
        }

        public void CleanProgressInfo()
        {
            if(this.ShowProgress) System.Console.Write("\r" + new string(' ', System.Console.WindowWidth) + "\r");
        }

        public void PrintProgressInfo()
        {
            if(this.ShowProgress)
            {
                System.Console.Write("\r  >> {0} {1} downloaded {2} of {3} bytes. [{4} %]",
                        this.DownloadFileName,
                        this.DownloadByteStatus,
                        this.DownloadedBytes,
                        this.TotalBytes,
                        this.ProgressPercentage);
            }
        }

        public void Cancel()
        {
            try
            {
                this.IsCancelled = true;
                this.Client.CancelAsync();
                this.Client.Dispose();
                this.Client = null;
            }            
            catch(Exception ex){this.Client = null;}

        }
    }
}
