using System;

namespace Jay.Reflection.Emission.Sandbox
{
    public interface IArrayInteraction<out TBuilder>
    {
        TBuilder GetLength();
        TBuilder LoadElement(Type elementType);
        TBuilder LoadElement<T>() => LoadElement(typeof(T));
        TBuilder LoadElementAddress(Type elementType);
        TBuilder LoadElementAddress<T>() => LoadElement(typeof(T));
        TBuilder StoreElement(Type elementType);
        TBuilder StoreElement<T>() => StoreElement(typeof(T));

        TBuilder New(Type elementType);
        TBuilder New<T>() => New(typeof(T));
    }
}