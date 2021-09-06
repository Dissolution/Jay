using System;
using System.Collections.Generic;
using System.Reflection;
using Jay.Comparison;
using Jay.Reflection.Comparison;
using Jay.Reflection.Emission;

namespace Jay.Reflection.Delegates
{
    public abstract class MethodSignature : IMethodSignature
    {
        public static implicit operator MethodSignature(MethodBase method) => new MethodBaseSignature(method);
        
        protected static readonly IEqualityComparer<ParameterInfo[]> _parametersEqualityComparer;

        static MethodSignature()
        {
            _parametersEqualityComparer =
                new EnumerableEqualityComparer<ParameterInfo>(new ParameterInfoEqualityComparer());
        }
        
        public static MethodSignature Of<TDelegate>()
            where TDelegate : Delegate
            => new MethodBaseSignature(MemberDelegateCache.GetInvokeMethod<TDelegate>());
        public static MethodSignature Of(MethodBase method) => new MethodBaseSignature(method);
        public static MethodSignature Of(Delegate delegat)
        {
            var method = delegat?.Method;
            if (method is null)
                throw new ArgumentNullException(nameof(delegat));
            return new MethodBaseSignature(method);
        }

        public static MethodSignature Of(Type delegateType)
        {
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));
            var invokeMethod = MemberDelegateCache.GetInvokeMethod(delegateType);
            if (invokeMethod is null)
                throw new ArgumentException("Invalid Delegate Type", nameof(delegateType));
            return new MethodBaseSignature(invokeMethod);
        }

        private ArgumentType[]? _argumentTypes = null;
        private Type[]? _parameterTypes = null;
        
        public ArgumentType[] ArgumentTypes
        {
            get
            {
                if (_argumentTypes is null)
                {
                    var parameters = Parameters;
                    _argumentTypes = new ArgumentType[parameters.Length];
                    for (var i = 0; i < parameters.Length; i++)
                    {
                        _argumentTypes[i] = new ParameterType(parameters[i]);
                    }
                }
                return _argumentTypes;
            }
        }
        
        public Type[] ParameterTypes
        {
            get
            {
                if (_parameterTypes is null)
                {
                    var parameters = Parameters;
                    _parameterTypes = new Type[parameters.Length];
                    for (var i = 0; i < parameters.Length; i++)
                    {
                        _parameterTypes[i] = parameters[i].ParameterType;
                    }
                }
                return _parameterTypes;
            }
        }

        /// <inheritdoc />
        public int ParameterCount => Parameters.Length;

        /// <inheritdoc />
        public abstract Type OwnerType { get; }

        /// <inheritdoc />
        public abstract string Name { get; }

        /// <inheritdoc />
        public abstract ParameterInfo[] Parameters { get; }

        /// <inheritdoc />
        public abstract Type ReturnType { get; }

        protected MethodSignature()
        {
            
        }
        
        /// <inheritdoc />
        public bool Equals(IMethodSignature? methodSignature)
        {
            if (methodSignature is not null)
            {
                return methodSignature.ReturnType == this.ReturnType &&
                       _parametersEqualityComparer.Equals(methodSignature.Parameters, this.Parameters);
            }
            return false;
        }
    }
}