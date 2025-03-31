# Rxcommunica.Aspire.Hosting.MariaDb library

Provides extension methods and resource definitions for a .NET Aspire AppHost to configure a MariaDb resource.
This library is specific for supporting MariaDb.

## Getting started

### Install the package

In your AppHost project, install the .NET Aspire MySQL Hosting library with [NuGet](https://www.nuget.org):

```dotnetcli
dotnet add package Rxcommunica.Aspire.Hosting.MariaDb
```

## Usage example

Then, in the _Program.cs_ file of `AppHost`, add a MySQL resource and consume the connection using the following methods:

```csharp
var db = builder.AddMySql("mariadb").AddDatabase("mydb");

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(db);
```

## Additional documentation
https://learn.microsoft.com/dotnet/aspire/database/mysql-component

## Feedback & contributing

https://github.com/dotnet/aspire
