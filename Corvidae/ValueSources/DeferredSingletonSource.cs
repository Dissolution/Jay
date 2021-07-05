using System;
using Jay;

namespace Corvidae
{
    public sealed class DeferredSingletonSource<TValue>  : ValueSource<TValue>,
                                                    IValueSource<TValue>,
                                                    IValueSource
    {
        private readonly Func<ISource, TValue?> _valueFactory;
        private readonly InitOnly<TValue> _value;

        public DeferredSingletonSource(Func<ISource, TValue?> valueFactory)
        {
            _valueFactory = valueFactory;
            _value = new InitOnly<TValue>();
        }

        /// <inheritdoc />
        public override TValue? Get(ISource source)
        {
            return _value.GetOrAdd(source, src => _valueFactory(src));
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            if (_value.TryGetValue(out var value) && value is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}