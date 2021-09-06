using System;
using System.Diagnostics.CodeAnalysis;

namespace Jay.Reflection.Emission
{
    [return: MaybeNull]
    public delegate TValue Getter<TInstance, out TValue>([DisallowNull] ref TInstance instance);

    public delegate void Setter<TInstance, in TValue>([DisallowNull] ref TInstance instance, [AllowNull] TValue value);

    
    public delegate void Adder<TInstance, in THandler>([DisallowNull] ref TInstance instance,
                                                    [DisallowNull] THandler handler)
        where THandler : Delegate;
    
    public delegate void Remover<TInstance, in THandler>([DisallowNull] ref TInstance instance,
                                                         [DisallowNull] THandler handler)
        where THandler : Delegate;

    public delegate void Raiser<TInstance>([DisallowNull] ref TInstance instance,
                                           params object?[] args);

    [return: NotNull]
    public delegate TInstance Constructor<out TInstance>(params object?[] args);

    public delegate void InstanceAction<TInstance>([DisallowNull] ref TInstance instance,
                                                   params object?[] args);
    
    [return: MaybeNull]
    public delegate TReturn InstanceFunc<TInstance, out TReturn>([DisallowNull] ref TInstance instance,
                                                                 params object?[] args);

}