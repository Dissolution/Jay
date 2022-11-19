namespace Jay.Reflection.Extensions;

public static class DelegateExtensions
{
    public static MethodInfo GetInvokeMethod(this Delegate @delegate) => @delegate.Method;
}