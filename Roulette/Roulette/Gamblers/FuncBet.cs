using System;
using System.Runtime.CompilerServices;

namespace Jay.Roulette.Gamblers
{
    public class FuncBet : Bet
    {
        protected readonly Func<Pocket, bool> _pocketPaysOut;
        protected readonly int _betMultiplier;

        public FuncBet(Func<Pocket, bool> pocketPaysOut, int betMultiplier,
                       [CallerMemberName] string? name = null)
            : base(name)
        {
            _pocketPaysOut = pocketPaysOut;
            _betMultiplier = betMultiplier;
        }

        /// <inheritdoc />
        public override int Payout(Pocket pocket, int amount)
        {
            if (_pocketPaysOut(pocket))
            {
                return _betMultiplier * amount;
            }
            else
            {
                return 0;
            }
        }
    }
}