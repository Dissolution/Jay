using System;
using Jay.Text;
using Jay;

namespace Jay.Sandbox.Fun.Creaturez
{
    public readonly struct Stat
    {
        private const int PLACES = 2;

        public int? Value { get; }
        public double? Multiplier { get; }

        public Stat(int value)
        {
            this.Value = value;
            this.Multiplier = null;
        }

        public Stat(double multiplier)
        {
            this.Value = null;
            this.Multiplier = Math.Round(multiplier, PLACES);
        }

        public static double Apply(Stat a, Stat b)
        {
            if (a.Value.TryGetValue(out var aValue))
            {
                if (b.Value.TryGetValue(out var bValue))
                {
                    return Math.Max(aValue, bValue);        //Biggest value stat
                }
                else if (b.Multiplier.TryGetValue(out var bMulti))
                {
                    return Math.Round(aValue * bMulti, PLACES); //Multiply
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            else if (a.Multiplier.TryGetValue(out var aMulti))
            {
                if (b.Value.TryGetValue(out var bValue))
                {
                    return Math.Round(aMulti * bValue, PLACES); //Multiply
                }
                else if (b.Multiplier.TryGetValue(out var bMulti))
                {
                    return Math.Round(1d, PLACES);      //Multiplicative identity = 1
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Value.HasValue ? $"{Value,3+PLACES:##.00}" : $"{Multiplier,2+PLACES:##.00}x";
        }
    }
}