using System.Reflection.Emit;

namespace Jay.Reflection.Building;

public class DynamicMethod<TDelegate>
    where TDelegate : Delegate
{
    public static implicit operator DynamicMethod(DynamicMethod<TDelegate> dynamicMethod) =>
        dynamicMethod._dynamicMethod;

    protected readonly DynamicMethod _dynamicMethod;

    public ILGenerator ILGenerator => _dynamicMethod.GetILGenerator();

    public DynamicMethod(DynamicMethod dynamicMethod)
    {
        _dynamicMethod = dynamicMethod;
    }

    public TDelegate CreateDelegate() => _dynamicMethod.CreateDelegate<TDelegate>();
}