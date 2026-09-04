using NarayaniInfotec.Core;
using NarayaniInfotec.DataLite.Core;
using NarayaniInfotec.DataLite.DataMapper;
using NarayaniInfotec.DataLite.StoredProcedures;
using NarayaniInfotec.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace NarayaniInfotec.StoredProcedures
{
    /// <summary>
    /// Default implementation of <see cref="IStoredProcedure"/>.
    ///
    /// Executes stored procedures in a database-agnostic manner using
    /// the <see cref="IDatabase"/> abstraction — identical to how
    /// <see cref="NarayaniInfotec.CRUD.CrudRepository"/> operates for raw SQL.
    ///
    /// Supports:
    /// - Strongly typed List&lt;T&gt; (sync + async) via <see cref="DataReaderMapper"/>
    /// - DataTable result sets (sync + async)
    /// - Non-query execution (rows affected)
    /// - Generic scalar results
    /// - Multiple result sets (DataSet and typed tuples)
    /// - Parameter passing via Dictionary or SpParam[] array
    /// </summary>
    public class StoredProcedure : IStoredProcedure
    {
        private readonly IDatabase _db;
        private readonly IDbLogger? _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="StoredProcedure"/>.
        /// </summary>
        /// <param name="db">Database provider — same instance used by CrudRepository.</param>
        /// <see cref="NarayaniInfotec.CRUD.CrudRepository"/> within one request.
        /// </param>
        /// <param name="logger">Optional query logger.</param>
        public StoredProcedure(IDatabase db, IDbLogger? logger = null)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _logger = logger;
        }

        // ----------------------------------------------------------------
        // LIST<T>
        // ----------------------------------------------------------------

        /// <inheritdoc/>
        public List<T> ExecuteList<T>(string procedureName,
                                      Dictionary<string, object>? parameters = null)
            where T : new()
        {
            try
            {
                LogExecution(procedureName, parameters);

                using var cmd = BuildSharedCommand(procedureName, parameters);
                using var reader = cmd.ExecuteReader();
                return DataReaderMapper.MapToList<T>(reader);
            }
            catch (Exception ex)
            {
                throw new DataLiteException(
                    $"ExecuteList<{typeof(T).Name}> failed for SP '{procedureName}'.", ex);
            }
        }

        /// <inheritdoc/>
        public List<T> ExecuteList<T>(string procedureName, params SpParam[] parameters)
            where T : new()
            => ExecuteList<T>(procedureName, ToDictionary(parameters));

        /// <inheritdoc/>
        public async Task<List<T>> ExecuteListAsync<T>(
            string procedureName,
            Dictionary<string, object>? parameters = null)
            where T : new()
        {
            try
            {
                LogExecution(procedureName, parameters);

                using var cmd = BuildSharedCommand(procedureName, parameters);
                using var reader = await cmd.ExecuteReaderAsync();
                return DataReaderMapper.MapToList<T>(reader);
            }
            catch (Exception ex)
            {
                throw new DataLiteException(
                    $"ExecuteListAsync<{typeof(T).Name}> failed for SP '{procedureName}'.", ex);
            }
        }

        /// <inheritdoc/>
        public Task<List<T>> ExecuteListAsync<T>(string procedureName,
                                                  params SpParam[] parameters)
            where T : new()
            => ExecuteListAsync<T>(procedureName, ToDictionary(parameters));

        // ----------------------------------------------------------------
        // DATATABLE
        // ----------------------------------------------------------------

        /// <inheritdoc/>
        public DataTable Execute(string procedureName,
                                 Dictionary<string, object>? parameters = null)
        {
            try
            {
                LogExecution(procedureName, parameters);

                using var cmd = BuildSharedCommand(procedureName, parameters);
                using var adapter = DbProviderFactories
                                        .GetFactory(_db.SharedConnection)
                                        .CreateDataAdapter()!;

                adapter.SelectCommand = cmd;
                var dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw new DataLiteException(
                    $"Execute (DataTable) failed for SP '{procedureName}'.", ex);
            }
        }

        /// <inheritdoc/>
        public DataTable Execute(string procedureName, params SpParam[] parameters)
            => Execute(procedureName, ToDictionary(parameters));

        /// <inheritdoc/>
        public async Task<DataTable> ExecuteAsync(string procedureName,
                                                   Dictionary<string, object>? parameters = null)
        {
            try
            {
                LogExecution(procedureName, parameters);

                using var cmd = BuildSharedCommand(procedureName, parameters);
                using var reader = await cmd.ExecuteReaderAsync();

                var dt = new DataTable();
                dt.Load(reader);
                return dt;
            }
            catch (Exception ex)
            {
                throw new DataLiteException(
                    $"ExecuteAsync (DataTable) failed for SP '{procedureName}'.", ex);
            }
        }

        /// <inheritdoc/>
        public Task<DataTable> ExecuteAsync(string procedureName, params SpParam[] parameters)
            => ExecuteAsync(procedureName, ToDictionary(parameters));

        // ─────────────────────────────────────────────────────────────
        // NON-QUERY — INSERT / UPDATE / DELETE via SP
        // ─────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public int ExecuteNonQuery(string procedureName,
                                   Dictionary<string, object>? parameters = null)
        {
            try
            {
                LogExecution(procedureName, parameters);

                using var cmd = BuildSharedCommand(procedureName, parameters);
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new DataLiteException(
                    $"ExecuteNonQuery failed for SP '{procedureName}'.", ex);
            }
        }

        /// <inheritdoc/>
        public int ExecuteNonQuery(string procedureName, params SpParam[] parameters)
            => ExecuteNonQuery(procedureName, ToDictionary(parameters));

        /// <inheritdoc/>
        public async Task<int> ExecuteNonQueryAsync(string procedureName,
                                                     Dictionary<string, object>? parameters = null)
        {
            try
            {
                LogExecution(procedureName, parameters);

                using var cmd = BuildSharedCommand(procedureName, parameters);
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new DataLiteException(
                    $"ExecuteNonQueryAsync failed for SP '{procedureName}'.", ex);
            }
        }

        /// <inheritdoc/>
        public Task<int> ExecuteNonQueryAsync(string procedureName, params SpParam[] parameters)
            => ExecuteNonQueryAsync(procedureName, ToDictionary(parameters));

        // ─────────────────────────────────────────────────────────────
        // SCALAR — single value return
        // ─────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public TResult ExecuteScalar<TResult>(string procedureName,
                                              Dictionary<string, object>? parameters = null)
        {
            try
            {
                LogExecution(procedureName, parameters);

                using var cmd = BuildSharedCommand(procedureName, parameters);
                var raw = cmd.ExecuteScalar();

                if (raw == null || raw == DBNull.Value) return default!;
                return (TResult)Convert.ChangeType(raw, typeof(TResult));
            }
            catch (Exception ex)
            {
                throw new DataLiteException(
                    $"ExecuteScalar<{typeof(TResult).Name}> failed for SP '{procedureName}'.", ex);
            }
        }

        /// <inheritdoc/>
        public TResult ExecuteScalar<TResult>(string procedureName, params SpParam[] parameters)
            => ExecuteScalar<TResult>(procedureName, ToDictionary(parameters));

        /// <inheritdoc/>
        public async Task<TResult> ExecuteScalarAsync<TResult>(
            string procedureName,
            Dictionary<string, object>? parameters = null)
        {
            try
            {
                LogExecution(procedureName, parameters);

                using var cmd = BuildSharedCommand(procedureName, parameters);
                var raw = await cmd.ExecuteScalarAsync();

                if (raw == null || raw == DBNull.Value) return default!;
                return (TResult)Convert.ChangeType(raw, typeof(TResult));
            }
            catch (Exception ex)
            {
                throw new DataLiteException(
                    $"ExecuteScalarAsync<{typeof(TResult).Name}> failed for SP '{procedureName}'.", ex);
            }
        }

        /// <inheritdoc/>
        public Task<TResult> ExecuteScalarAsync<TResult>(string procedureName,
                                                          params SpParam[] parameters)
            => ExecuteScalarAsync<TResult>(procedureName, ToDictionary(parameters));

        // ----------------------------------------------------------------
        // MULTIPLE RESULT SETS
        // ----------------------------------------------------------------

        /// <inheritdoc/>
        public DataSet ExecuteMultiple(string procedureName,
                                       Dictionary<string, object>? parameters = null)
        {
            try
            {
                LogExecution(procedureName, parameters);

                using var cmd = BuildSharedCommand(procedureName, parameters);
                using var adapter = DbProviderFactories
                                        .GetFactory(_db.SharedConnection)
                                        .CreateDataAdapter()!;

                adapter.SelectCommand = cmd;
                var ds = new DataSet();
                adapter.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw new DataLiteException(
                    $"ExecuteMultiple (DataSet) failed for SP '{procedureName}'.", ex);
            }
        }

        /// <inheritdoc/>
        public DataSet ExecuteMultiple(string procedureName, params SpParam[] parameters)
            => ExecuteMultiple(procedureName, ToDictionary(parameters));

        /// <inheritdoc/>
        public (List<T1> First, List<T2> Second) ExecuteMultiple<T1, T2>(
            string procedureName,
            Dictionary<string, object>? parameters = null)
            where T1 : new()
            where T2 : new()
        {
            try
            {
                LogExecution(procedureName, parameters);

                using var cmd = BuildSharedCommand(procedureName, parameters);
                using var reader = cmd.ExecuteReader();

                var first = DataReaderMapper.MapToList<T1>(reader);
                reader.NextResult();                           // advance to second set
                var second = DataReaderMapper.MapToList<T2>(reader);

                return (first, second);
            }
            catch (DataLiteException) { throw; }
            catch (Exception ex)
            {
                throw new DataLiteException(
                    $"ExecuteMultiple<{typeof(T1).Name},{typeof(T2).Name}> failed for SP '{procedureName}'.", ex);
            }
        }

        /// <inheritdoc/>
        public (List<T1> First, List<T2> Second) ExecuteMultiple<T1, T2>(
            string procedureName, params SpParam[] parameters)
            where T1 : new()
            where T2 : new()
            => ExecuteMultiple<T1, T2>(procedureName, ToDictionary(parameters));

        // ----------------------------------------------------------------
        // Private helpers
        // ----------------------------------------------------------------

        /// <summary>
        /// Builds a stored-procedure command tied to <see cref="IDatabase.SharedConnection"/>.
        /// The connection is already open — no Open() call needed.
        /// </summary>
        private DbCommand BuildSharedCommand(string procedureName,
                                             Dictionary<string, object>? parameters)
        {
            var conn = _db.SharedConnection;     // already open
            var cmd = conn.CreateCommand();
            cmd.CommandText = procedureName;
            cmd.CommandType = CommandType.StoredProcedure;
            AddParameters(cmd, parameters);
            return cmd;
        }

        /// <summary>Converts SpParam[] to a Dictionary for the core overloads.</summary>
        private static Dictionary<string, object>? ToDictionary(SpParam[]? parameters)
        {
            if (parameters == null || parameters.Length == 0) return null;
            return parameters.ToDictionary(p => p.Name, p => p.Value as object ?? DBNull.Value);
        }

        /// <summary>
        /// Adds parameters to a command. Handles '@' prefix automatically.
        /// </summary>
        private static void AddParameters(DbCommand cmd,
                                          Dictionary<string, object>? parameters)
        {
            if (parameters == null || parameters.Count == 0) return;

            foreach (var p in parameters)
            {
                var param = cmd.CreateParameter();
                param.ParameterName = p.Key.StartsWith("@", StringComparison.Ordinal)
                    ? p.Key
                    : "@" + p.Key;

                param.Value = p.Value ?? DBNull.Value;
                cmd.Parameters.Add(param);
            }
        }

        private void LogExecution(string procedureName,
                                   Dictionary<string, object>? parameters)
            => _logger?.Log($"EXEC {procedureName}", parameters!);
    }
}
