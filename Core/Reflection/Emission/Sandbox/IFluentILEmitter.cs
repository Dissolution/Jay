using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;

namespace Jay.Reflection.Emission.Sandbox
{
    public interface IFluentILEmitter<TEmitter> : IInstructions
        where TEmitter : IFluentILEmitter<TEmitter>
    {
        TEmitter Try(Action<ITryBuilder<TEmitter>> buildTry);

        TEmitter Scoped(Action<TEmitter> scopedBlock);

        TEmitter Define(out Label label);
        TEmitter Mark(Label label);

        TEmitter Branch(Label label);
        TEmitter Branch(out Label label) => Define(out label).Branch(label);

        TEmitter BranchIf(CompareCondition condition, Label label);
        TEmitter BranchIf(CompareCondition condition, out Label label) => Define(out label).BranchIf(condition, label);
       
        TEmitter Return();

        TEmitter Declare<T>(out LocalBuilder local) => Declare(typeof(T), out local);
        TEmitter Declare<T>(bool pinned, out LocalBuilder local) => Declare(typeof(T), pinned, out local);
        
        TEmitter Declare(Type localType, out LocalBuilder local);
        TEmitter Declare(Type localType, bool pinned, out LocalBuilder local);

        TEmitter LoadLocal(LocalBuilder local);
        TEmitter LoadLocalAddress(LocalBuilder local);
        TEmitter StoreLocal(LocalBuilder local);

        TEmitter LoadArgument(int index);
        TEmitter LoadArgumentAddress(int index);
        TEmitter StoreArgument(int index);

        TEmitter LoadConstant(int value);
        TEmitter LoadConstant(long value);
        TEmitter LoadConstant(float value);
        TEmitter LoadConstant(double value);
        TEmitter LoadNull();
        TEmitter LoadString(string? text);
        TEmitter LoadToken(Type type);
        TEmitter LoadToken(FieldInfo field);
        TEmitter LoadToken(MethodInfo method);
        TEmitter LoadValue(object? value);
        TEmitter LoadValue<T>([AllowNull] T value);

        TEmitter LoadField(FieldInfo field);
        TEmitter LoadFieldAddress(FieldInfo field);
        TEmitter StoreField(FieldInfo field);

        TEmitter LoadFromAddress(Type type);
        TEmitter LoadFromAddress<T>() => LoadFromAddress(typeof(T));
        TEmitter StoreToAddress(Type type);
        TEmitter StoreToAddress<T>() => StoreToAddress(typeof(T));
        
        TEmitter Break();
        

        TEmitter ReThrow();
        TEmitter Throw<TException>()
            where TException : Exception, new();
        TEmitter Throw<TException>(string? message)
            where TException : Exception;
        TEmitter Throw<TException>(params object?[] exceptionConstructorParameters)
            where TException : Exception;

        TEmitter Add();
        TEmitter Divide();
        TEmitter Multiply();
        TEmitter Remainder();
        TEmitter Subtract();
        TEmitter And();
        TEmitter Negate();
        TEmitter Not();
        TEmitter Or();
        TEmitter ShiftLeft();
        TEmitter ShiftRight();
        TEmitter Xor();

        TEmitter Box(Type stackType);
        TEmitter Box<T>() => Box(typeof(T));
        TEmitter UnboxToValuePointer(Type outputType);
        TEmitter UnboxToValuePointer<T>() 
            where T : struct => UnboxToValuePointer(typeof(T));

        TEmitter Convert(Type stackType, Type outputType);
        TEmitter Convert<TIn, TOut>() => Convert(typeof(TIn), typeof(TOut));

        TEmitter IsInstance(Type type);
        TEmitter IsInstance<T>() => IsInstance(typeof(T));

        TEmitter Compare(CompareCondition condition);

        TEmitter SizeOf(Type type);
        TEmitter SizeOf<T>() where T : unmanaged => SizeOf(typeof(T));
        
        TEmitter Call(MethodInfo method);
        TEmitter New(ConstructorInfo constructor);

        TEmitter Pop();
        TEmitter Duplicate();
        
        IWriter<TEmitter> Console { get; }
        IArrayInteraction<TEmitter> Array { get; }

        TEmitter Append(IEnumerable<Instruction> instructions);
    }
}