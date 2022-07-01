using System.Reflection;
using System.Reflection.Emit;

namespace Jay.Reflection.Building.Backing;

public interface IMethodImplementer
{
    MethodBuilder ImplementMethod(MethodInfo method);
}