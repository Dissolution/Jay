using System.Reflection;

namespace Jay.Reflection.Building.Backing;

public interface IPropertyBacker
{
    PropertyImpl ImplementProperty(PropertyInfo interfaceProperty);
}