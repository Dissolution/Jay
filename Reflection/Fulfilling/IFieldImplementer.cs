namespace Jay.Reflection.Fulfilling;

public interface IFieldImplementer
{
    FieldBuilder ImplementField(FieldInfo field);
}