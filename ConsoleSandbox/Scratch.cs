namespace ConsoleSandbox;

using static InlineIL.IL;

public static class ILScratch
{

    public static TClass UnboxToClass<TClass>(object obj)
        where TClass : class
    {
        Emit.Ldarg(nameof(obj));
        Emit.Unbox_Any<TClass>();
        return Return<TClass>();
    }

    public static TClass CastClass<TClass>(object obj)
        where TClass : class
    {
        Emit.Ldarg(nameof(obj));
        Emit.Castclass<TClass>();
        return Return<TClass>();
    }
}