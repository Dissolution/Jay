using System.Reflection;

namespace Jay.Reflection;

public static class EventInfoExtensions
{
    public static MethodInfo? GetAdder(this EventInfo? eventInfo)
    {
        return eventInfo?.GetAddMethod(false) ??
               eventInfo?.GetAddMethod(true);
    }
        
    public static MethodInfo? GetRemover(this EventInfo? eventInfo)
    {
        return eventInfo?.GetRemoveMethod(false) ??
               eventInfo?.GetRemoveMethod(true);
    }
        
    public static MethodInfo? GetRaiser(this EventInfo? eventInfo)
    {
        return eventInfo?.GetRaiseMethod(false) ??
               eventInfo?.GetRaiseMethod(true);
    }
        
    public static Access Access(this EventInfo? eventInfo)
    {
        Access access = Reflection.Access.None;
        if (eventInfo is null)
            return access;
        access |= eventInfo.GetAdder().Access();
        access |= eventInfo.GetRemover().Access();
        access |= eventInfo.GetRaiser().Access();
        return access;
    }
}