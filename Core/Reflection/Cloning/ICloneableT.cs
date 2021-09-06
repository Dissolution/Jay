using System;

namespace Jay.Reflection.Cloning
{
    public enum CloneType
    {
        Shallow,
        Deep,
    }
    
    public interface ICloneable<out T> : ICloneable
    {
        new T Clone() => Clone(CloneType.Shallow);
        T Clone(CloneType cloneType);
        object ICloneable.Clone() => ((object)Clone()!)!;
    }
}