/*using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Jay.Reflection.Emission
{
    public static class EventRaiser
    {
         internal static Result TryCreateStaticRaiser<TEventArgs>(EventInfo @event, out StaticEventRaiser<TEventArgs>? raiser)
            where TEventArgs : EventArgs
        {
            if (@event is null)
            {
                raiser = null;
                return new ArgumentNullException(nameof(@event));
            }
            //Get the field the event is attached to
            var eventField = @event.Source().GetField(@event.Name, BindingFlags.Instance | BindingFlags.NonPublic);
            if (eventField is null || !eventField.IsStatic)
            {
                raiser = null;
                return new ArgumentException("The event does not have a static backing field", nameof(@event));
            }

            Type? eventHandlerType = @event.EventHandlerType;
            if (eventHandlerType is null)
            {
                raiser = null;
                return new InvalidOperationException("The event's handler must not be null");
            }

            //Delegates have an InvocationList that we can use
            var getInvocationListMethod = eventHandlerType.GetMethod(nameof(Delegate.GetInvocationList), INSTANCE_FLAGS);
            if (getInvocationListMethod is null)
            {
                raiser = null;
                return new InvalidOperationException();
            }
            var eventHandlerInvokeMethod = Reflect.GetInvokeMethod(eventHandlerType);
            var dm = CreateDynamicMethod<StaticEventRaiser<TEventArgs>>($"{Stringify.Member(@event)}_raise");
            var emitter = dm.GetILEmitter();

            //Locals
            emitter.DeclareLocal<Delegate[]>(out Local delegates)
                   .DeclareLocal<int>(out Local i);
            //Load the event field, pull the delegate list out of it and store them
            emitter.Ldfld(eventField)
                   .Call(getInvocationListMethod)
                   .Stloc(delegates);
            //for loop setup
            emitter.Ldc_I4(0)
                   .Stloc(i)
                   .Br(out Label forEnd)
                   .DefineAndMarkLabel(out Label forStart);
            //for loop body
            emitter.Ldloc(delegates)
                   .Ldloc(i)
                   .Ldelem_Ref();       //object = delegates[i]
            //Load the args to be passed to the delegate
            emitter.Ldarg(0) //sender as object
                   .Ldarg(1);        //eventargs
            //Call the delegate
            emitter.Call(eventHandlerInvokeMethod.MethodInfo);
            //We shouldn't get anything back, if we do, pop it off the stack
            if (eventHandlerInvokeMethod.ReturnType != typeof(void))
                emitter.Pop();
            //i++
            emitter.Ldloc(i).Ldc_I4(1).Add().Stloc(i);
            //for loop end
            emitter.MarkLabel(forEnd)
                   .Ldloc(i)
                   .Ldloc(delegates)
                   .Ldlen()
                   .Conv_I4()
                   .Blt(forStart);
            //Done
            emitter.Ret();

            raiser = dm.CreateDelegate<StaticEventRaiser<TEventArgs>>();
            return true;
        }
    }
}*/