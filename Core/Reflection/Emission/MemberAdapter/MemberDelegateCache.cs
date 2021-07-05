using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Jay.Collections;
using Jay.Debugging;

namespace Jay.Reflection.Emission
{
    internal class MemberDelegateCache : ConcurrentDictionary<MemberDelegate, Delegate?>
    {
        private static readonly ConcurrentTypeCache<MethodInfo?> _invokeMethodCache;

        static MemberDelegateCache()
        {
            _invokeMethodCache = new ConcurrentTypeCache<MethodInfo?>();
        }
        
        public static MethodInfo GetInvokeMethod<TDelegate>()
            where TDelegate : Delegate
        {
            var invokeMethod = _invokeMethodCache
                   .GetOrAdd<TDelegate>(type => type.GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance));
            if (invokeMethod is null)
            {
                var type = typeof(TDelegate);
                var methods = type.GetMethods(Reflect.AllFlags)
                                  .Where(method => method.Name == "Invoke")
                                  .ToArray();
                Hold.Debug(methods);
                Debugger.Break();
            }
            return invokeMethod!;
        }
        
        public static MethodInfo? GetInvokeMethod(Type? delegateType)
        {
            if (delegateType is null) return null;
            return _invokeMethodCache
                .GetOrAdd(delegateType, type => type.GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance));
        }
        
        public static MethodInfo? GetInvokeMethod(Delegate? @delegate)
        {
            if (@delegate is null) return null;
            return _invokeMethodCache.GetOrAdd(@delegate.GetType(), @delegate.Method);
        }

       
        public TDelegate GetOrAdd<TMember, TDelegate>(TMember memberInfo,
                                                      Func<TMember, TDelegate> createDelegate)
            where TMember : MemberInfo
            where TDelegate : Delegate
        {
            var key = MemberDelegate.Create<TDelegate>(memberInfo);
            var del = base.GetOrAdd(key, _ => createDelegate(memberInfo));
            return (del as TDelegate)!;
        }
        
        public TDelegate GetOrAdd<TMember, TDelegate>(TMember memberInfo,
                                                      Func<MemberDelegate<TDelegate>, TDelegate> createDelegate)
            where TMember : MemberInfo
            where TDelegate : Delegate
        {
            var key = MemberDelegate.Create<TDelegate>(memberInfo);
            var del = base.GetOrAdd(key, _ => createDelegate(key));
            return (del as TDelegate)!;
        }
    }
}