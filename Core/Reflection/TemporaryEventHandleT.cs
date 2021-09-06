using Jay.Debugging;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Jay.Reflection
{
    public static class TemporaryEventHandle
    {
        public static IDisposable Attach<T, THandle>(T obj, Expression<Func<T, string>> selectEventHandlerName, THandle handler)
            where T : class
            where THandle : Delegate
        {
            if (obj is null) throw new ArgumentNullException(nameof(obj));
            var eventInfo = selectEventHandlerName.GetMember<EventInfo>();
            if (eventInfo is null)
            {
                var constEx = selectEventHandlerName.EnumerateExpressions()
                                                    .OfType<ConstantExpression>()
                                                    .FirstOrDefault();
                string? eventName = constEx?.Value?.ToString();
                if (string.IsNullOrEmpty(eventName))
                {
                    // We can't find it!
                    throw new ArgumentException("The given expression doesn't contain enough information to find an Event", nameof(selectEventHandlerName));
                }
                eventInfo = typeof(T).GetEvent(eventName, Reflect.InstanceFlags);
                if (eventInfo is null)
                {
                    // Can't find it
                    throw new ArgumentException($"{typeof(T).Name} does not have an instance event named '{eventName}'", 
                                                nameof(selectEventHandlerName));
                }
            }

            //Cast the delegate to the correct type
            var castHandler = Delegate.CreateDelegate(eventInfo.EventHandlerType!, handler.Target, handler.Method);
            //Add it to the event handler
            eventInfo.AddEventHandler(obj, castHandler);
            //Create our remove action
            void Remove() => eventInfo.RemoveEventHandler(obj, castHandler);
            //Return it in a dispose action
            return Disposable.Action(Remove);
        }
    }
}