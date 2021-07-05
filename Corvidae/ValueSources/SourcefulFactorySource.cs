using System;

namespace Corvidae
{
    internal sealed class SourcefulFactorySource<TValue> : FactorySource<TValue>,
                                                           IValueSource<TValue>,
                                                           IValueSource
    {
        private readonly Func<ISource, TValue?> _factory;
   
        public SourcefulFactorySource(Func<ISource, TValue?> factory)
        {
            _factory = factory;
        }

        /// <inheritdoc />
        public override TValue? Get(ISource source) => _factory(source);

        /// <inheritdoc />
        object? IValueSource.Get(ISource source) => (object?)_factory(source);
    }
}