﻿// // ReSharper disable UnusedParameter.Local
//
// using TextWriter = Jay.Text.Building.TextWriter;
//
// namespace Jay.Reflection.CodeBuilding;
//
// [InterpolatedStringHandler]
// public ref struct InterpolatedCode
// {
//     public static implicit operator InterpolatedCode(string? text)
//     {
//         var builder = new InterpolatedCode();
//         builder.AppendFormatted(text);
//         return builder;
//     }
//     
//     
//     private readonly CodeBuilder? _codeBuilder;
//
//     public InterpolatedCode()
//     {
//         _codeBuilder = new();
//     }
//     
//     public InterpolatedCode(int literalLength, int formattedCount)
//     {
//         _codeBuilder = new();
//     }
//     
//     public InterpolatedCode(int literalLength, int formattedCount, CodeBuilder codeBuilder)
//     {
//         _codeBuilder = codeBuilder;
//     }
//     
//     public void AppendLiteral(string literal)
//     {
//         _codeBuilder?.Write(literal);
//     }
//
//     public void AppendFormatted(char ch)
//     {
//         _codeBuilder?.Write(ch);
//     }
//     
//     public void AppendFormatted(scoped ReadOnlySpan<char> text)
//     {
//         _codeBuilder?.Write(text);
//     }
//     
//     public void AppendFormatted(string? str)
//     {
//         _codeBuilder?.Write(str);
//     }
//     
//     public void AppendFormatted<T>(T? value)
//     {
//         _codeBuilder?.Format<T>(value);
//     }
//     
//     public void AppendFormatted<T>(T? value, string? format)
//     {
//         _codeBuilder?.Format<T>(value, format);
//     }
//
//     public void Dispose()
//     {
//         TextWriter? toReturn = _codeBuilder;
//         this = default;
//         toReturn?.Dispose();
//     }
//
//     public string ToStringAndDispose()
//     {
//         var str = this.ToString();
//         this.Dispose();
//         return str;
//     }
//     
//     public override string ToString()
//     {
//         return _codeBuilder?.ToString() ?? "";
//     }
//
//     public override bool Equals(object? obj) => throw new NotSupportedException();
//     public override int GetHashCode() => throw new NotSupportedException();
// }