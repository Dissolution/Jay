using System.Collections.Concurrent;
using Jay.Reflection.Exceptions;

namespace Jay.Reflection.Caching;

public static class MemberDelegateCache
{
    private static readonly ConcurrentDictionary<MemberDelegate, Delegate> _cache = new();
    
    public static bool TryGetDelegate<TDelegate>(
        MemberInfo member, 
        [NotNullWhen(true)] out TDelegate? @delegate)
        where TDelegate : Delegate
    {
        if (_cache.TryGetValue(MemberDelegate.Create<TDelegate>(member), out var del))
        {
            return del.Is(out @delegate);
        }
        
        @delegate = default;
        return false;
    }

    public static TDelegate GetOrAdd<TDelegate>(MemberInfo member, Func<MemberDelegate, TDelegate> createDelegate)
        where TDelegate : Delegate
    {
        var key = MemberDelegate.Create<TDelegate>(member);
        var del = _cache.GetOrAdd(key, createDelegate(key));
        if (del is not TDelegate typedDelegate)
        {
            throw new ReflectedException($"{member.MemberType} '{member}' had a delegate '{del}' stored, not the '{typeof(TDelegate)}' requested");
        }
        return typedDelegate;
    }

    public static TDelegate GetOrAdd<TMember, TDelegate>(TMember member, Func<TMember, TDelegate> createDelegate)
        where TMember : MemberInfo
        where TDelegate : Delegate
    {
        var key = MemberDelegate.Create<TDelegate>(member);
        var del = _cache.GetOrAdd(key, createDelegate(member));
        if (del is not TDelegate typedDelegate)
        {
            throw new ReflectedException($"{member.MemberType} '{member}' had a delegate '{del}' stored, not the '{typeof(TDelegate)}' requested");
        }
        return typedDelegate;
    }
}