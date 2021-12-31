using System.Collections.Concurrent;
using System.Reflection;

namespace Jay.Reflection;

public sealed class DelegateMemberCache : ConcurrentDictionary<DelegateMember, Delegate?>
{
    public bool TryGetDelegate<TDelegate>(MemberInfo member, out TDelegate? @delegate)
        where TDelegate : Delegate
    {
        if (base.TryGetValue(DelegateMember.Create<TDelegate>(member), out var del))
        {
            return del.Is(out @delegate);
        }
        @delegate = default;
        return false;
    }

    public TDelegate? GetOrAdd<TDelegate>(MemberInfo member, Func<DelegateMember, TDelegate?> createDelegate)
        where TDelegate : Delegate
    {
        return base.GetOrAdd(DelegateMember.Create<TDelegate>(member), createDelegate) as TDelegate;
    }
}