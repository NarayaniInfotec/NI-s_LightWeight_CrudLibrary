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
    /// </remarks>
    public interface IDatabase
    {
        /// <summary>
        /// Creates and returns a new database connection.
        /// </summary>
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
