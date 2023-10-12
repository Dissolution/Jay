namespace Jay;

public interface ICloneable<out TSelf> : ICloneable
    where TSelf : ICloneable<TSelf>
{
    new TSelf Clone();
#if NET6_0_OR_GREATER
    object ICloneable.Clone() => Clone();
#endif
}