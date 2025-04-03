using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Microsoft.Extensions.DependencyInjection;

namespace Rxcommunica.Aspire.Hosting.MariaDb
{
    /// <summary>
    /// Provides extension methods for adding MariaDb resources to an Aspire's IDistributedApplicationBuilder. />.
    /// </summary>
    /// <remarks>
    /// <para>This class is different from Aspire's MySql builder extension, as this class only support MariaDb and it will be
    /// further evolve to have MariaDb specific (or unique) features and needs that are different from MySql.</para>
    /// <para>For more information about IDistributedApplicationBuilder, see the latest 
    /// official documentation for IDistributedApplicationBuilder at: 
    /// https://learn.microsoft.com/en-us/dotnet/api/aspire.hosting.idistributedapplicationbuilder?view=dotnet-aspire-9.1
    /// </para>
    /// </remarks>
    public static class MariaDbBuilderExtensions
    {
        /// <summary>
        /// This is a static readonly string that acts as constant to store MariaDb specific env variable 
        /// to set MariaDb's "root" password.
        /// </summary>
        private static readonly string PasswordEnvVarName = "MARIADB_ROOT_PASSWORD";

        private const UnixFileMode FileMode644 = UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.GroupRead | UnixFileMode.OtherRead;

        /// <summary>
        /// Adds a MariaDb server resource to the application model. For local development a container is used.
        /// </summary>
        /// <remarks>
        /// This version of the package defaults to the <inheritdoc cref="MySqlContainerImageTags.Tag"/> tag of the <inheritdoc cref="MySqlContainerImageTags.Image"/> container image.
        /// </remarks>
        /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
        /// <param name="name">The name of the resource. This name will be used as the connection string name when referenced in a dependency.</param>
        /// <param name="password">The parameter used to provide the root password for the MariaDb resource. If <see langword="null"/> a random password will be generated.</param>
        /// <param name="port">The host port for MariaDb.</param>
        /// <param name="usingLTS">True if MariaDb LTS is used. The default value is true.</param>
        /// <returns>A reference to the <see cref="IResourceBuilder{T}"/>.</returns>
        public static IResourceBuilder<MariaDbServerResource> AddMariaDb(this IDistributedApplicationBuilder builder, [ResourceName] string name, IResourceBuilder<ParameterResource>? password = null, int? port = null, bool usingLTS = true)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentException.ThrowIfNullOrEmpty(name);

            var passwordParameter = password?.Resource ?? ParameterResourceBuilderExtensions.CreateDefaultPasswordParameter(builder, $"{name}-password");

            var resource = new MariaDbServerResource(name, passwordParameter);

            string? connectionString = null;

            builder.Eventing.Subscribe<ConnectionStringAvailableEvent>(resource, async (@event, ct) =>
            {
                connectionString = await resource.ConnectionStringExpression.GetValueAsync(ct).ConfigureAwait(false);

                if (connectionString == null)
                {
                    throw new DistributedApplicationException($"ConnectionStringAvailableEvent was published for the '{resource.Name}' resource but the connection string was null.");
                }
            });

            var healthCheckKey = $"{name}_check";
            builder.Services.AddHealthChecks().AddMySql(sp => connectionString ?? throw new InvalidOperationException("Connection string is unavailable"), name: healthCheckKey);

            string mariaDbImageTagToUse = "";
            if (usingLTS)
            {
                mariaDbImageTagToUse = MariaDbContainerImageTags.TagLTS;
            }
            else
            {
                mariaDbImageTagToUse = MariaDbContainerImageTags.TagRollingRelease;
            }
            return builder.AddResource(resource)
                              .WithEndpoint(port: port, targetPort: 3306, name: MariaDbServerResource.PrimaryEndpointName) // Internal port is always 3306.
                              .WithImage(MariaDbContainerImageTags.Image, mariaDbImageTagToUse)
                              .WithImageRegistry(MariaDbContainerImageTags.Registry)
                              .WithEnvironment(context =>
                              {
                                  context.EnvironmentVariables[PasswordEnvVarName] = resource.PasswordParameter;
                              })
                              .WithHealthCheck(healthCheckKey);
        }

        /// <summary>
        /// Adds a MariaDb database to the application model.
        /// </summary>
        /// <param name="builder">The MySQL server resource builder.</param>
        /// <param name="name">The name of the resource. This name will be used as the connection string name when referenced in a dependency.</param>
        /// <param name="databaseName">The name of the database. If not provided, this defaults to the same value as <paramref name="name"/>.</param>
        /// <returns>A reference to the <see cref="IResourceBuilder{T}"/>.</returns>
        public static IResourceBuilder<MariaDbDatabaseResource> AddDatabase(this IResourceBuilder<MariaDbServerResource> builder, [ResourceName] string name, string? databaseName = null)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentException.ThrowIfNullOrEmpty(name);

            // Use the resource name as the database name if it's not provided
            databaseName ??= name;

            builder.Resource.AddDatabase(name, databaseName);
            var mySqlDatabase = new MariaDbDatabaseResource(name, databaseName, builder.Resource);
            return builder.ApplicationBuilder.AddResource(mySqlDatabase);
        }

    }
}
