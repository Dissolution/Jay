using System.Diagnostics.CodeAnalysis;

namespace Jay.Reflection
{
    public static partial class Clay
    {
        public static dynamic Wrap<T>([DisallowNull] T value)
        {
            return new Wrapper<T>(value);
        }
    }
}