using System.Reflection;
using System.Reflection.Emit;

namespace Jay.Reflection.Building.Backing;

public interface IAttributeImplementer
{
    void ImplementAttributes(MemberInfo copyAttributesFrom,
        Action<CustomAttributeBuilder> addAttribute);
}