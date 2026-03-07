using Aspire.Hosting.ApplicationModel;

namespace Rxcommunica.Aspire.Hosting.ApplicationModel
{
    /// <summary>
    /// Resource representing PhpMyAdmin container for MariaDb.
    /// </summary>
    public sealed class PhpMyAdminMariaDbContainerResource : ContainerResource
    {
        /// <summary>
        /// Create new instance of PhpMyAdminMariaDbContainerResource.
        /// </summary>
        /// <param name="name"></param>
        public PhpMyAdminMariaDbContainerResource(string name) : base(name)
        {
        }
    }
}
