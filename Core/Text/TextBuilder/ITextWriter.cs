using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Jay.Constraints;

namespace Jay.Text
{
    public interface ITextWriter<out TWriter> : IDisposable
        where TWriter : ITextWriter<TWriter>
    {
        /// <summary>
        /// Writes a character
        /// </summary>
        /// <param name="c"></param>
        void Write(char c);
        /// <summary>
        /// Writes a span of characters
        /// </summary>
        /// <param name="text"></param>
        void Write(ReadOnlySpan<char> text);
        void Write(params char[] text);
        void Write(string? text);
        
        TWriter Append(char c);
        TWriter Append(ReadOnlySpan<char> text);
        TWriter Append(params char[] text);
        TWriter Append(string? text);
        TWriter Append(IEnumerable<char> text);
        
        TWriter Append(bool boolean);
        TWriter Append(byte value);
        TWriter Append(sbyte value);
        TWriter Append(short value);
        TWriter Append(ushort value);
        TWriter Append(int value);
        TWriter Append(uint value);
        TWriter Append(long value);
        TWriter Append(ulong value);
        TWriter Append(float value);
        TWriter Append(double value);
        TWriter Append(decimal value);
        TWriter Append(TimeSpan value);
        TWriter Append(DateTime value);
        TWriter Append(DateTimeOffset value);
        TWriter Append(Guid value);
        TWriter Append(object? value);
        TWriter Append<T>(T? value);
        
        TWriter Append(WriteText<TWriter> writeText);
        TWriter Append<TState>(TState state, WriteStateText<TWriter, TState> writeText);
        TWriter Append<T>(ReadOnlySpan<T> readOnlySpan, WriteSpanText<TWriter, T> writeText);
        TWriter Append(ReadOnlySpan<char> text, WriteSpanCharText<TWriter> writeText);
        
        TWriter AppendFormat(FormattableString formattableString);
        TWriter AppendFormat(NonFormattableString format, params object?[] args);
        TWriter AppendFormat<TFormattable>([AllowNull] TFormattable formattable,
                                           string? format = null,
                                           IFormatProvider? provider = null)
            where TFormattable : IFormattable;
        
        TWriter AppendNewLine();

        TWriter AppendIf(bool check, WriteText<TWriter> ifTrue, WriteText<TWriter> ifFalse);

        TWriter AppendJoin<T>(ReadOnlySpan<T> values);
        TWriter AppendJoin<T>(params T[] values);
        TWriter AppendJoin<T>(IEnumerable<T> values);

        TWriter AppendDelimit(char delimiter, params string?[] strings);
        TWriter AppendDelimit(char delimiter, IEnumerable<string?> strings);
        TWriter AppendDelimit(char delimiter, Array array);
        TWriter AppendDelimit<T>(char delimiter, ReadOnlySpan<T> values);
        TWriter AppendDelimit<T>(char delimiter, params T[] values);
        TWriter AppendDelimit<T>(char delimiter, IEnumerable<T> values);
        
        TWriter AppendDelimit<T>(char delimiter, ReadOnlySpan<T> values, WriteStateText<TWriter, T> action);
        TWriter AppendDelimit<T>(char delimiter, IEnumerable<T> values, WriteStateText<TWriter, T> action);
        
        TWriter AppendDelimit(ReadOnlySpan<char> delimiter, params string?[] strings);
        TWriter AppendDelimit(ReadOnlySpan<char> delimiter, IEnumerable<string?> strings);
        TWriter AppendDelimit(ReadOnlySpan<char> delimiter, Array array);
        TWriter AppendDelimit<T>(ReadOnlySpan<char> delimiter, ReadOnlySpan<T> values);
        TWriter AppendDelimit<T>(ReadOnlySpan<char> delimiter, params T[] values);
        TWriter AppendDelimit<T>(ReadOnlySpan<char> delimiter, IEnumerable<T> values);
        
        TWriter AppendDelimit<T>(ReadOnlySpan<char> delimiter, ReadOnlySpan<T> values, WriteStateText<TWriter, T> action);
        TWriter AppendDelimit<T>(ReadOnlySpan<char> delimiter, IEnumerable<T> values, WriteStateText<TWriter, T> action);

        TWriter AppendNewLine(params string?[] strings);
        TWriter AppendNewLine(IEnumerable<string?> strings);
        TWriter AppendNewLine(Array array);
        TWriter AppendNewLine<T>(ReadOnlySpan<T> values);
        TWriter AppendNewLine<T>(params T[] values);
        TWriter AppendNewLine<T>(IEnumerable<T> values);
        
        TWriter AppendNewLine<T>(ReadOnlySpan<T> values, WriteStateText<TWriter, T> action);
        TWriter AppendNewLine<T>(IEnumerable<T> values, WriteStateText<TWriter, T> action);

        
        TWriter AppendAlign(ReadOnlySpan<char> text, 
                            Alignment alignment, 
                            int width, 
                            char trimIndicationChar = '…');

        TWriter AppendRepeat(char character, int count);
        TWriter AppendRepeat(ReadOnlySpan<char> text, int count);
        TWriter AppendRepeat(char[] text, int count);
        TWriter AppendRepeat(string? text, int count);
    }
}