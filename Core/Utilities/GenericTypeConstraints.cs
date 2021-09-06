namespace Jay
{
    /// <summary>
    /// Constraints upon generic types
    /// </summary>
    public static class GenericTypeConstraints
    {
        public readonly struct IsClass<T> where T : class { }
        public readonly struct IsStruct<T> where T : struct { }
        public readonly struct IsUnmanaged<T> where T : unmanaged { }
        public readonly struct IsNew<T> where T : new() { }
    }
}