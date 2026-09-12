using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace Rxcommunica.Aspire.Hosting.ApplicationModel;


/// <summary>
/// A resource that represents a MariaDb container.
/// </summary>
/// <remarks>This class is specific for MariaDb, not to be used for MySql.</remarks>
[AspireExport(ExposeProperties = true)]
public class MariaDbServerResource : ContainerResource, IResourceWithConnectionString
{
    internal static string PrimaryEndpointName => "tcp";
    private const string DefaultUserName = "root";

    private readonly Dictionary<string, string> _databases = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<MariaDbDatabaseResource> _databaseResources = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="MariaDbServerResource"/> class.
    /// </summary>
    /// <param name="name">The name of the resource.</param>
    /// <param name="password">A parameter that contains the MariaDB server password.</param>
    public MariaDbServerResource(string name, ParameterResource password) : base(name)
    {
        ArgumentNullException.ThrowIfNull(password);

        PrimaryEndpoint = new(this, PrimaryEndpointName);
        PasswordParameter = password;
    }

    /// <summary>
    /// Gets the primary endpoint for the MariaDB server.
    /// </summary>
    public EndpointReference PrimaryEndpoint { get; }

    /// <summary>
    /// Gets the host endpoint reference for this resource.
    /// </summary>
    public EndpointReferenceExpression Host => PrimaryEndpoint.Property(EndpointProperty.Host);

    /// <summary>
    /// Gets the port endpoint reference for this resource.
    /// </summary>
    public EndpointReferenceExpression Port => PrimaryEndpoint.Property(EndpointProperty.Port);

    /// <summary>
    /// Gets the parameter that contains the MariaDB server password.
    /// </summary>
    public ParameterResource PasswordParameter { get; set; }

    /// <summary>
    /// Gets the connection string expression for the MariaDB server.
    /// </summary>
    public ReferenceExpression ConnectionStringExpression =>
        ReferenceExpression.Create(
            $"Server={PrimaryEndpoint.Property(EndpointProperty.Host)};Port={PrimaryEndpoint.Property(EndpointProperty.Port)};User ID={DefaultUserName};Password={PasswordParameter}");

    private static ReferenceExpression UserNameReference => ReferenceExpression.Create($"{DefaultUserName}");

    /// <summary>
    /// Gets the connection URI expression for the MySQL server.
    /// </summary>
    /// <remarks>
    /// Format: <c>mysql://{user}:{password}@{host}:{port}</c>.
    /// </remarks>
    public ReferenceExpression UriExpression => BuildUri();

    internal ReferenceExpression BuildUri(string? databaseName = null)
    {
        var builder = new ReferenceExpressionBuilder();
        builder.AppendLiteral("mariadb://");
        builder.Append($"{DefaultUserName:uri}:{PasswordParameter:uri}@{Host}:{Port}");

        if (databaseName is not null)
        {
            builder.Append($"/{databaseName:uri}");
        }

        return builder.Build();
    }

    internal ReferenceExpression BuildJdbcConnectionString(string? databaseName = null)
    {
        var builder = new ReferenceExpressionBuilder();
        builder.AppendLiteral("jdbc:mariadb://");
        builder.Append($"{Host}");
        builder.AppendLiteral(":");
        builder.Append($"{Port}");

        if (databaseName is not null)
        {
            builder.AppendLiteral("/");
            builder.Append($"{databaseName:uri}");
        }

        return builder.Build();
    }

    /// <summary>
    /// Gets the JDBC connection string for the MySQL server.
    /// </summary>
    /// <remarks>
    /// <para>Format: <c>jdbc:mysql://{host}:{port}</c>.</para>
    /// <para>User and password credentials are not included in the JDBC connection string. Use the <c>Username</c> and <c>Password</c> connection properties to access credentials.</para>
    /// </remarks>
    public ReferenceExpression JdbcConnectionString => BuildJdbcConnectionString();

    /// <summary>
    /// A dictionary where the key is the resource name and the value is the database name.
    /// </summary>
    public IReadOnlyDictionary<string, string> Databases => _databases;

    internal IReadOnlyList<MariaDbDatabaseResource> DatabaseResources => _databaseResources;

    internal void AddDatabase(MariaDbDatabaseResource database)
    {
        _databases.TryAdd(database.Name, database.DatabaseName);
        _databaseResources.Add(database);
    }

    IEnumerable<KeyValuePair<string, ReferenceExpression>> IResourceWithConnectionString.GetConnectionProperties()
    {
        yield return new("Host", ReferenceExpression.Create($"{Host}"));
        yield return new("Port", ReferenceExpression.Create($"{Port}"));
        yield return new("Username", UserNameReference);
        yield return new("Password", ReferenceExpression.Create($"{PasswordParameter}"));
        yield return new("Uri", UriExpression);
        yield return new("JdbcConnectionString", JdbcConnectionString);
    }
}
