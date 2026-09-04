using NarayaniInfotec.Core;
using NarayaniInfotec.DataLite.Core;
using NarayaniInfotec.DataLite.DataMapper;
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
    /// Default implementation of <see cref="ICrudRepository"/>.
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
        /// <param name="logger">Optional query logger.</param>
        public CrudRepository(IDatabase db, IDbLogger? logger = null)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _logger = logger;
        }

        // ----------------------------------------------------------------
        // Shared-connection helper — used by every operation below
        // ----------------------------------------------------------------

        /// <summary>
        /// Returns a command bound to the shared connection.
        /// The connection is already open; callers must NOT open or dispose it.
        /// </summary>
        private DbCommand CreateSharedCommand(string sql,
                                              CommandType type = CommandType.Text)
        {
            var conn = _db.SharedConnection;   // already open
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = type;
            return cmd;                        // cmd.Connection is set automatically
        }

        // ----------------------------------------------------------------
        // Execute / ExecuteAsync
        // ----------------------------------------------------------------

        /// <inheritdoc/>
        public int Execute(string sql, Dictionary<string, object> parameters)
        {
            try
            {
                _logger?.Log(sql, parameters);

                using var cmd = CreateSharedCommand(sql);
                AddParameters(cmd, parameters);
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new DataLiteException($"Execute failed on '{sql}'.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<int> ExecuteAsync(string sql,
                                            Dictionary<string, object> parameters)
        {
            try
            {
                _logger?.Log(sql, parameters);

                using var cmd = CreateSharedCommand(sql);
                AddParameters(cmd, parameters);
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new DataLiteException($"Execute Async failed on '{sql}'.", ex);
            }
        }

        // ----------------------------------------------------------------
        // ExecuteScalar / ExecuteScalarAsync
        // ----------------------------------------------------------------

        /// <inheritdoc/>
        public int ExecuteScalar(string sql, Dictionary<string, object> parameters)
        {
            try
            {
                _logger?.Log(sql, parameters);

                using var cmd = CreateSharedCommand(sql);
                AddParameters(cmd, parameters);

                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                throw new DataLiteException($"Execute Scalar failed on '{sql}'.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<int> ExecuteScalarAsync(string sql,
                                                   Dictionary<string, object> parameters)
        {
            try
            {
                _logger?.Log(sql, parameters);

                using var cmd = CreateSharedCommand(sql);
                AddParameters(cmd, parameters);

                var result = await cmd.ExecuteScalarAsync();
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                throw new DataLiteException($"Execute Scalar Async failed on '{sql}'.", ex);
            }
        }

        // ----------------------------------------------------------------
        // Query / QueryAsync
        // ----------------------------------------------------------------
        /// </summary>
        /// <param name="sql">A SQL statement to be executed.</param>
        /// <param name="parameters">Collection of parameters in Dictionary object.</param>
        /// <returns>Returns datatable as a result.</returns>
        public DataTable Query(string sql, Dictionary<string, object> parameters)
        {
            try
            {
                _logger?.Log(sql, parameters);

                using var cmd = CreateSharedCommand(sql);
                using var adapter = DbProviderFactories
                                        .GetFactory(_db.SharedConnection)
                                        .CreateDataAdapter()!;

                AddParameters(cmd, parameters);
                adapter.SelectCommand = cmd;

                var dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw new DataLiteException($"Query failed. '{sql}'.", ex);
            }
        }

        /// <inheritdoc/>
        [Obsolete("Use QueryListAsync<T> for async data retrieval.")]
        public Task<DataTable> QueryAsync(string sql,
                                          Dictionary<string, object> parameters)
            => throw new NotImplementedException();

        // ----------------------------------------------------------------
        // Insert / Update / Delete / Select
        // ----------------------------------------------------------------

        /// <param name="fields">Dictionary of column names and values.
        /// Null values are converted to DBNull.</param>
        /// <inheritdoc/>
        public int Insert(string tableName, IDictionary<string, object> fields)
        {
            try
            {
                var columns = string.Join(", ", fields.Keys);
                var paramNames = string.Join(", ", fields.Keys.Select(k => "@" + k));
                var sql = $"INSERT INTO {tableName} ({columns}) VALUES ({paramNames})";

                return ExecuteInternal(sql, fields);
            }
            catch (Exception ex)
            {
                throw new DataLiteException($"Insert failed on '{tableName}'.", ex);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        public int Update(string tableName,
                          IDictionary<string, object> fields,
                          string whereClause,
                          IDictionary<string, object> whereParams)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(whereClause))
                    throw new ArgumentException("WHERE clause is required for UPDATE.");

                var setClause = string.Join(", ",
                    fields.Keys.Select(k => $"{k} = @set_{k}"));

                var sql = $"UPDATE {tableName} SET {setClause} WHERE {whereClause}";

                var setParams = fields.ToDictionary(
                    k => $"@set_{k.Key}",
                    v => v.Value ?? DBNull.Value as object);

                var allParams = setParams
                    .Concat(whereParams)
                    .ToDictionary(k => k.Key, v => v.Value);

                return ExecuteInternal(sql, allParams);
            }
            catch (Exception ex)
            {
                throw new DataLiteException(
                    $"Update failed on '{tableName}'. SQL: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public int Delete(string tableName,
                          string whereClause,
                          IDictionary<string, object> whereParams)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(whereClause))
                    throw new ArgumentException("WHERE clause is required for DELETE.");

                var sql = $"DELETE FROM {tableName} WHERE {whereClause}";
                return ExecuteInternal(sql, whereParams);
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
        public DataTable Select(string tableName,
                                string columns = "*",
                                string? whereClause = null,
                                IDictionary<string, object>? whereParams = null)
        {
            try
            {
                var sql = $"SELECT {columns} FROM {tableName}";
                if (!string.IsNullOrWhiteSpace(whereClause))
                    sql += " WHERE " + whereClause;

                return QueryInternal(sql, whereParams);
            }
            catch (Exception ex)
            {
                throw new DataLiteException($"Select failed for '{tableName}'.", ex);
            }
        }

        // --------------------
        // INTERNAL CORE METHODS
        // --------------------

        // InsertAndReturnId
        // ----------------------------------------------------------------

        /// <inheritdoc/>
        public int InsertAndReturnId(string tableName,
                                     IDictionary<string, object> values,
                                     string identityColumn = "Id")
        {
            try
            {
                if (values == null || values.Count == 0)
                    throw new ArgumentException("Insert values cannot be empty.");

                var columns = string.Join(", ", values.Keys);
                var paramNames = string.Join(", ", values.Keys.Select(k => "@" + k));

                // SCOPE_IDENTITY() is SQL Server specific;
                // for MySQL use LAST_INSERT_ID() instead.
                var sql = $@"INSERT INTO {tableName} ({columns})
                             VALUES ({paramNames});
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";

                return ExecuteScalar(sql,
                    values.ToDictionary(k => k.Key, v => v.Value));
            }
            catch (Exception ex)
            {
                throw new DataLiteException(
                    $"InsertAndReturnId failed on table '{tableName}'.", ex);
            }
        }

        // ----------------------------------------------------------------
        // High-performance generic List<T> methods
        // ----------------------------------------------------------------

        /// <inheritdoc/>
        public List<T> SelectList<T>(string tableName,
                                     string columns = "*",
                                     string? whereClause = null,
                                     IDictionary<string, object>? whereParams = null)
            where T : new()
        {
            var sql = $"SELECT {columns} FROM {tableName}";
            if (!string.IsNullOrWhiteSpace(whereClause))
                sql += " WHERE " + whereClause;

            var parameters = whereParams?.ToDictionary(k => k.Key, v => v.Value)
                             ?? new Dictionary<string, object>();

            return QueryList<T>(sql, parameters);
        }

        /// <inheritdoc/>
        public async Task<List<T>> SelectListAsync<T>(string tableName,
                                                       string columns = "*",
                                                       string? whereClause = null,
                                                       IDictionary<string, object>? whereParams = null)
            where T : new()
        {
            var sql = $"SELECT {columns} FROM {tableName}";
            if (!string.IsNullOrWhiteSpace(whereClause))
                sql += " WHERE " + whereClause;

            var parameters = whereParams?.ToDictionary(k => k.Key, v => v.Value)
                             ?? new Dictionary<string, object>();

            return await QueryListAsync<T>(sql, parameters);
        }

        /// <inheritdoc/>
        public List<T> QueryList<T>(string sql,
                                    Dictionary<string, object> parameters)
            where T : new()
        {
            try
            {
                _logger?.Log(sql, parameters);

                using var cmd = CreateSharedCommand(sql);
                AddParameters(cmd, parameters);

                using var reader = cmd.ExecuteReader();
                return DataReaderMapper.MapToList<T>(reader);
            }
            catch (Exception ex)
            {
                throw new DataLiteException($"QueryList<{typeof(T).Name}> failed.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<List<T>> QueryListAsync<T>(string sql,
                                                      Dictionary<string, object> parameters)
            where T : new()
        {
            try
            {
                _logger?.Log(sql, parameters);

                using var cmd = CreateSharedCommand(sql);
                AddParameters(cmd, parameters);

                using var reader = await cmd.ExecuteReaderAsync();
                return DataReaderMapper.MapToList<T>(reader);
            }
            catch (Exception ex)
            {
                throw new DataLiteException(
                    $"QueryListAsync<{typeof(T).Name}> failed.", ex);
            }
        }

        // ----------------------------------------------------------------
        // Private helpers
        // ----------------------------------------------------------------

        /// <summary>Internal Execute using IDictionary (avoids parameter duplication).</summary>
        private int ExecuteInternal(string sql, IDictionary<string, object> parameters)
        {
            using var cmd = CreateSharedCommand(sql);
            AddParameters(cmd, parameters);
            return cmd.ExecuteNonQuery();
        }

        /// <summary>Internal Query using IDictionary.</summary>
        private DataTable QueryInternal(string sql,
                                        IDictionary<string, object>? parameters)
        {
            using var cmd = CreateSharedCommand(sql);
            using var adapter = DbProviderFactories
                                    .GetFactory(_db.SharedConnection)
                                    .CreateDataAdapter()!;

            AddParameters(cmd, parameters);
            adapter.SelectCommand = cmd;

            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        /// <summary>
        /// Adds parameters to a command. Handles '@' prefix automatically.
        /// Converts null values to DBNull.Value.
        /// </summary>
        private static void AddParameters(DbCommand cmd,
                                          IDictionary<string, object>? parameters)
        {
            if (parameters == null || parameters.Count == 0) return;

            foreach (var p in parameters)
            {
                var param = cmd.CreateParameter();
                param.ParameterName = p.Key.StartsWith("@") ? p.Key : "@" + p.Key;
                param.Value = p.Value ?? DBNull.Value;
                cmd.Parameters.Add(param);
            }
        }

        /// <summary>Helper to create a named parameter without adding it yet.</summary>
        private static DbParameter CreateParameter(DbCommand cmd,
        //                        var value = reader.GetValue(i);
        //                        var convertedValue = Convert.ChangeType(value, mapping.PropertyType);
        //                        mapping.Property.SetValue(obj, convertedValue);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        System.Diagnostics.Debug.WriteLine(
        //                            $"Warning: Failed to map column '{dbColumnName}' to property '{mapping.Property.Name}' - {ex.Message}");
        //                    }
        //                }
        //            }
                                                    string name,
                                                    object? value)
        {
            var param = cmd.CreateParameter();
            param.ParameterName = name.StartsWith("@") ? name : "@" + name;
            param.Value = value ?? DBNull.Value;
            return param;
        }
    }
}
