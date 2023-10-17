using System.Diagnostics;
using Jay.Text.Splitting;

namespace Jay.Reflection;

public static class VisibilityExtensions
{
    public static BindingFlags ToBindingFlags(this Visibility visibility)
    {
        BindingFlags bindingFlags = default;
        if (visibility.HasFlags(Visibility.Instance))
            bindingFlags |= BindingFlags.Instance;
        if (visibility.HasFlags(Visibility.Static))
            bindingFlags |= BindingFlags.Static;
        if (visibility.HasFlags(Visibility.NonPublic))
            bindingFlags |= BindingFlags.NonPublic;
        if (visibility.HasFlags(Visibility.Public))
            bindingFlags |= BindingFlags.Public;
        return bindingFlags;
    }
    
    public static Visibility ToVisibility(this BindingFlags bindingFlags)
    {
        Visibility visibility = default;
        if (bindingFlags.HasFlags(BindingFlags.Instance))
            visibility |= Visibility.Instance;
        if (bindingFlags.HasFlags(BindingFlags.Static))
            visibility |= Visibility.Static;
        if (bindingFlags.HasFlags(BindingFlags.NonPublic))
            visibility |= Visibility.NonPublic;
        if (bindingFlags.HasFlags(BindingFlags.Public))
            visibility |= Visibility.Public;
        return visibility;
    }
   
    public static bool TryParse([AllowNull, NotNullWhen(true)] string? str, out Visibility visibility)
    {
        if (Enum.TryParse(str, true, out visibility))
            return true;
        visibility = default;
        var e = new TextSplitEnumerator(str.AsSpan(), " ".AsSpan(), SplitOptions.RemoveEmptyLines);
        while (e.MoveNext())
        {
            if (!Enum.TryParse<Visibility>(e.Text.ToString(), true, out var flag))
            {
                Debugger.Break();
                visibility = default;
                return false;
            }
            visibility |= flag;
        }
        return true;
    }
}