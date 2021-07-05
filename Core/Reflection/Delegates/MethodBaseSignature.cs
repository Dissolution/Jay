using System;
using System.Reflection;
using Jay.Reflection.Emission;

namespace Jay.Reflection.Delegates
{
    internal sealed class MethodBaseSignature : MethodSignature,
                                                IEquatable<MethodBase>,
                                                IEquatable<Delegate>,
                                                IEquatable<Type>
    {
        private readonly MethodBase _method;
        private Type? _ownerType = null;
        private ParameterInfo[]? _parameters = null;
        private Type? _returnType = null;

        public override Type OwnerType => _ownerType ??= (_method.ReflectedType ?? _method.DeclaringType ?? typeof(MethodSignature));
        public override string Name => _method.Name;
        public override ParameterInfo[] Parameters => _parameters ??= _method.GetParameters();
        public override Type ReturnType => _returnType ??= _method.GetReturnType();
        
        internal MethodBaseSignature(MethodBase method)
        {
            _method = method;
        }

        /// <inheritdoc />
        public bool Equals(MethodBase? method)
        {
            if (method is not null)
            {
                return method.GetReturnType() == this.ReturnType &&
                       _parametersEqualityComparer.Equals(method.GetParameters(), this.Parameters);
            }
            return false;
        }

        /// <inheritdoc />
        public bool Equals(Delegate? delegat) => Equals(delegat?.Method);

        /// <inheritdoc />
        public bool Equals(Type? type)
        {
            if (type is null) return false;
            if (type.Implements<Delegate>())
            {
                var method = MemberDelegateCache.GetInvokeMethod(type);
                return Equals(method);
            }
            // TODO: Possible other types?
            return false;
        }
    }
}