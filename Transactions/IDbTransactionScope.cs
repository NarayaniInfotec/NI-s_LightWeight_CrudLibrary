using NarayaniInfotec.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace NarayaniInfotec.Transactions
{
    /// <summary>
    /// Provides a lightweight transactional scope for database operations.
    /// </summary>
    /// <remarks>
    /// This class simplifies transaction handling by managing a shared
    /// <see cref="DbConnection"/> and <see cref="DbTransaction"/> lifecycle.
    /// It is intended to be used with a <c>using</c> statement to ensure
    /// proper commit, rollback, and resource cleanup.
    /// </remarks>
    public class DbTransactionScope : IDisposable
    {
        /// <summary>Gets the active database connection associated with the transaction.</summary>
        public DbConnection Connection { get; }

        /// <summary>Gets the active database transaction.</summary>
        public DbTransaction Transaction { get; }

        /// <summary>Initializes a new instance of the <see cref="DbTransactionScope"/> class
        /// and begins a database transaction.</summary>
        /// <param name="db">An implementation of <see cref="IDatabase"/> used to create the database connection.</param>
        /// <exception cref="ArgumentNullException">Thrown when the provided database instance is null.</exception>
        public DbTransactionScope(IDatabase db)
        {
            Connection = db.CreateConnection();
            Connection.Open();
            Transaction = Connection.BeginTransaction();
        }

        /// <summary>Commits the current database transaction.</summary>
        public void Commit() => Transaction.Commit();

        /// <summary>Rolls back the current database transaction.</summary>
        public void Rollback() => Transaction.Rollback();

        /// <summary>Releases all resources used by the transaction scope.</summary>
        /// <remarks>If <see cref="Commit"/> is not explicitly called,
        /// the transaction should be rolled back by the consumer before disposal.</remarks>
        public void Dispose()
        {
            Transaction?.Dispose();
            Connection?.Dispose();
        }
    }

}
