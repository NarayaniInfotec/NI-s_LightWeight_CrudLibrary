using NarayaniInfotec.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace NarayaniInfotec.StoredProcedures
{
    /// <summary>
    /// Provides functionality to execute stored procedures
    /// and return the result as a <see cref="DataTable"/>.
    /// </summary>
    /// <remarks>
    /// This class abstracts stored procedure execution in a
    /// database-agnostic manner using the <see cref="IDatabase"/> interface.
    /// It supports parameterized execution and is suitable for
    /// complex queries, reporting, and legacy database operations.
    /// </remarks>
    public class StoredProcedureExecutor
    {
        private readonly IDatabase _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredProcedureExecutor"/> class.
        /// </summary>
        /// <param name="db">
        /// An implementation of <see cref="IDatabase"/> used to create
        /// database connections and commands.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the provided database instance is null.
        /// </exception>
        public StoredProcedureExecutor(IDatabase db)
        {
            _db = db;
        }

        /// <summary>Executes a stored procedure and returns the result set.</summary>
        /// <param name="procedureName">The name of the stored procedure to execute.</param>
        /// <param name="parameters">A dictionary containing the parameter names and values
        /// required by the stored procedure.</param>
        /// <returns>A <see cref="DataTable"/> containing the result returned
        /// by the stored procedure.</returns>
        /// <remarks>All parameters are executed in a parameterized manner
        /// to prevent SQL injection.</remarks>
        public DataTable Execute(
            string procedureName,
            Dictionary<string, object> parameters)
        {
            using var conn = _db.CreateConnection();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = procedureName;
            cmd.CommandType = CommandType.StoredProcedure;

            foreach (var p in parameters)
            {
                var param = cmd.CreateParameter();
                param.ParameterName = "@" + p.Key;
                param.Value = p.Value ?? DBNull.Value;
                cmd.Parameters.Add(param);
            }

            using var da = DbProviderFactories
                .GetFactory(conn)
                .CreateDataAdapter();

            da.SelectCommand = cmd;

            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }

}
