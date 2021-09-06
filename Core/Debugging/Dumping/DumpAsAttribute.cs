using System;

namespace Jay.Debugging.Dumping
{
    [AttributeUsage(AttributeTargets.All)]
    public class DumpAsAttribute : Attribute
    {
        private readonly string? _representation;

        public DumpAsAttribute(string representation)
        {
            _representation = representation;
        }

        public bool HasRepresentation(out string representation)
        {
            if (string.IsNullOrWhiteSpace(_representation))
            {
                representation = string.Empty;
                return false;
            }
            else
            {
                representation = _representation;
                return true;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return _representation ?? "T.ToString()";
        }
    }
}