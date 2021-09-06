using System;
using System.Diagnostics.CodeAnalysis;
using Jay.Constraints;
using Jay.Debugging;
using Jay.Debugging.Dumping;
using JetBrains.Annotations;

namespace Jay.Text
{
    public partial class TextBuilder
    {
        // Undocumented exclusive limits on the range for Argument Hole Index and Argument Hole Alignment.
        private const int IndexLimit = 1000000; // Note:            0 <= ArgIndex < IndexLimit
        private const int WidthLimit = 1000000; // Note:  -WidthLimit <  ArgAlign < WidthLimit

        
        internal void WriteFormat(IFormatProvider? provider, 
                                  string format, 
                                  object?[] args,
                                  bool dump)
        {
            //if (format is null) throw new ArgumentNullException(nameof(format));
            int pos = 0;
            int len = format.Length;
            char ch = '\0';
            ICustomFormatter? cf = provider?.GetFormat(typeof(ICustomFormatter)) as ICustomFormatter;

            // Processing loop
            while (true)
            {
                // While we have characters to process
                while (pos < len)
                {
                    ch = format[pos];
                    pos++;
                    // Closing brace?
                    if (ch == '}')
                    {
                        // Check if the next character (if there is one) to see if this is just an escape (eg: '}}')
                        if (pos < len && format[pos] == '}')
                        {
                            // append ch, skip the repeat
                            pos++;
                        }
                        // Otherwise, treat as an error (mismatched closing brace)
                        else
                        {
                            throw new FormatException($"Mismatched closing brace {{ at {pos - 1}");
                        }
                    }
                    // Opening brace?
                    else if (ch == '{')
                    {
                        // Escaped?
                        if (pos < len && format[pos] == '{')
                        {
                            // append ch, skip the repeat
                            pos++;
                        }
                        else
                        {
                            // Opening brace of an argument hole
                            pos--;
                            break;
                        }
                    }
                    
                    // Text to append
                    Write(ch);
                }

                // Start parsing an Argument Hole
                // Argument Hole ::= { Index (, WS* Alignment WS*)? (: Formatting)? }
                
                // Ran out of characters?
                if (pos == len) break;
                
                // Start parsing required Index Parameter
                // Index ::= ('0'-'9')+ WS*

                pos++;
                // If reached end of text then error (Unexpected end of text)
                // or character is not a digit then error (Unexpected Character)
                if (pos == len)
                    throw new FormatException("Ran out of characters for Argument Hole");
                if ((ch = format[pos]) < '0' || ch > '9')
                    throw new FormatException($"Invalid Argument Index at {pos}");
                int index = 0;
                do
                {
                    index = index * 10 + ch - '0';
                    pos++;
                    // If reached end of text then error (Unexpected end of text)
                    if (pos == len)
                        throw new FormatException("Ran out of characters for Argument Index");
                    ch = format[pos];
                    // so long as character is digit and value of the index is less than 1000000 ( index limit )
                } while (ch >= '0' && ch <= '9' && index < IndexLimit);
                
                // If value of index is not within the range of the arguments passed in then error (Index out of range)
                if (index >= args.Length)
                    throw new FormatException($"Argument Index of {index} is not present in provided {nameof(args)}");
                
                // Consume optional whitespace.
                while (pos < len && char.IsWhiteSpace(ch = format[pos])) pos++;
                // End of parsing index parameter.
                
                // Start of parsing optional Alignment
                // Alignment ::= comma WS* minus? ('0'-'9')+ WS*

                Alignment alignment = Alignment.Left;
                int width = 0;
                
                 // Is the character a comma, which indicates the start of alignment parameter.
                if (ch == ',')
                {
                    pos++;

                    // Consume Optional whitespace
                    while (pos < len && format[pos] == ' ') pos++;

                    // If reached the end of the text then error (Unexpected end of text)
                    if (pos == len)
                        throw new FormatException($"Unexpected end-of-text in Argument {index} Alignment");

                    // What alignment did they specify?
                    ch = format[pos];
                    if (ch == '-' || ch == 'l' || ch == 'L' || ch == '<')
                    {
                        // Yes, then alignment is left justified.
                        alignment = Alignment.Left;
                        pos++;
                        // If reached end of text then error (Unexpected end of text)
                        if (pos == len)
                            throw new FormatException($"Unexpected end-of-text in Argument {index} Alignment");
                        ch = format[pos];
                    }
                    else if (ch == '+' || ch == 'r' || ch == 'R' || ch == '>')
                    {
                        // Right-aligned
                        alignment = Alignment.Right;
                        pos++;
                        // If reached end of text then error (Unexpected end of text)
                        if (pos == len)
                            throw new FormatException($"Unexpected end-of-text in Argument {index} Alignment");
                        ch = format[pos];
                    }
                    else if (ch == '_' || ch == 'c' || ch == 'C' || ch == '|')
                    {
                        // Center-Aligned
                        alignment = Alignment.Center;
                        pos++;
                        // If reached end of text then error (Unexpected end of text)
                        if (pos == len)
                            throw new FormatException($"Unexpected end-of-text in Argument {index} Alignment");
                        ch = format[pos];
                    }

                    // If current character is not a digit then error (Unexpected character)
                    if (ch < '0' || ch > '9')
                        throw new FormatException($"Non-digit character '{ch}' in Argument {index} Alignment Width");
                    
                    // Parse alignment digits.
                    do
                    {
                        width = ((width * 10) + ch) - '0';
                        pos++;
                        // If reached end of text then error. (Unexpected end of text)
                        if (pos == len)
                            throw new FormatException($"Unexpected end-of-text in Argument {index} Alignment Width");
                        ch = format[pos];
                        // So long a current character is a digit and the value of width is less than 100000 ( width limit )
                    }
                    while (ch >= '0' && ch <= '9' && width < WidthLimit);
                    // end of parsing Argument Alignment
                    
                    // Consume optional whitespace
                    while (pos < len && (ch = format[pos]) == ' ') pos++;
                }
                
                // Start of parsing optional formatting parameter
                object? arg = args[index];
                ReadOnlySpan<char> argFormatSpan = default;
                
                // Is current character a colon? which indicates start of formatting parameter.
                if (ch == ':')
                {
                    pos++;
                    int startPos = pos;

                    while (true)
                    {
                        // If reached end of text then error. (Unexpected end of text)
                        if (pos == len)
                            throw new FormatException($"Unexpected end-of-text in Argument {index} Format");
                        ch = format[pos];

                        if (ch == '}')
                        {
                            // Argument hole closed
                            break;
                        }
                        if (ch == '{')
                        {
                            // Braces inside the argument hole are not supported
                            throw new FormatException($"Illegal {{ in Argument {index} Format");
                        }
                        pos++;
                    }
                    if (pos > startPos)
                    {
                        argFormatSpan = format.AsSpan(startPos, pos - startPos);
                    }
                }
                else if (ch != '}')
                {
                    // Unexpected character
                    throw new FormatException($"Unexpected character '{ch}' Argument {index}");
                }

                // Construct the output for this arg hole.
                pos++;
                string? s = null;
                string? itemFormat = null;

                if (cf != null)
                {
                    if (argFormatSpan.Length != 0)
                    {
                        itemFormat = new string(argFormatSpan);
                    }
                    s = cf.Format(itemFormat, arg, provider);
                }
                
                if (s is null)
                {
                    if (dump)
                    {
                        DumpOptions options = new DumpOptions(provider,
                                                              new string(argFormatSpan));
                        this.AppendDump(arg, options);
                        continue;
                    }
                    
                    // If arg is ISpanFormattable and the beginning doesn't need padding,
                    // try formatting it into the remaining current chunk.
                    if ((alignment == Alignment.Left || width == 0) &&
                        Formatter.TryFormat(arg, Available, out int charsWritten, argFormatSpan, provider))
                    {
                        _length += charsWritten;

                        // Pad the end, if needed.
                        int padding = width - charsWritten;
                        if (alignment == Alignment.Left && padding > 0)
                        {
                            AppendRepeat(' ', padding);
                        }
                        // Continue to parse other characters.
                        continue;
                    }

                    // Otherwise, fallback to trying IFormattable or calling ToString.
                    if (arg is IFormattable formattableArg)
                    {
                        if (argFormatSpan.Length != 0)
                        {
                            itemFormat ??= new string(argFormatSpan);
                        }
                        s = formattableArg.ToString(itemFormat, provider);
                    }
                    else if (arg != null)
                    {
                        s = arg.ToString();
                    }
                }
                
                // Append it to the final output of the Format String
                int pad = width - (s?.Length ?? 0);
                if (alignment == Alignment.Left)
                {
                    Write(s);
                    AppendRepeat(' ', pad);
                }
                else if (alignment == Alignment.Right)
                {
                    AppendRepeat(' ', pad);
                    Write(s);
                }
                else
                {
                    int padLeft;
                    int padRight;
                    if (alignment.HasFlag<Alignment>(Alignment.Right))
                    {
                        padLeft = (int)Math.Ceiling(pad / 2f);
                        padRight = (int)Math.Floor(pad / 2f);
                    }
                    else
                    {
                        padLeft = (int)Math.Floor(pad / 2f);
                        padRight = (int)Math.Ceiling(pad / 2f);
                    }

                    this.AppendRepeat(' ', padLeft)
                         .Append(s)
                         .AppendRepeat(' ', padRight);
                }

                // Continue to parse other characters.
            }
        }


        public override TextBuilder AppendFormat(FormattableString formattableString)
        {
            string format = formattableString.Format;
            object?[] args = formattableString.GetArguments();
            WriteFormat(null, format, args, false);
            return this;
        }
        
        [StringFormatMethod("format")]
        public override TextBuilder AppendFormat(NonFormattableString format, params object?[] args)
        {
            WriteFormat(null, format.Value, args, false);
            return this;
        }

        public TextBuilder AppendFormat(IFormattable? formattable,
                                        string? format = null,
                                        IFormatProvider? provider = null)
        {
            if (formattable != null)
            {
                Write(formattable.ToString(format, provider));
            }
            return this;
        }

        /// <inheritdoc />
        public override TextBuilder AppendFormat<TFormattable>([AllowNull] TFormattable formattable, string? format = null, IFormatProvider? provider = null)
        {
            if (formattable != null)
            {
                Write(formattable.ToString(format, provider));
            }
            return this;
        }
    }
}