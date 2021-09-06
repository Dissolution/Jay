using System;

namespace Jay.Roulette
{
    public class CountWatcher : WheelWatcher
    {
        protected Func<Pocket, bool> _predicate;
        protected int _recordCount;
        protected int _currentCount;

        /// <inheritdoc />
        public override bool IsActive => true;

        public CountWatcher(string name, Func<Pocket, bool> predicate)
            : base(name)
        {
            _predicate = predicate;
            _recordCount = 0;
            _currentCount = 0;
        }
            
        /// <inheritdoc />
        protected override void Process(Pocket pocket)
        {
            if (!_predicate(pocket))
            {
                _currentCount = 0;
            }
            else
            {
                _currentCount++;
                if (_currentCount > _recordCount)
                {
                    _recordCount = _currentCount;
                    _interests.Enqueue(Interest.Create($"New {_name} Count Record: {_recordCount}x"));
                }
            }
        }
            
        /// <inheritdoc />
        public override Interest GetFinalInterest()
        {
            return Interest.Create($"{_name} Count Record: {_recordCount}x");
        }
    }
}