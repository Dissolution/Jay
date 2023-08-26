namespace Jay.CodeGen.Extensions;

/// <summary>
/// Extensions on <see cref="Delegate"/> or generic types implementing <see cref="Delegate"/>
/// </summary>
public static class DelegateExtensions
{
    public static MethodInfo GetInvokeMethod<TDelegate>(this TDelegate @delegate)
        where TDelegate : Delegate => @delegate.Method;

}