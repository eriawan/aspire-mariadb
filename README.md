
# Aspire support for MariaDb focusing on .NET 9.0 and later

---

This is .NET Aspire support for MariaDb database.

## Rationale background

The reason's main goal is simple: we want to support MariaDb's own releases, features, and variants/flavors as currently available from MariaDb general offering.

These are the detailed reasons/rationales of why this repo exists:

1. Current Aspire and Aspire Community Toolkit does not have Aspire AppHost's support for MariaDb.
2. If we want to use containerized MariaDb, MariaDb needs its own pull of MariaDb Docker image and MariaDb's specific environment variables.
3. We need to support MariaDb's specific/unique server setting that can be included in the connection string.
4. We need to support MariaDb container images of both general MariaDb and the UBI-based of MariaDb.
5. Keeping up with MariaDb releases and features, separated from MySql.
6. Keeping up with different flavors of MariaDb releases, both LTS and non LTS (usually called "rolling release").

Based on those rationales, therefore I develop my own Aspire AppHost's support for MariaDb, starting from MariaDb 11.4.x LTS release and the upcoming release of MariaDb 12.x.x that is still in preview as of April, 2025.

**NOTE**

1. For reason number 2, it is described in the source code of Aspire AppHost for MySql itself:
 [MySqlContainerImageTags code] and [MySqlBuilderExtension code]
2. On 1st April 2025, the MariaDb's current LTS release is 11.4.5 focusing on 11.4 and the current rolling release is 11.7.2. Therefore for the LTS we could just use mariadb:11.4 (and mariadb:11.4-ubi)

## Build code Requirement

To ensure you are able to compile the solution in this repo successfully, these are the requirements:

1. Windows 11 24H1 or later. You can also use Windows 10 22H2 or Windows 10 release after 22H2, but I personally won't recommend it as Windows 10 is now entering end of support phase.
2. Visual Studio 2022 17.13.4 or later with .NET and ASP.NET workload installed.
3. The .NET 9.0.201 SDK installed. If you install Visual Studio 17.13.3 to 17.13.5, this .NET SDK is included within the .NET workload.
4. Docker Desktop 4.x (or later) or Podman to ensure you have local Docker container support.

[MySqlContainerImageTags code]: https://github.com/dotnet/aspire/blob/main/src/Aspire.Hosting.MySql/MySqlContainerImageTags.cs
[MySqlBuilderExtension code]: https://github.com/dotnet/aspire/blob/main/src/Aspire.Hosting.MySql/MySqlBuilderExtensions.cs
