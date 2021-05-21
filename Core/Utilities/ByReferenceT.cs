using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Jay
{
    public readonly ref struct ByReference<T>
    {
        private readonly Span<T> _span;

        public ref T Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref MemoryMarshal.GetReference(_span);
        }

        public ByReference(ref T value)
        {
            _span = MemoryMarshal.CreateSpan(ref value, 1);
        }
        
        public override bool Equals(object? obj) => false;

        public override int GetHashCode() => GetHashCodeException.Throw(typeof(ByReference<T>));

        public override string ToString() => $"ref {_span[0]}";
    }
}