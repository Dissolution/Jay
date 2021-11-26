using System;
using System.Collections.Concurrent;

namespace Jay
{
    internal static class Identifiers
    {
        private static readonly ConcurrentDictionary<Type, int> _stringIdentifierCache =
            new ConcurrentDictionary<Type, int>();

        static Identifiers()
        {
            
        }

        public static int GetNextId<T>(T? instance = default)
        {
            return _stringIdentifierCache.AddOrUpdate(typeof(T), _ => 1, (_, i) => i + 1);
        }
    }
    
    
}