using System;
using System.Reflection.Emit;
using Jay.Reflection.Emission;

namespace Jay.Reflection
{
    public static class BuilderExtensions
    {
        public static void Emit(this ConstructorBuilder constructorBuilder,
                                Action<FluentILEmitter> emit)
        {
            var emitter = new FluentILEmitter(constructorBuilder.GetILGenerator());
            emit(emitter);
        }
        
        public static void Emit(this MethodBuilder methodBuilder,
                                Action<FluentILEmitter> emit)
        {
            var emitter = new FluentILEmitter(methodBuilder.GetILGenerator());
            emit(emitter);
        }
        
        public static FluentILEmitter GetEmitter(this MethodBuilder methodBuilder)
        {
            return new FluentILEmitter(methodBuilder.GetILGenerator());
        }
    
    }
}