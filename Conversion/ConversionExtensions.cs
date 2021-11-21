using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Conversion
{
    public static class ConversionExtensions
    {
        public static bool Is<TOut>(this object? obj, out TOut output)
        {
            if (obj is TOut)
            {
                output = (TOut)obj;
                return true;
            }
            else
            {
                output = default;
                return false;
            }
        }

        public static ReadOnlySpan<T> ToReadOnlySpan<T>(ref this T value)
            where T : unmanaged
        {
            unsafe
            {
                return new ReadOnlySpan<T>(Unsafe.AsPointer(ref value), 1);
            }
        }

        public static bool CanBeNull(this Type? type)
        {
            return type is null || !type.IsValueType || type == typeof(DBNull) || type.IsAssignableTo(typeof(Nullable<>));

        }
    }
}
