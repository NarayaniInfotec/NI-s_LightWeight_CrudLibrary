using NarayaniInfotec.CRUD;
using NarayaniInfotec.StoredProcedures;
using System;
using System.Data;
using System.Data.Common;

namespace NarayaniInfotec.Core
{
    /// <summary>
    /// Defines a database abstraction used by the DataLite library
    /// to create database connections and commands in a provider-agnostic manner.
    /// </summary>
    /// <remarks>
    /// Implementations of this interface are responsible for:
    /// <list type="bullet">
    /// <item>Creating database connections using a configured provider</item>
    /// <item>Creating parameterized commands</item>
    /// <item>Abstracting provider-specific logic</item>
    /// </list>
    /// This allows DataLite to work with different database engines
    /// without changing the core CRUD logic.
    /// builder.Services.AddScoped&lt;IStoredProcedure, StoredProcedure&gt;();
    /// </code>
    /// Because <c>IDatabase</c> is Scoped, both <c>CrudRepository</c> and
    /// <c>StoredProcedure</c> receive the <em>same</em> <c>IDatabase</c> instance
    /// within a request, and therefore share the same physical connection.
    /// </remarks>
    public interface IDatabase : IDisposable
    {
        // ----------------------------------------------------------------
        // Shared connection — opened once per DI scope, reused by all ops
        // ----------------------------------------------------------------

        /// <summary>
        /// A lazily-opened connection that is shared across all operations
        /// within the current DI scope (e.g., a single HTTP request).
        ///
        /// The connection is opened on first access and disposed when the
        /// <see cref="IDatabase"/> instance itself is disposed by the DI container.
        /// </summary>
        DbConnection SharedConnection { get; }

        // ----------------------------------------------------------------
        // Factory methods — kept for backward compatibility and for code
        // that deliberately wants an isolated, short-lived connection
        // (e.g. DbTransactionScope, bulk operations, long-running scripts).
        // ----------------------------------------------------------------
        /// <returns>
        /// An unopened <see cref="DbConnection"/> instance configured
        /// with the appropriate connection string.
        /// </returns>
        DbConnection CreateConnection();

        /// <summary>
        /// Creates a database command with the specified SQL and command type.
        /// </summary>
        /// <param name="sql">
        /// The SQL statement or stored procedure name to execute.
        /// </param>
        /// <param name="type">
        /// The type of command to execute
        /// (e.g., Text or StoredProcedure).
        /// </param>
        /// <returns>
        /// A configured <see cref="DbCommand"/> instance ready for
        /// parameter assignment and execution.
        /// </returns>
        DbCommand CreateCommand(string sql, CommandType type);
    }
}
