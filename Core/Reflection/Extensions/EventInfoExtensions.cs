using System;
using System.Reflection;

namespace Jay.Reflection
{
    public static class EventInfoExtensions
    {
        public static bool IsStatic(this EventInfo? eventInfo)
        {
            if (eventInfo is null) return false;
            return eventInfo.GetAdder().IsStatic() ||
                   eventInfo.GetRemover().IsStatic() ||
                   eventInfo.GetRaiser().IsStatic();
        }
        
        public static Visibility GetVisibility(this EventInfo? eventInfo)
        {
            if (eventInfo is null)
                return default;
            return eventInfo.GetAdder().GetVisibility() |
                   eventInfo.GetRemover().GetVisibility() |
                   eventInfo.GetRaiser().GetVisibility();
        }
        
        public static MethodInfo? GetAdder(this EventInfo? eventInfo)
        {
            if (eventInfo is null) return null;
            return eventInfo.GetAddMethod(false) ??
                   eventInfo.GetAddMethod(true);
        }
        
        public static MethodInfo? GetRemover(this EventInfo? eventInfo)
        {
            if (eventInfo is null) return null;
            return eventInfo.GetRemoveMethod(false) ??
                   eventInfo.GetRemoveMethod(true);
        }
        
        public static MethodInfo? GetRaiser(this EventInfo? eventInfo)
        {
            if (eventInfo is null) return null;
            return eventInfo.GetRaiseMethod(false) ??
                   eventInfo.GetRaiseMethod(true);
        }
        
        /// <summary>
        /// Gets the backing <see cref="FieldInfo"/> for the specified <see cref="EventInfo"/>.
        /// </summary>
        /// <param name="event">The <see cref="EventInfo"/> to get the backing field of.</param>
        /// <returns>The backing <see cref="FieldInfo"/> if found; otherwise null.</returns>
        public static FieldInfo? GetBackingField(this EventInfo @event)
        {
            if (@event is null)
                throw new ArgumentNullException(nameof(@event));
            Type? owner = @event.ReflectedType ?? @event.DeclaringType;
            if (owner is null)
                throw new ArgumentException("Cannot determine which Type the event belongs to", nameof(@event));
            BindingFlags flags = BindingFlags.NonPublic;
            flags |= @event.IsStatic() ? BindingFlags.Static : BindingFlags.Instance;
            FieldInfo? backingField = owner.GetField(@event.Name, flags);
            //TODO: Smarter way of finding possible backing fields?
            return backingField;
        }

    }
}