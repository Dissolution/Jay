using System;

namespace Jay.Debugging.Dumping
{
    [AttributeUsage(AttributeTargets.All)]
    public class DumpAsAttribute : Attribute
    {
        private readonly string? _representation;

        public DumpAsAttribute(string? representation)
        {
            _representation = representation;
        }

        public override string ToString()
        {
            return _representation ?? string.Empty;
        }
    }
}