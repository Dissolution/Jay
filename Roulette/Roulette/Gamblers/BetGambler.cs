namespace Jay.Roulette.Gamblers
{
    public class BetGambler : Gambler
    {
        public Bet Bet { get; }
        public int BetAmount { get; }

        /// <inheritdoc />
        public BetGambler(int startingMoney, int betAmount, Bet bet)
            : base("Bet Gambler", startingMoney)
        {
            this.BetAmount = betAmount;
            this.Bet = bet;
        }

        /// <inheritdoc />
        protected override void Process(Pocket pocket)
        {
            var money = this.Money - this.BetAmount;
            var reward = this.Bet.Payout(pocket, BetAmount);
            if (reward > 0) _hits++;
            this.Money = money + reward;
        }

        /// <inheritdoc />
        public override Interest GetFinalInterest()
        {
            return Interest.Create($"{Bet} Gambler: {Money:C}/E:{Engagement:P1} failed after {_pocketsSeen} pockets");
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Bet} Gambler: {Money:C} / E:{Engagement:P1}";
        }
    }
}