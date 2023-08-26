namespace Jay.Reflection.Extensions;

/// <summary>
/// Extensions on <see cref="ConstructorInfo"/>
/// </summary>
public static class ConstructorInfoExtensions
{
    /// <summary>
    /// Does this <paramref name="constructor"/> have exactly the given <paramref name="parameterTypes"/>?
    /// </summary>
    /// <param name="constructor"></param>
    /// <param name="parameterTypes"></param>
    /// <returns></returns>
    public static bool HasParameterTypes(this ConstructorInfo constructor,
                                         params Type[] parameterTypes)
    {
        var ctorParams = constructor.GetParameters();
        var len = ctorParams.Length;
        if (parameterTypes.Length != len) return false;
        for (var i = 0; i < len; i++)
        {
            if (ctorParams[i].ParameterType != parameterTypes[i])
                return false;
        }
        return true;
    }
}