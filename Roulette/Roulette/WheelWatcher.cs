using System.Collections.Generic;

namespace Jay.Roulette
{
    public abstract class WheelWatcher : IWheelWatcher
    {
        protected readonly string _name;
        protected readonly Queue<Interest> _interests;
        protected int _pocketsSeen;
        
        public abstract bool IsActive { get; }
      
        protected WheelWatcher(string name)
        {
            _name = name;
            _pocketsSeen = 0;
            _interests = new Queue<Interest>(0);
        }
            
        protected abstract void Process(Pocket pocket);
        
        public void Notify(Pocket pocket)
        {
            if (!IsActive) return;
            _pocketsSeen++;
            Process(pocket);
        }

        /// <inheritdoc />
        public IEnumerable<Interest> GetInterests()
        {
            while (_interests.TryDequeue(out var interest))
            {
                yield return interest;
            }
        }

        /// <inheritdoc />
        public abstract Interest GetFinalInterest();

        /// <inheritdoc />
        public override string ToString()
        {
            return _name;
        }
    }
}