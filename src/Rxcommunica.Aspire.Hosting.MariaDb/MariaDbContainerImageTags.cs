using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rxcommunica.Aspire.Hosting.MariaDb
{
    internal class MariaDbContainerImageTags
    {
        /// <summary>Docker official registry address</summary>
        /// <remarks>docker.io</remarks>
        public const string Registry = "docker.io";

        /// <summary>MariaDb image subdomain</summary>
        /// <remarks>library/mariadb</remarks>
        public const string Image = "library/mariadb";

        /// <summary>Maria Db 12.3 LTS</summary>
        /// <remarks>MariaDb 12.3 LTS release as of September 2026.</remarks>
        public const string TagLTS = "12.3";

        /// <summary>MariaDb rolling release.</summary>
        /// <remarks>MariaDb 12.2 rolling release as of September 2026. It is not an LTS release.</remarks>
        public const string TagRollingRelease = "12.2";

        /// <summary>PHPMyAdmin image</summary>
        /// <remarks>PHPMyAdmin image library/phpmyadmin</remarks>
        public const string PhpMyAdminImage = "library/phpmyadmin";

        /// <summary>PHPMyAdmin version tag as of September 2026.</summary>
        /// <remarks>PHPMyAdmin version as of September 2026, v5.2</remarks>
        public const string PhpMyAdminTag = "5.2";
    }
}
