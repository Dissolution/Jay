namespace Jay.Reflection.Emitting.Scratch;

public interface ITokenEmitter<out Self>
    where Self : ISmartEmitter<Self>
{
    Self LoadToken<T>();
    Self LoadToken(Type type);
    Self LoadToken(FieldInfo field);
    Self LoadToken(MethodInfo method);
}