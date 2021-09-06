using System;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Emit;
using Jay;
using Jay.Debugging;
using Jay.Reflection;
using Jay.Reflection.Runtime;

namespace Jay.Reflection.Runtime
{
    public record EventPackage(FieldInfo EventField, EventInfo EventInfo, 
                               MethodInfo AddMethod, MethodInfo RemoveMethod, MethodInfo RaiseMethod);
}
    
    public static class EventBuilder
    {
        public static EventPackage AddEvent(TypeBuilder typeBuilder,
                                            string name,
                                            Type eventHandlerType)
        {
            if (!RuntimeBuilder.IsValidIdentifier(name))
                throw new ArgumentException("Invalid Event Name", nameof(name));
            if (!eventHandlerType.Implements<MulticastDelegate>())
                throw new ArgumentException("Event Handler Type must be a Multicast Delegate", nameof(eventHandlerType));
            var fieldName = RuntimeBuilder.ToFieldName(name);
            var fieldBuilder = typeBuilder.DefineField(fieldName, eventHandlerType, FieldAttributes.Private);
            var eventBuilder = typeBuilder.DefineEvent(name, EventAttributes.None, eventHandlerType);

            var addMethod = typeBuilder.DefineMethod($"add_{name}",
                                                     MethodAttributes.Private,
                                                     CallingConventions.Standard | CallingConventions.HasThis,
                                                     typeof(void),
                                                     new Type[] {eventHandlerType});
            var delegateCombineMethod = typeof(Delegate).GetMethod("Combine", new Type[] {typeof(Delegate), typeof(Delegate)})
                                                .ThrowIfNull();
            addMethod.Emit(emit =>
            {
                emit.LoadArgument(0)
                    .LoadArgument(0)
                    .LoadField(fieldBuilder)
                    .LoadArgument(1)
                    .Call(delegateCombineMethod)
                    .CastClass(eventHandlerType)
                    .StoreField(fieldBuilder)
                    .Return();
            });
            eventBuilder.SetAddOnMethod(addMethod);

            var removeMethod = typeBuilder.DefineMethod($"remove_{name}",
                                                        MethodAttributes.Private,
                                                        CallingConventions.Standard | CallingConventions.HasThis,
                                                        typeof(void),
                                                        new Type[] {eventHandlerType});
            var delegateRemoveMethod = typeof(Delegate).GetMethod("Remove", new Type[] {typeof(Delegate), typeof(Delegate)})
                                                       .ThrowIfNull();
            removeMethod.Emit(emit =>
            {
                emit.LoadArgument(0)
                    .LoadArgument(0)
                    .LoadField(fieldBuilder)
                    .LoadArgument(1)
                    .Call(delegateRemoveMethod)
                    .CastClass(eventHandlerType)
                    .StoreField(fieldBuilder)
                    .Return();
            });
            eventBuilder.SetRemoveOnMethod(removeMethod);

            var raiseMethod = typeBuilder.DefineMethod($"on_{name}",
                                                       MethodAttributes.Private,
                                                       CallingConventions.Standard | CallingConventions.HasThis,
                                                       typeof(void),
                                                       new Type[] {eventHandlerType});
            throw new NotImplementedException();
            raiseMethod.Generate(gen =>
            {
              
                    
            });
        }
    }