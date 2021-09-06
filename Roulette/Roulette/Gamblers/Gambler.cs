using System.Collections.Generic;

namespace Jay.Roulette.Gamblers
{
    public abstract class Gambler : WheelWatcher
    {
        protected int _money;
        protected int _hits;
       
        public int Money
        {
            get => _money;
            set
            {
                if (value <= 0)
                {
                    _interests.Enqueue(Interest.Create($"Bankrupt after {_pocketsSeen} pockets"));
                }
                _money = value;
            }
        }

        public double Engagement => (double)_hits / (double)_pocketsSeen;

        /// <inheritdoc />
        public override bool IsActive => _money > 0;

        protected Gambler(string name, int startingMoney)
            : base(name)
        {
            _money = startingMoney;
            _hits = 0;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{_name}: ${Money:C} / E:{Engagement}";
        }
    }
}