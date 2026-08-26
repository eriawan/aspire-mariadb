using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Microsoft.Extensions.DependencyInjection;
using Rxcommunica.Aspire.Hosting.ApplicationModel;
using Rxcommunica.Aspire.Hosting.MariaDb;

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
        /// This version of the package defaults to the <inheritdoc cref="MariaDbContainerImageTags.TagLTS"/> tag of the <inheritdoc cref="MySqlContainerImageTags.Image"/> container image.
        /// </remarks>
        /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
        /// <param name="name">The name of the resource. This name will be used as the connection string name when referenced in a dependency.</param>
        /// <param name="password">The parameter used to provide the root password for the MariaDb resource. If <see langword="null"/> a random password will be generated.</param>
        /// <param name="port">The host port for MariaDb.</param>
        /// <param name="usingLTS">True if MariaDb LTS is used, otherwise rolling release is used. The default value is true.</param>
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

        /// <summary>
        /// Adds a phpMyAdmin administration and development platform for MySql to the application model.
        /// </summary>
        /// <remarks>
        /// This version of the package defaults to the <inheritdoc cref="MySqlContainerImageTags.PhpMyAdminTag"/> tag of the <inheritdoc cref="MySqlContainerImageTags.PhpMyAdminImage"/> container image.
        /// </remarks>
        /// <param name="builder">The MySql server resource builder.</param>
        /// <param name="configureContainer">Callback to configure PhpMyAdmin container resource.</param>
        /// <param name="containerName">The name of the container (Optional).</param>
        /// <returns>A reference to the <see cref="IResourceBuilder{T}"/>.</returns>
        public static IResourceBuilder<T> WithPhpMyAdmin<T>(this IResourceBuilder<T> builder, Action<IResourceBuilder<PhpMyAdminMariaDbContainerResource>>? configureContainer = null, string? containerName = null) where T : MariaDbServerResource
        {
            ArgumentNullException.ThrowIfNull(builder);

            if (builder.ApplicationBuilder.Resources.OfType<PhpMyAdminMariaDbContainerResource>().Any())
            {
                return builder;
            }

            containerName ??= $"{builder.Resource.Name}-phpmyadmin";

            var phpMyAdminContainer = new PhpMyAdminMariaDbContainerResource(containerName);
            var phpMyAdminContainerBuilder = builder.ApplicationBuilder.AddResource(phpMyAdminContainer)
                                                    .WithImage(MariaDbContainerImageTags.PhpMyAdminImage, MariaDbContainerImageTags.PhpMyAdminTag)
                                                    .WithImageRegistry(MariaDbContainerImageTags.Registry)
                                                    .WithHttpEndpoint(targetPort: 80, name: "http")
                                                    .ExcludeFromManifest();

            builder.ApplicationBuilder.Eventing.Subscribe<BeforeResourceStartedEvent>(async (e, ct) =>
            {
                var mySqlInstances = builder.ApplicationBuilder.Resources.OfType<MariaDbServerResource>();

                if (!mySqlInstances.Any())
                {
                    // No-op if there are no MariaDb resources present.
                    return;
                }

                if (mySqlInstances.Count() == 1)
                {
                    var singleInstance = mySqlInstances.Single();
                    var endpoint = singleInstance.PrimaryEndpoint;
                    phpMyAdminContainerBuilder.WithEnvironment(context =>
                    {
                        // PhpMyAdmin assumes MySql is being accessed over a default Aspire container network and hardcodes the resource address
                        // This will need to be refactored once updated service discovery APIs are available
                        context.EnvironmentVariables.Add("PMA_HOST", $"{endpoint.Resource.Name}:{endpoint.TargetPort}");
                        context.EnvironmentVariables.Add("PMA_USER", "root");
                        context.EnvironmentVariables.Add("PMA_PASSWORD", singleInstance.PasswordParameter);
                    });
                }
                else
                {
                    var tempConfigFile = await WritePhpMyAdminConfiguration(mySqlInstances, ct).ConfigureAwait(false);

                    try
                    {
                        var aspireStore = e.Services.GetRequiredService<IAspireStore>();

                        // Deterministic file path for the configuration file based on its content
                        var configStoreFilename = aspireStore.GetFileNameWithContent($"{builder.Resource.Name}-config.user.inc.php", tempConfigFile);

                        // Need to grant read access to the config file on unix like systems.
                        if (!OperatingSystem.IsWindows())
                        {
                            File.SetUnixFileMode(configStoreFilename, FileMode644);
                        }

                        phpMyAdminContainerBuilder.WithBindMount(configStoreFilename, "/etc/phpmyadmin/config.user.inc.php");
                    }
                    finally
                    {
                        try
                        {
                            File.Delete(tempConfigFile);
                        }
                        catch
                        {
                        }
                    }
                }
            });

            configureContainer?.Invoke(phpMyAdminContainerBuilder);

            return builder;
        }

        /// <summary>
        /// Configures the host port that the PGAdmin resource is exposed on instead of using randomly assigned port.
        /// </summary>
        /// <param name="builder">The resource builder for PGAdmin.</param>
        /// <param name="port">The port to bind on the host. If <see langword="null"/> is used, a random port will be assigned.</param>
        /// <returns>The resource builder for PGAdmin.</returns>
        public static IResourceBuilder<PhpMyAdminMariaDbContainerResource> WithHostPort(this IResourceBuilder<PhpMyAdminMariaDbContainerResource> builder, int? port)
        {
            ArgumentNullException.ThrowIfNull(builder);

            return builder.WithEndpoint("http", endpoint =>
            {
                endpoint.Port = port;
            });
        }

        /// <summary>
        /// Adds a named volume for the data folder to a MySql container resource.
        /// </summary>
        /// <param name="builder">The resource builder.</param>
        /// <param name="name">The name of the volume. Defaults to an auto-generated name based on the application and resource names.</param>
        /// <param name="isReadOnly">A flag that indicates if this is a read-only volume.</param>
        /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
        public static IResourceBuilder<MariaDbServerResource> WithDataVolume(this IResourceBuilder<MariaDbServerResource> builder, string? name = null, bool isReadOnly = false)
        {
            ArgumentNullException.ThrowIfNull(builder);

            return builder.WithVolume(name ?? VolumeNameGenerator.Generate(builder, "data"), "/var/lib/mysql", isReadOnly);
        }

        /// <summary>
        /// Adds a bind mount for the data folder to a MySql container resource.
        /// </summary>
        /// <param name="builder">The resource builder.</param>
        /// <param name="source">The source directory on the host to mount into the container.</param>
        /// <param name="isReadOnly">A flag that indicates if this is a read-only mount.</param>
        /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
        public static IResourceBuilder<MariaDbServerResource> WithDataBindMount(this IResourceBuilder<MariaDbServerResource> builder, string source, bool isReadOnly = false)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentException.ThrowIfNullOrEmpty(source);

            return builder.WithBindMount(source, "/var/lib/mysql", isReadOnly);
        }

        /// <summary>
        /// Adds a bind mount for the init folder to a MySql container resource.
        /// </summary>
        /// <param name="builder">The resource builder.</param>
        /// <param name="source">The source directory on the host to mount into the container.</param>
        /// <param name="isReadOnly">A flag that indicates if this is a read-only mount.</param>
        /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
        public static IResourceBuilder<MariaDbServerResource> WithInitBindMount(this IResourceBuilder<MariaDbServerResource> builder, string source, bool isReadOnly = true)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentException.ThrowIfNullOrEmpty(source);

            return builder.WithBindMount(source, "/docker-entrypoint-initdb.d", isReadOnly);
        }

        private static async Task<string> WritePhpMyAdminConfiguration(IEnumerable<MariaDbServerResource> mariaDbInstances, CancellationToken cancellationToken)
        {
            // This temporary file is not used by the container, it will be copied and then deleted
            var filePath = Path.GetTempFileName();

            using var writer = new StreamWriter(filePath);

            writer.WriteLine("<?php");
            writer.WriteLine();
            writer.WriteLine("$i = 0;");
            writer.WriteLine();
            foreach (var mariaDbInstance in mariaDbInstances)
            {
                var endpoint = mariaDbInstance.PrimaryEndpoint;
                var pwd = await mariaDbInstance.PasswordParameter.GetValueAsync(cancellationToken).ConfigureAwait(false);
                writer.WriteLine("$i++;");
                // PhpMyAdmin assumes MySql is being accessed over a default Aspire container network and hardcodes the resource address
                // This will need to be refactored once updated service discovery APIs are available
                writer.WriteLine($"$cfg['Servers'][$i]['host'] = '{endpoint.Resource.Name}:{endpoint.TargetPort}';");
                writer.WriteLine($"$cfg['Servers'][$i]['verbose'] = '{mariaDbInstance.Name}';");
                writer.WriteLine($"$cfg['Servers'][$i]['auth_type'] = 'cookie';");
                writer.WriteLine($"$cfg['Servers'][$i]['user'] = 'root';");
                writer.WriteLine($"$cfg['Servers'][$i]['password'] = '{pwd}';");
                writer.WriteLine($"$cfg['Servers'][$i]['AllowNoPassword'] = true;");
                writer.WriteLine();
            }
            writer.WriteLine("$cfg['DefaultServer'] = 1;");
            writer.WriteLine("?>");

            return filePath;
        }
    }
}
