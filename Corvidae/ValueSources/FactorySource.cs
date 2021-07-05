using System;

namespace Corvidae
{
    public abstract class FactorySource<TValue> : ValueSource<TValue>
    {
        public static FactorySource<TValue> Create(Func<TValue?> valueFactory)
        {
            return new SourcelessFactorySource<TValue>(valueFactory);
        }

        public static FactorySource<TValue> Create(Func<ISource, TValue?> valueFactory)
        {
            return new SourcefulFactorySource<TValue>(valueFactory);
        }
    }
}