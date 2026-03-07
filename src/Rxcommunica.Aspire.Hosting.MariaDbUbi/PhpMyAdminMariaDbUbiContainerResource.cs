using Aspire.Hosting.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rxcommunica.Aspire.Hosting.MariaDbUbi
{
    /// <summary>
    /// Resource representing PhpMyAdmin container for MariaDb UBI.
    /// </summary>
    public sealed class PhpMyAdminMariaDbUbiContainerResource : ContainerResource
    {
        /// <summary>
        /// Create new instance of PhpMyAdminMariaDbUbiContainerResource.
        /// </summary>
        /// <param name="name"></param>
        public PhpMyAdminMariaDbUbiContainerResource(string name) : base(name)
        {
        }
    }
}
