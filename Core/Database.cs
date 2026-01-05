using System;
using System.Data;
using System.Data.Common;

namespace NarayaniInfotec.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class Database : IDatabase
    {
        private readonly DbProviderFactory _factory;
        private readonly DbConfig _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="Database"/> class
        /// using the provided database configuration.
        /// </summary>
        /// <param name="config">
        /// Configuration settings required to establish a database connection,
        /// including the provider name (e.g., Microsoft.Data.SqlClient)
        /// and the connection string.
        /// </param>
        /// <remarks>
        /// This constructor uses <see cref="DbProviderFactories"/> to create
        /// a database-agnostic provider factory, allowing the library to work
        /// with different database engines without code changes.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the provided configuration is null.
        /// </exception>
        public Database(DbConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _factory = DbProviderFactories.GetFactory(config.ProviderName);
        }

        /// <summary>
        /// Creates connection with database.
        /// </summary>
        /// <returns></returns>
        public DbConnection CreateConnection()
        {
            var conn = _factory.CreateConnection();
            conn.ConnectionString = _config.ConnectionString;
            return conn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql">Query statement to be executed</param>
        /// <param name="type">Type of command whether Text or StoreProcedure.</param>
        /// <returns></returns>
        public DbCommand CreateCommand(string sql, CommandType type)
        {
            var cmd = _factory.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = type;
            return cmd;
        }
    }
}
