using Jay.Reflection.Emitting;

namespace Jay.Reflection.Adapters.Args;

public static class ArgExtensions
{
    private static InvalidOperationException GetEx(Arg sourceArg, Arg destArg,
        string? info = null)
    {
        return new InvalidOperationException(
            message: CodePart.ToCode($"Cannot load {sourceArg} and store {destArg}: {info}"));
    }

    public static Result CanCast(Arg sourceArg, Arg destArg)
    {
        // ?T -> ?T
        if (destArg.RootType == sourceArg.RootType) return true;
        // ? -> void        (pop)
        if (destArg.Type == typeof(void)) return true;
        // void ->          (create)
        if (sourceArg.Type == typeof(void)) return false;
        // ? -> object
        if (destArg.Type == typeof(object)) return true;
        // ?object -> ?
        if (sourceArg.RootType == typeof(object)) return true;
        // ?T -> ?U
        if (sourceArg.RootType.Implements(destArg.RootType)) return true;

        // do not know how (yet)
        return GetEx(sourceArg, destArg);
    }
    
    public static Result TryEmitCast<TEmitter>(
        this TEmitter emitter,
        Arg sourceArg,
        Arg destArg)
        where TEmitter : FluentEmitter<TEmitter>
    {
        // ?T -> ?T
        if (destArg.RootType == sourceArg.RootType)
        {
            // ref T -> ?T
            if (sourceArg.IsByRef)
            {
                // ref T -> ref T
                if (destArg.IsByRef)
                {
                    // just be sure value is on the stack
                    sourceArg.EmitLoad(emitter);
                }
                // ref T -> T
                else
                {
                    // just be sure value is on the stack
                    sourceArg.EmitLoad(emitter);
                    // then load from the address
                    emitter.Ldind(destArg.RootType);
                }
            }
            // T -> ?T
            else
            {
                // T -> ref T
                if (destArg.IsByRef)
                {
                    sourceArg.EmitLoadAddress(emitter);
                }
                // T -> T
                else
                {
                    // Just ensure value is on the stack
                    sourceArg.EmitLoad(emitter);
                }
            }

            return true;
        }
        
        // ? -> void        (pop)
        if (destArg.Type == typeof(void))
        {
            // if we're coming from stack, pop it
            if (sourceArg is Arg.Stack)
            {
                emitter.Pop();
            }
            // anything else, no need to load nor 
            return true;
        }
        
        // void -> ?
        if (sourceArg.Type == typeof(void))
        {
            return GetEx(sourceArg, destArg, "Use .LoadDefault() instead");
        }
        
        // ? -> object      (box)
        if (destArg.RootType == typeof(object))
        {
            if (destArg.IsByRef)
                return new NotImplementedException();
            
            // Try to get value onto the stack
            sourceArg.EmitLoad(emitter);
            
            // If we're not already object, box!
            if (sourceArg.RootType != typeof(object))
            {
                emitter.Box(sourceArg.RootType);
            }
            // we have object
            return true;
        }
        
        // ?object -> ?      (unbox)
        if (sourceArg.RootType == typeof(object))
        {
            // get the object on the stack
            sourceArg.EmitLoad(emitter);
            
            // if we had `ref object`, convert it to just `object`
            if (sourceArg.IsByRef)
            {
                emitter.Ldind(sourceArg.RootType);
            }
            
            // object -> ref T
            if (destArg.IsByRef)
            {
                // object -> ref struct
                if (destArg.RootType.IsValueType)
                {
                    emitter.Unbox(destArg.RootType);
                }
                // object -> ref class
                else
                {
                    emitter.Castclass(destArg.RootType)
                        .DeclareLocal(destArg.RootType, out var localDest)
                        .Stloc(localDest)
                        .Ldloca(localDest);     // address
                }
            }
            // object -> T
            else
            {
                // object -> struct
                if (destArg.RootType.IsValueType)
                {
                    emitter.Unbox_Any(destArg.RootType);
                }
                // object -> class
                else
                {
                    emitter.Castclass(destArg.RootType);
                }
            }

            return true;
        }
        
        // ?T -> ?U
        if (sourceArg.RootType.Implements(destArg.RootType))
        {
            // T:U -> U
            if (sourceArg.IsByRef || destArg.IsByRef)
                return GetEx(sourceArg, destArg);
            
            // struct T : I
            if (sourceArg.RootType.IsValueType)
            {
                // has to be an interface, as object was covered above
                if (!destArg.RootType.IsInterface)
                    return GetEx(sourceArg, destArg);
                
                // Get the value on the stack
                sourceArg.EmitLoad(emitter);
                
                // Castclass (also works for interfaces)
                emitter.Castclass(destArg.RootType); 
            }
            // class T : U
            else
            {
                // Get the value on the stack
                sourceArg.EmitLoad(emitter);
                
                // Castclass (also works for interfaces)
                emitter.Castclass(destArg.RootType); 
            }

            return true;
        }
        
        // do not know how to convert
        return GetEx(sourceArg, destArg);
    }

    public static TEmitter EmitCast<TEmitter>(this TEmitter emitter,
        Arg sourceArg, Arg destArg)
        where TEmitter : FluentEmitter<TEmitter>
    {
        TryEmitCast<TEmitter>(emitter, sourceArg, destArg).ThrowIfError();
        return emitter;
    }
}