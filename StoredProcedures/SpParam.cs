using System;
using System.Collections.Generic;
using System.Text;

namespace NarayaniInfotec.DataLite.StoredProcedures
{
    /// <summary>
    /// Represents a stored procedure parameter key-value pair.
    /// </summary>
    public class SpParam
    {
        public string Name { get; set; }
        public object? Value { get; set; }

        public SpParam(string name, object? value)
        {
            Name = name;
            Value = value;
        }
    }
}
