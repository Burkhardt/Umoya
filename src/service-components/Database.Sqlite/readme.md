# Umoya.'s SQLite Database Provider

This project contains Umoya.'s SQLite database provider.

## Migrations

Add a migration with:

```
dotnet ef migrations add MigrationName --context SqliteContext --output-dir Migrations --startup-project ..\Umoya.\Umoya.csproj

dotnet ef database update --context SqliteContext
```
