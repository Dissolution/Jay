namespace Jay.Reflection.Emitting.Args;

public static class ArgumentExtensions
{
    private static InvalidOperationException GetEx(Emitting.Args.Argument sourceArgument, Emitting.Args.Argument destArgument,
        string? info = null)
    {
        return new InvalidOperationException(
            message: CodePart.ToCode($"Cannot load {sourceArgument} and store {destArgument}: {info}"));
    }

    public static Result CanCast(Emitting.Args.Argument sourceArgument, Emitting.Args.Argument destArgument)
    {
        // ?T -> ?T
        if (destArgument.RootType == sourceArgument.RootType) return true;
        // ? -> void        (pop)
        if (destArgument.Type == typeof(void)) return true;
        // void ->          (create)
        if (sourceArgument.Type == typeof(void)) return false;
        // ? -> object
        if (destArgument.Type == typeof(object)) return true;
        // ?object -> ?
        if (sourceArgument.RootType == typeof(object)) return true;
        // ?T -> ?U
        if (sourceArgument.RootType.Implements(destArgument.RootType)) return true;

        // do not know how (yet)
        return GetEx(sourceArgument, destArgument);
    }
    
    public static Result TryEmitCast<TEmitter>(
        this TEmitter emitter,
        Emitting.Args.Argument sourceArgument,
        Emitting.Args.Argument destArgument)
        where TEmitter : FluentEmitter<TEmitter>
    {
        // ?T -> ?T
        if (destArgument.RootType == sourceArgument.RootType)
        {
            // ref T -> ?T
            if (sourceArgument.IsByRef)
            {
                // ref T -> ref T
                if (destArgument.IsByRef)
                {
                    // just be sure value is on the stack
                    sourceArgument.EmitLoad(emitter);
                }
                // ref T -> T
                else
                {
                    // just be sure value is on the stack
                    sourceArgument.EmitLoad(emitter);
                    // then load from the address
                    emitter.Ldind(destArgument.RootType);
                }
            }
            // T -> ?T
            else
            {
                // T -> ref T
                if (destArgument.IsByRef)
                {
                    sourceArgument.EmitLoadAddress(emitter);
                }
                // T -> T
                else
                {
                    // Just ensure value is on the stack
                    sourceArgument.EmitLoad(emitter);
                }
            }

            return true;
        }
        
        // ? -> void        (pop)
        if (destArgument.Type == typeof(void))
        {
            // if we're coming from stack, pop it
            if (sourceArgument is Emitting.Args.Argument.Stack)
            {
                emitter.Pop();
            }
            // anything else, no need to load nor 
            return true;
        }
        
        // void -> ?
        if (sourceArgument.Type == typeof(void))
        {
            return GetEx(sourceArgument, destArgument, "Use .LoadDefault() instead");
        }
        
        // ? -> object      (box)
        if (destArgument.RootType == typeof(object))
        {
            if (destArgument.IsByRef)
                return new NotImplementedException();
            
            // Try to get value onto the stack
            sourceArgument.EmitLoad(emitter);
            
            // If we're not already object, box!
            if (sourceArgument.RootType != typeof(object))
            {
                emitter.Box(sourceArgument.RootType);
            }
            // we have object
            return true;
        }
        
        // ?object -> ?      (unbox)
        if (sourceArgument.RootType == typeof(object))
        {
            // get the object on the stack
            sourceArgument.EmitLoad(emitter);
            
            // if we had `ref object`, convert it to just `object`
            if (sourceArgument.IsByRef)
            {
                emitter.Ldind(sourceArgument.RootType);
            }
            
            // object -> ref T
            if (destArgument.IsByRef)
            {
                // object -> ref struct
                if (destArgument.RootType.IsValueType)
                {
                    emitter.Unbox(destArgument.RootType);
                }
                // object -> ref class
                else
                {
                    emitter.Castclass(destArgument.RootType)
                        .DeclareLocal(destArgument.RootType, out var localDest)
                        .Stloc(localDest)
                        .Ldloca(localDest);     // address
                }
            }
            // object -> T
            else
            {
                // object -> struct
                if (destArgument.RootType.IsValueType)
                {
                    emitter.Unbox_Any(destArgument.RootType);
                }
                // object -> class
                else
                {
                    emitter.Castclass(destArgument.RootType);
                }
            }

            return true;
        }
        
        // ?T -> ?U
        if (sourceArgument.RootType.Implements(destArgument.RootType))
        {
            // T:U -> U
            if (sourceArgument.IsByRef || destArgument.IsByRef)
                return GetEx(sourceArgument, destArgument);
            
            // struct T : I
            if (sourceArgument.RootType.IsValueType)
            {
                // has to be an interface, as object was covered above
                if (!destArgument.RootType.IsInterface)
                    return GetEx(sourceArgument, destArgument);
                
                // Get the value on the stack
                sourceArgument.EmitLoad(emitter);
                
                // Castclass (also works for interfaces)
                emitter.Castclass(destArgument.RootType); 
            }
            // class T : U
            else
            {
                // Get the value on the stack
                sourceArgument.EmitLoad(emitter);
                
                // Castclass (also works for interfaces)
                emitter.Castclass(destArgument.RootType); 
            }

            return true;
        }
        
        // do not know how to convert
        return GetEx(sourceArgument, destArgument);
    }

    public static TEmitter EmitCast<TEmitter>(this TEmitter emitter,
        Emitting.Args.Argument sourceArgument, Emitting.Args.Argument destArgument)
        where TEmitter : FluentEmitter<TEmitter>
    {
        TryEmitCast<TEmitter>(emitter, sourceArgument, destArgument).ThrowIfError();
        return emitter;
    }
}