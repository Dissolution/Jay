using System;

namespace Corvidae
{
    internal sealed class SourcelessFactorySource<TValue> : FactorySource<TValue>,
                                                            IValueSource<TValue>,
                                                            IValueSource
    {
        private readonly Func<TValue?> _factory;
            
        public SourcelessFactorySource(Func<TValue?> factory)
        {
            _factory = factory;
        }

        /// <inheritdoc />
        public override TValue? Get(ISource _) => _factory();

        /// <inheritdoc />
        object? IValueSource.Get(ISource _) => (object?)_factory();
    }
}