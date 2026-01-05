using System.Data.Common;

namespace NarayaniInfotec.Core
{
    /// <summary>
    /// Represents the configuration required to connect to a database.
    /// </summary>
    /// /// <remarks>
    /// This class is used by the DataLite library to create
    /// database connections in a provider-agnostic manner.
    /// Consumers must supply a valid provider name and
    /// connection string.
    /// </remarks>
    public class DbConfig
    {
        /// <summary>
        /// Gets or sets the database provider name.
        /// </summary>
        /// <remarks>
        /// This value is used by <see cref="DbProviderFactories"/> to resolve
        /// the appropriate database provider.
        /// <para>Example values:</para>
        /// <list type="bullet">
        /// <item>Microsoft.Data.SqlClient</item>
        /// <item>System.Data.SqlClient</item>
        /// <item>Npgsql</item>
        /// <item>Oracle.ManagedDataAccess.Client</item>
        /// </list>
        /// </remarks>
        public string? ProviderName { get; set; }

        /// <summary>
        /// Gets or sets the database connection string.
        /// </summary>
        /// <remarks>
        /// The connection string contains the necessary information
        /// to establish a connection with the database, such as
        /// server address, database name, credentials, and security options.
        /// </remarks>
        public string? ConnectionString { get; set; }
    }
}
