using Jay.Debugging;
using Jay.Debugging.Dumping;
using Jay.Text;
using System;
using System.Reflection;

namespace Jay.Reflection
{
    public readonly struct MethodSig : IEquatable<MethodSig>,
                                       IEquatable<MethodBase>,
                                       IEquatable<Type>,
                                       IEquatable<Delegate>
    {
        public static bool operator ==(MethodSig sig, MethodSig check) => sig.Equals(check);
        public static bool operator !=(MethodSig sig, MethodSig check) => !sig.Equals(check);
        
        public static MethodSig For(MethodBase methodBase)
        {
            if (methodBase is null)
                throw new ArgumentNullException(nameof(methodBase));
            return new MethodSig(methodBase.GetParameterTypes(),
                                 methodBase.GetReturnType(),
                                 methodBase.IsStatic);
        }

        public static MethodSig For(Delegate @delegate)
        {
            if (@delegate is null)
                throw new ArgumentNullException(nameof(@delegate));
            return For(@delegate.Method);
        }

        public static MethodSig For(Type delegateType)
        {
            if (delegateType is null)
                throw new ArgumentNullException(nameof(delegateType));
            if (!delegateType.Implements<Delegate>())
                throw new ArgumentException("The given type is not a delegate type", nameof(delegateType));
            var invokeMethod = delegateType.GetMethod("Invoke", Reflect.AllFlags);
            if (invokeMethod is null)
                throw new ArgumentException("The given delegate type does not have an Invoke method", nameof(delegateType));
            return For(invokeMethod);
        }

        public static MethodSig For<TDelegate>()
            where TDelegate : Delegate
        {
            var invokeMethod = typeof(TDelegate).GetMethod("Invoke", Reflect.AllFlags);
            if (invokeMethod is null)
                throw new ArgumentException("The given delegate type does not have an Invoke method", nameof(TDelegate));
            return For(invokeMethod);
        }

        public readonly Type[] ParameterTypes;
        public int ParameterCount => ParameterTypes.Length;
        public readonly Type ReturnType;
        public readonly bool IsStatic;

        private MethodSig(Type[] parameterTypes, Type returnType, bool isStatic)
        {
            this.ParameterTypes = parameterTypes;
            this.ReturnType = returnType;
            this.IsStatic = isStatic;
        }

        public bool Equals(MethodSig methodSig)
        {
            if (methodSig.ReturnType != this.ReturnType) return false;
            if (methodSig.ParameterTypes.Length != this.ParameterTypes.Length) return false;
            for (var i = 0; i < this.ParameterTypes.Length; i++)
            {
                if (methodSig.ParameterTypes[i] != this.ParameterTypes[i]) return false;
            }
            return true;
        }

        public bool Equals(MethodBase? methodBase)
        {
            if (methodBase is null) return false;
            if (methodBase.GetReturnType() != this.ReturnType) return false;
            var methodParamTypes = methodBase.GetParameterTypes();
            if (methodParamTypes.Length != this.ParameterTypes.Length) return false;
            for (var i = 0; i < this.ParameterTypes.Length; i++)
            {
                if (methodParamTypes[i] != this.ParameterTypes[i]) return false;
            }
            return true;
        }

        public bool Equals(Type? delegateType)
        {
            var invokeMethod = delegateType.GetInvokeMethod();
            if (invokeMethod is null) return false;
            if (invokeMethod.ReturnType != this.ReturnType) return false;
            var methodParamTypes = invokeMethod.GetParameterTypes();
            if (methodParamTypes.Length != this.ParameterTypes.Length) return false;
            for (var i = 0; i < this.ParameterTypes.Length; i++)
            {
                if (methodParamTypes[i] != this.ParameterTypes[i]) return false;
            }
            return true;
        }

        public bool Equals(Delegate? @delegate)
        {
            var invokeMethod = @delegate?.Method;
            if (invokeMethod is null) return false;
            if (invokeMethod.ReturnType != this.ReturnType) return false;
            var methodParamTypes = invokeMethod.GetParameterTypes();
            if (methodParamTypes.Length != this.ParameterTypes.Length) return false;
            for (var i = 0; i < this.ParameterTypes.Length; i++)
            {
                if (methodParamTypes[i] != this.ParameterTypes[i]) return false;
            }
            return true;
        }

        public override bool Equals(object? obj)
        {
            if (obj is MethodSig methodSig)
                return Equals(methodSig);
            if (obj is MethodBase methodBase)
                return Equals(methodBase);
            if (obj is Type delegateType)
                return Equals(delegateType);
            if (obj is Delegate del)
                return Equals(del);
            return false;
        }

        public override int GetHashCode()
        {
            return Hasher.New
                         .Add<Type>(ParameterTypes)
                         .Add<Type>(ReturnType)
                         .ToHashCode();
        }

        public override string ToString()
        {
            return TextBuilder.Build(this, (builder, sig) 
                   => builder.AppendDump(sig.ReturnType)
                             .Append('(')
                             .AppendDelimit(',', sig.ParameterTypes, (tb, t) => tb.AppendDump(t))
                             .Append(')'));
        }
    }
}