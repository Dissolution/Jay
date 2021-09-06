using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Jay
{
    internal static class Identifiers
    {
        private static readonly ConcurrentDictionary<string, int> _stringIdentifierCache =
            new ConcurrentDictionary<string, int>();

        static Identifiers()
        {
            
        }

        public static int GetNextId(string? key)
        {
            return _stringIdentifierCache.AddOrUpdate(key ?? string.Empty, k => 0, (k, id) => id + 1);
        }
        
        public static void GetMyId<T>([AllowNull] T instance, out int id)
        {
            id = GetNextId(typeof(T).GUID.ToString("N"));
        }
    }
    
    
}