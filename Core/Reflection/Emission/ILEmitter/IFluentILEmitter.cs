using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;

namespace Jay.Reflection.Emission
{
    public interface IFluentILEmitter : IFluentILStream<IFluentILEmitter>
    {
        IFluentILGenerator Generator { get; }

        IFluentILEmitter Generate(Action<AttachedFluentILGenerator> generate);
        
        IFluentILEmitter Try(Action<ITryBuilder<IFluentILEmitter>> buildTry);

        IFluentILEmitter Scoped(Action<IFluentILEmitter> scopedBlock);

        IFluentILEmitter Define(out Label label);
        IFluentILEmitter MarkLabel(Label label);

        IFluentILEmitter Branch(Label label);
        IFluentILEmitter Branch(out Label label) => Define(out label).Branch(label);

        IFluentILEmitter BranchIf(CompareCondition condition, Label label);
        IFluentILEmitter BranchIf(CompareCondition condition, out Label label) => Define(out label).BranchIf(condition, label);
       
        IFluentILEmitter Return();

        IFluentILEmitter DeclareLocal<T>(out LocalBuilder local) => DeclareLocal(typeof(T), out local);
        IFluentILEmitter DeclareLocal<T>(bool pinned, out LocalBuilder local) => DeclareLocal(typeof(T), pinned, out local);
        
        IFluentILEmitter DeclareLocal(Type localType, out LocalBuilder local);
        IFluentILEmitter DeclareLocal(Type localType, bool pinned, out LocalBuilder local);

        IFluentILEmitter LoadFromLocal(LocalBuilder local);
        IFluentILEmitter LoadLocalAddress(LocalBuilder local);
        IFluentILEmitter StoreInLocal(LocalBuilder local);

        IFluentILEmitter LoadArgument(int index);
        IFluentILEmitter LoadArgumentAddress(int index);
        IFluentILEmitter StoreArgument(int index);

        IFluentILEmitter LoadConstant(bool value) => value ? LoadConstant(1) : LoadConstant(0);
        IFluentILEmitter LoadConstant(int value);
        IFluentILEmitter LoadConstant(long value);
        IFluentILEmitter LoadConstant(float value);
        IFluentILEmitter LoadConstant(double value);
        IFluentILEmitter LoadNull();
        IFluentILEmitter LoadString(string? text);
        IFluentILEmitter LoadToken(Type type);
        IFluentILEmitter LoadToken(FieldInfo field);
        IFluentILEmitter LoadToken(MethodInfo method);
        IFluentILEmitter LoadValue(object? value);
        IFluentILEmitter LoadValue<T>([AllowNull] T value);

        IFluentILEmitter LoadField(FieldInfo field);
        IFluentILEmitter LoadFieldAddress(FieldInfo field);
        IFluentILEmitter StoreField(FieldInfo field);

        IFluentILEmitter LoadFromAddress(Type type);
        IFluentILEmitter LoadFromAddress<T>() => LoadFromAddress(typeof(T));
        IFluentILEmitter StoreToAddress(Type type);
        IFluentILEmitter StoreToAddress<T>() => StoreToAddress(typeof(T));
        
        IFluentILEmitter Break();
        

        IFluentILEmitter ReThrow();
        IFluentILEmitter Throw<TException>()
            where TException : Exception, new();
        IFluentILEmitter Throw<TException>(string? message)
            where TException : Exception;
        IFluentILEmitter Throw<TException>(params object?[] exceptionConstructorParameters)
            where TException : Exception;

        IFluentILEmitter Add();
        IFluentILEmitter Divide();
        IFluentILEmitter Multiply();
        IFluentILEmitter Remainder();
        IFluentILEmitter Subtract();
        IFluentILEmitter And();
        IFluentILEmitter Negate();
        IFluentILEmitter Not();
        IFluentILEmitter Or();
        IFluentILEmitter ShiftLeft();
        IFluentILEmitter ShiftRight();
        IFluentILEmitter Xor();

        IFluentILEmitter Box(Type type);
        IFluentILEmitter Box<T>() => Box(typeof(T));
        IFluentILEmitter Unbox(Type type);
        IFluentILEmitter Unbox<T>() => Unbox(typeof(T));
        IFluentILEmitter UnboxToPointer(Type type);
        IFluentILEmitter UnboxToPointer<T>() 
            where T : struct => UnboxToPointer(typeof(T));

        IFluentILEmitter CastClass(Type type);
        IFluentILEmitter CastClass<T>() => CastClass(typeof(T));

        IFluentILEmitter IsInstance(Type type);
        IFluentILEmitter IsInstance<T>() => IsInstance(typeof(T));

        IFluentILEmitter Compare(CompareCondition condition);

        IFluentILEmitter SizeOf(Type type);
        IFluentILEmitter SizeOf<T>() where T : unmanaged => SizeOf(typeof(T));
        
        IFluentILEmitter Call(MethodInfo method);
        IFluentILEmitter New(ConstructorInfo constructor);

        IFluentILEmitter Pop();
        IFluentILEmitter Duplicate();
        
        IWriter<IFluentILEmitter> Console { get; }
        IWriter<IFluentILEmitter> Debug { get; }
        IArrayInteraction<IFluentILEmitter> Array { get; }
    }
}