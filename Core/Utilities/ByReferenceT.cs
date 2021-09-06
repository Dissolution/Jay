using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Jay.Exceptions;

namespace Jay
{
    /// <summary>
    /// An implementation of <see cref="ByReference{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly ref struct ByReference<T>
    {
        private readonly Span<T> _span;

        /// <summary>
        /// Gets a reference to the value
        /// </summary>
        public ref T Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref MemoryMarshal.GetReference(_span);
        }

        /// <summary>
        /// Construct a new <see cref="ByReference{T}"/> referencing the given <paramref name="value"/>
        /// </summary>
        /// <param name="value">The <see langword="ref"/> value to capture</param>
        public ByReference(ref T value)
        {
            _span = MemoryMarshal.CreateSpan(ref value, 1);
        }
        
        public override bool Equals(object? obj) => false;

        public override int GetHashCode() => GetHashCodeException.Throw(typeof(ByReference<T>));

        public override string ToString() => $"ref {_span[0]}";
    }
}