
# Aspire support for MariaDb focusing on .NET 9.0 and later

---

This is .NET Aspire support for MariaDb database.

## Rationale background

These are the reasons/rationales of why this repo exists:

1. Current Aspire and Aspire Community Toolkit does not have Aspire AppHost's support for MariaDb.
2. If we want to use containerized MariaDb, MariaDb needs its own pull of MariaDb Docker image and MariaDb's specific environment variables.
3. We need to support MariaDb's server setting that can be included in the connection string. 

Based on those rationales, therefore I develop my own Aspire AppHost's support for MariaDb, starting from MariaDb 11.4.x LTS release and the upcoming release of MariaDb 12.x.x that is still in preview as of April, 2025.

**NOTE**
For reason number 2, it is described in the source code of Aspire AppHost for MySql itself:
 [MySqlContainerImageTags code] and [MySqlBuilderExtension code]

## Build code Requirement

To ensure you are able to compile the solution in this repo successfully, these are the requirements:

1. Windows 11 24H1 or later. You can also use Windows 10 22H2 or Windows 10 release after 22H2, but I personally won't recommend it as Windows 10 is now entering end of support phase.
2. Visual Studio 2022 17.13.4 or later with .NET and ASP.NET workload installed.
3. The .NET 9.0.201 SDK installed. If you install Visual Studio 17.13.3 to 17.13.5, this .NET SDK is included within the .NET workload.
4. Docker Desktop 4.x (or later) or Podman to ensure you have local Docker container support.

[MySqlContainerImageTags code]: https://github.com/dotnet/aspire/blob/main/src/Aspire.Hosting.MySql/MySqlContainerImageTags.cs
[MySqlBuilderExtension code]: https://github.com/dotnet/aspire/blob/main/src/Aspire.Hosting.MySql/MySqlBuilderExtensions.cs
