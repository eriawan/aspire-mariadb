using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rxcommunica.Aspire.Hosting.MariaDbUbi
{
    /// <summary>
    /// Provides extension methods for adding MariaDb UBI resources to an Aspire's IDistributedApplicationBuilder. />.
    /// </summary>
    /// <remarks>
    /// <para>This class is different from Aspire's MySql builder extension, as this class only support MariaDb and it will be
    /// further evolve to have MariaDb specific (or unique) features and needs that are different from MySql.</para>
    /// <para>This class is used to support MariaDb Docker image that based on RedHat UBI.</para>
    /// <para>For more information about IDistributedApplicationBuilder, see the latest 
    /// official documentation for IDistributedApplicationBuilder at: 
    /// https://learn.microsoft.com/en-us/dotnet/api/aspire.hosting.idistributedapplicationbuilder?view=dotnet-aspire-9.1
    /// </para>
    /// <para>The official information from MariaDb about MariaDb's support for Docker image of MariaDb based on RedHat UBI is here:
    /// https://mariadb.org/mariadb-release-ubi-based-docker-official-images/
    /// </para>
    /// </remarks>
    public static class MariaDbUbiBuilderExtensions
    {
    }
}
