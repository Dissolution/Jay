namespace Jay.Reflection.Emitting;

public static class Extensions
{
    /// <summary>
    /// Gets the correct <see cref="OpCode"/> for calling this <paramref name="method"/>
    /// </summary>
    public static OpCode GetCallOpCode(this MethodBase method)
    {
        /* Call is for calling non-virtual, static, or superclass methods
         *   i.e. the target of the Call is not subject to overriding
         * Callvirt is for calling virtual methods
         *   i.e. the target of the Callvirt may be overridden
         *
         * Callvirt also checks for a null instance and will throw a NullReferenceException, whereas
         * Call will jump and execute.
         * We always want to use Callvirt when in doubt, it is absolutely the safest option
         */
        
        // Static?
        if (method.IsStatic) return OpCodes.Call;
        
        // Value-Type (all methods will automatically be sealed)
        if (method.OwnerType().IsValueType && !method.IsVirtual) return OpCodes.Call;
        
        // One would think that we could do this:
        // if (method.IsSealed()) return OpCodes.Call;
        // However, a sealed method can still belong to a class that can be null,
        // so we need the null check of callvirt
        
        // not just a null check, we might have to check the virtual method table
        return OpCodes.Callvirt;
    }
}