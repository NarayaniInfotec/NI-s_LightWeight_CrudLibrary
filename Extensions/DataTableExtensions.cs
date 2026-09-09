using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NarayaniInfotec.DataLite.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="DataTable"/> instances.
    /// </summary>
    public static class DataTableExtensions
    {
        /// <summary>
        /// Converts the rows of the specified <see cref="DataTable"/> into a list of dictionaries where each dictionary represents a row.
        /// Column names are used as dictionary keys and DBNull values are converted to null.
        /// </summary>
        /// <param name="table">The <see cref="DataTable"/> to convert.</param>
        /// <returns>A list of dictionaries representing the table rows.</returns>
        public static List<Dictionary<string, object?>> ToList(this DataTable table)
        {
            var result = new List<Dictionary<string, object?>>();

            foreach (DataRow row in table.Rows)
            {
                var dict = new Dictionary<string, object?>();
                foreach (DataColumn col in table.Columns)
                {
                    dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
                }
                result.Add(dict);
            }

            return result;
        }
    }
}
