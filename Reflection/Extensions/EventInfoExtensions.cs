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
        
    public static Visibility Access(this EventInfo? eventInfo)
    {
        Visibility visibility = Reflection.Visibility.None;
        if (eventInfo is null)
            return visibility;
        visibility |= eventInfo.GetAdder().Visibility();
        visibility |= eventInfo.GetRemover().Visibility();
        visibility |= eventInfo.GetRaiser().Visibility();
        return visibility;
    }

    public static bool IsStatic(this EventInfo? eventInfo)
    {
        if (eventInfo is null)
            return false;
        return eventInfo.GetAdder().IsStatic() ||
               eventInfo.GetRemover().IsStatic() ||
               eventInfo.GetRaiser().IsStatic();
    }
}