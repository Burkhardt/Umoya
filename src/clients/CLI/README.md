UMOYA CLI
============

UMOYA CLI is one of the Client for Repo. It helps to manage resources for ai projects with command line.

## Installation 

### .NET Core 3.1 & higher
```
dotnet tool install --global umoya
```
## Usage

### Help

```
$ umoya --help

Usage: umoya [arguments] [options]

Arguments:
  init  <>
  {TBD}

Options:
  --version             Show version information
  {TBD}
```

## Build

```
git clone https://github.com/nimeshgit/repo/tree/master
```
```
cd src\clients\CLI
```
```
dotnet pack -c release -o publish
```

Output is located in ```src/Clients/CLI/Publish```

### Install

```
dotnet tool install --global --add-source ./publish umoya
```

### Uninstall

```
dotnet tool uninstall -g umoya
```