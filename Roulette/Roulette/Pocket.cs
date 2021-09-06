using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Jay.Roulette
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// The pockets of the roulette wheel are numbered from 0 to 36.
    /// There is a green pocket numbered 0 (zero). In American roulette, there is a second green pocket marked 00.
    /// In number ranges from 1 to 10 and 19 to 28, odd numbers are red and even are black.
    /// In ranges from 11 to 18 and 29 to 36, odd numbers are black and even are red.
    /// Pocket number order on the roulette wheel adheres to the following clockwise sequence in most casinos:
    /// Double-zero wheel
    /// 0-28-9-26-30-11-7-20-32-17-5-22-34-15-3-24-36-13-1-00-27-10-25-29-12-8-19-31-18-6-21-33-16-4-23-35-14-2
    /// </remarks>
    [StructLayout(LayoutKind.Explicit, Size = 1)]
    public readonly struct Pocket : IEquatable<Pocket>
    {
        public enum PocketColor : byte
        {
            Green = 0,
            Black = 1,
            Red = 2,
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Pocket(byte value) => NotSafe.As<byte, Pocket>(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator int(Pocket pocket) => pocket._code;
        
        public static bool operator ==(Pocket x, Pocket y) => x._code == y._code;
        public static bool operator !=(Pocket x, Pocket y) => x._code != y._code;

        public static readonly Pocket Zero =            new Pocket(0b000000); // Green
        public static readonly Pocket One =             new Pocket(0b000001); // Red
        public static readonly Pocket Two =             new Pocket(0b000010); // Black
        public static readonly Pocket Three =           new Pocket(0b000011); // Red
        public static readonly Pocket Four =            new Pocket(0b000100); // Black
        public static readonly Pocket Five =            new Pocket(0b000101); // Red
        public static readonly Pocket Six =             new Pocket(0b000110); // Black
        public static readonly Pocket Seven =           new Pocket(0b000111); // Red
        public static readonly Pocket Eight =           new Pocket(0b001000); // Black
        public static readonly Pocket Nine =            new Pocket(0b001001); // Red
        public static readonly Pocket Ten =             new Pocket(0b001010); // Black
        public static readonly Pocket Eleven =          new Pocket(0b001011); // Black
        public static readonly Pocket Twelve =          new Pocket(0b001100); // Red
        public static readonly Pocket Thirteen =        new Pocket(0b001101); // Black
        public static readonly Pocket Fourteen =        new Pocket(0b001110); // Red
        public static readonly Pocket Fifteen =         new Pocket(0b001111); // Black
        public static readonly Pocket Sixteen =         new Pocket(0b010000); // Red
        public static readonly Pocket Seventeen =       new Pocket(0b010001); // Black
        public static readonly Pocket Eighteen =        new Pocket(0b010010); // Red
        public static readonly Pocket Nineteen =        new Pocket(0b010011); // Red
        public static readonly Pocket Twenty =          new Pocket(0b010100); // Black
        public static readonly Pocket TwentyOne =       new Pocket(0b010101); // Red
        public static readonly Pocket TwentyTwo =       new Pocket(0b010110); // Black
        public static readonly Pocket TwentyThree =     new Pocket(0b010111); // Red
        public static readonly Pocket TwentyFour =      new Pocket(0b011000); // Black
        public static readonly Pocket TwentyFive =      new Pocket(0b011001); // Red
        public static readonly Pocket TwentySix =       new Pocket(0b011010); // Black
        public static readonly Pocket TwentySeven =     new Pocket(0b011011); // Red
        public static readonly Pocket TwentyEight =     new Pocket(0b011100); // Black
        public static readonly Pocket TwentyNine =      new Pocket(0b011101); // Black
        public static readonly Pocket Thirty =          new Pocket(0b011110); // Red
        public static readonly Pocket ThirtyOne =       new Pocket(0b011111); // Black
        public static readonly Pocket ThirtyTwo =       new Pocket(0b100000); // Red
        public static readonly Pocket ThirtyThree =     new Pocket(0b100001); // Black
        public static readonly Pocket ThirtyFour =      new Pocket(0b100010); // Red
        public static readonly Pocket ThirtyFive =      new Pocket(0b100011); // Black
        public static readonly Pocket ThirtySix =       new Pocket(0b100100); // Red
        public static readonly Pocket ZeroZero =        new Pocket(0b100101); // Black

        public static bool TryParse(ReadOnlySpan<char> mark, out Pocket pocket)
        {
            mark = mark.Trim();
            if (byte.TryParse(mark, out byte b))
            {
                if (b == 0)
                {
                    if (mark.Length == 1)
                    {
                        pocket = Zero;
                    }
                    else
                    {
                        Debug.Assert(mark.Length == 2);
                        pocket = ZeroZero;
                    }

                    return true;
                }

                if (b >= 1 && b <= 36)
                {
                    pocket = NotSafe.As<byte, Pocket>(b);
                    return true;
                }
            }
            pocket = default;
            return false;
        }
        
        [FieldOffset(0)] 
        private readonly byte _code;

        public bool IsRed
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return _code switch
                       {
                           // In number ranges from 1 to 10 and 19 to 28, odd numbers are red and even are black.
                           (>= 1 and <= 10) or (>= 19 and <= 28) => _code % 2 != 0,
                           // In ranges from 11 to 18 and 29 to 36, odd numbers are black and even are red.
                           (>= 11 and <= 18) or (>= 29 and <= 36) => _code % 2 == 0,
                           _ => false
                       };
            }
        }

        public bool IsBlack
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return _code switch
                       {
                           // In number ranges from 1 to 10 and 19 to 28, odd numbers are red and even are black.
                           (>= 1 and <= 10) or (>= 19 and <= 28) => _code % 2 == 0,
                           // In ranges from 11 to 18 and 29 to 36, odd numbers are black and even are red.
                           (>= 11 and <= 18) or (>= 29 and <= 36) => _code % 2 != 0,
                           _ => false
                       };
            }
        }
        
        public bool IsGreen
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _code is (0 or 37);
        }

        public int Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _code;
        }

        public PocketColor Color
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return _code switch
                       {
                           (0 or 37) => PocketColor.Green,
                           // In number ranges from 1 to 10 and 19 to 28, odd numbers are red and even are black.
                           (>= 1 and <= 10) or (>= 19 and <= 28) => ((_code % 2) == 0) ? PocketColor.Black : PocketColor.Red,
                           // In ranges from 11 to 18 and 29 to 36, odd numbers are black and even are red.
                           (>= 11 and <= 18) or (>= 29 and <= 36) => ((_code % 2) == 0) ? PocketColor.Red : PocketColor.Black,
                           _ => PocketColor.Green,
                       };
                
            }
        }

        private Pocket(byte code)
        {
            _code = code;
        }
        
        /// <inheritdoc />
        public bool Equals(Pocket pocket) => pocket._code == _code;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Pocket pocket && pocket._code == _code;

        /// <inheritdoc />
        public override int GetHashCode() => (int) _code;

        /// <inheritdoc />
        public override string ToString()
        {
            if (_code == 37)
                return "00";
            return $"{_code,2:#0}";
        }
    }
}