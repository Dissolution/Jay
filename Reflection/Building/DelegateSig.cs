using System.Diagnostics;
using System.Reflection;
using Jay.Comparision;
using Jay.Text;

namespace Jay.Reflection;

public readonly struct DelegateSig : IEquatable<DelegateSig>, IRenderable
{
    public static bool operator ==(DelegateSig x, DelegateSig y) => x.Equals(y);
    public static bool operator !=(DelegateSig x, DelegateSig y) => !x.Equals(y);

    public static DelegateSig Of<TDelegate>()
        where TDelegate : Delegate
    {
        var invokeMethod = typeof(TDelegate).GetMethod("Invoke",
            BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        Debug.Assert(invokeMethod != null);
        return DelegateSig.Of(invokeMethod);
    }

    public static DelegateSig Of(MethodBase method)
    {
        return new DelegateSig(method.GetParameters(), method.ReturnType());
    }

    public static DelegateSig Of(Type delegateType)
    {
        var invokeMethod = delegateType.GetMethod("Invoke",
            BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        if (invokeMethod is null)
            throw new ArgumentException("Invalid Delegate Type: Does not have an Invoke method", nameof(delegateType));
        return DelegateSig.Of(invokeMethod);
    }

    public readonly Type ReturnType;
    public readonly ParameterInfo[] Parameters;
    public readonly Type[] ParameterTypes;
    public int ParameterCount => Parameters.Length;
    public bool IsAction => ReturnType == typeof(void);
    public bool IsFunc => ReturnType != typeof(void);

    private DelegateSig(ParameterInfo[] parameters, Type? returnType)
    {
        this.ReturnType = returnType ?? typeof(void);
        this.Parameters = parameters;
        this.ParameterTypes = new Type[parameters.Length];
        for (var i = 0; i < parameters.Length; i++)
        {
            ParameterTypes[i] = parameters[i].ParameterType;
        }
    }

    public bool Equals(DelegateSig sig)
    {
        return sig.ReturnType == this.ReturnType &&
               EnumerableEqualityComparer<ParameterInfo>.Default.Equals(sig.Parameters, this.Parameters);
    }

    public override bool Equals(object? obj)
    {
        if (obj is DelegateSig sig)
            return Equals(sig);
        if (obj is MethodBase method)
            return Equals(Of(method));
        if (obj is Type delegateType)
            return Equals(Of(delegateType));
        if (obj is Delegate @delegate)
            return Equals(Of(@delegate.Method));
        return false;
    }

    public override int GetHashCode()
    {
        var hashcode = new HashCode();
        for (var i = 0; i < Parameters.Length; i++)
        {
            hashcode.Add(Parameters[i]);
        }
        hashcode.Add(ReturnType);
        return hashcode.ToHashCode();
    }

    public void Render(TextBuilder builder)
    {
        builder.Append(ReturnType)
            .Append(" (")
            .AppendDelimit(", ", Parameters)
            .Append(')');
    }
}