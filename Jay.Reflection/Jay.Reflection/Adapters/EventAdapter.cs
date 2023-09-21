//     public static RaiseHandler<TInstance> CreateRaiser<TInstance>(EventInfo eventInfo)
//     {
//         Validate.IsNotNull(eventInfo);
//         var raiser = eventInfo.GetRaiser();
//         if (raiser is null)
//         {
//             /* We can do some hackery here
//              * There is always a backing Delegate field that the event is interacting with
//              * we can just load up that field's value as a MulticastDelegate
//              * call GetInvocationList -> Delegate[]
//              * and then fire off each one in turn, feeding them the instance as sender and the eventargs
//             */
//             var backingField = eventInfo.GetBackingField();
//             if (backingField is null)
//                 throw new ReflectedException($"Unable to find {eventInfo}'s backing field for a Raiser");
//             var handlerSig = DelegateInfo.For(backingField.FieldType);
//             Debug.Assert(handlerSig.ParameterCount == 2);
//             var senderParam = handlerSig.Parameters[0];
//             Debug.Assert(senderParam.ParameterType == typeof(object));
//             var eventArgsParam = handlerSig.Parameters[1];
//             Debug.Assert(eventArgsParam.ParameterType.IsAssignableTo(typeof(EventArgs)));
//
//             return RuntimeBuilder.BuildDelegate<RaiseHandler<TInstance>>(
//              $"{typeof(TInstance)}.{eventInfo.Name}.Raiser",
//              builder =>
//              {
//                  var emitter = builder.Emitter;
//                  emitter.EmitLoadParameterAsInstance(backingField, builder.Parameters[0], out int offset);
//                  Debug.Assert(offset == 1);
//
//                  // Check for null backing field
//                  emitter.Ldfld(backingField)
//                         .DefineLabel(out var lblEnd)
//                         .Brfalse(lblEnd);
//
//                  //Locals
//                  emitter.DeclareLocal(typeof(Delegate[]), out var delegates)
//                         .DeclareLocal<object>(out var sender)
//                         .DeclareLocal<int>(out var i);
//                  if (backingField.IsStatic)
//                  {
//                      emitter.LoadType(typeof(TInstance))
//                             .Stloc(sender);
//                  }
//                  else
//                  {
//                      emitter.LoadInstanceFor(backingField, builder.Parameters[0], out offset);
//                      emitter.Stloc(sender);
//                  }
//
//                  // Load and store our Delegate[] into a local variables
//                  emitter.LoadInstanceFor(backingField, builder.Parameters[0], out offset)
//                         .Ldfld(backingField)
//                         .Cast(backingField.FieldType, typeof(MulticastDelegate))
//                         .Call(MethodInfoCache.MulticastDelegate_GetInvocationList)
//                         .Stloc(delegates)
//
//                         // For loop
//                         .Ldc_I4_0()
//                         .Stloc(i)
//                         .DefineLabel(out var lblWhile)
//                         .Br(lblWhile)
//
//                         // Start of loop
//                         .DefineLabel(out var lblStart).MarkLabel(lblStart)
//                         // Load Delegate[]
//                         .Ldloc(delegates)
//                         .Ldloc(i)
//                         // Load Delegate[i]
//                         .Ldelem_Ref()
//                         // As the appropriate Delegate Type (which is basically EventHandler or EventHandler<T>)
//                         .Castclass(backingField.FieldType)
//                         // Sender
//                         .Ldloc(sender)
//                         // Args
//                         .LoadAs(builder.Parameters[1], eventArgsParam.ParameterType)
//                         // Call
//                         .Call(invokeMethod)
//
//                         // i++
//                         .Ldloc(i)
//                         .Ldc_I4_1()
//                         .Add()
//                         .Stloc(i)
//
//                         // While
//                         .MarkLabel(lblWhile)
//                         .Ldloc(i)
//                         .Ldloc(delegates)
//                         .Ldlen()
//                         .Conv_I4()
//                         .Blt(lblStart)
//
//                         // End
//                         .MarkLabel(lblEnd)
//                         .Ret();
//              });
//         }
//         else
//         {
//             raiser.TryAdapt<EventRaiser<TInstance, TEventArgs>>(out var del).ThrowIfFailed();
//             return del!;
//         }
//     }

using System.Diagnostics;
using Jay.Reflection.Builders;
using Jay.Reflection.Caching;
using Jay.Reflection.Emitting.Args;
using Jay.Reflection.Exceptions;
using Jay.Reflection.Validation;

namespace Jay.Reflection.Adapters;

public static class EventAdapter
{
    private static AddHandler<TInstance, THandler> CreateAddHandlerDelegate<TInstance, THandler>(
        EventInfo eventInfo)
        where THandler : Delegate
    {
        // Find Adder
        var adder = eventInfo.GetAdder();
        if (adder is null)
        {
            var backingField = eventInfo.GetBackingField();
            if (backingField is not null)
            {
                throw new NotImplementedException();
            }
            throw new ReflectedException($"Cannot find a way to add to {eventInfo}");
        }

        return RuntimeMethodAdapter.Adapt<AddHandler<TInstance, THandler>>(adder);
    }

    public static AddHandler<TInstance, THandler> GetAddHandlerDelegate<TInstance, THandler>(
        EventInfo eventInfo)
        where THandler : Delegate
    {
        return MemberDelegateCache.GetOrAdd(eventInfo, CreateAddHandlerDelegate<TInstance, THandler>);
    }

    public static void AddHandler<TInstance, THandler>(this EventInfo eventInfo,
        ref TInstance instance,
        THandler eventHandler)
        where THandler : Delegate
    {
        GetAddHandlerDelegate<TInstance, THandler>(eventInfo)(ref instance, eventHandler);
    }
    
    
    private static RemoveHandler<TInstance, THandler> CreateRemoveHandlerDelegate<TInstance, THandler>(
        EventInfo eventInfo)
        where THandler : Delegate
    {
        // Find Remover
        var remover = eventInfo.GetRemover();
        if (remover is null)
        {
            var backingField = eventInfo.GetBackingField();
            if (backingField is not null)
            {
                throw new NotImplementedException();
            }
            throw new ReflectedException($"Cannot find a way to remove from {eventInfo}");
        }

        return RuntimeMethodAdapter.Adapt<RemoveHandler<TInstance, THandler>>(remover);
    }

    public static RemoveHandler<TInstance, THandler> GetRemoveHandlerDelegate<TInstance, THandler>(
        EventInfo eventInfo)
        where THandler : Delegate
    {
        return MemberDelegateCache.GetOrAdd(eventInfo, CreateRemoveHandlerDelegate<TInstance, THandler>);
    }

    public static void RemoveHandler<TInstance, THandler>(this EventInfo eventInfo,
        ref TInstance instance,
        THandler eventHandler)
        where THandler : Delegate
    {
        GetRemoveHandlerDelegate<TInstance, THandler>(eventInfo)(ref instance, eventHandler);
    }
    
    
    private static RaiseHandler<TInstance> CreateRaiseHandlerDelegate<TInstance>(EventInfo eventInfo)
    {
        // Find Raiser
        var adder = eventInfo.GetRaiser();
        if (adder is not null)
        {
            // We somehow have one!
            return RuntimeMethodAdapter.Adapt<RaiseHandler<TInstance>>(adder);
        }
        
        var backingField = eventInfo.GetBackingField();
        if (backingField is not null)
        {
            /* If we have an event such as:
             * event PropertyChangedEventHandler? PropertyChanged;
             * Then it automatically has a backing field that stores that handler delegate
             * All delegates in C# are MulticastDelegates (*mostly)
             * so they have .GetInvocationList() method that returns an array of those delegates
             * which can be invoked.
             */
            var eventHandlerType = backingField.FieldType;
            ValidateType.IsDelegateType(eventHandlerType);
            var invokeMethod = eventHandlerType
                .GetInvokeMethod()
                .ThrowIfNull();
            
            return RuntimeBuilder.BuildDelegate<RaiseHandler<TInstance>>(
                $"raise_{eventInfo.OwnerType()}_{eventInfo.Name}",
                builder =>
                {
                    builder.Emitter
                        // check for null in field -> no handlers added
                        .EmitCast(builder.DelegateInfo.Parameters[0], backingField.InstanceType())
                        .Ldfld(backingField)
                        .Brfalse(out var finished)
                        // get the invocation list from the handler and store it
                        .EmitCast(builder.DelegateInfo.Parameters[0], backingField.InstanceType())
                        .Ldfld(backingField)
                        .Call(MemberCache.Methods.Delegate_GetInvocationList)
                        .DeclareLocal<Delegate[]>(out var delegates)
                        .Stloc(delegates)
                        // for (i in len)
                        .Ldloc(delegates)
                        .Ldlen()
                        .DeclareLocal<int>(out var len)
                        .Stloc(len)
                        .DeclareLocal<int>(out var i)
                        .Ldc_I4(0)
                        .Stloc(i)
                        .Br(out var whileCheck)
                        .DefineAndMarkLabel(out var doStart)
                        // get the delegate at i
                        .Ldloc(delegates)
                        .Ldloc(i)
                        .Ldelem<Delegate>()
                        // load all the arguments it needs to invoke
                        .EmitLoadParams(builder.DelegateInfo.Parameters[1], invokeMethod.GetParameters())
                        // invoke it
                        .Call(invokeMethod)
                        .PopIf(invokeMethod.ReturnType())
                        // continue to the next
                        .Ldloc(i)
                        .Ldc_I4_1()
                        .Add()
                        .Stloc(i)
                        .MarkLabel(whileCheck)
                        .Ldloc(i)
                        .Ldloc(len)
                        .Blt(doStart)
                        .MarkLabel(finished)
                        // finished, return
                        .Ret();

                    string il = builder.Emitter.ToString()!;
                    Debugger.Break();
                });
        }
        throw new ReflectedException($"Cannot find a way to raise {eventInfo}");
    }

    public static RaiseHandler<TInstance> GetRaiseHandlerDelegate<TInstance>(EventInfo eventInfo)
    {
        return MemberDelegateCache.GetOrAdd(eventInfo, CreateRaiseHandlerDelegate<TInstance>);
    }
    
    public static void RaiseHandler<TInstance>(this EventInfo eventInfo,
        ref TInstance instance,
        params object?[] eventArgs)
    {
        GetRaiseHandlerDelegate<TInstance>(eventInfo)(ref instance, eventArgs);
    }
}