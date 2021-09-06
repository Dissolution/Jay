using System;

namespace Jay
{
    public readonly struct StartLen : IEquatable<StartLen>
    {
        public static bool operator ==(StartLen a, StartLen b) => a.Start == b.Start && a.Length == b.Length;
        public static bool operator !=(StartLen a, StartLen b) => a.Start != b.Start || a.Length != b.Length;

        public readonly int Start;
        public readonly int Length;
        public int End => Start + Length;

        public StartLen(int start, int length)
        {
            this.Start = start;
            this.Length = length;
        }

        public void Deconstruct(out int start, out int length)
        {
            start = this.Start;
            length = this.Length;
        }

                
        /// <inheritdoc />
        public bool Equals(StartLen startLen)
        {
            return this.Start == startLen.Start &&
                   this.Length == startLen.Length;
        }
                
        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is StartLen startLen && Equals(startLen);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Hasher.Create(Start, Length);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"[{Start}:{Length})";
        }
    }
}