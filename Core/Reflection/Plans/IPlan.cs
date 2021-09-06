using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Jay.Collections;

namespace Jay.Reflection.Plans
{
    public static class Plan
    {
        private static readonly ConcurrentTypeCache<IPlan> _cache;

        static Plan()
        {
            _cache = new ConcurrentTypeCache<IPlan>();
        }

        private static IPlan CreatePlan(Type type)
        {
            throw new NotImplementedException();
        }
        
        public static IPlan For(Type type)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            return _cache.GetOrAdd(type, t => CreatePlan(t));
        }

        public static IPlan<T> For<T>()
        {
            return (_cache.GetOrAdd<T>(type => CreatePlan(type)) as IPlan<T>)!;
        }

        public static Result TryFindMember<T>(Expression<Func<T, object?>> memberExpr,
                                              [NotNullWhen(true)] out MemberInfo member)
        {
            return For<T>().TryFindMember(memberExpr, out member);
        }

        public static Result TryFindMember<T, TMember>(Expression<Func<Type, TMember>> findMember,
                                                       [NotNullWhen(true)] out TMember member)
            where TMember : MemberInfo
        {
            //return For<T>().TryFindMember<TMember>(findMember, out member);
            throw new NotImplementedException();
        }
    }

    [Flags]
    public enum NameMatch
    {
        Exact = 0,
        
        IgnoreCase = 1 << 0,
        StartsWith = 1 << 1,
        EndsWith = 1 << 2,
        Contains = 1 << 3 | StartsWith | EndsWith,
        
        Any = IgnoreCase | Contains,
    }

    public interface IMembers<TMember>
        where TMember : MemberInfo
    {
        IEnumerable<T> Select<T>(Func<TMember, T> selection);

        IMembers<TMember> Where(string name,
                                NameMatch nameMatch = NameMatch.Exact);

        IMembers<TMember> Where(Visibility visibility);

        IMembers<UMember> Where<UMember>() 
            where UMember : TMember;

        List<TMember> ToList();
    }
    
    
    public interface IPlan
    {
        Type Type { get; }

        IMembers<TMember> Members<TMember>()
            where TMember : MemberInfo;

        IMembers<MemberInfo> Members();

        Result TryFindMember<TMember>(Expression<Func<IMembers<TMember>, TMember>> findMember,
                                      [NotNullWhen(true)] out TMember member)
            where TMember : MemberInfo;
        
        
    }

    public interface IPlan<T> : IPlan
    {
        Result TryFindMember<TMember>(Expression<Func<T, object?>> memberExpr,
                                      [NotNullWhen(true)] out TMember member)
            where TMember : MemberInfo;
    }
}