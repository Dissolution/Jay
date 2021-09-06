using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace Jay.Reflection.Emission
{
    public class AttachedFluentILGenerator : FluentILGenerator
    {
        protected readonly ILGenerator _ilGenerator;

        /// <inheritdoc />
        public override FluentILEmitter Emitter => new FluentILEmitter(this);

        public int ILOffset => _ilGenerator.ILOffset;
        
        internal AttachedFluentILGenerator(ILGenerator ilGenerator)
        {
            _ilGenerator = ilGenerator ?? throw new ArgumentNullException(nameof(ilGenerator));
        }

        /// <inheritdoc />
        public override IFluentILGenerator BeginCatchBlock(Type exceptionType)
        {
            base.BeginCatchBlock(exceptionType);
            _ilGenerator.BeginCatchBlock(exceptionType);
            return this;
        }

        /// <inheritdoc />
        public override IFluentILGenerator BeginExceptionBlock(out Label label)
        {
            label = _ilGenerator.BeginExceptionBlock();
            return this;
        }

        /// <inheritdoc />
        public override IFluentILGenerator DeclareLocal(Type localType, out LocalBuilder local)
        {
            local = _ilGenerator.DeclareLocal(localType);
            return this;
        }

        /// <inheritdoc />
        public override IFluentILGenerator DeclareLocal(Type localType, bool pinned, out LocalBuilder local)
        {
            local = _ilGenerator.DeclareLocal(localType, pinned);
            return this;
        }

        /// <inheritdoc />
        public override IFluentILGenerator DefineLabel(out Label label)
        {
            label = _ilGenerator.DefineLabel();
            return this;
        }

        public IFluentILGenerator EmitCall(OpCode opCode, MethodInfo methodInfo, params Type[]? optionalParameterTypes)
        {
            AddInstruction(ILGeneratorMethod.EmitCall, (opCode, methodInfo, optionalParameterTypes));
            _ilGenerator.EmitCall(opCode, methodInfo, optionalParameterTypes);
            return this;
        }
        
        public IFluentILGenerator EmitCalli(OpCode opCode, CallingConvention callingConvention, Type? returnType, Type[]? parameterTypes)
        {
            AddInstruction(ILGeneratorMethod.EmitCalli, (opCode, callingConvention, returnType, parameterTypes));
            _ilGenerator.EmitCalli(opCode, callingConvention, returnType, parameterTypes);
            return this;
        }
        
        public IFluentILGenerator EmitCalli(OpCode opCode, CallingConventions callingConvention, Type? returnType, Type[]? parameterTypes, Type[] optionalParameterTypes)
        {
            AddInstruction(ILGeneratorMethod.EmitCalli, (opCode, callingConvention, returnType, parameterTypes, optionalParameterTypes));
            _ilGenerator.EmitCalli(opCode, callingConvention, returnType, parameterTypes, optionalParameterTypes);
            return this;
        }

        public IFluentILGenerator UnboxOrCastclass(Type type)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            if (type.IsValueType)
                return Unbox_Any(type);
            return Castclass(type);
        }

        /// <inheritdoc />
        public override IFluentILGenerator Emit(OpCode opCode)
        {
            base.Emit(opCode);
            _ilGenerator.Emit(opCode);
            return this;
        }

        /// <inheritdoc />
        public override IFluentILGenerator Emit(OpCode opCode, byte value)
        {
            base.Emit(opCode, value);
            _ilGenerator.Emit(opCode, value);
            return this;
        }

        /// <inheritdoc />
        public override IFluentILGenerator Emit(OpCode opCode, sbyte value)
        {
            base.Emit(opCode, value);
            _ilGenerator.Emit(opCode, value);
            return this;
        }

        /// <inheritdoc />
        public override IFluentILGenerator Emit(OpCode opCode, short value)
        {
            base.Emit(opCode, value);
            _ilGenerator.Emit(opCode, value);
            return this;
        }

        /// <inheritdoc />
        public override IFluentILGenerator Emit(OpCode opCode, int value)
        {
            base.Emit(opCode, value);
            _ilGenerator.Emit(opCode, value);
            return this;
        }

        /// <inheritdoc />
        public override IFluentILGenerator Emit(OpCode opCode, long value)
        {
            base.Emit(opCode, value);
            _ilGenerator.Emit(opCode, value);
            return this;
        }

        /// <inheritdoc />
        public override IFluentILGenerator Emit(OpCode opCode, float value)
        {
            base.Emit(opCode, value);
            _ilGenerator.Emit(opCode, value);
            return this;
        }

        /// <inheritdoc />
        public override IFluentILGenerator Emit(OpCode opCode, double value)
        {
            base.Emit(opCode, value);
            _ilGenerator.Emit(opCode, value);
            return this;
        }

        /// <inheritdoc />
        public override IFluentILGenerator Emit(OpCode opCode, string? str)
        {
            base.Emit(opCode, str);
            _ilGenerator.Emit(opCode, str ?? string.Empty);
            return this;
        }

        /// <inheritdoc />
        public override IFluentILGenerator Emit(OpCode opCode, FieldInfo field)
        {
            base.Emit(opCode, field);
            _ilGenerator.Emit(opCode, field);
            return this;
        }

        /// <inheritdoc />
        public override IFluentILGenerator Emit(OpCode opCode, MethodInfo method)
        {
            base.Emit(opCode, method);
            _ilGenerator.Emit(opCode, method);
            return this;
        }

        /// <inheritdoc />
        public override IFluentILGenerator Emit(OpCode opCode, ConstructorInfo ctor)
        {
            base.Emit(opCode, ctor);
            _ilGenerator.Emit(opCode, ctor);
            return this;
        }

        /// <inheritdoc />
        public override IFluentILGenerator Emit(OpCode opCode, SignatureHelper signature)
        {
            base.Emit(opCode, signature);
            _ilGenerator.Emit(opCode, signature);
            return this;
        }

        /// <inheritdoc />
        public override IFluentILGenerator Emit(OpCode opCode, Type type)
        {
            base.Emit(opCode, type);
            _ilGenerator.Emit(opCode, type);
            return this;
        }

        /// <inheritdoc />
        public override IFluentILGenerator Emit(OpCode opCode, LocalBuilder local)
        {
            base.Emit(opCode, local);
            _ilGenerator.Emit(opCode, local);
            return this;
        }

        /// <inheritdoc />
        public override IFluentILGenerator Emit(OpCode opCode, Label label)
        {
            base.Emit(opCode, label);
            _ilGenerator.Emit(opCode, label);
            return this;
        }

        /// <inheritdoc />
        public override IFluentILGenerator Emit(OpCode opCode, params Label[] labels)
        {
            base.Emit(opCode, labels);
            _ilGenerator.Emit(opCode, labels);
            return this;
        }

        /// <inheritdoc />
        public override IFluentILGenerator BeginCatchBlock<TException>()
        {
            base.BeginCatchBlock<TException>();
            _ilGenerator.BeginCatchBlock(typeof(TException));
            return this;
        }

        /// <inheritdoc />
        public override IFluentILGenerator BeginExceptFilterBlock()
        {
            base.BeginExceptFilterBlock();
            _ilGenerator.BeginExceptFilterBlock();
            return this;
        }

        /// <inheritdoc />
        public override IFluentILGenerator EndExceptionBlock()
        {
            base.EndExceptionBlock();
            _ilGenerator.EndExceptionBlock();
            return this;
        }

        /// <inheritdoc />
        public override IFluentILGenerator BeginFaultBlock()
        {
            base.BeginFaultBlock();
            _ilGenerator.BeginFaultBlock();
            return this;
        }

        /// <inheritdoc />
        public override IFluentILGenerator BeginFinallyBlock()
        {
            base.BeginFinallyBlock();
            _ilGenerator.BeginFinallyBlock();
            return this;
        }

        /// <inheritdoc />
        public override IFluentILGenerator MarkLabel(Label label)
        {
            _ilGenerator.MarkLabel(label);
            base.MarkLabel(label);
            return this;
        }

        /// <inheritdoc />
        public override IFluentILGenerator Call(MethodInfo method, params Type[]? optionParameterTypes)
        {
            return base.Call(method, optionParameterTypes);
        }

        /// <inheritdoc />
        public override IFluentILGenerator Calli(CallingConvention convention, Type? returnType, params Type[]? parameterTypes)
        {
            return base.Calli(convention, returnType, parameterTypes);
        }

        /// <inheritdoc />
        public override IFluentILGenerator Calli(CallingConventions conventions, Type? returnType, Type[]? parameterTypes,
                                                 params Type[]? optionParameterTypes)
        {
            return base.Calli(conventions, returnType, parameterTypes, optionParameterTypes);
        }

        /// <inheritdoc />
        public override IFluentILGenerator WriteLine(string? text)
        {
            return base.WriteLine(text);
        }

        /// <inheritdoc />
        public override IFluentILGenerator WriteLine(FieldInfo field)
        {
            return base.WriteLine(field);
        }

        /// <inheritdoc />
        public override IFluentILGenerator WriteLine(LocalBuilder local)
        {
            return base.WriteLine(local);
        }

        /// <inheritdoc />
        public override IFluentILGenerator ThrowException(Type exceptionType)
        {
            return base.ThrowException(exceptionType);
        }

        /// <inheritdoc />
        public override IFluentILGenerator Append(IMSILStream instructions)
        {
            return base.Append(instructions);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return base.ToString();
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}