using System.Collections.Generic;
using Jay.Debugging.Dumping;

namespace Jay.Roulette
{
    public interface IWheelWatcher
    {
        void Notify(Pocket pocket);

        IEnumerable<Interest> GetInterests();
        
        Interest GetFinalInterest();
    }
}