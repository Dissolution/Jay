﻿using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace Jay.Reflection.Emission;

public sealed class ILGeneratorEmitter : IILGeneratorEmitter
{
    private readonly ILGenerator _ilGenerator;

    public InstructionStream Instructions { get; }
    public int ILOffset => _ilGenerator.ILOffset;

    public ILGeneratorEmitter(ILGenerator ilGenerator)
    {
        ArgumentNullException.ThrowIfNull(ilGenerator);
        _ilGenerator = ilGenerator;
        this.Instructions = new();
    }

    public IILGeneratorEmitter BeginCatchBlock(Type exceptionType)
    {
        ArgumentNullException.ThrowIfNull(exceptionType);
        if (!exceptionType.Implements<Exception>())
            throw new ArgumentException($"{nameof(exceptionType)} is not a valid Exception Type", nameof(exceptionType));
        _ilGenerator.BeginCatchBlock(exceptionType);
        var inst = new Instruction(this.ILOffset, ILGeneratorMethod.BeginCatchBlock, exceptionType);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter BeginExceptFilterBlock()
    {
        _ilGenerator.BeginExceptFilterBlock();
        var inst = new Instruction(this.ILOffset, ILGeneratorMethod.BeginExceptFilterBlock);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter BeginExceptionBlock(out Label label)
    {
        label = _ilGenerator.BeginExceptionBlock();
        var inst = new Instruction(this.ILOffset, ILGeneratorMethod.BeginExceptionBlock, label);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter EndExceptionBlock()
    {
        _ilGenerator.EndExceptionBlock();
        var inst = new Instruction(this.ILOffset, ILGeneratorMethod.EndExceptionBlock);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter BeginFaultBlock()
    {
        _ilGenerator.BeginFaultBlock();
        var inst = new Instruction(this.ILOffset, ILGeneratorMethod.BeginFaultBlock);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter BeginFinallyBlock()
    {
        _ilGenerator.BeginFinallyBlock();
        var inst = new Instruction(this.ILOffset, ILGeneratorMethod.BeginFinallyBlock);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter BeginScope()
    {
        _ilGenerator.BeginScope();
        var inst = new Instruction(this.ILOffset, ILGeneratorMethod.BeginScope);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter EndScope()
    {
        _ilGenerator.EndScope();
        var inst = new Instruction(this.ILOffset, ILGeneratorMethod.EndScope);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter UsingNamespace(string @namespace)
    {
        // TODO: Validate namespace
        _ilGenerator.UsingNamespace(@namespace);
        var inst = new Instruction(this.ILOffset, ILGeneratorMethod.UsingNamespace, @namespace);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter DeclareLocal(Type localType, out LocalBuilder local)
    {
        ArgumentNullException.ThrowIfNull(localType);
        local = _ilGenerator.DeclareLocal(localType);
        var inst = new Instruction(this.ILOffset, ILGeneratorMethod.DeclareLocal, localType, local);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter DeclareLocal(Type localType, bool pinned, out LocalBuilder local)
    {
        ArgumentNullException.ThrowIfNull(localType);
        local = _ilGenerator.DeclareLocal(localType, pinned);
        var inst = new Instruction(this.ILOffset, ILGeneratorMethod.DeclareLocal, localType, pinned, local);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter DefineLabel(out Label label)
    {
        label = _ilGenerator.DefineLabel();
        var inst = new Instruction(this.ILOffset, ILGeneratorMethod.DefineLabel, label);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter MarkLabel(Label label)
    {
        _ilGenerator.MarkLabel(label);
        var inst = new Instruction(this.ILOffset, ILGeneratorMethod.MarkLabel, label);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter EmitCall(MethodInfo method, params Type[]? optionParameterTypes)
    {
        _ilGenerator.EmitCall(method.GetCallOpCode(),
            method,
            optionParameterTypes);
        var inst = new Instruction(this.ILOffset, ILGeneratorMethod.EmitCall, method, optionParameterTypes);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter EmitCalli(CallingConvention convention, Type? returnType, Type[]? parameterTypes)
    {
        _ilGenerator.EmitCalli(
            OpCodes.Calli,
            convention,
            returnType,
            parameterTypes);
        var inst = new Instruction(this.ILOffset, ILGeneratorMethod.EmitCalli, convention, returnType, parameterTypes);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter EmitCalli(CallingConventions conventions, Type? returnType, Type[]? parameterTypes, params Type[]? optionParameterTypes)
    {
        _ilGenerator.EmitCalli(OpCodes.Calli,
            conventions,
            returnType,
            parameterTypes,
            optionParameterTypes);
        var inst = new Instruction(this.ILOffset, ILGeneratorMethod.EmitCalli, conventions, returnType, parameterTypes, optionParameterTypes);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter ThrowException(Type exceptionType)
    {
        ArgumentNullException.ThrowIfNull(exceptionType);
        if (!exceptionType.IsAssignableTo(typeof(Exception)))
            throw new ArgumentException($"{nameof(exceptionType)} is not a valid Exception Type", nameof(exceptionType));
        _ilGenerator.ThrowException(exceptionType);
        var inst = new Instruction(this.ILOffset, ILGeneratorMethod.ThrowException, exceptionType);
        this.Instructions.AddLast(inst);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode)
    {
        var inst = new Instruction(ILOffset, opCode);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, byte value)
    {
        var inst = new Instruction(ILOffset, opCode, value);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, value);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, sbyte value)
    {
        var inst = new Instruction(ILOffset, opCode, value);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, value);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, short value)
    {
        var inst = new Instruction(ILOffset, opCode, value);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, value);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, int value)
    {
        var inst = new Instruction(ILOffset, opCode, value);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, value);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, long value)
    {
        var inst = new Instruction(ILOffset, opCode, value);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, value);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, float value)
    {
        var inst = new Instruction(ILOffset, opCode, value);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, value);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, double value)
    {
        var inst = new Instruction(ILOffset, opCode, value);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, value);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, string str)
    {
        var inst = new Instruction(ILOffset, opCode, str);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, str);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, FieldInfo field)
    {
        var inst = new Instruction(ILOffset, opCode, field);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, field);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, MethodInfo method)
    {
        var inst = new Instruction(ILOffset, opCode, method);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, method);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, ConstructorInfo ctor)
    {
        var inst = new Instruction(ILOffset, opCode, ctor);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, ctor);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, SignatureHelper signature)
    {
        var inst = new Instruction(ILOffset, opCode, signature);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, signature);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, Type type)
    {
        var inst = new Instruction(ILOffset, opCode, type);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, type);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, LocalBuilder local)
    {
        var inst = new Instruction(ILOffset, opCode, local);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, local);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, Label label)
    {
        var inst = new Instruction(ILOffset, opCode, label);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, label);
        return this;
    }

    public IILGeneratorEmitter Emit(OpCode opCode, params Label[] labels)
    {
        var inst = new Instruction(ILOffset, opCode, labels);
        this.Instructions.AddLast(inst);
        _ilGenerator.Emit(opCode, labels);
        return this;
    }

    public override string ToString() => Instructions.ToString();
}
