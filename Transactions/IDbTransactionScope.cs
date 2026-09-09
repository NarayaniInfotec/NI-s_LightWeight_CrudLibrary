using NarayaniInfotec.Core;
using NarayaniInfotec.DataLite.Core;
using System;
using System.Data.Common;

namespace NarayaniInfotec.DataLite.Transactions
{
    /// <summary>
    /// Provides a lightweight transactional scope that wraps the
    /// <see cref="IDatabase.SharedConnection"/> in a database transaction.
    /// <remarks>
    /// This class simplifies transaction handling by managing a shared
    /// <see cref="DbConnection"/> and <see cref="DbTransaction"/> lifecycle.
    /// It is intended to be used with a <c>using</c> statement to ensure
    /// proper commit, rollback, and resource cleanup.
    /// </remarks>
    /// </summary>  
    public class DbTransactionScope : IDisposable
    {
        /// <summary>Gets the active database connection associated with the transaction.</summary>
        public DbConnection Connection { get; }

        /// <summary>The active transaction.</summary>
        public DbTransaction Transaction { get; }

        private bool _disposed;

        /// <summary>
        /// Begins a transaction on <see cref="IDatabase.SharedConnection"/>.
        /// The connection is already open — no Open() call is made here.
        /// </summary>
        /// <param name="db">An implementation of <see cref="IDatabase"/> used to create the database connection and within the current DI scope.</param>
        public DbTransactionScope(IDatabase db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            Connection = db.SharedConnection;   // already open
            Transaction = Connection.BeginTransaction();
        }

        /// <summary>Commits the active transaction.</summary>
        public void Commit() => Transaction.Commit();

        /// <summary>Rolls back the current database transaction.</summary>
        public void Rollback() => Transaction.Rollback();

        /// <summary>
        /// Disposes the transaction object only.
        /// The underlying connection is owned by <see cref="IDatabase"/>
        /// and will be disposed by the DI container at scope end.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            Transaction?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
