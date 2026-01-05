using NarayaniInfotec.Core;
using NarayaniInfotec.DataLite.Core;
using NarayaniInfotec.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace NarayaniInfotec.CRUD
{
    /// <summary>
    /// Default implementation of <see cref="ICrudRepository"/>
    /// Executes database-agnostic CRUD operations using
    /// an <see cref="IDatabase"/> abstraction.
    /// </summary>
          
    
    public class CrudRepository : ICrudRepository
    {   
        private readonly IDatabase _db;
        private readonly IDbLogger? _logger;
        
        /// <summary>
        /// Initializes a new instance of the CrudRepository.
        /// <param name="db">
        /// Database provider implementation responsible for
        /// creating connections and commands.
        /// </param>
        /// <param name="logger">
        /// Optional logger for tracing queries and errors.
        /// </param>
        /// </summary>
      
        public CrudRepository(IDatabase db, IDbLogger? logger = null)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// Executes an SQL statement against a connection object.
        /// </summary>
        /// <param name="sql">A SQL statement.</param>
        /// <param name="parameters">Parameters collection in Dictionary object.</param>
        /// <returns>The number of rows affected.</returns>
        public int Execute(string sql, Dictionary<string, object> parameters)
        {

            try
            { //Added Try..Catch block. - 01st Jan. 2026 0732
                _logger?.Log(sql, parameters);

                using var conn = _db.CreateConnection();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = sql;

                AddParams(cmd, parameters);

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new DataLiteException($"Execute failed on '{sql}'.", ex);
            }
        }

        /// <summary>
        /// Executes the query and returns the first column of the first row in the result set returned by the query. All other columns and the rows are ignored.
        /// </summary>
        /// <param name="sql">A SQL statement.</param>
        /// <param name="parameters">Parameters collection in Dictionary object.</param>
        /// <returns>Returns the first column of the first row in the result set.</returns>
        public int ExecuteScalar(string sql, Dictionary<string, object> parameters)
        {   //Newly added. - 01st Jan. 2026

            try
            { //Added Try..Catch block. - 01st Jan. 2026 0732
                _logger?.Log(sql, parameters);

                using var conn = _db.CreateConnection();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = sql;

                AddParams(cmd, parameters);

                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw new DataLiteException($"Execute Scalar failed on '{sql}'.", ex);
            }
        }

        /// <summary>
        /// An ansyncronous version of DBCommnd.ExecuteScalar() which executes a SQL statement against a connection object.
        /// Invokes DBCommnd.ExecuteScalarAsync() with CancellationToken.None.
        /// </summary>
        /// <param name="sql">A SQL statement</param>
        /// <param name="parameters">Parameters collection in Dictionary object</param>
        /// <returns>A task representing the asyncronous operation</returns>
        public async Task<int> ExecuteScalarAsync(string sql, Dictionary<string, object> parameters)
        {   //Newly added. - 01st Jan. 2026

            try
            { //Added Try..Catch block. - 01st Jan. 2026 0732
                _logger?.Log(sql, parameters);

                // Ensure _db.CreateConnection() returns a DbConnection 
                // that supports async operations (like SqlConnection)
                using var conn = _db.CreateConnection();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = sql;

                AddParams(cmd, parameters);

                // Use OpenAsync to prevent blocking the thread
                await conn.OpenAsync();

                // ExecuteScalarAsync returns an 'object?'. 
                // We safely convert it to int, handling potential nulls.
                var result = await cmd.ExecuteScalarAsync();

                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                throw new DataLiteException($"Execute Scalar Async failed on '{sql}'.", ex);
            }
        }
      
        /// <summary>
        /// An ansyncronous version of DBCommnd.ExecuteNonQuery() which executes a SQL statement against a connection object.
        /// Invokes DBCommnd.ExecuteNonQueryAsync() with CancellationToken.None. 
        /// </summary>
        /// <param name="sql">A SQL statement</param>
        /// <param name="parameters">Parameters collection in Dictionary object</param>
        /// <returns>A task representing the asyncronous operation</returns>
        public async Task<int> ExecuteAsync(string sql, Dictionary<string, object> parameters)
        {
            try
            { //Added Try..Catch block. - 01st Jan. 2026 0732
                _logger?.Log(sql, parameters);

            using var conn = _db.CreateConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            AddParams(cmd, parameters);

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new DataLiteException($"Execute Async failed on '{sql}'.", ex);
            }
        }

        /// <summary>
        /// Executes provided query with parameters.
        /// </summary>
        /// <param name="sql">A SQL statement to be executed.</param>
        /// <param name="parameters">Collection of parameters in Dictionary object.</param>
        /// <returns>Returns datatable as a result.</returns>
        public DataTable Query(string sql, Dictionary<string, object> parameters)
        {
            try
            { //Added Try..Catch block. - 01st Jan. 2026 0732
                _logger?.Log(sql, parameters);

            using var conn = _db.CreateConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            AddParams(cmd, parameters);

            using var da = DbProviderFactories
                .GetFactory(conn)
                .CreateDataAdapter();

            da.SelectCommand = cmd;

            var dt = new DataTable();
            da.Fill(dt);
            return dt;
            }
            catch (Exception ex)
            {
                throw new DataLiteException($"Query failed. '{sql}'.", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [Obsolete]
        public Task<DataTable> QueryAsync(string sql, Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds new parameters in given DBCommand.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="parameters"></param>
        private void AddParams(DbCommand cmd, Dictionary<string, object> parameters)
        {
            try
            { //Added Try..Catch block. - 01st Jan. 2026 0732
                if (parameters == null) return;

            foreach (var p in parameters)
            {
                var param = cmd.CreateParameter();
                param.ParameterName = "@" + p.Key;
                param.Value = p.Value ?? DBNull.Value;
                cmd.Parameters.Add(param);
            }
            }
            catch (Exception ex)
            {
                throw new DataLiteException($"Add Parameter failed on '{cmd}'.", ex);
            }
        }

        /// <summary>
        /// Inserts a record into the specified table using
        /// parameterized SQL to prevent injection attacks.
        /// </summary>
        /// <param name="tableName">Target table name.</param>
        /// <param name="fields">Dictionary of column names and values.
        /// Null values are converted to DBNull.</param>
        /// <returns></returns>
        //New methods/ functions. - 21st Dec. 2025 2007
        public int Insert(string tableName, IDictionary<string, object> fields)
        {
            try
            { //Added Try..Catch block. - 01st Jan. 2026 0732
                var columns = string.Join(",", fields.Keys);
                var parameters = string.Join(",", fields.Keys.Select(k => "@" + k));

                string sql = $"INSERT INTO {tableName} ({columns}) VALUES ({parameters})";

                return Execute(sql, fields);
            }
            catch (Exception ex)
            {
                throw new DataLiteException($"Insert failed on '{tableName}'.", ex);
            }
        }

        /// <summary>
        /// Updates records in the specified table.
        /// </summary>
        /// <remarks>
        /// Only columns provided in the fields dictionary
        /// will be updated.
        /// </remarks>
        /// <param name="tableName">Target table name.</param>
        /// <param name="fields"></param>
        /// <param name="whereClause"></param>
        /// <param name="whereParams"></param>
        /// <returns></returns>
        public int Update(string tableName, IDictionary<string, object> fields, string whereClause,
                            IDictionary<string, object> whereParams)
        {
            try
            { //Added Try..Catch block. - 01st Jan. 2026 0732
                var setClause =
                string.Join(",", fields.Keys.Select(k => $"{k}=@{k}"));

                string sql = $"UPDATE {tableName} SET {setClause} WHERE {whereClause}";

                var allParams = fields
                    .Concat(whereParams)
                    .ToDictionary(k => k.Key, v => v.Value);

                return Execute(sql, allParams);
            }
            catch (Exception ex)
            {
                throw new DataLiteException($"Update failed on '{tableName}'.", ex);
            }
        }

        /// <summary>
        /// Deletes records from the specified table based on the provided WHERE condition.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="whereClause"></param>
        /// <param name="whereParams"></param>
        /// <returns></returns>
        public int Delete(string tableName, string whereClause, IDictionary<string, object> whereParams)
        {
            try
            { //Added Try..Catch block. - 01st Jan. 2026 0732
                string sql =
                    $"DELETE FROM {tableName} WHERE {whereClause}";

                return Execute(sql, whereParams);
            }
            catch (Exception ex)
            {
                throw new DataLiteException($"Delete failed on '{tableName}'.", ex);
            }
}

        /// <summary>
        /// Executes a SELECT query and returns the result as a DataTable.
        /// </summary>
        /// <remarks>
        /// This method supports optional filtering via WHERE clause and parameterized inputs.
        /// </remarks>
        /// <param name="tableName">Target table name.</param>
        /// <param name="columns">Comma-separated list of columns to select.
        /// Use "*" to select all columns.</param>
        /// <param name="whereClause">Optional SQL WHERE condition without the 'WHERE' keyword.</param>
        /// <param name="whereParams">Optional parameters for the WHERE clause.</param>
        /// <returns></returns>
        public DataTable Select(string tableName, string columns = "*", string? whereClause = null,
                        IDictionary<string, object>? whereParams = null)
        {
            try { 
                string sql = $"SELECT {columns} FROM {tableName}";

                if (!string.IsNullOrWhiteSpace(whereClause))
                    sql += " WHERE " + whereClause;

                return Query(sql, whereParams);
            }
            catch (Exception ex)
            {
                throw new DataLiteException($"Select failed for '{tableName}'.", ex);
            }
}

        // --------------------
        // INTERNAL CORE METHODS
        // --------------------

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private int Execute(string sql, IDictionary<string, object> parameters)
        {
            using var conn = _db.CreateConnection();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = sql;
            AddParameters(cmd, parameters);

            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private DataTable Query(string sql, IDictionary<string, object>? parameters)
        {
            using var conn = _db.CreateConnection();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = sql;
            AddParameters(cmd, parameters);

            using var adapter =
                DbProviderFactories.GetFactory(conn).CreateDataAdapter();

            adapter.SelectCommand = cmd;

            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="parameters"></param>
        private static void AddParameters(DbCommand cmd, IDictionary<string, object>? parameters)
        {
            if (parameters == null) return;

            foreach (var p in parameters)
            {
                var param = cmd.CreateParameter();
                param.ParameterName = "@" + p.Key;
                param.Value = p.Value ?? DBNull.Value;
                cmd.Parameters.Add(param);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private DbParameter CreateParameter(DbCommand cmd, string name, object value)
        {

            try
            { //Added Try..Catch block. - 01st Jan. 2026 0732
                var param = cmd.CreateParameter();
            param.ParameterName = "@" + name;
            param.Value = value ?? DBNull.Value;
            return param;
            }
            catch (Exception ex)
            {
                throw new DataLiteException($"Create Parameter failed on parameter '{name}', Value '{value}'.", ex);
            }
        }

        /// <summary>
        /// Inserts record and returns inserted ID.
        /// </summary>
        /// <param name="tableName">Name of the database table where you want to insert record.</param>
        /// <param name="values">List of values</param>
        /// <param name="identityColumn">ID column which will be returned.</param>
        /// <returns>Returns inserted ID.</returns>
        /// <exception cref="ArgumentException"></exception>
        public int InsertAndReturnId(string tableName, IDictionary<string, object> values, string identityColumn = "Id")
        {
            string sql = "";
            try
            { //Added Try..Catch block. - 01st Jan. 2026 0801
                if (values == null || values.Count == 0)
                     throw new ArgumentException("Insert values cannot be empty");

                var columns = string.Join(", ", values.Keys);
                var parameters = string.Join(", ", values.Keys.Select(k => "@" + k));

                sql = $@"INSERT INTO {tableName} ({columns})
                                VALUES ({parameters});
                                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                using var conn = _db.CreateConnection();
                using var cmd = _db.CreateCommand(sql, CommandType.Text);

                cmd.Connection = conn;

                foreach (var kv in values)
                    cmd.Parameters.Add(CreateParameter(cmd, kv.Key, kv.Value));

                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw new DataLiteException($"Insert And ReturnId failed on table '{tableName}'/n {sql}.", ex);
            }
            
        }
    }

}
