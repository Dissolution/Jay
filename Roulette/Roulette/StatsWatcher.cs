using System.Linq;

namespace Jay.Roulette
{
    public class StatsWatcher : WheelWatcher
    {
        private readonly int[] _counts;

        /// <inheritdoc />
        public override bool IsActive => true;

        public StatsWatcher(string name)
            : base(name)
        {
            _counts = new int[38];
            _counts.Initialize();
        }
            
        /// <inheritdoc />
        protected override void Process(Pocket pocket)
        {
            _counts[(int) pocket] += 1;
        }

        /// <inheritdoc />
        public override Interest GetFinalInterest()
        {
            var min = _counts.Min();
            var ave = _counts.Average();
            var max = _counts.Max();
            return Interest.Create($"Min: {min}   Ave: {ave}   Max: {max}");
        }
    }
}