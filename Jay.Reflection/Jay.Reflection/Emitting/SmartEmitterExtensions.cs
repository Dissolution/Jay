using Jay.Reflection.Emitting.Args;

namespace Jay.Reflection.Emitting;

public static class SmartEmitterExtensions
{
    public static TEmitter LoadInstanceFor<TEmitter>(
        this TEmitter emitter,
        Argument argument, MemberInfo member)
        where TEmitter : FluentEmitter<TEmitter>
    {
        if (member.TryGetInstanceType(out var instanceType))
        {
            // Do we need a value type's instance?
            if (instanceType.IsValueType)
            {
                // We need a reference to the value

                // We have a T?
                if (argument.RootType == instanceType)
                {
                    if (argument.IsByRef)
                    {
                        argument.EmitLoad(emitter);
                    }
                    else
                    {
                        argument.EmitLoadAddress(emitter);
                    }
                    return emitter;
                }

                // We have an object?
                if (argument.RootType == typeof(object))
                {
                    argument.EmitLoad(emitter);
                    // ref object?
                    if (argument.IsByRef)
                    {
                        emitter.Ldind_Ref();
                    }
                    return emitter.Unbox(instanceType);
                }

                // cannot
                throw new InvalidOperationException();
            }
            // we just need a non-value
            else
            {
                // We have a T?
                if (argument.RootType == instanceType)
                {
                    argument.EmitLoad(emitter);
                    if (argument.IsByRef)
                    {
                        emitter.Ldind(argument.RootType);
                    }
                    return emitter;
                }

                // source is object or implements instance type
                if (argument.RootType == typeof(object) ||
                    argument.RootType.Implements(instanceType))
                {
                    argument.EmitLoad(emitter);
                    // ref object?
                    if (argument.IsByRef)
                    {
                        emitter.Ldobj(argument.RootType);
                    }
                    return emitter.Castclass(instanceType);
                }

                // cannot
                throw new InvalidOperationException();
            }
        }
        // no instance to load, ok!
        return emitter;
    }
}