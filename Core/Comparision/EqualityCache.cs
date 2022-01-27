/*
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jay.Comparision
{
    public class EqualityCache
    {
        private readonly Dictionary<Type, Delegate> _typeEqualsCache;
        private readonly List<(Type Type, Delegate EqualsDelegate, Delegate? GetHashCodeDelegate)> _delegates;

        public EqualityCache()
        {
            _typeEqualsCache = new();
            _delegates = new();
        }

        private void AddDelegate(Type type, Func<object?, object?, bool> equals, Func<object?, int>? getHashCode)
        {
            _delegates.Add((type, equals, getHashCode));
        }

        public void Add(Type type, IEqualityComparer equalityComparer)
        {
            ArgumentNullException.ThrowIfNull(type);
            ArgumentNullException.ThrowIfNull(equalityComparer);
            AddDelegate(type, 
                (x, y) => equalityComparer.Equals(x, y),
                (obj) => obj is null ? 0 : equalityComparer.GetHashCode(obj));
        }
    }
}
*/
