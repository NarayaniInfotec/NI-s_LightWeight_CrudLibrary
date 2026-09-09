using NarayaniInfotec.Core;
using NarayaniInfotec.DataLite.Core;
using System;
using System.Data;
using System.Data.Common;

namespace NarayaniInfotec.DataLite.Core
{
    /// <summary>
    /// The <see cref="Database"/> class provides a database-agnostic implementation of <see cref="IDatabase"/>,
    /// </summary>
    public class Database : IDatabase
    {
        private readonly DbProviderFactory _factory;
        private readonly DbConfig _config;

        // Lazy-initialized shared connection
        private DbConnection? _sharedConnection;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of <see cref="Database"/>.
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
        public Database(DbConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _factory = DbProviderFactories.GetFactory(config.ProviderName
                ?? throw new ArgumentException("DbConfig.ProviderName must not be null."));
        }

        // ----------------------------------------------------------------
        // IDatabase.SharedConnection
        // ----------------------------------------------------------------

        /// <inheritdoc/>
        /// <remarks>
        /// Thread-safety note: a single <see cref="Database"/> instance is
        /// consumed by one HTTP request at a time (Scoped lifetime), so no
        /// locking is needed here.
        /// </remarks>
        public DbConnection SharedConnection
        {
            get
            {
                if (_sharedConnection == null)
                {
                    _sharedConnection = _factory.CreateConnection()!;
                    _sharedConnection.ConnectionString = _config.ConnectionString;
                    _sharedConnection.Open();          // opened once per scope
                }

                // Re-open if the server closed the connection (e.g. idle timeout)
                if (_sharedConnection.State == ConnectionState.Closed ||
                    _sharedConnection.State == ConnectionState.Broken)
                {
                    _sharedConnection.Open();
                }

                return _sharedConnection;
            }
        }

        // ----------------------------------------------------------------
        // IDatabase factory methods (backward-compatible)
        // ----------------------------------------------------------------

        /// <inheritdoc/>
        public DbConnection CreateConnection()
        {
            var conn = _factory.CreateConnection()!;
            conn.ConnectionString = _config.ConnectionString;
            return conn;           // caller must Open() and Dispose()
        }

        /// <inheritdoc/>
        public DbCommand CreateCommand(string sql, CommandType type)
        {
            var cmd = _factory.CreateCommand()!;
            cmd.CommandText = sql;
            cmd.CommandType = type;
            return cmd;
        }

        // ----------------------------------------------------------------
        // ----------------------------------------------------------------

        /// <summary>
        /// Closes and disposes the shared connection.
        /// Called automatically by the DI container at the end of the scope.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (_sharedConnection != null)
            {
                if (_sharedConnection.State != ConnectionState.Closed)
                    _sharedConnection.Close();

                _sharedConnection.Dispose();
                _sharedConnection = null;
            }

            GC.SuppressFinalize(this);
        }
    }
}
