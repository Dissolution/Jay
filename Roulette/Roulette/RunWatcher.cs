using System;
using System.Collections.Generic;

namespace Jay.Roulette
{
    public class RunWatcher<T> : WheelWatcher
    {
        private readonly Func<Pocket, T> _fromPocket;
            
        private T? _recordValue;
        private int _recordCount;

        private T? _currentValue;
        private int _currentCount;

        /// <inheritdoc />
        public override bool IsActive => true;

        public RunWatcher(string name, Func<Pocket, T> fromPocket)
            : base(name)
        {
            _fromPocket = fromPocket;
            _recordValue = default;
            _recordCount = 0;
            _currentValue = default;
            _currentCount = 0;
        }

        /// <inheritdoc />
        protected override void Process(Pocket pocket)
        {
            var value = _fromPocket(pocket);
            if (!EqualityComparer<T>.Default.Equals(value, _currentValue))
            {
                _currentValue = value;
                _currentCount = 1;
            }
            else
            {
                _currentCount++;
                if (_currentCount > _recordCount)
                {
                    _recordValue = value;
                    _recordCount = _currentCount;
                    _interests.Enqueue(Interest.Create($"New {_name} Run Record: {_recordCount}x  {_recordValue}"));
                }
            }
        }
            
        /// <inheritdoc />
        public override Interest GetFinalInterest()
        {
            return Interest.Create($"{_name} Run Record: {_recordCount}x  {_recordValue}");
        }
    }
}