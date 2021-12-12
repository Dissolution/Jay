using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace Jay.Reflection.Building
{
    public readonly struct MethodSig : IEquatable<MethodSig>
    {
        public static MethodSig Of<TDelegate>()
            where TDelegate : Delegate
        {
            var invokeMethod = typeof(TDelegate).GetMethod("Invoke",
                                                           BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            Debug.Assert(invokeMethod != null);
            return MethodSig.Of(invokeMethod);
        }

        public static MethodSig Of(MethodBase method)
        {
            return new MethodSig(method.GetParameters(), method.ReturnType());
        }

        public static MethodSig Of(Type delegateType)
        {
            var invokeMethod = delegateType.GetMethod("Invoke",
                                                      BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if (invokeMethod is null)
                throw new ArgumentException("Invalid Delegate Type: Does not have an Invoke method", nameof(delegateType));
            return MethodSig.Of(invokeMethod);
        }

        public readonly Type ReturnType;
        public readonly ParameterInfo[] Parameters;
        public readonly Type[] ParameterTypes;
        public int ParameterCount => Parameters.Length;

        private MethodSig(ParameterInfo[] parameters, Type? returnType)
        {
            this.ReturnType = returnType ?? typeof(void);
            this.Parameters = parameters;
            this.ParameterTypes = new Type[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                ParameterTypes[i] = parameters[i].ParameterType;
            }
        }

        public bool Equals(MethodSig sig)
        {
            return sig.ReturnType == this.ReturnType &&
                Arraye
        }

        public override bool Equals(object? obj)
        {
            if (obj is MethodSig sig)
                return Equals(sig);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

       
    }

    public static class RuntimeBuilder
    {
        public static AssemblyBuilder AssemblyBuilder { get; }
        public static ModuleBuilder ModuleBuilder { get; }

        static RuntimeBuilder()
        {
            AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Jay.Reflection.Building.Dynamic"), AssemblyBuilderAccess.Run);
            ModuleBuilder = AssemblyBuilder.DefineDynamicModule("RuntimeModuleBuilder");
        }

        public static DynamicMethod CreateDynamicMethod(string? name)
        {

        }
    }
}
