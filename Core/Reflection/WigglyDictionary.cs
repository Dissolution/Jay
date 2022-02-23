using System.Collections.Concurrent;

namespace Jay.Reflection;

public sealed class ConcurrentWigglyDictionary
{
    private readonly ConcurrentDictionary<Box, Box> _concurrentDictionary;

    public ConcurrentWigglyDictionary()
    {
        _concurrentDictionary = new();
    }

    public bool ContainsKey<TKey>(TKey key)
    {
        return _concurrentDictionary.ContainsKey(Box.Wrap(key));
    }
}