using System;

namespace Corvidae
{
    public abstract class ValueSource<T> : IValueSource<T>, IValueSource
    {
        /// <inheritdoc />
        public Type ValueType { get; } = typeof(T);

        /// <inheritdoc />
        public abstract T? Get(ISource source);

        /// <inheritdoc />
        object? IValueSource.Get(ISource source) => Get(source);

        /// <inheritdoc />
        public virtual void Dispose()
        {
            // Do nothing
        }
    }
}