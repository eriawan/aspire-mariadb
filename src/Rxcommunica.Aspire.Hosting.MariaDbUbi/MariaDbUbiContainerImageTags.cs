using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rxcommunica.Aspire.Hosting.MariaDbUbi
{
    internal class MariaDbUbiContainerImageTags
    {
        /// <remarks>docker.io</remarks>
        public const string Registry = "docker.io";

        /// <remarks>library/mysql</remarks>
        public const string Image = "library/mariadb";

        /// <remarks>MariaDb 11.4 LTS release</remarks>
        public const string TagLTS = "11.4-ubi";

        /// <remarks>MariaDb 11.7 rolling release. It is not an LTS release.</remarks>
        public const string TagRollingRelease = "11.7-ubi";

        /// <remarks>library/phpmyadmin</remarks>
        public const string PhpMyAdminImage = "library/phpmyadmin";

        /// <remarks>5.2</remarks>
        public const string PhpMyAdminTag = "5.2";
    }
}
