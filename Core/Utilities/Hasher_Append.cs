namespace Jay;

using static InlineIL.IL;

public ref partial struct Hasher
{
    public ref Hasher Append<T>(T? value)
    {
        this.Add(value);
        
        Emit.Ldarg(0); // should be ref this
        Emit.Ret();
        throw Unreachable();
    }
    
    public ref Hasher Append<T>(params T?[] values)
    {
        this.Add(values);
        
        Emit.Ldarg(0); // should be ref this
        Emit.Ret();
        throw Unreachable();
    }
}