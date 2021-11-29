namespace Jay.Text
{
    /// <summary>
    /// A constraint on a value that implicitly converts to and from <see cref="string"/> and allows for a <see cref="FormattableString"/> to be passed directly to a method.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class allows for <see cref="string"/>/<see langword="params"/> <see cref="object"/>[] args methods to exist alongside
    /// <see cref="FormattableString"/> methods without the compiler automatically converting the <see cref="FormattableString"/>
    /// to a <see cref="string"/>.
    /// </para>
    /// <para>
    /// e.g.:
    /// void Thing(string format, params object[] args)
    /// void Thing(FormattableString fStr)
    /// In that case, passing in `$"Blah{1}"` would use the first overload
    /// </para>
    /// <para>
    /// Whereas:
    /// void Thing(NonFormattableString format, params object[] args)
    /// void Thing(FormattableString fstr)
    /// In this case, passing in `$"Blah{1}"` would use the second overload
    /// </para>
    /// </remarks>
    public readonly ref struct RawString
    {
        public static implicit operator RawString(string? str) => new RawString(str);
        public static implicit operator string(RawString nfStr) => nfStr.String;
        // This exists to ensure that the compiler does the right behavior
        public static implicit operator RawString(FormattableString fStr) 
            => throw new ArgumentException("RawString is used alongside FormattableString", nameof(fStr));

        private readonly string? _string;

        public string String => _string ?? string.Empty;

        public RawString(string? str)
        {
            _string = str;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is string str)
                return string.Equals(str, _string);
            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return string.GetHashCode(_string);
        }

        /// <inheritdoc />
        public override string ToString() => String;
    }
}