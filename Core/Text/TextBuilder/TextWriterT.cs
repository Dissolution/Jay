using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Jay.Constraints;
using Jay.Exceptions;

namespace Jay.Text
{
    public abstract class TextWriter<TWriter> : ITextWriter<TWriter>
        where TWriter : class, ITextWriter<TWriter>, new()
    {
        protected readonly TWriter _this;
        protected string _newLine;
        
        protected TextWriter()
        {
            _this = (this as TWriter)!;
            _newLine = Environment.NewLine;
        }

#region Write

        /// <inheritdoc />
        public abstract void Write(char c);

        /// <inheritdoc />
        public abstract void Write(ReadOnlySpan<char> text);

        /// <inheritdoc />
        public abstract void Write(params char[] text);

        /// <inheritdoc />
        public abstract void Write(string? text);

        public abstract void Write(object? obj);
        
        public abstract void Write<T>([AllowNull] T value);

#endregion
        
#region Append
#region Text
        public TWriter Append(char c)
        {
            Write(c);
            return _this;
        }
        
        public TWriter Append(ReadOnlySpan<char> text)
        {
            Write(text);
            return _this;
        }
        
        public TWriter Append(params char[] chars)
        {
            Write(chars);
            return _this;
        }

        /// <inheritdoc />
        public TWriter Append(IEnumerable<char> text)
        {
            foreach (var c in text)
            {
                Write(c);
            }
            return _this;
        }

        public TWriter Append(string? text)
        {
            Write(text);
            return _this;
        }
#endregion
        
#region Value       
     
        public virtual TWriter Append(bool boolean)
        {
            Write(boolean ? bool.TrueString : bool.FalseString);
            return _this;
        }
        
        public virtual TWriter Append(byte value)
        {
            Write(value.ToString());
            return _this;
        }
       
        public virtual TWriter Append(sbyte value)
        {
            Write(value.ToString());
            return _this;
        }
        
        
        public virtual TWriter Append(short value)
        {
            Write(value.ToString());
            return _this;
        }
        
        
        public virtual TWriter Append(ushort value)
        {
            Write(value.ToString());
            return _this;
        }
        
        
        public virtual TWriter Append(int value)
        {
            Write(value.ToString());
            return _this;
        }
        
        
        public virtual TWriter Append(uint value)
        {
            Write(value.ToString());
            return _this;
        }
        
        
        public virtual TWriter Append(long value)
        {
            Write(value.ToString());
            return _this;
        }
        
        
        public virtual TWriter Append(ulong value)
        {
            Write(value.ToString());
            return _this;
        }
        
        
        public virtual TWriter Append(float value)
        {
            Write(value.ToString(CultureInfo.CurrentCulture));
            return _this;
        }
        
        
        public virtual TWriter Append(double value)
        {
            Write(value.ToString(CultureInfo.CurrentCulture));
            return _this;
        }
        
        
        public virtual TWriter Append(decimal value)
        {
            Write(value.ToString(CultureInfo.CurrentCulture));
            return _this;
        }
        
        
        public virtual TWriter Append(TimeSpan value)
        {
            Write(value.ToString());
            return _this;
        }
        
        
        public virtual TWriter Append(DateTime value)
        {
            Write(value.ToString(CultureInfo.CurrentCulture));
            return _this;
        }
        
        
        public virtual TWriter Append(DateTimeOffset value)
        {
            Write(value.ToString(CultureInfo.CurrentCulture));
            return _this;
        }
        
        
        public virtual TWriter Append(Guid value)
        {
            Write(value.ToString());
            return _this;
        }
        
        
        public virtual TWriter Append(object? value)
        {
            Write(value);
            return _this;
        }
        
        
        public virtual TWriter Append<T>(T? value)
        {
            Write<T>(value);
            return _this;
        }
#endregion

        #region Format

        /// <inheritdoc />
        public virtual TWriter AppendFormat(FormattableString formattableString)
        {
            Write(string.Format(formattableString.Format, formattableString.GetArguments()));
            return _this;
        }

        /// <inheritdoc />
        public virtual TWriter AppendFormat(NonFormattableString format, params object?[] args)
        {
            Write(string.Format((string)format, args));
            return _this;
        }

        /// <inheritdoc />
        public virtual TWriter AppendFormat<TFormattable>([AllowNull] TFormattable formattable, 
                                                          string? format = null, 
                                                          IFormatProvider? provider = null) 
            where TFormattable : IFormattable
        {
            Write(formattable?.ToString(format, provider));
            return _this;
        }

#endregion
        
#region Delegate / State
        public TWriter Append(WriteText<TWriter> writeText)
        {
            writeText.Invoke(_this);
            return _this;
        }
        
        public TWriter Append<TState>(TState state, WriteStateText<TWriter, TState> writeText)
        {
            writeText.Invoke(_this, state);
            return _this;
        }
        
        public TWriter Append<T>(ReadOnlySpan<T> readOnlySpan, WriteSpanText<TWriter, T> writeTExt)
        {
            writeTExt.Invoke(_this, readOnlySpan);
            return _this;
        }
        
        public TWriter Append(ReadOnlySpan<char> text, WriteSpanCharText<TWriter> writeText)
        {
            writeText.Invoke(_this, text);
            return _this;
        }
#endregion
        
#region New Line
        public virtual TWriter AppendNewLine()
        {
            Write(_newLine);
            return _this;
        }
#endregion

#region If
        public TWriter AppendIf(bool check,
                                WriteText<TWriter> ifTrue,
                                WriteText<TWriter> ifFalse)
        {
            var writer = _this;
            if (check)
            {
                ifTrue(writer);
            }
            else
            {
                ifFalse(writer);
            }
            return writer;
        }
#endregion

        #region Join
        /// <inheritdoc />
        public TWriter AppendJoin<T>(ReadOnlySpan<T> values)
        {
            for (var i = 0; i < values.Length; i++)
            {
                Write<T>(values[i]);
            }
            return _this;
        }
        
        public TWriter AppendJoin<T>(params T[] values)
        {
            for (var i = 0; i < values.Length; i++)
            {
                Write<T>(values[i]);
            }
            return _this;
        }

        public TWriter AppendJoin<T>(IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                Write<T>(value);
            }
            return _this;
        }
        #endregion

        #region Delimit
        public TWriter AppendDelimit(char delimiter, params string?[] strings)
        {
            int len = strings.Length;
            if (len > 0)
            {
                Write(strings[0]);
            }
            for (var i = 1; i < len; i++)
            {
                Write(delimiter);
                Write(strings[i]);
            }
            return _this;
        }
        public TWriter AppendDelimit(char delimiter, IEnumerable<string?> strings)
        {
            using (var e = strings.GetEnumerator())
            {
                if (!e.MoveNext())
                    return _this;
                Write(e.Current);
                while (e.MoveNext())
                {
                    Write(delimiter);
                    Write(e.Current);
                }
            }
            return _this;
        }
        
        public TWriter AppendDelimit(char delimiter, Array array)
        {
            int len = array.Length;
            if (len > 0)
            {
                Write(array.GetValue(0));
                for (var i = 1; i < len; i++)
                {
                    Write(delimiter);
                    Write(array.GetValue(i));
                }
            }
           
            return _this;
        }
        public TWriter AppendDelimit<T>(char delimiter, ReadOnlySpan<T> values)
        {
            int len = values.Length;
            if (len > 0)
            {
                Write<T>(values[0]);
                for (var i = 1; i < len; i++)
                {
                    Write(delimiter);
                    Write<T>(values[i]);
                }
            }
            return _this;
        }
        public TWriter AppendDelimit<T>(char delimiter, params T[] values)
        {
            int len = values.Length;
            if (len > 0)
            {
                Write<T>(values[0]);
                for (var i = 1; i < len; i++)
                {
                    Write(delimiter);
                    Write<T>(values[i]);
                }
            }
            return _this;
        }
       
        public TWriter AppendDelimit<T>(char delimiter, IEnumerable<T?> values)
        {
            using (var e = values.GetEnumerator())
            {
                if (!e.MoveNext())
                    return _this;
                Write<T>(e.Current);
                while (e.MoveNext())
                {
                    Write(delimiter);
                    Write<T>(e.Current);
                }
            }
            return _this;
        }

        public TWriter AppendDelimit<T>(char delimiter, ReadOnlySpan<T> values, WriteStateText<TWriter, T> action)
        {
            int len = values.Length;
            var writer = _this;
            if (len > 0)
            {
                action(writer, values[0]);
                for (var i = 1; i < len; i++)
                {
                    Write(delimiter);
                    action(writer, values[i]);
                }
            }
            return writer;
        }

        public TWriter AppendDelimit<T>(char delimiter, IEnumerable<T> values, WriteStateText<TWriter, T> action)
        {
            var writer = _this;
            using (var e = values.GetEnumerator())
            {
                if (!e.MoveNext())
                    return writer;
                action(writer, e.Current);
                while (e.MoveNext())
                {
                    Write(delimiter);
                    action(writer, e.Current);
                }
            }
            return writer;
        }
        
        /// <inheritdoc />
        public TWriter AppendDelimit(ReadOnlySpan<char> delimiter, params string?[] strings)
        {
            int len = strings.Length;
            if (len > 0)
            {
                Write(strings[0]);
            }
            for (var i = 1; i < len; i++)
            {
                Write(delimiter);
                Write(strings[i]);
            }
            return _this;
        }

        /// <inheritdoc />
        public TWriter AppendDelimit(ReadOnlySpan<char> delimiter, IEnumerable<string?> strings)
        {
            using (var e = strings.GetEnumerator())
            {
                if (!e.MoveNext())
                    return _this;
                Write(e.Current);
                while (e.MoveNext())
                {
                    Write(delimiter);
                    Write(e.Current);
                }
            }
            return _this;
        }

        public TWriter AppendDelimit(ReadOnlySpan<char> delimiter, Array array)
        {
            int len = array.Length;
            if (len > 0)
            {
                Write(array.GetValue(0));
                for (var i = 1; i < len; i++)
                {
                    Write(delimiter);
                    Write(array.GetValue(i));
                }
            }
            return _this;
        }
      
        public TWriter AppendDelimit<T>(ReadOnlySpan<char> delimiter, ReadOnlySpan<T> values)
        {
            int len = values.Length;
            if (len > 0)
            {
                Write<T>(values[0]);
                for (var i = 1; i < len; i++)
                {
                    Write(delimiter);
                    Write<T>(values[i]);
                }
            }
            
            return _this;
        }
        public TWriter AppendDelimit<T>(ReadOnlySpan<char> delimiter, params T[] values)
        {
            int len = values.Length;
            if (len > 0)
            {
                Write<T>(values[0]);
                for (var i = 1; i < len; i++)
                {
                    Write(delimiter);
                    Write<T>(values[i]);
                }
            }
            
            return _this;
        }
     
        public TWriter AppendDelimit<T>(ReadOnlySpan<char> delimiter, IEnumerable<T> values)
        {
            using (var e = values.GetEnumerator())
            {
                if (!e.MoveNext())
                    return _this;
                Write<T>(e.Current);
                while (e.MoveNext())
                {
                    Write(delimiter);
                    Write<T>(e.Current);
                }
            }
            return _this;
        }
      
        public TWriter AppendDelimit<T>(ReadOnlySpan<char> delimiter, ReadOnlySpan<T> values, WriteStateText<TWriter, T> action)
        {
            int len = values.Length;
            var writer = _this;
            if (len > 0)
            {
                action(writer, values[0]);
                for (var i = 1; i < len; i++)
                {
                    Write(delimiter);
                    action(writer, values[i]);
                }
            }
            return writer;
        }
      
        public TWriter AppendDelimit<T>(ReadOnlySpan<char> delimiter, IEnumerable<T> values, WriteStateText<TWriter, T> action)
        {
            var writer = _this;
            using (var e = values.GetEnumerator())
            {
                if (!e.MoveNext())
                    return writer;
                action(writer, e.Current);
                while (e.MoveNext())
                {
                    Write(delimiter);
                    action(writer, e.Current);
                }
            }
            return writer;
        }


        /// <inheritdoc />
        public TWriter AppendNewLine(params string?[] strings)
        {
            return AppendDelimit(_newLine, strings);
        }

        /// <inheritdoc />
        public TWriter AppendNewLine(IEnumerable<string?> strings)
        {
            return AppendDelimit(_newLine, strings);
        }

        /// <inheritdoc />
        public TWriter AppendNewLine(Array array)
        {
            return AppendDelimit(_newLine, array);
        }

        /// <inheritdoc />
        public TWriter AppendNewLine<T>(ReadOnlySpan<T> values)
        {
            return AppendDelimit<T>(_newLine, values);
        }

        /// <inheritdoc />
        public TWriter AppendNewLine<T>(params T[] values)
        {
            return AppendDelimit<T>(_newLine, values);
        }

        /// <inheritdoc />
        public TWriter AppendNewLine<T>(IEnumerable<T> values)
        {
            return AppendDelimit<T>(_newLine, values);
        }

        /// <inheritdoc />
        public TWriter AppendNewLine<T>(ReadOnlySpan<T> values, WriteStateText<TWriter, T> action)
        {
            return AppendDelimit<T>(_newLine, values, action);
        }

        /// <inheritdoc />
        public TWriter AppendNewLine<T>(IEnumerable<T> values, WriteStateText<TWriter, T> action)
        {
            return AppendDelimit<T>(_newLine, values, action);
        }

#endregion

#region Align
        public TWriter AppendAlign(ReadOnlySpan<char> text, 
                                   Alignment alignment, 
                                   int width, 
                                   char trimIndicationChar = '…')
        {
            if (width > 0)
            {
                var len = text.Length;
                if (width == len)
                {
                    Write(text);
                }
                else if (width > len)
                {
                    int gap = width - len;
                    if (alignment == Alignment.Left)
                    {
                        Write(text);
                        for (var i = 0; i < gap; i++)
                            Write(' ');
                    }
                    else if (alignment == Alignment.Right)
                    {
                        for (var i = 0; i < gap; i++)
                            Write(' ');
                        Write(text);
                    }
                    else // alignment.HasFlag(Alignment.Center)
                    {
                        int leftSpaces;
                        int rightSpaces;
                        if (alignment.HasFlag<Alignment>(Alignment.Right))
                        {
                            leftSpaces = Bits.HalfRoundUp(gap);
                            rightSpaces = Bits.HalfRoundDown(gap);
                        }
                        else // alignment.HasFlag(Alignment.Left) || alignment == Alignment.Center
                        {
                            leftSpaces = Bits.HalfRoundDown(gap);
                            rightSpaces = Bits.HalfRoundUp(gap);
                        }
                        
                        for (var i = 0; i < leftSpaces; i++)
                            Write(' ');
                        Write(text);
                        for (var i = 0; i < rightSpaces; i++)
                            Write(' ');
                    }
                }
                else // width < len
                {
                    if (alignment.HasFlag<Alignment>(Alignment.Right))
                    {
                        Write(trimIndicationChar);
                        Write(text[^(width-1)..]);
                    }
                    else
                    {
                        Write(text[..(width-1)]);
                        Write(trimIndicationChar);
                    }
                }
            }
            return _this;
        }
#endregion

#region Repeat
        public virtual TWriter AppendRepeat(char character, int count)
        {
            for (var i = 0; i < count; i++)
            {
                Write(character);
            }
            return _this;
        }
        
        public virtual TWriter AppendRepeat(ReadOnlySpan<char> text, int count)
        {
            for (var i = 0; i < count; i++)
            {
                Write(text);
            }
            return _this;
        }

        /// <inheritdoc />
        public virtual TWriter AppendRepeat(char[] text, int count)
        {
            for (var i = 0; i < count; i++)
            {
                Write(text);
            }
            return _this;
        }

        /// <inheritdoc />
        public virtual TWriter AppendRepeat(string? text, int count)
        {
            for (var i = 0; i < count; i++)
            {
                Write(text);
            }
            return _this;
        }
#endregion
#endregion

        public abstract void Dispose();

        public sealed override int GetHashCode() => GetHashCodeException.Throw<TWriter>();
    }
}