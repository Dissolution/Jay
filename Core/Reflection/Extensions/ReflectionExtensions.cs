namespace Jay.Reflection.Extensions;

internal static class ReflectionExtensions
{
    public static Type[] ToTypeArray(this object?[]? objectArray)
    {
        if (objectArray is null) return Type.EmptyTypes;
        var len = objectArray.Length;
        if (len == 0) return Type.EmptyTypes;
        Type[] types = new Type[len];
        for (var i = 0; i < len; i++)
        {
            types[i] = objectArray[i]?.GetType() ?? typeof(void);
        }
        return types;
    }
}