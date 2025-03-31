using Aspire.Hosting.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Rxcommunica.Aspire.Hosting.MariaDb
{
    public class MariaDbDatabaseResource : Resource, IResourceWithParent<MariaDbServerResource>, IResourceWithConnectionString
    {
        public MariaDbDatabaseResource(string name, string databaseName, MariaDbServerResource parent) : base(name)
        {
            _parent = parent;
            _databaseName = databaseName;
        }

        private readonly MariaDbServerResource _parent;
        private readonly string _databaseName = string.Empty;

        /// <summary>
        /// Gets the parent MySQL container resource.
        /// </summary>
        public MariaDbServerResource Parent
        {
            get
            {
                return _parent ?? throw new ArgumentNullException(nameof(_parent));
            }
        }

        /// <summary>
        /// Gets the connection string expression for the MySQL database.
        /// </summary>
        public ReferenceExpression ConnectionStringExpression =>
           ReferenceExpression.Create($"{Parent};Database={DatabaseName}");

        /// <summary>
        /// Gets the database name.
        /// </summary>
        public string DatabaseName
        {
            get
            {
                return ThrowIfNullOrEmpty(_databaseName);
            }
        }

        private static string ThrowIfNullOrEmpty([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(argument, paramName);
            return argument;
        }
    }
}
