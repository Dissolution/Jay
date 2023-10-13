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
    
    Self IsInstance<T>();
    Self IsInstance(Type instanceType);
}

public interface IValueEmitter<out Self>
    where Self : ISmartEmitter<Self>
{
    Self Push(int int32);
    Self Push(long int64);
    Self Push(float f32);
    Self Push(double f64);
    Self Push(string str);
    // handles null, Type
    Self Push<T>(T? value);
    Self PushDefault<T>();
    Self PushDefault(Type type);
}