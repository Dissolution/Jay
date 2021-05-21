using Jay.Randomization;
using Jay.Reflection.Emission;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Jay.Reflection
{
    public class DelegateBuilder
    {
        internal static Module Module { get; } = typeof(DelegateBuilder).Module;
        
        public static string CreateName()
        {
            return Randomizer.Instance.BitString();
        }
        
        public static DynamicMethod CreateDynamicMethod(string? name,
                                                        MethodSig sig) =>
            new DynamicMethod(name ?? CreateName(),
                              MethodAttributes.Public | MethodAttributes.Static,
                              CallingConventions.Standard,
                              sig.ReturnType,
                              sig.ParameterTypes,
                              Module,
                              true);

        public static DynamicMethod CreateDynamicMethod<TDelegate>(string? name)
            where TDelegate : Delegate
            => CreateDynamicMethod(name, MethodSig.For<TDelegate>());

        public static TDelegate Build<TDelegate>(Action<ILEmitter> emission)
            where TDelegate : Delegate
        {
            if (emission is null) 
                throw new ArgumentNullException(nameof(emission));
            var dynamicMethod = CreateDynamicMethod<TDelegate>(null);
            var emitter = new ILEmitter(dynamicMethod.GetILGenerator());
            emission(emitter);
            return dynamicMethod.CreateDelegate<TDelegate>();
        }
    }
}