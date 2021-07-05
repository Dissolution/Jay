using System;

namespace Corvidae
{
    public interface ISingletonSource : IValueSource
    {
        
    }
    
    public sealed class SingletonSource<TValue> : ValueSource<TValue>,
                                                  ISingletonSource,
                                                  IValueSource<TValue>,
                                                  IValueSource
    {
        private readonly Lazy<TValue?> _lazyValue;

        public SingletonSource(TValue? value)
        {
            _lazyValue = new Lazy<TValue?>(value);
        }

        public SingletonSource(Func<TValue?> valueFactory)
        {
            _lazyValue = new Lazy<TValue?>(valueFactory);
        }

        /// <inheritdoc />
        public override TValue? Get(ISource _) => _lazyValue.Value;
        /// <inheritdoc />
        object? IValueSource.Get(ISource _) => _lazyValue.Value;

        /// <inheritdoc />
        public override void Dispose()
        {
            if (_lazyValue.IsValueCreated && _lazyValue.Value is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}