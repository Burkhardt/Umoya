using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using DotNetSearch.Extensions;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using static Repo.Clients.CLI.Resources;

namespace Repo.Clients.CLI.Commands
{
    [Command(
        Name = "umoya list",
        FullName = "UMOYA (CLI) action list from local (default) and repo",
        Description = Constants.ListCommandDescription
    )]
    [HelpOption]
    [VersionOptionFromMember(MemberName = nameof(GetVersion))]
    class ListCommand
    {
        private static int MaxWordWrapLineLength = 20;
        private static int MaxWordWrapColumnWidth = 25;
        private static int SkipResultsDefault = 0;
        private static int TakeDefault = 10;
        private static string FromOptionValue = "local";

        [Option("-f|--from", "Show resource(s) from Repo server or ZMOD local directory (default: local)", CommandOptionType.SingleValue)]
        public string From { get; set; } = FromOptionValue;

        [Option("-q|--query", "Query string to filter resource(s)", CommandOptionType.SingleValue)]
        public string Query { get; set; }

        [Option("-s|--skip", "Skip number of results (Default: 0)", CommandOptionType.SingleValue)]
        public int Skip { get; set; } = SkipResultsDefault;

        [Option("-t|--take", "Set number of results to display. (Default: 10)", CommandOptionType.SingleValue)]
        public int Take { get; set; } = TakeDefault;

        [Option("-c|--class", "Set class/type of resource (model, data, code or other) to filter i.e. --type model", CommandOptionType.SingleValue)]
        public string Type { get; set; }

        [Option("-u|--repo-url", "Give Repo server URL.", CommandOptionType.SingleValue)]
        public string RepoSourceURL { get; set; }

        [Option("-j|--json", "To output in json file i.e. --json myresources.json", CommandOptionType.SingleValue)]
        public string OutputJSONFile { get; set; }

        private async Task OnExecuteAsync()
        {
            Logger.Do("From " + From);
            try
            {
                if (!Console.IsZMODConfigured()) throw new Exceptions.ConfigurationNotFoundException(Constants.ListCommandName);
                #region List from Repo
                //Add Invalid From option exception
                // System.Console.WriteLine(From);
                if (From.ToLower() == "repo")
                {
                    var httpClient = new HttpClient();
                    var url = Resources.GetRepoSearchURL();
                    //Check Query Option entered from User
                    if (!String.IsNullOrEmpty(Query)) url = Resources.GetRepoSearchURLByQuery(Query);
                    if(!String.IsNullOrEmpty(Type))
                    {
                        if(Resources.IsResourceTypeValid(Type)) url = RestOps.AppendQueryInEndPoint(url, "packageType", Type); 
                        else throw new Exceptions.ResourceTypeInvalidException();
                    }                        
                    HttpResponseMessage ResponseFromRepo = await RestOps.GetResponseAsync(url);
                    if (!ResponseFromRepo.IsSuccessStatusCode) throw new Exceptions.ActionNotSuccessfullyPerformException(Constants.ListCommandName, "Repo server response " + ResponseFromRepo.StatusCode.ToString() + " " + ResponseFromRepo.ReasonPhrase + " " + ResponseFromRepo.Content);
                    var SearchResultSetInListOfResources = await ResponseFromRepo.Content.ReadAsAsync<ListResponse>();
                    List<Package> ListOfResources;                    
                    ListOfResources = SearchResultSetInListOfResources.Data.Skip(Skip).Take(Take).ToList();
                    PrintResults(ListOfResources, SearchResultSetInListOfResources.Data.Count());                    
                }
                #endregion
                #region List from local ZMOD
                else if (From.ToLower() == "local")
                {
                    Logger.Do("From Local directory");
                    await GetLocalLists();
                }
                else throw new Exception("Invalid input for option 'from'. You can give 'repo' or 'local'");
                #endregion
            }
            catch (Exceptions.ActionNotSuccessfullyPerformException erx) { Logger.Do(erx.Message); }
            catch (Exceptions.ResourceNotFoundException e) { Logger.Do(e.Message); }
            catch (Exceptions.ResourceInfoInvalidFormatException etr) { Logger.Do(etr.Message); }
            catch (Exceptions.ConfigurationNotFoundException e) { Logger.Do(e.Message); }
            catch (Exceptions.ResourceTypeInvalidException e) { Logger.Do(e.Message); }
            catch (Exception ex) { Console.LogError(ex.Message); }
        }
        private void PrintResults(List<Package> package, int TotalHits)
        {
            PrintTable(package);
            var numResultsToDisplay = Take < TotalHits ? Take : TotalHits;
            var starting = Skip + 1;
            var page = (Skip + Take) / Take;
            var ending = page * Take;
            if (ending > TotalHits)
                ending = TotalHits;
            System.Console.WriteLine($"{Skip + 1} - {ending} of {TotalHits} results");
        }

        private void PrintResultsForLocal(List<ResourceIdentifier> package, int TotalHits)
        {
            PrintTableForLocal(package);
            var numResultsToDisplay = Take < TotalHits ? Take : TotalHits;
            var starting = Skip + 1;
            var page = (Skip + Take) / Take;
            var ending = page * Take;
            if (ending > TotalHits)
                ending = TotalHits;
            System.Console.WriteLine($"{Skip + 1} - {ending} of {TotalHits} results");
        }

        private static string GetResourceType(string TypeofResource)
        {
            string ResourceType = "Other";
            if (TypeofResource.Equals("model")) ResourceType = "Model";
            else if (TypeofResource.Equals("data")) ResourceType = "Data";
            else if (TypeofResource.Equals("code")) ResourceType = "Code";
            return ResourceType;
        }

        private static string GetResourceType(List<string> Tags)
        {
            string ResourceType = "Other";

            for (int i = 0; i < Tags.Count; i++)
            {
                string TempTag = Tags[i].ToLower();
                if (TempTag.Equals("model")) ResourceType = "Model";
                else if (TempTag.Equals("data")) ResourceType = "Data";
                else if (TempTag.Equals("code")) ResourceType = "Code";
                if (!ResourceType.Equals("Other")) break;
            }


            return ResourceType;
        }
        private static List<string> GetWordWrapRows(string value)
        {
            var words = value.Split(' ');
            var rows = new List<string>();
            var line = "";
            for (var i = 0; i < words.Length; i++)
            {
                var word = words[i].Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
                if ((line + word).Length > MaxWordWrapLineLength)
                {
                    rows.Add(line);
                    line = "";
                }

                line += $"{word} ";
            }

            rows.Add(line);

            return rows;
        }
        private static string GetVersion()
                    => typeof(ListCommand).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

        private void PrintTable(List<Package> packageList)
        {
            System.Console.WriteLine();
            var data = packageList;

            var columnPad = "   ";
            var headerBoarder = '_';
            var rowDivider = '-';

            var items = new List<(string header, bool wordWrap, Func<Package, object> valueFunc)>
             {
                 ("Class", false, x => Resources.GetResourceType(x.Id).ToString()),
                 ("Name of Resource", false, x => x.Id),
                 ("Description", true, x => x.Description),
                 ("Authors", true, x => string.Join(",", x.Authors)),
                 ("Version", false, x => x.Version),
                 ("Downloads", false, x => x.TotalDownloads.ToAbbrString())
             };

            var columns = new List<string>[items.Count];
            var headers = new string[items.Count];
            var columnWidths = new int[items.Count];
            var valueCount = 0;

            foreach (var d in data)
            {
                int index = 0;
                foreach (var act in items)
                {
                    headers[index] = act.header;
                    var columnValue = act.valueFunc(d)?.ToString() ?? "(null)";
                    columnValue = columnValue.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");

                    if (columns[index] == null)
                    {
                        columns[index] = new List<string>();
                    }

                    columns[index++].Add(columnValue);
                }
                valueCount++;
            }

            if (valueCount == 0)
            {
                System.Console.WriteLine("No results found");
                return;
            }

            for (var i = 0; i < columns.Length; i++)
            {
                var wordWrap = items[i].wordWrap;

                var width = Math.Max(columns[i].Max(x => x.Length), headers[i].Length);
                if (wordWrap)
                {
                    width = Math.Min(width, MaxWordWrapColumnWidth);
                }

                columnWidths[i] = width;
            }

            int headerWidth = columnWidths.Sum() + columnPad.Length * (items.Count - 1);

            // Determine final layout, word wrapping etc
            var rows = new List<string[]>();
            for (var i = 0; i < valueCount; i++)
            {
                var rowsToAdd = new List<string[]>();
                var newRow = new string[columns.Length];
                rowsToAdd.Add(newRow);
                for (var j = 0; j < columns.Length; j++)
                {
                    var value = columns[j][i];
                    if (value.Length <= columnWidths[j])
                    {
                        newRow[j] = value;
                        continue;
                    }

                    // Word wrap it
                    var wordWrapRows = GetWordWrapRows(value);
                    newRow[j] = wordWrapRows[0];

                    for (var x = 1; x < wordWrapRows.Count; x++)
                    {
                        if (rowsToAdd.Count < x + 1)
                        {
                            rowsToAdd.Add(new string[columns.Length]);
                        }

                        rowsToAdd[x][j] = wordWrapRows[x];
                    }
                }

                rows.AddRange(rowsToAdd);
            }

            // Print output

            //Print headers
            for (var i = 0; i < headers.Length; i++)
            {
                System.Console.Write(headers[i].PadRight(columnWidths[i]));

                if (i < headers.Length - 1)
                    System.Console.Write(columnPad);
            }

            System.Console.WriteLine();

            // Print header border
            System.Console.WriteLine("".PadRight(headerWidth, headerBoarder));

            // Print rows
            for (var i = 0; i < rows.Count; i++)
            {
                int j = 0;
                for (; j < columns.Length; j++)
                {
                    var value = rows[i][j] ?? "";

                    if (j == 0 && i > 0 && !string.IsNullOrEmpty(value))
                    {
                        // We found a new package. Print divider
                        System.Console.WriteLine("".PadRight(headerWidth, rowDivider));

                    }

                    System.Console.Write(value.PadRight(columnWidths[j]));

                    if (j < columnWidths.Length - 1)
                    {
                        System.Console.Write(columnPad);
                    }
                }

                System.Console.WriteLine();
            }

            System.Console.WriteLine();
        }

        private void PrintTableForLocal(List<ResourceIdentifier> resourceList)
        {
            System.Console.WriteLine();
            var data = resourceList;
            var columnPad = "   ";
            var headerBoarder = '_';
            var rowDivider = '-';

            var items = new List<(string header, bool wordWrap, Func<ResourceIdentifier, object> valueFunc)>
             {
                 ("Class", false, x => Resources.GetResourceType(x.ResourceName)),
                 ("Name of Resource", false, x => x.ResourceName.ToString()),
                 ("Description", true, x => x.Description),
                 ("Authors", true, x => x.Authors),
                 ("Version", false, x => x.Version),
                 ("Size", false, x => x.Size)
             };

            var columns = new List<string>[items.Count];
            var headers = new string[items.Count];
            var columnWidths = new int[items.Count];
            var valueCount = 0;

            foreach (var d in data)
            {
                int index = 0;
                foreach (var act in items)
                {
                    headers[index] = act.header;
                    var columnValue = act.valueFunc(d)?.ToString() ?? "(null)";
                    columnValue = columnValue.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");

                    if (columns[index] == null)
                    {
                        columns[index] = new List<string>();
                    }

                    columns[index++].Add(columnValue);
                }
                valueCount++;
            }

            if (valueCount == 0)
            {
                System.Console.WriteLine("No results found");
                return;
            }

            for (var i = 0; i < columns.Length; i++)
            {
                var wordWrap = items[i].wordWrap;

                var width = Math.Max(columns[i].Max(x => x.Length), headers[i].Length);
                if (wordWrap)
                {
                    width = Math.Min(width, MaxWordWrapColumnWidth);
                }

                columnWidths[i] = width;
            }

            int headerWidth = columnWidths.Sum() + columnPad.Length * (items.Count - 1);

            // Determine final layout, word wrapping etc
            var rows = new List<string[]>();
            for (var i = 0; i < valueCount; i++)
            {
                var rowsToAdd = new List<string[]>();
                var newRow = new string[columns.Length];
                rowsToAdd.Add(newRow);
                for (var j = 0; j < columns.Length; j++)
                {
                    var value = columns[j][i];
                    if (value.Length <= columnWidths[j])
                    {
                        newRow[j] = value;
                        continue;
                    }

                    // Word wrap it
                    var wordWrapRows = GetWordWrapRows(value);
                    newRow[j] = wordWrapRows[0];

                    for (var x = 1; x < wordWrapRows.Count; x++)
                    {
                        if (rowsToAdd.Count < x + 1)
                        {
                            rowsToAdd.Add(new string[columns.Length]);
                        }

                        rowsToAdd[x][j] = wordWrapRows[x];
                    }
                }

                rows.AddRange(rowsToAdd);
            }

            // Print output

            //Print headers
            for (var i = 0; i < headers.Length; i++)
            {
                System.Console.Write(headers[i].PadRight(columnWidths[i]));

                if (i < headers.Length - 1)
                    System.Console.Write(columnPad);
            }

            System.Console.WriteLine();

            // Print header border
            System.Console.WriteLine("".PadRight(headerWidth, headerBoarder));

            // Print rows
            for (var i = 0; i < rows.Count; i++)
            {
                int j = 0;
                for (; j < columns.Length; j++)
                {
                    var value = rows[i][j] ?? "";

                    if (j == 0 && i > 0 && !string.IsNullOrEmpty(value))
                    {
                        // We found a new package. Print divider
                        System.Console.WriteLine("".PadRight(headerWidth, rowDivider));

                    }

                    System.Console.Write(value.PadRight(columnWidths[j]));

                    if (j < columnWidths.Length - 1)
                    {
                        System.Console.Write(columnPad);
                    }
                }

                System.Console.WriteLine();
            }

            System.Console.WriteLine();
        }

        private async Task GetLocalLists()
        {
            Dictionary<Resources.ResourceType, Dictionary<string, ResourceIdentifier>> ListOfLocalResources =
            new Dictionary<Resources.ResourceType, Dictionary<string, ResourceIdentifier>>();

            ListOfLocalResources = await Resources.GetLocalResourceList(Resources.ResourceType.Any);

            List<ResourceIdentifier> ListOfResourceIdentifier = new List<ResourceIdentifier>();

            foreach (Resources.ResourceType InterestedType in ListOfLocalResources.Keys)
            {
                foreach (string ResourceName in ListOfLocalResources[InterestedType].Keys)
                {
                    ListOfResourceIdentifier.Add(ListOfLocalResources[InterestedType][ResourceName]);
                }
            }
            if (!String.IsNullOrEmpty(OutputJSONFile))
            {
                if (!GenerateOutputJSONFile(null, ListOfResourceIdentifier, From, OutputJSONFile))
                    throw new Exceptions.OutputJsonFileException(Constants.ListCommandName);
            }
            if (!String.IsNullOrEmpty(Query))
            {
                ListOfResourceIdentifier = ListOfResourceIdentifier.
                    Where(x =>
                    x.ResourceName.ToLower().Contains(Query.ToLower())
                    || x.Description.ToLower().Contains(Query.ToLower())
                    ).ToList();
            }
            if (!String.IsNullOrEmpty(Type))
            {
                if (GetResourceType(Type.ToLower()).ToLower() != "other")
                {
                    ListOfResourceIdentifier = ListOfResourceIdentifier.Where(x => x.TypeOfResource.ToString().ToLower().Contains(Type.ToLower())).ToList();
                    //items = items.Where(i => i.Tags.Any(e => e.Contains(e)));
                }
                else
                {
                    System.Console.WriteLine(Type + " does not exists in the resource type. Specify Type as Data, Code, Model");
                    return;
                }
            }
            PrintResultsForLocal(ListOfResourceIdentifier, ListOfResourceIdentifier.Count);
        }
    }
}
