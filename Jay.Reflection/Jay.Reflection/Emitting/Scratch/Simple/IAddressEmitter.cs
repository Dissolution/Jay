namespace Jay.Reflection.Emitting.Scratch.Simple;

public interface IAddressEmitter<out Self>
    where Self : ISmartEmitter<Self>
{
    Self Volatile();
    Self Unaligned(int alignment);
    
    Self CopyValue<T>() where T : struct;
    Self CopyValue(Type valueType);

    Self InitValue<T>() where T : struct;
    Self InitValue(Type valueType);

    Self Load<T>();
    Self Load(Type type);

    Self Store<T>();
    Self Store(Type type);
}