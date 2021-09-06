using System;
using System.Runtime.CompilerServices;

namespace Jay.Roulette.Gamblers
{
    public abstract class Bet : IEquatable<Bet>
    {
        public static Bet Black { get; } = new FuncBet(pocket => pocket.IsBlack, 2);
        public static Bet Red { get; } = new FuncBet(pocket => pocket.IsRed, 2);

        protected readonly string _name;
        
        protected Bet(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                _name = this.GetType().Name;
            }
            else
            {
                _name = name;
            }
        }

        public abstract int Payout(Pocket pocket, int amount);

        /// <inheritdoc />
        public bool Equals(Bet? bet) => bet?.GetType() == this.GetType();

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is Bet bet && Equals(bet);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Hasher.Create(GetType());
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return _name;
        }
    }
}