namespace Jay.Reflection.Fulfilling;

public interface IPropertyImplementer
{
    PropertyImpl ImplementProperty(PropertyInfo property);
}