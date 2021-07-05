namespace Jay.Collections
{
    public static class TypeDictionary
    {
        public static ITypeDictionary<TValue> Create<TValue>(int capacity = 64) => new TypeDictionary<TValue>(capacity);

        public static ITypeDictionary<TValue> Create<TValue>(bool concurrent)
        {
            if (concurrent)
                return new ConcurrentTypeDictionary<TValue>();
            return new TypeDictionary<TValue>();
        }
    }
}