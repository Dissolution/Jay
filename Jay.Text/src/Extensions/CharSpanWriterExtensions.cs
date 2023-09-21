using Jay.Memory;

namespace Jay.Text.Extensions;

/// <summary>
/// Extensions on <see cref="SpanWriter{T}">SpanWriter&lt;char&gt;</see>
/// </summary>
/// <remarks>
/// 
/// </remarks>
public static class CharSpanWriterExtensions
{
    public static bool TryWrite(this SpanWriter<char> writer, string? str) => writer.TryWrite(str.AsSpan());


}

