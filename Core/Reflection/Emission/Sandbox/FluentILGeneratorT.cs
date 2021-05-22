using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace Jay.Reflection.Emission.Sandbox
{
    public abstract class FluentILGenerator<TEmitter> : FluentILGeneratorBase<TEmitter>
        where TEmitter : FluentILGenerator<TEmitter>
    {
        protected readonly ILGenerator _ilGenerator;

        public int ILOffset => _ilGenerator.ILOffset;
        
        internal FluentILGenerator(ILGenerator ilGenerator)
        {
            _ilGenerator = ilGenerator ?? throw new ArgumentNullException(nameof(ilGenerator));
        }

        /// <inheritdoc />
        public override TEmitter BeginCatchBlock(Type exceptionType)
        {
            base.BeginCatchBlock(exceptionType);
            _ilGenerator.BeginCatchBlock(exceptionType);
            return _emitter;
        }

        /// <inheritdoc />
        public override TEmitter BeginExceptionBlock(out Label label)
        {
            label = _ilGenerator.BeginExceptionBlock();
            return _emitter;
        }

        /// <inheritdoc />
        public override TEmitter DeclareLocal(Type localType, out LocalBuilder local)
        {
            local = _ilGenerator.DeclareLocal(localType);
            return _emitter;
        }

        /// <inheritdoc />
        public override TEmitter DeclareLocal(Type localType, bool pinned, out LocalBuilder local)
        {
            local = _ilGenerator.DeclareLocal(localType, pinned);
            return _emitter;
        }

        /// <inheritdoc />
        public override TEmitter DefineLabel(out Label label)
        {
            label = _ilGenerator.DefineLabel();
            return _emitter;
        }

        public TEmitter EmitCall(OpCode opCode, MethodInfo methodInfo, params Type[]? optionalParameterTypes)
        {
            AddInstruction(ILGeneratorMethod.EmitCall, (opCode, methodInfo, optionalParameterTypes));
            _ilGenerator.EmitCall(opCode, methodInfo, optionalParameterTypes);
            return _emitter;
        }
        
        public TEmitter EmitCalli(OpCode opCode, CallingConvention callingConvention, Type? returnType, Type[]? parameterTypes)
        {
            AddInstruction(ILGeneratorMethod.EmitCalli, (opCode, callingConvention, returnType, parameterTypes));
            _ilGenerator.EmitCalli(opCode, callingConvention, returnType, parameterTypes);
            return _emitter;
        }
        
        public TEmitter EmitCalli(OpCode opCode, CallingConventions callingConvention, Type? returnType, Type[]? parameterTypes, Type[] optionalParameterTypes)
        {
            AddInstruction(ILGeneratorMethod.EmitCalli, (opCode, callingConvention, returnType, parameterTypes, optionalParameterTypes));
            _ilGenerator.EmitCalli(opCode, callingConvention, returnType, parameterTypes, optionalParameterTypes);
            return _emitter;
        }
    }
}