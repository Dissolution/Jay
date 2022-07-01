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
    protected readonly IAttributeImplementer _attributeImplementer;

    protected Implementer(TypeBuilder typeBuilder,
        IAttributeImplementer attributeImplementer)
    {
        _typeBuilder = typeBuilder;
        _attributeImplementer = attributeImplementer;
    }
}