using Jay.Text;

// ReSharper disable TypeParameterCanBeVariant
// ReSharper disable UseNullableAnnotationInsteadOfAttribute

namespace Jay.Dumping;

/// <summary>
/// Dumps a <typeparamref name="T"/> <paramref name="value"/> to a <paramref name="textBuilder"/>
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of <paramref name="value"/> to be dumped
/// </typeparam>
/// <param name="value">
/// The value to be dumped, may be <c>null</c>
/// </param>
/// <param name="textBuilder">
/// The <see cref="TextBuilder"/> to write a dump representation of the <paramref name="value"/> to
/// </param>
/// <remarks>
/// Any backing implementation will never throw any exceptions
/// </remarks>
public delegate void DumpValueTo<T>([AllowNull] T value, TextBuilder textBuilder);