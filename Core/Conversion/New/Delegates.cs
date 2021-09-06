using System;
using System.Diagnostics.CodeAnalysis;

namespace Jay.Conversion.New
{
    /// <summary>
    /// Tries to convert an input <paramref name="value"/> into a <paramref name="converted"/> value.
    /// </summary>
    /// <typeparam name="TIn">The <see cref="Type"/> of the input value.</typeparam>
    /// <typeparam name="TOut">The <see cref="Type"/> of the output value.</typeparam>
    /// <param name="value">The input value.</param>
    /// <param name="options">Options related to the conversion.</param>
    /// <param name="converted">The output value.</param>
    /// <returns>A <see cref="Result"/> that describes whether the conversion succeeded.</returns>
    public delegate Result TryConvert<in TIn, TOut>([AllowNull] TIn value, ConvertOptions options, [MaybeNull] out TOut converted);
}