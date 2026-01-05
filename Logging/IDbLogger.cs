using System.Collections.Generic;

namespace NarayaniInfotec.Logging
{
    /// <summary>
    /// Defines a lightweight logging abstraction for database operations
    /// executed by the DataLite library.
    /// </summary>
    /// <remarks>
    /// This interface allows consumers to plug in custom logging logic
    /// without taking a dependency on a specific logging framework.
    /// Typical use cases include:
    /// <list type="bullet">
    /// <item>Tracing executed SQL statements</item>
    /// <item>Debugging parameter values</item>
    /// <item>Auditing database operations</item>
    /// </list>
    /// Implementations should ensure that sensitive data
    /// is handled or masked appropriately.
    /// </remarks>
    public interface IDbLogger
    {
        /// <summary>
        /// Logs a database command along with its parameters.
        /// </summary>
        /// <param name="sql">
        /// The SQL statement or stored procedure name that is being executed.
        /// </param>
        /// <param name="parameters">
        /// A dictionary containing the parameter names and values
        /// associated with the database command.
        /// </param>
        void Log(string sql, Dictionary<string, object> parameters);
    }

}
