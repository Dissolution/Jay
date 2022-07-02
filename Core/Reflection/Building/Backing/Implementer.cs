using System.Reflection;
using System.Reflection.Emit;

namespace Jay.Reflection.Building.Backing;

public abstract class Implementer
{
    protected static CallingConventions GetCallingConventions(MemberInfo member)
    {
        if (member.IsStatic()) return CallingConventions.Standard;
        return CallingConventions.HasThis;
    }

    protected readonly TypeBuilder _typeBuilder;

    protected Implementer(TypeBuilder typeBuilder)
    {
        _typeBuilder = typeBuilder;
    }
}