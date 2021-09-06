/*using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Jay.Collections.Uber
{
    internal class Testing
    {
        static Testing()
        {
            IUEnumerable<Testing> u = default;
            foreach (var k in u)
            {
                
            }
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <see cref="IEnumerator"/>
    /// <see cref="IEnumerator{T}"/>
    public interface IUnumerator<T> : IEnumerator<T>, IEnumerator, IDisposable
    {
        int Index { get; }

        bool TryMoveNext([MaybeNullWhen(false)] out T value);
    }
    
    public interface IUEnumerable<T>
    {
        IUnumerator<T> GetEnumerator();
    }
}*/