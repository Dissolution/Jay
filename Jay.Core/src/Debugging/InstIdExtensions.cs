#if NET7_0_OR_GREATER
using System.Numerics;
#endif
namespace Jay.Debugging;

public static class InstIdExtensions
{
    public static UIID InstId<T>(this T value)
        where T : class
    {
        return InstIdCounter<T>.GetInstanceId(value);
    }
}