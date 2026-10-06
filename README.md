# Aspire support for MariaDb focusing on .NET 10.0 and later

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

Based on those rationales, this repository provides Aspire AppHost support for MariaDB with active focus on MariaDB current supported release lines of 12.2 LTS and 12.3 Rolling release.

**NOTE**

1. For reason number 2, it is described in the source code of Aspire AppHost for MySql itself:
   [MySqlContainerImageTags code] and [MySqlBuilderExtension code]
2. As of August 2026, MariaDB LTS is `12.2.x` and rolling release is `12.3.x`.  
   Use `mariadb:12.2` for LTS and `mariadb:12.3` for rolling (and their corresponding UBI tags where available).

## Build code Requirement

To ensure you are able to compile the solution in this repo successfully, these are the requirements:

1. Windows 11 24H1 or later. You can also use Windows 10 22H2 or Windows 10 release after 22H2, but I personally won't recommend it as Windows 10 is now entering end of support phase.
2. Visual Studio 2026 18.9.0 or later with .NET and ASP.NET workload installed.
3. The .NET 10.0.400 SDK installed. If you install Visual Studio 18.9.0 or later, this .NET SDK is included within the .NET workload.
4. Docker Desktop 4.x (or later) or Podman to ensure you have local Docker container support.

[MySqlContainerImageTags code]: https://github.com/dotnet/aspire/blob/main/src/Aspire.Hosting.MySql/MySqlContainerImageTags.cs
[MySqlBuilderExtension code]: https://github.com/dotnet/aspire/blob/main/src/Aspire.Hosting.MySql/MySqlBuilderExtensions.cs
