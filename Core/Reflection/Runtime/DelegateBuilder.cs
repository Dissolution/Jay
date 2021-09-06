using System;
using System.Reflection;
using System.Reflection.Emit;
using Jay.Debugging.Dumping;
using Jay.Reflection.Delegates;
using Jay.Reflection.Emission;
using Jay.Text;

namespace Jay.Reflection.Runtime
{
    public static class DelegateBuilder
    {
        internal static string CreateDynamicMethodName(MethodSignature signature)
        {
            return TextBuilder.Build(signature, (text, sig) =>
            {
                if (sig.ReturnType == typeof(void))
                {
                    text.Append("DMAction");
                    if (sig.ParameterCount == 0)
                    {
                        return;
                    }
                }
                else
                {
                    text.Append("DMFunc");
                }
                text.Append('<')
                    .AppendDelimit(',', sig.ParameterTypes, (tb, t) => tb.AppendDump(t));
                if (sig.ReturnType != typeof(void))
                {
                    text.Append(',')
                        .AppendDump(sig.ReturnType);
                }

                text.Append('>')
                    .Transform(RuntimeBuilder.FixChar);
            });
        }

        internal static string FixDynamicMethodName(string name)
        {
            return TextBuilder.Start(name,
                                     text => text.Transform((c, i) => RuntimeBuilder.IsValidIdentifierChar(c, i == 0)
                                                                ? c
                                                                : '_'));
        }

        public static DynamicMethod CreateDynamicMethod(MethodSignature signature)
        {
            return new DynamicMethod(CreateDynamicMethodName(signature),
                                     MethodAttributes.Public | MethodAttributes.Static,
                                     CallingConventions.Standard,
                                     signature.ReturnType,
                                     signature.ParameterTypes,
                                     RuntimeBuilder.Module,
                                     true);
        }
        
        public static DynamicMethod CreateDynamicMethod(string? name, 
                                                        MethodSignature signature)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = CreateDynamicMethodName(signature);
            }
            else
            {
                name = FixDynamicMethodName(name);
            }
            return new DynamicMethod(name,
                                     MethodAttributes.Public | MethodAttributes.Static,
                                     CallingConventions.Standard,
                                     signature.ReturnType,
                                     signature.ParameterTypes,
                                     RuntimeBuilder.Module,
                                     true);
        }

        public static DynamicMethod<TDelegate> CreateDynamicMethod<TDelegate>(string? name = null)
            where TDelegate : Delegate
        {
            return new DynamicMethod<TDelegate>(CreateDynamicMethod(name, MethodSignature.Of<TDelegate>()));
        }

        public static Delegate Generate(Type delegateType, Action<IFluentILGenerator> generate)
        {
            var dynamicMethod = CreateDynamicMethod(MethodSignature.Of(delegateType));
            var generator = new AttachedFluentILGenerator(dynamicMethod.GetILGenerator());
            generate(generator);
            return dynamicMethod.CreateDelegate(delegateType);
        }
        
        public static TDelegate Generate<TDelegate>(Action<IFluentILGenerator> emission)
            where TDelegate : Delegate
        {
            var dynamicMethod = CreateDynamicMethod<TDelegate>();
            var emitter = new AttachedFluentILGenerator(dynamicMethod.GetILGenerator());
            emission(emitter);
            return dynamicMethod.CreateDelegate();
        }
        
        public static Delegate Emit(Type delegateType, Action<FluentILEmitter> emission)
        {
            var dynamicMethod = CreateDynamicMethod(MethodSignature.Of(delegateType));
            var emitter = new FluentILEmitter(dynamicMethod.GetILGenerator());
            emission(emitter);
            return dynamicMethod.CreateDelegate(delegateType);
        }
        
        public static TDelegate Emit<TDelegate>(Action<FluentILEmitter> emission)
            where TDelegate : Delegate
        {
            var dynamicMethod = CreateDynamicMethod<TDelegate>();
            var emitter = new FluentILEmitter(dynamicMethod.GetILGenerator());
            emission(emitter);
            return dynamicMethod.CreateDelegate();
        }
           
        public static TDelegate Build<TDelegate>(Action<DynamicMethod<TDelegate>> buildMethod)
            where TDelegate : Delegate
        {
            var dynamicMethod = CreateDynamicMethod<TDelegate>();
            buildMethod(dynamicMethod);
            return dynamicMethod.CreateDelegate();
        }
    }
}