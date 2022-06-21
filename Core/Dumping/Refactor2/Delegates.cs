using Jay.Text;
// ReSharper disable TypeParameterCanBeVariant
// ReSharper disable UseNullableAnnotationInsteadOfAttribute

namespace Jay.Dumping.Refactor2;

/// <summary>
/// Dumps a <typeparamref name="T"/> <paramref name="value"/> to a <paramref name="textBuilder"/> with optional <paramref name="dumpOptions"/>
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
/// <param name="dumpOptions">
/// Optional <see cref="DumpOptions"/> to refine the output, may be <c>null</c>
/// </param>
/// <remarks>
/// Any backing implementation should never throw an exception
/// </remarks>
public delegate void DumpValueTo<T>([AllowNull] T value, TextBuilder textBuilder);