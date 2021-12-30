using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Jay.Reflection.Building.Emission;

public interface IEmitter
{
    InstructionStream Instructions { get; }
}

public interface IEmitter<TEmitter> : IEmitter
    where TEmitter : IEmitter<TEmitter>
{
    TEmitter AppendAll(InstructionStream instructions);

    TEmitter Emit(OpCode opCode);
    TEmitter Emit(OpCode opCode, byte arg);
    TEmitter Emit(OpCode opCode, sbyte arg);
    TEmitter Emit(OpCode opCode, short arg);
    TEmitter Emit(OpCode opCode, int arg);
    TEmitter Emit(OpCode opCode, long arg);
    TEmitter Emit(OpCode opCode, float arg);
    TEmitter Emit(OpCode opCode, double arg);
    TEmitter Emit(OpCode opCode, string str);
    TEmitter Emit(OpCode opCode, FieldInfo field);
    TEmitter Emit(OpCode opCode, MethodInfo method);
    TEmitter Emit(OpCode opCode, ConstructorInfo ctor);
    TEmitter Emit(OpCode opCode, Type type);
    TEmitter Emit(OpCode opCode, SignatureHelper signature);
    TEmitter Emit(OpCode opCode, LocalBuilder local);
    TEmitter Emit(OpCode opCode, Label label);
    TEmitter Emit(OpCode opCode, params Label[] labels);
    //
    // TEmitter EmitCalli(OpCode opCode, CallingConventions callingConventions,
    //                    Type? returnType, Type[]? parameterTypes, Type[]? optionalParameterTypes);
    //
    // TEmitter EmitCalli(OpCode opCode, CallingConvention unmanagedCallingConvention,
    //                    Type? returnType, Type[]? parameterTypes);
    //
    // TEmitter EmitCall(OpCode opCode, MethodInfo method, Type[]? optionalParameterTypes);
    //
    // TEmitter Try(Action<ITryEmitter<TEmitter>> tryEmitter);
    //
    // /// <summary>
    // /// Begin an Exception block.  Creating an Exception block records some information,
    // /// but does not actually emit any IL onto the stream.  Exceptions should be created and
    // /// marked in the following form:
    // /// Emit Some IL
    // /// BeginExceptionBlock
    // /// Emit the IL which should appear within the "try" block
    // /// BeginCatchBlock
    // /// Emit the IL which should appear within the "catch" block
    // /// Optional: BeginCatchBlock (this can be repeated an arbitrary number of times
    // /// EndExceptionBlock
    // /// </summary>
    // /// <param name="label"></param>
    // /// <returns></returns>
    // TEmitter BeginExceptionBlock(out Label label);

}

internal abstract class Emitter : IEmitter
{
    protected readonly InstructionStream _instructions;
    protected readonly IInstructionFactory _instructionFactory;

    public InstructionStream Instructions => _instructions;

    protected Emitter(IInstructionFactory instructionFactory)
    {
        _instructions = new InstructionStream();
        _instructionFactory = instructionFactory;
    }

    protected virtual void AddInstruction(OpCode opCode, object? operand = null)
    {
        var instruction = _instructionFactory.Create(opCode, operand);
        _instructions.AddLast(instruction);
    }

    protected virtual void AddInstruction(ILGeneratorMethod method, object? arg = null)
    {
        var instruction = _instructionFactory.Create(method, arg);
        _instructions.AddLast(instruction);
    }
}

internal abstract class Emitter<T> : Emitter,
                                     IEmitter<T>
    where T : Emitter<T>
{
    protected T _this;

    protected Emitter(IInstructionFactory instructionFactory)
        : base(instructionFactory)
    {
        _this = (T)this;
    }

    public T Emit(OpCode opCode)
    {
        AddInstruction(opCode);
        return _this;
    }
}


public interface ITryEmitter<TEmitter>
{
    ITryEmitter<TEmitter> TryBlock(Action<TEmitter> tryBlock);
    ITryEmitter<TEmitter> CatchBlock(Action<TEmitter> catchBlock);
}

