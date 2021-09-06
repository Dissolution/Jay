namespace Jay.Randomization
{
    /// <summary>
    /// The mode that an <see cref="IRandomizer"/> runs under
    /// </summary>
    public enum RandomizerMode
    {
        /// <summary>
        /// Prefer speed, even if every possible value cannot be generated or a bias exists
        /// </summary>
        Speed = 0,
        /// <summary>
        /// Prefer precision, being able to generate every possible value or eliminate bias
        /// </summary>
        Precision = 1,
    }
}