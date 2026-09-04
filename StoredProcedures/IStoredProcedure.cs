using NarayaniInfotec.DataLite.StoredProcedures;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace NarayaniInfotec.StoredProcedures
{
    /// <summary>
    /// Defines a database-agnostic contract for executing stored procedures.
    /// Mirrors the <see cref="NarayaniInfotec.CRUD.ICrudRepository"/> pattern
    /// so callers can use stored procedures with the same fluency as CRUD operations.
    ///
    /// Supports:
    /// - Strongly typed List&lt;T&gt; results (sync + async)
    /// - DataTable results (sync + async)
    /// - Non-query execution (INSERT/UPDATE/DELETE via SP)
    /// - Scalar execution (single value return)
    /// - Multiple result sets
    /// - Parameter passing via Dictionary or SpParam[] array
    /// </summary>
    public interface IStoredProcedure
    {
        // ─────────────────────────────────────────────────────────────
        // LIST<T> — strongly typed, high-performance (DbDataReader)
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Executes a stored procedure and maps the first result set to a strongly typed list.
        /// </summary>
        /// <typeparam name="T">Target type. Must have a parameterless constructor.
        /// Property names are matched to column names (case-insensitive).
        /// Use <c>[Column("db_col_name")]</c> attribute when names differ.</typeparam>
        /// <param name="procedureName">Name of the stored procedure.</param>
        /// <param name="parameters">SP parameters as key-value pairs (without the '@' prefix).
        /// Pass <c>null</c> or empty dictionary for procedures with no parameters.</param>
        /// <returns>A <see cref="List{T}"/> of mapped objects. Returns an empty list when no rows match.</returns>
        List<T> ExecuteList<T>(string procedureName,
                               Dictionary<string, object>? parameters = null)
            where T : new();

        /// <summary>
        /// Executes a stored procedure using <see cref="SpParam"/> parameters array and maps the first result set to a strongly typed list.
        /// </summary>
        List<T> ExecuteList<T>(string procedureName,
                               params SpParam[] parameters)
            where T : new();

        /// <summary>
        /// Asynchronously executes a stored procedure and maps the first result set to a strongly typed list.
        /// </summary>
        Task<List<T>> ExecuteListAsync<T>(string procedureName,
                                          Dictionary<string, object>? parameters = null)
            where T : new();

        /// <summary>
        /// Asynchronously executes a stored procedure using <see cref="SpParam"/> parameters array and maps the first result set to a strongly typed list.
        /// </summary>
        Task<List<T>> ExecuteListAsync<T>(string procedureName,
                                          params SpParam[] parameters)
            where T : new();

        // ─────────────────────────────────────────────────────────────
        // DATATABLE — raw, untyped result set
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Executes a stored procedure and returns the first result set as a <see cref="DataTable"/>.
        /// Useful when the schema is not known at compile time.
        /// </summary>
        DataTable Execute(string procedureName,
                          Dictionary<string, object>? parameters = null);

        /// <summary>
        /// Executes a stored procedure using <see cref="SpParam"/> parameters array and returns the first result set as a <see cref="DataTable"/>.
        /// </summary>
        DataTable Execute(string procedureName,
                          params SpParam[] parameters);

        /// <summary>
        /// Asynchronously executes a stored procedure and returns the first result set as a <see cref="DataTable"/>.
        /// </summary>
        Task<DataTable> ExecuteAsync(string procedureName,
                                     Dictionary<string, object>? parameters = null);

        /// <summary>
        /// Asynchronously executes a stored procedure using <see cref="SpParam"/> parameters array and returns the first result set as a <see cref="DataTable"/>.
        /// </summary>
        Task<DataTable> ExecuteAsync(string procedureName,
                                     params SpParam[] parameters);

        // ─────────────────────────────────────────────────────────────
        // NON-QUERY — INSERT / UPDATE / DELETE via SP
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Executes a stored procedure that performs a data modification (INSERT, UPDATE, DELETE)
        /// and returns the number of rows affected.
        /// </summary>
        int ExecuteNonQuery(string procedureName,
                            Dictionary<string, object>? parameters = null);

        /// <summary>
        /// Executes a stored procedure using <see cref="SpParam"/> parameters array that performs a data modification and returns rows affected.
        /// </summary>
        int ExecuteNonQuery(string procedureName,
                            params SpParam[] parameters);

        /// <summary>
        /// Asynchronously executes a stored procedure that performs a data modification
        /// and returns the number of rows affected.
        /// </summary>
        Task<int> ExecuteNonQueryAsync(string procedureName,
                                       Dictionary<string, object>? parameters = null);

        /// <summary>
        /// Asynchronously executes a stored procedure using <see cref="SpParam"/> parameters array that performs a data modification and returns rows affected.
        /// </summary>
        Task<int> ExecuteNonQueryAsync(string procedureName,
                                       params SpParam[] parameters);

        // ─────────────────────────────────────────────────────────────
        // SCALAR — single value return
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Executes a stored procedure and returns the first column of the first row.
        /// All other columns and rows are ignored.
        /// Converts the result to <typeparamref name="TResult"/>.
        /// </summary>
        TResult ExecuteScalar<TResult>(string procedureName,
                                       Dictionary<string, object>? parameters = null);

        /// <summary>
        /// Executes a stored procedure using <see cref="SpParam"/> parameters array and returns the first column of the first row.
        /// </summary>
        TResult ExecuteScalar<TResult>(string procedureName,
                                       params SpParam[] parameters);

        /// <summary>
        /// Asynchronously executes a stored procedure and returns the first column of the first row.
        /// </summary>
        Task<TResult> ExecuteScalarAsync<TResult>(string procedureName,
                                                   Dictionary<string, object>? parameters = null);

        /// <summary>
        /// Asynchronously executes a stored procedure using <see cref="SpParam"/> parameters array and returns the first column of the first row.
        /// </summary>
        Task<TResult> ExecuteScalarAsync<TResult>(string procedureName,
                                                   params SpParam[] parameters);

        // ─────────────────────────────────────────────────────────────
        // MULTIPLE RESULT SETS
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Executes a stored procedure that returns multiple result sets.
        /// Each result set is mapped to its own <see cref="DataTable"/>.
        /// </summary>
        /// <returns>A <see cref="DataSet"/> where each <see cref="DataTable"/> corresponds
        /// to one result set returned by the stored procedure, in order.</returns>
        DataSet ExecuteMultiple(string procedureName,
                                Dictionary<string, object>? parameters = null);

        /// <summary>
        /// Executes a stored procedure using <see cref="SpParam"/> parameters array that returns multiple result sets.
        /// </summary>
        DataSet ExecuteMultiple(string procedureName,
                                params SpParam[] parameters);

        /// <summary>
        /// Executes a stored procedure that returns exactly two result sets,
        /// mapping each to its own strongly typed list.
        /// </summary>
        /// <typeparam name="T1">Type for the first result set.</typeparam>
        /// <typeparam name="T2">Type for the second result set.</typeparam>
        /// <returns>A tuple of two lists — one per result set.</returns>
        (List<T1> First, List<T2> Second) ExecuteMultiple<T1, T2>(
            string procedureName,
            Dictionary<string, object>? parameters = null)
            where T1 : new()
            where T2 : new();

        /// <summary>
        /// Executes a stored procedure using <see cref="SpParam"/> parameters array that returns exactly two result sets.
        /// </summary>
        (List<T1> First, List<T2> Second) ExecuteMultiple<T1, T2>(
            string procedureName,
            params SpParam[] parameters)
            where T1 : new()
            where T2 : new();
    }
}