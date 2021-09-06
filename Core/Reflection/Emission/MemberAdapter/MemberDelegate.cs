using System;
using System.Reflection;
using Jay.Debugging.Dumping;
using Jay.Text;
using JetBrains.Annotations;

namespace Jay.Reflection.Emission
{
    internal class MemberDelegate<TDelegate> : MemberDelegate,
                                               IEquatable<MemberDelegate<TDelegate>>
        where TDelegate : Delegate
    {
        /// <inheritdoc />
        public MemberDelegate([NotNull] MemberInfo memberInfo) 
            : base(memberInfo, typeof(TDelegate))
        {
        }

        /// <inheritdoc />
        public bool Equals(MemberDelegate<TDelegate>? memberDelegate)
        {
            if (ReferenceEquals(null, memberDelegate)) return false;
            if (ReferenceEquals(this, memberDelegate)) return true;
            return memberDelegate.Member == this.Member &&
                   memberDelegate.DelegateType == this.DelegateType;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is MemberDelegate<TDelegate> memberDelegate)
                return Equals(memberDelegate);
            return base.Equals(obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return base.ToString();
        }
    }
    
    internal class MemberDelegate : IEquatable<MemberDelegate>
    {
        public static MemberDelegate<TDelegate> Create<TDelegate>(MemberInfo memberInfo)
            where TDelegate : Delegate
            => new MemberDelegate<TDelegate>(memberInfo);
        
        public MemberInfo Member { get; }
        public Type DelegateType { get; }

        public MemberDelegate(MemberInfo memberInfo, Type delegateType)
        {
            this.Member = memberInfo;
            this.DelegateType = delegateType;
        }

        /// <inheritdoc />
        public bool Equals(MemberDelegate? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Member == this.Member &&
                   other.DelegateType == this.DelegateType;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is MemberDelegate memberDelegate)
                return Equals(memberDelegate);
            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode() => Hasher.Create(Member, DelegateType);

        /// <inheritdoc />
        public override string ToString()
        {
            return TextBuilder.Build(this, (tb, memberDelegate) => tb.Append('(')
                                                                     .AppendDump(memberDelegate)
                                                                     .Append(')')
                                                                     .AppendDump(Member));
        }
    }
}