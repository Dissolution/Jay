using System.Collections.Concurrent;
using Jay.Reflection.Caching;

namespace Jay.Reflection.Collections;


public class ConcurrentMemberDelegateMap : ConcurrentDictionary<MemberDelegate, Delegate>
{
    public ConcurrentMemberDelegateMap()
        : base() { }
    public ConcurrentMemberDelegateMap(int capacity)
        : base(Environment.ProcessorCount, capacity) { }

    public TDelegate GetOrAdd<TMember, TDelegate>(MemberDelegate<TMember, TDelegate> memberDelegate,
        Func<MemberDelegate<TMember, TDelegate>, TDelegate> createDelegate)
        where TMember : MemberInfo
        where TDelegate : Delegate
    {
        var del = base.GetOrAdd(memberDelegate, _ => createDelegate(memberDelegate));
        if (del is TDelegate tDelegate)
            return tDelegate;
        throw new InvalidOperationException();
    }
}