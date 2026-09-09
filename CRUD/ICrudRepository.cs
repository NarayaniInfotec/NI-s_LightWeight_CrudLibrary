using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace NarayaniInfotec.DataLite.CRUD
{

    /// <summary>
    /// DataLite is a lightweight, database-agnostic CRUD utility library.
    /// It provides simple Insert, Update, Delete, and Select operations
    /// without requiring ORM frameworks or database-specific code.
    /// 
    /// Key features:
    /// - Database-agnostic design
    /// - Safe parameterized queries
    /// - Optional logging support
    /// - Suitable for Web, Desktop, API, and Console applications
    /// 
    /// Consumers supply table names, column-value dictionaries,
    /// and optional WHERE clauses as parameters.
    /// Defines generic CRUD (Create, Read, Update, Delete) operations
    /// that are independent of any specific database or schema.
    /// </summary>
    
    public interface ICrudRepository
    {
        /// <summary>
        /// Executes a non-query SQL statement (INSERT, UPDATE, DELETE) and returns the number of affected rows.
        /// </summary>
        /// <param name="sql">The SQL statement to execute.</param>
        /// <param name="parameters">Dictionary of parameter names and values.</param>
        /// <returns>The number of rows affected by the statement.</returns>
        int Execute(string sql, Dictionary<string, object> parameters);
        
        /// <summary>
        /// Asynchronously executes a non-query SQL statement and returns the number of affected rows.
        /// </summary>
        /// <param name="sql">The SQL statement to execute.</param>
        /// <param name="parameters">Dictionary of parameter names and values.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the number of rows affected.</returns>
        Task<int> ExecuteAsync(string sql, Dictionary<string, object> parameters);

        /// <summary>
        /// Executes a SQL query and returns the result set as a DataTable.
        /// </summary>
        /// <param name="sql">The SQL query to execute.</param>
        /// <param name="parameters">Dictionary of parameter names and values.</param>
        /// <returns>A DataTable containing the query results.</returns>
        DataTable Query(string sql, Dictionary<string, object> parameters);
       
        /// <summary>
        /// Asynchronously executes a SQL query and returns the result set as a DataTable.
        /// </summary>
        /// <param name="sql">The SQL query to execute.</param>
        /// <param name="parameters">Dictionary of parameter names and values.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is a DataTable containing the query results.</returns>
        Task<DataTable> QueryAsync(string sql, Dictionary<string, object> parameters);

        //new definitions. - 21st Dec. 2025 2026
        /// <summary>
        /// Inserts a new record into the specified table.
        /// </summary>
        /// <param name="tableName">Name of the database table.
        /// This should come from a trusted source (e.g., constant or DbTables class),
        /// not from user input.
        /// </param>
        /// <param name="fields">
        /// Name of the database table.
        /// This should come from a trusted source (e.g., constant or DbTables class),
        /// not from user input.
        /// </param>
        /// <returns>Returns number of records inserted.</returns>
        int Insert(string tableName, IDictionary<string, object> fields);

        /// <summary>
        /// Updates existing records in the specified table.
        /// </summary>
        /// <param name="tableName">Target table name.</param>
        /// <param name="fields">
        /// Columns and values to update.
        /// Only the provided fields will be updated.
        /// </param>
        /// <param name="whereClause">
        /// SQL WHERE condition without the 'WHERE' keyword.
        /// Example: "Id = @Id".</param>
        /// <param name="whereParams">Parameters required by the WHERE clause.</param>
        /// <returns></returns>
        int Update(string tableName, IDictionary<string, object> fields, string whereClause, IDictionary<string, object> whereParams);

        /// <summary>
        /// Deletes records from the specified table.
        /// </summary>
        /// <param name="tableName">Target table name.</param>
        /// <param name="whereClause">SQL WHERE condition without the 'WHERE' keyword.</param>
        /// <param name="whereParams">Parameters required by the WHERE clause.</param>
        /// <returns></returns>
        int Delete(string tableName, string whereClause, IDictionary<string, object> whereParams);

        /// <summary>
        ///  Retrieves data from the specified table.
        /// </summary>
        /// <param name="tableName">Target table name.</param>
        /// <param name="columns">Comma-separated list of columns to select.
        /// Use "*" to select all columns.</param>
        /// <param name="whereClause">Optional SQL WHERE condition without the 'WHERE' keyword.</param>
        /// <param name="whereParams">Optional parameters for the WHERE clause.</param>
        /// <returns></returns>
        DataTable Select(string tableName, string columns = "*", string? whereClause = null, IDictionary<string, object>? whereParams = null);

        /// <summary>
        /// Executes the query and returns the first column of the first row in the result set returned by the query. All other columns and the rows are ignored.
        /// </summary>
        /// <param name="sql">A SQL statement.</param>
        /// <param name="parameters">Parameters collection in Dictionary object.</param>
        /// <returns>Returns the first column of the first row in the result set.</returns>
        int ExecuteScalar(string sql, Dictionary<string, object> parameters);   //Added. - 01st Jan. 2026 0520

        /// <summary>
        /// An ansyncronous version of DBCommnd.ExecuteScalar() which executes a SQL statement against a connection object.
        /// Invokes DBCommnd.ExecuteScalarAsync() with CancellationToken.None.
        /// </summary>
        /// <param name="sql">A SQL statement</param>
        /// <param name="parameters">Parameters collection in Dictionary object</param>
        /// <returns>A task representing the asyncronous operation</returns>
        Task<int> ExecuteScalarAsync(string sql, Dictionary<string, object> parameters); //Added. - 01st Jan. 2026 0530

        /// <summary>
        /// Inserts a new record into the specified table and returns the generated identity value.
        /// </summary>
        /// <param name="tableName">Target table name.</param>
        /// <param name="values">Columns and values to insert.</param>
        /// <param name="identityColumn">Name of the identity/auto-increment column to return (default: "Id").</param>
        /// <returns>The generated identity value as an integer.</returns>
        int InsertAndReturnId(string tableName, IDictionary<string, object> values, string identityColumn = "Id");   //Added. - 01st Jan. 2026 0548

        // Add these methods for return List<T> and Task<List<T>> for SelectList, SelectListAsync and QueryList -11 July 2026
        /// <summary>
        /// Selects records from the specified table and maps each row to an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type to map each data row to. Must have a parameterless constructor.</typeparam>
        /// <param name="tableName">Target table name.</param>
        /// <param name="columns">Comma-separated list of columns to select. Use "*" to select all columns.</param>
        /// <param name="whereClause">Optional SQL WHERE condition without the 'WHERE' keyword.</param>
        /// <param name="whereParams">Optional parameters for the WHERE clause.</param>
        /// <returns>List of mapped objects of type <typeparamref name="T"/>.</returns>
        List<T> SelectList<T>(string tableName, string columns = "*", string? whereClause = null,
                             IDictionary<string, object>? whereParams = null) where T : new();

        /// <summary>
        /// Asynchronously selects records from the specified table and maps each row to an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type to map each data row to. Must have a parameterless constructor.</typeparam>
        /// <param name="tableName">Target table name.</param>
        /// <param name="columns">Comma-separated list of columns to select. Use "*" to select all columns.</param>
        /// <param name="whereClause">Optional SQL WHERE condition without the 'WHERE' keyword.</param>
        /// <param name="whereParams">Optional parameters for the WHERE clause.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is a list of mapped objects of type <typeparamref name="T"/>.</returns>
        Task<List<T>> SelectListAsync<T>(string tableName, string columns = "*", string? whereClause = null,
                                        IDictionary<string, object>? whereParams = null) where T : new();

        /// <summary>
        /// Executes a SQL query and maps each result row to an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type to map each data row to. Must have a parameterless constructor.</typeparam>
        /// <param name="sql">The SQL query to execute.</param>
        /// <param name="parameters">Dictionary of parameter names and values.</param>
        /// <returns>List of mapped objects of type <typeparamref name="T"/>.</returns>
        List<T> QueryList<T>(string sql, Dictionary<string, object> parameters) where T : new();

        /// <summary>
        /// Asynchronously executes a SQL query and maps each result row to an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type to map each data row to. Must have a parameterless constructor.</typeparam>
        /// <param name="sql">The SQL query to execute.</param>
        /// <param name="parameters">Dictionary of parameter names and values.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is a list of mapped objects of type <typeparamref name="T"/>.</returns>
        Task<List<T>> QueryListAsync<T>(string sql, Dictionary<string, object> parameters) where T : new();
    }
}
