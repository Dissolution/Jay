/*using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Jay.Randomization
{
    public interface IRepeatRandomizer<out T>
        where T : unmanaged
    {
        T NextValue();
    }

    internal abstract class RepeatRandomizer<T> : IRepeatRandomizer<T>
        where T : unmanaged
    {
        protected readonly IRandomizer _randomizer;
        protected static readonly Type _type = typeof(T);
        protected static readonly int _size = Unsafe.SizeOf<T>();

        protected RepeatRandomizer(IRandomizer randomizer)
        {
            _randomizer = randomizer;
        }

        /// <inheritdoc />
        public abstract T NextValue();
    }

    internal sealed class RepeatRandomizerUnder64Bits<T> : RepeatRandomizer<T>
        where T : unmanaged
    {
        private ulong _bitStock;
        private int _usedBits;
        
        public RepeatRandomizerUnder64Bits(IRandomizer randomizer)
            : base(randomizer)
        {
            Debug.Assert(_size < 8);
            _bitStock = 0UL;
            _usedBits = 0;
        }

        /// <inheritdoc />
        public override T NextValue()
        {
            
        }
    }
}*/