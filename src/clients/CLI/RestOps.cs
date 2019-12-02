using System;
using System.Net.Http;

namespace Repo.Clients.CLI
{
    public static class RestOps
    {
        public static async System.Threading.Tasks.Task<bool> IsEndPointPresentAsync(string EndPointURL)
        {
            bool Status = false;
            Logger.Do("IsEndPointPresentAsync URL " + EndPointURL);
            HttpResponseMessage Response = await GetResponseAsync(EndPointURL);
            if (Response.IsSuccessStatusCode) return true;
            return Status;
        }

        public static async System.Threading.Tasks.Task<HttpResponseMessage> GetResponseAsync(string EndPointURL)
        {
            Logger.Do("GetEndPointResponseAsync URL " + EndPointURL);
            var httpClient = new HttpClient();
            return await httpClient.GetAsync(EndPointURL);
        }

        public static string AppendQueryInEndPoint(string EndPointURL, string VariableName, string VariableValue)
        {
            string AppendChar = "&";
            if(!EndPointURL.Contains('?')) AppendChar = "?";
            return EndPointURL + AppendChar + VariableName + "=" + VariableValue;
        }
    }
}
