
namespace NarayaniInfotec.DataLite.StoredProcedures
{
    /// <summary>
    /// The SpParam class represents a stored procedure parameter, encapsulating the parameter name and its corresponding value.
    /// Represents a stored procedure parameter key-value pair.
    /// </summary>
    public class SpParam
    {
        /// <summary>
        /// Gets or sets the stored procedure parameter name (for example, "@Id").
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the stored procedure parameter value. May be null.
        /// </summary>
        public object? Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpParam"/> class with the specified name and value.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The parameter value. May be null.</param>
        public SpParam(string name, object? value)
        {
            Name = name;
            Value = value;
        }
    }
}
