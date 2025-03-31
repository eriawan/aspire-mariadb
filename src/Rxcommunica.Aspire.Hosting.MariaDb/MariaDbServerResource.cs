using Aspire.Hosting.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rxcommunica.Aspire.Hosting.MariaDb
{

    /// <summary>
    /// A resource that represents a MariaDb container.
    /// </summary>
    /// <remarks>This class is specific for MariaDb, not to be used for MySql.</remarks>
    public class MariaDbServerResource : ContainerResource, IResourceWithConnectionString
    {
        internal static string PrimaryEndpointName => "tcp";

        /// <summary>
        /// Initializes a new instance of the <see cref="MariaDbServerResource"/> class.
        /// </summary>
        /// <param name="name">The name of the resource.</param>
        /// <param name="password">A parameter that contains the MySQL server password.</param>
        public MariaDbServerResource(string name, ParameterResource password) : base(name)
        {
            ArgumentNullException.ThrowIfNull(password);

            PrimaryEndpoint = new(this, PrimaryEndpointName);
            PasswordParameter = password;
        }

        /// <summary>
        /// Gets the primary endpoint for the MariaDb server.
        /// </summary>
        public EndpointReference PrimaryEndpoint { get; }

        /// <summary>
        /// Gets the parameter that contains the MariaDb server password.
        /// </summary>
        public ParameterResource PasswordParameter { get; }

        /// <summary>
        /// Gets the connection string expression for the MySQL server.
        /// </summary>
        public ReferenceExpression ConnectionStringExpression =>
            ReferenceExpression.Create(
                $"Server={PrimaryEndpoint.Property(EndpointProperty.Host)};Port={PrimaryEndpoint.Property(EndpointProperty.Port)};User ID=root;Password={PasswordParameter}");

        private readonly Dictionary<string, string> _databases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// A dictionary where the key is the resource name and the value is the database name.
        /// </summary>
        public IReadOnlyDictionary<string, string> Databases => _databases;

        internal void AddDatabase(string name, string databaseName)
        {
            _databases.TryAdd(name, databaseName);
        }
    }
}
