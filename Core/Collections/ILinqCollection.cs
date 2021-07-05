using System;
using System.Collections.Generic;

namespace Jay.Collections
{
    public interface ILinqCollection<T> : IEnumerable<T>, IDisposable
    {
        int Count { get; }

        ILinqCollection<T> Prepend(params T[] items);
        ILinqCollection<T> Prepend(IEnumerable<T> items);
        
        ILinqCollection<T> Append(params T[] items);
        ILinqCollection<T> Append(IEnumerable<T> items);

        ILinqCollection<T> Where(Func<T, bool> predicate);
        ILinqCollection<U> Select<U>(Func<T, U> selector);



    }
}