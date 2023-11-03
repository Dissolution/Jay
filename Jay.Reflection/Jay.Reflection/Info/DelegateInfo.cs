﻿using Jay.Reflection.Builders;
using Jay.Reflection.Validation;
using Jay.Utilities;
#if NET7_0_OR_GREATER
using System.Numerics;
#endif


namespace Jay.Reflection.Info;


public sealed class DelegateInfo : 
#if NET7_0_OR_GREATER
    IEqualityOperators<DelegateInfo, DelegateInfo, bool>,
#endif
    IEquatable<DelegateInfo>
{
    public static bool operator ==(DelegateInfo? left, DelegateInfo? right) => Easy.FastEqual(left, right);
    public static bool operator !=(DelegateInfo? left, DelegateInfo? right) => !Easy.FastEqual(left, right);

    public static DelegateInfo For<TDelegate>(TDelegate? _ = null)
        where TDelegate : Delegate
        => new DelegateInfo(typeof(TDelegate));

    public static DelegateInfo For(MethodBase method)
    {
        if (method is null)
            throw new ArgumentNullException(nameof(method));
        return new DelegateInfo(method);
    }

    public static DelegateInfo For(Type delegateType)
    {
        ValidateType.IsDelegateType(delegateType);
        return new DelegateInfo(delegateType);
    }

    private readonly MethodBase _invokeMethod;
    private Type? _delegateType;

    public Type DelegateType => _delegateType ??= 
        ReturnType == typeof(void)
        ? Delegates.GetActionType(ParameterTypes)
        : Delegates.GetFuncType(ReturnType, ParameterTypes);

    public Type ReturnType { get; }

    public Type[] ParameterTypes { get; }

    public int ParameterCount => ParameterTypes.Length;

    public string Name => _invokeMethod.Name;

    public MethodBase InvokeMethod => _invokeMethod;

    public ParameterInfo[] Parameters { get; }

    private DelegateInfo(Type delegateType)
        : this(delegateType.GetInvokeMethod().ThrowIfNull())
    {
        _delegateType = delegateType;
    }
    
    private DelegateInfo(MethodBase method)
    {
        _delegateType = null;
        _invokeMethod = method;
        ReturnType = method.ReturnType();
        Parameters = method.GetParameters();
        ParameterTypes = method.GetParameterTypes();
    }

    public bool Equals(DelegateInfo? delegateInfo)
    {
        return delegateInfo is not null &&
            delegateInfo.Name == Name &&
            delegateInfo.ReturnType == ReturnType &&
            Easy.SeqEqual<Type>(delegateInfo.ParameterTypes, ParameterTypes);
    }

    public bool Equals(MethodBase? method)
    {
        return method is not null &&
            method.Name == this.Name &&
            method.ReturnType() == this.ReturnType &&
            Easy.SeqEqual<ParameterInfo>(method.GetParameters(), this.Parameters);
    }
    
    public override bool Equals(object? obj)
    {
        return obj switch
        {
            MethodBase methodBase => Equals(methodBase),
            Delegate @delegate => Equals(@delegate.Method),
            Type type when type.Implements<Delegate>() => Equals(type.GetInvokeMethod()),
            DelegateInfo delegateInfo => Equals(delegateInfo),
            _ => false,
        };
    }
    public override int GetHashCode()
    {
        var hasher = new Hasher();
        hasher.Add<Type>(ReturnType);
        foreach (var type in ParameterTypes)
            hasher.Add<Type>(type);
        return hasher.ToHashCode();
    }

    public override string ToString()
    {
        return TextBuilder.New
            .Append($"{ReturnType} Name (")
            .Delimit(
                ",", Parameters, (tb, param) =>
                {
                    var access = param.GetAccess(out var parameterType);
                    tb.Append(access).Append(' ').Append(parameterType).Append(param.Name);
                    if (param.HasDefaultValue)
                    {
                        tb.Append(" = ").Append(param.DefaultValue);
                    }
                })
            .Append(')')
            .ToStringAndDispose();
    }
}