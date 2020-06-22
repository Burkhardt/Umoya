# Repo for AI Projects

[![Build status](https://dev.azure.com/zementis-ai/AI%20Projects/_apis/build/status/1.%20Repo%20(UMOYA)%20CI%20Pipeline)](https://dev.azure.com/zementis-ai/AI%20Projects/_build/latest?definitionId=4)

The spirit of Machine Learning and Deep Learning.

Versioning resource(like model, data and code) and manage its dependencies for ai projects.

<p align="center">
  <img width="100%" src="https://github.com/Umoya-ai/UMOYA/blob/master/docs/media/icons/UmoyaUI.PNG">
</p>

## Feature(s)
* Versioning resources like Models, Data and Code with [Semantic specification](https://semver.org) for AI projects in [Software AG MLW](https://github.com/SoftwareAG/MLW).
* Manange direct and in-direct dependencies for resources.
* Lightweight Server : Umoya (Repo) Server.
* Client Tools : CLI, Browser and Rest APIs to perform operations like add, upgrade, delete and publish resources.
* Cross-platform (i.e. You can use Sever/Client Tools on Windows, Linux or Mac OS) with .net core 3.1.301 (LTS)
* Dockerized
* Authenticate and Authorize with Software AG MLW's Identity Server (i.e. OAuth 2.0 RedHat KeyCloak)
* Supports read-through caching, It can index the entire resources. 
* Stay tuned, more features are planned with Software AG ZMOD.

## Getting Started
1. Install [.NET Core SDK](https://www.microsoft.com/net/download)
2. Download latest release from [here](https://github.com/nimeshgit/umoya/releases) and extract.
3. Start the service with `dotnet Umoya.dll`
4. Browse "http://localhost:8007/" in your browser.
For more information, Please refer to [our documentation](https://zmod.org/).

## Development
1. Install [.NET Core SDK](https://www.microsoft.com/net/download) and [Node.js](https://nodejs.org/)
2. Do clone of master branch.
3. Navigate to `clients\browser\src`
4. Install the frontend's dependencies with `npm install`
5. Navigate to `service-components\Umoya`
6. Start the service with `dotnet run`
7. Open the URL `https://localhost:8007/` in your browser

## Resources
*  To do testing, You can setup baseline test-data (resource types i.e. Models, Data and Code) from [Here](https://github.com/nimeshgit/umoya-resources).

## Release cycle
Releases are automated from the master branch, executed by [DevOps pipeline](https://dev.azure.com/zementis-ai/Umoya/_release?definitionId=1&view=mine&_a=releases), release is published only if all tests have passed along with prod go / no go strategies.

## Submitting patches
1. Fork or create branch.
2. Commit your changes.
3. Submit a PR, DevOps pipeline will run all tests.
4. Address issues in the review and build failures.
5. Before merge rebase on master `git rebase -i master` and possibly squash some of the commits.

## Issues ?
If you have an idea or found a bug, open an issue to discuss it.

## Credits to open source projects used as reference
This project is inspired from couple of open source projects on nuget server. ZMOD Repo has many customize features for Software AG MLW, However It also has code/stacks from other open source projects of nuget servers, either as reference or actually porting pieces of code.

Credits:
 * [Official NuGet.Server](https://github.com/NuGet/NuGet.Server) under Apache License Version 2.0
 * [NetCoreNugetServer another minimal dotnet core server](https://github.com/emresenturk/NetCoreNugetServer) under MIT License
 * [LiGet](https://github.com/ai-traders/liget) and its forks under MIT License
 * [NuGet.Lucene](https://github.com/themotleyfool/NuGet.Lucene/tree/master/source) which is part of klondike under Apache License 2.0
 * NuGet protocol [NuGet.Protocol](https://github.com/NuGet/NuGet.Client) under Apache License Version 2.0
 * NuGet CommandLine [NuGet commandLine](https://github.com/NuGet/NuGet.Client) under Apache License Version 2.0
 * Command line parsing by [McMaster.Extensions](https://github.com/natemcmaster/CommandLineUtils) under Apache License Version 2.0
