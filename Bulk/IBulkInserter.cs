using System.Data;

namespace NarayaniInfotec.Bulk
{
    /// <summary>
    /// Interface for BulkInserter class.
    /// </summary>
    public interface IBulkInserter
    {
        /// <summary>
        /// Inserts records in bulk.
        /// </summary>
        /// <param name="tableName">Target table name.</param>
        /// <param name="data">Datatable as data</param>
        void Insert(string tableName, DataTable data);
    }
}
