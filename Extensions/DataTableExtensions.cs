using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NarayaniInfotec.DataLite.Extensions
{
    public static class DataTableExtensions
    {
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
