using System.Numerics;

namespace Jay
{
    public interface ICloneable<TSelf> : ICloneable
        where TSelf : ICloneable<TSelf>
    {
        [return: NotNullIfNotNull(nameof(value))]
        static TSelf? Clone(TSelf? value)
        {
            if (value is null) return default;
            return value.Clone();
        }
        [return: NotNullIfNotNull(nameof(value))]
        static TSelf? DeepClone(TSelf? value)
        {
            if (value is null) return default;
            return value.DeepClone();
        }

        object ICloneable.Clone() => (object)Clone();

        new TSelf Clone();

        TSelf DeepClone();
    }
}
