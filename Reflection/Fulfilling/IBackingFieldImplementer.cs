namespace Jay.Reflection.Fulfilling;

public interface IBackingFieldImplementer
{
    FieldBuilder ImplementBackingField(PropertyInfo property);
}