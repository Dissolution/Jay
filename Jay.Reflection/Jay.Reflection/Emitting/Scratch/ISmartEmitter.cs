﻿using Argument = Jay.Reflection.Emitting.Args.Argument;

namespace Jay.Reflection.Emitting.Scratch;

public interface ISmartEmitter<out Self>
    where Self : ISmartEmitter<Self>
{
    IAddressEmitter<Self> Address { get; }
    IBitwiseEmitter<Self> Bitwise { get; }
    IBreakEmitter<Self> Break { get; }
    IArrayEmitter<Self> Array { get; }
    IMathEmitter<Self> Math { get; }
    ITryCatchFinallyEmitter<Self> Try { get; }
    IExceptionEmitter<Self> Exception { get; }
    IDebugEmitter<Self> Debug { get; }
    ICompareEmitter<Self> Compare { get; }
    IConvertEmitter<Self> Convert { get; }
    IMemoryEmitter<Self> Memory { get; }
    ITokenEmitter<Self> Token { get; }
    ITypeEmitter<Self> Type { get; }

    Self Scoped(Action<Self> scopedBlock);

    Self IsInstance<T>();
    Self IsInstance(Type instanceType);
    Self LoadInstanceFor(MemberInfo instanceMember, Argument instanceArg);
    Self LoadParamsFor(MethodBase method, Argument paramsArg);

    Self Load(Argument sourceArg);
    Self LoadCast(Argument sourceArg, Argument.Stack destArg);
    Self Cast(Argument.Stack sourceArg, Argument.Stack destArg);
    Self Store(Argument destArg);
    Self CastStore(Argument.Stack sourceArg, Argument destArg);

    Self Load(int int32);
    Self Load(long int64);
    Self Load(float f32);
    Self Load(double f64);
    Self Load(string str);
    // handles null, Type
    Self Load<T>(T? value);
    Self LoadDefault<T>();
    Self LoadDefault(Type type);

    Self Construct(ConstructorInfo ctor);
    Self Call(MethodBase method);

    Self Declare<T>(out EmitterLocal local, [CallerArgumentExpression(nameof(local))] string? localName = null);
    Self Declare<T>(bool isPinned, out EmitterLocal local, [CallerArgumentExpression(nameof(local))] string? localName = null);
    Self Declare(Type type, out EmitterLocal local, [CallerArgumentExpression(nameof(local))] string? localName = null);
    Self Declare(Type type, bool isPinned, out EmitterLocal local, [CallerArgumentExpression(nameof(local))] string? localName = null);

    Self Define(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null);
    Self Mark(EmitterLabel label);
    Self Mark(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null);
    
    Self Duplicate();
    Self TryPop<T>();
    Self TryPop(Argument.Stack stackArg);

    Self Return();
}