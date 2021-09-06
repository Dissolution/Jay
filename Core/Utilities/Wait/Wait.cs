using System;
using System.Threading;

namespace Jay
{
    public static class Wait
    {
        public static TResult For<TResult>(Func<TResult> function, 
                                           Func<TResult, bool> resultCheck,
                                           TimeSpan? coolDown = null)
        {
            TResult result;
            while (true)
            {
                result = function();
                if (resultCheck(result))
                    return result;
                if (coolDown.HasValue)
                    Thread.Sleep(coolDown.Value);
            }
        }
    }
}