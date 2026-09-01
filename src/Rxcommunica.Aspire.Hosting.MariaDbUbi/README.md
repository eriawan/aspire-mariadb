# Rxcommunica.Aspire.Hosting.MariaDbUbi library

Provides extension methods and resource definitions for a .NET Aspire AppHost to configure a MariaDb UBI based resource.
This library is specific for supporting MariaDb UBI based features and releases only, not intended to be compatible with MySql.

For MariaDb (non-UBI) support, you can use Rxcommunica.Aspire.Hosting.MariaDb nuget instead.

## Getting started

### Install the package

In your AppHost project, install the .NET Aspire MariaDb Hosting library with [NuGet](https://www.nuget.org):

```dotnetcli
dotnet add package Rxcommunica.Aspire.Hosting.MariaDbUbi
```

## Usage example

Then, in the _Program.cs_ file of `AppHost`, add a MariaDb UBI resource and consume the connection using the following methods:

```csharp
var db = builder.AddMariaDbUbi("mariadb").AddDatabase("mydb");

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(db);
```

## Additional documentation
https://learn.microsoft.com/dotnet/aspire/database/mysql-component

## Feedback & contributing

https://github.com/eriawan/aspire
