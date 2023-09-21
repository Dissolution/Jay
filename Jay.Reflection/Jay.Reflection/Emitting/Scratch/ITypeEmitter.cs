namespace Jay.Reflection.Emitting.Scratch;

public interface ITypeEmitter<out Self>
    where Self : ISmartEmitter<Self>
{
    Self SizeOf<T>();
    Self SizeOf(Type type);

    Self LoadTypedRef<T>();
    Self LoadTypedRef(Type type);

    Self LoadAddrFromTypedRef<T>();
    Self LoadAddrFromTypedRef(Type type);
}