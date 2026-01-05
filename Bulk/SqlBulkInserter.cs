using System.Data;
using Microsoft.Data.SqlClient;

namespace NarayaniInfotec.Bulk
{
    /// <summary>
    /// To transfer bulk quantity data to table.
    /// </summary>
    public class SqlBulkInserter
    {
        /// <summary>
        /// Insert bullk data.
        /// </summary>
        /// <param name="connectionString">DB connection string</param>
        /// <param name="tableName">Target table</param>
        /// <param name="data">Pass data as datatable </param>
        public static void Insert(
            string connectionString,
            string tableName,
            DataTable data)
        {
            using var bulk = new SqlBulkCopy(connectionString);
            bulk.DestinationTableName = tableName;
            bulk.WriteToServer(data);
        }
    }

}
