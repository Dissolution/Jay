using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace Jay.Reflection.Emission;

internal class GenEmitter : IILGeneratorEmitter, IILGeneratorFluentEmitter
{
    internal readonly ILGenerator _ilGenerator;
    internal readonly IILGeneratorEmitter _this;

    public InstructionStream<Instruction> Instructions { get; }
    public int ILOffset => _ilGenerator.ILOffset;

    public IILGeneratorEmitter Generator => _this;
    public IILGeneratorEmitter OpCode => _this;

    public GenEmitter(ILGenerator ilGenerator)
    {
        ArgumentNullException.ThrowIfNull(ilGenerator);
        _ilGenerator = ilGenerator;
        this.Instructions = new();
        _this = this;
    }

    public IILGeneratorEmitter AppendAll(InstructionStream<Instruction> instructions)
    {
        throw new NotImplementedException();
    }

    public IILGeneratorEmitter BeginCatchBlock(Type exceptionType)
    {
        ArgumentNullException.ThrowIfNull(exceptionType);
        if (!exceptionType.IsAssignableTo(typeof(Exception)))
            throw new ArgumentException($"{nameof(exceptionType)} is not a valid Exception Type", nameof(exceptionType));
        _ilGenerator.BeginCatchBlock(exceptionType);
        var inst = new GeneratorInstruction(ILGeneratorMethod.BeginCatchBlock, exceptionType);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter BeginExceptFilterBlock()
    {
        _ilGenerator.BeginExceptFilterBlock();
        var inst = new GeneratorInstruction(ILGeneratorMethod.BeginExceptFilterBlock);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter BeginExceptionBlock(out Label label)
    {
        label = _ilGenerator.BeginExceptionBlock();
        var inst = new GeneratorInstruction(ILGeneratorMethod.BeginExceptionBlock, label);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter EndExceptionBlock()
    {
        _ilGenerator.EndExceptionBlock();
        var inst = new GeneratorInstruction(ILGeneratorMethod.EndExceptionBlock);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter BeginFaultBlock()
    {
        _ilGenerator.BeginFaultBlock();
        var inst = new GeneratorInstruction(ILGeneratorMethod.BeginFaultBlock);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter BeginFinallyBlock()
    {
        _ilGenerator.BeginFinallyBlock();
        var inst = new GeneratorInstruction(ILGeneratorMethod.BeginFinallyBlock);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter BeginScope()
    {
        _ilGenerator.BeginScope();
        var inst = new GeneratorInstruction(ILGeneratorMethod.BeginScope);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter EndScope()
    {
        _ilGenerator.EndScope();
        var inst = new GeneratorInstruction(ILGeneratorMethod.EndScope);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter UsingNamespace(string @namespace)
    {
        // TODO: Validate namespace
        _ilGenerator.UsingNamespace(@namespace);
        var inst = new GeneratorInstruction(ILGeneratorMethod.UsingNamespace, @namespace);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter DeclareLocal(Type localType, out LocalBuilder local)
    {
        ArgumentNullException.ThrowIfNull(localType);
        local = _ilGenerator.DeclareLocal(localType);
        var inst = new GeneratorInstruction(ILGeneratorMethod.DeclareLocal, localType, local);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter DeclareLocal(Type localType, bool pinned, out LocalBuilder local)
    {
        ArgumentNullException.ThrowIfNull(localType);
        local = _ilGenerator.DeclareLocal(localType, pinned);
        var inst = new GeneratorInstruction(ILGeneratorMethod.DeclareLocal, localType, pinned, local);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter DefineLabel(out Label label)
    {
        label = _ilGenerator.DefineLabel();
        var inst = new GeneratorInstruction(ILGeneratorMethod.DefineLabel, label);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter MarkLabel(Label label)
    {
        _ilGenerator.MarkLabel(label);
        var inst = new GeneratorInstruction(ILGeneratorMethod.MarkLabel, label);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter Call(MethodInfo method, params Type[] optionParameterTypes)
    {
        _ilGenerator.EmitCall(method.GetCallOpCode(),
            method,
            optionParameterTypes);
        var inst = new GeneratorInstruction(ILGeneratorMethod.EmitCall, method, optionParameterTypes);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter Calli(CallingConvention convention, Type returnType, params Type[] parameterTypes)
    {
        _ilGenerator.EmitCalli(
            OpCodes.Calli,
            convention,
            returnType,
            parameterTypes);
        var inst = new GeneratorInstruction(ILGeneratorMethod.EmitCalli, convention, returnType, parameterTypes);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter Calli(CallingConventions conventions, Type returnType, Type[] parameterTypes,
                                     params Type[] optionParameterTypes)
    {
        _ilGenerator.EmitCalli(OpCodes.Calli,
            conventions,
            returnType,
            parameterTypes,
            optionParameterTypes);
        var inst = new GeneratorInstruction(ILGeneratorMethod.EmitCalli, conventions, returnType, parameterTypes, optionParameterTypes);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IWriter<IILGeneratorEmitter> Write => new Writer(this);

    public IILGeneratorEmitter ThrowException(Type exceptionType)
    {
        ArgumentNullException.ThrowIfNull(exceptionType);
        if (!exceptionType.IsAssignableTo(typeof(Exception)))
            throw new ArgumentException($"{nameof(exceptionType)} is not a valid Exception Type", nameof(exceptionType));
        _ilGenerator.ThrowException(exceptionType);
        var inst = new GeneratorInstruction(ILGeneratorMethod.ThrowException, exceptionType);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode)
    {
        var inst = new OpCodeInstruction(ILOffset, opCode);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, byte value)
    {
        var inst = new OpCodeInstruction(ILOffset, opCode, value);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, value);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, sbyte value)
    {
        var inst = new OpCodeInstruction(ILOffset, opCode, value);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, value);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, short value)
    {
        var inst = new OpCodeInstruction(ILOffset, opCode, value);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, value);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, int value)
    {
        var inst = new OpCodeInstruction(ILOffset, opCode, value);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, value);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, long value)
    {
        var inst = new OpCodeInstruction(ILOffset, opCode, value);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, value);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, float value)
    {
        var inst = new OpCodeInstruction(ILOffset, opCode, value);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, value);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, double value)
    {
        var inst = new OpCodeInstruction(ILOffset, opCode, value);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, value);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, string str)
    {
        var inst = new OpCodeInstruction(ILOffset, opCode, str);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, str);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, FieldInfo field)
    {
        var inst = new OpCodeInstruction(ILOffset, opCode, field);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, field);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, MethodInfo method)
    {
        var inst = new OpCodeInstruction(ILOffset, opCode, method);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, method);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, ConstructorInfo ctor)
    {
        var inst = new OpCodeInstruction(ILOffset, opCode, ctor);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, ctor);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, SignatureHelper signature)
    {
        var inst = new OpCodeInstruction(ILOffset, opCode, signature);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, signature);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, Type type)
    {
        var inst = new OpCodeInstruction(ILOffset, opCode, type);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, type);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, LocalBuilder local)
    {
        var inst = new OpCodeInstruction(ILOffset, opCode, local);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, local);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, Label label)
    {
        var inst = new OpCodeInstruction(ILOffset, opCode, label);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, label);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, params Label[] labels)
    {
        var inst = new OpCodeInstruction(ILOffset, opCode, labels);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, labels);
        return this;
    }
}