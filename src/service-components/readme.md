# Umoya Source Code :

These folders contain the core components of Umoya:

* `Umoya` - the app's entry point that glues everything together
* `Umoya.Core` - Umoya's core logic and services
* `Umoya.Core.Server` - the services that implement [the NuGet server APIs](https://docs.microsoft.com/en-us/nuget/api/overview) using `Umoya.Core`
* `Umoya.Protocol` - libraries to interact with [NuGet servers' APIs](https://docs.microsoft.com/en-us/nuget/api/overview)

These folders contain database-specific components of Umoya:

* `Umoya.Database.Sqlite` - SQLite database provider
