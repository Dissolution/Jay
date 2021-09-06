using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Jay.Collections;
using Jay.Debugging;
using Jay.Debugging.Dumping;
using Jay.Text;

namespace Jay.Logging.Temporary
{
    public class LogEvent
    {
        public DateTimeOffset Timestamp { get; }
        public LogLevel Level { get; }
        public Exception? Error { get; }
        public LogMessage Message { get; }
    }

    public interface ILogEvent
    {
        DateTimeOffset Timestamp { get; }
        LogLevel Level { get; }
        Exception? Error { get; }
    }

    public class LogArguments : TinyListDictionary<string, LogArgument>
    {
        
    }
    public enum DestructureType
    {
        Depends,
        Always,
        Never,
    }
        
    public class LogArgument
    {
        internal StartLen Range { get; }
        
        public DestructureType Destructure { get; }
        
        public object? Value { get; }

        internal LogArgument(StartLen range, object? value, DestructureType destructureType)
        {
            this.Range = range;
            this.Value = value;
            this.Destructure = destructureType;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if (Destructure == DestructureType.Never)
                return Value?.ToString() ?? string.Empty;
            return Dumper.Dump(Value);
        }
    }

    public class FormattableLogArgument : LogArgument
    {
        public Alignment Alignment { get; }
        public int Width { get; }
        public string? Format { get; }

        internal FormattableLogArgument(StartLen range,
                                        object? value,  
                                      DestructureType destructureType = DestructureType.Depends,
                                      Alignment alignment = default,
                                      int width = 0,
                                      string? format = null)
            : base(range, value, destructureType)
        {
            this.Alignment = alignment;
            this.Width = width;
            this.Format = format;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if (Destructure == DestructureType.Never)
                return Value?.ToString() ?? string.Empty;
            return TextBuilder.Build(this, (text, arg) =>
            {
                string valueStr;
                if (!string.IsNullOrWhiteSpace(arg.Format) &&
                    arg.Value is IFormattable formattable)
                {
                    valueStr = formattable.ToString(arg.Format, null);
                }
                else
                {
                    valueStr = arg.Value?.ToString() ?? string.Empty;
                }

                if (arg.Width > 0)
                {
                    text.AppendAlign(valueStr, arg.Alignment, arg.Width);
                }
                else
                {
                    text.Write(valueStr);
                }
            });
        }
    }
        
        
    public class LogMessageArgument
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsValidNameChar(char c)
        {
            return (c >= '0' && c <= '9') ||
                   (c >= 'A' && c <= 'Z') ||
                   c == '_' ||
                   (c >= 'a' && c <= 'z');
        }

        internal static string FixArgName(string? argName)
        {
            return TextBuilder.Build(argName, (text, name) =>
            {
                if (name is null)
                {
                    text.Write(Guid.NewGuid().ToString("N"));
                    return;
                }

                int len = name.Length;
                if (len == 0)
                {
                    text.Write(Guid.NewGuid().ToString("N"));
                    return;
                }

                int pos = 0;
                char ch;
                // Eat initial whitespace
                while (pos < len && (char.IsWhiteSpace(name[pos]))) pos++;
                if (pos == len)
                {
                    text.Write(Guid.NewGuid().ToString("N"));
                    return;
                }

                for (; pos < len; pos++)
                {
                    ch = name[pos];
                    if (IsValidNameChar(ch))
                    {
                        text.Write(ch);
                    }
                    else
                    {
                        text.Write('_');
                    }
                }

                text.TrimEnd('_');

                if (text.Length == 0)
                {
                    text.Write(Guid.NewGuid().ToString("N"));
                }
            });
        }
        
        private string _name;
        
        public StartLen MessageRange { get; }
        
        public bool? Destructure { get; set; }

        public string Name
        {
            get => _name;
            set => _name = FixArgName(value);
        }
        public Alignment Alignment { get; set; }
        public int Width { get; set; }
        public string? Format { get; set; }

        internal LogMessageArgument(StartLen range)
        {
            this.MessageRange = range;
        }

        internal void Write(TextBuilder text)
        {
            if (Destructure == true)
                text.Write('@');
            else if (Destructure == false)
                text.Write('$');
            text.Write(Name);
            if (Width > 0)
            {
                text.Write(',');
                switch (Alignment)
                {
                           
                    case Alignment.Center:
                        text.Write('|');
                        break;
                    case Alignment.Right:
                        text.Write('+');
                        break;
                    case Alignment.Left:
                        text.Write('-');
                        break;
                    default:
                        break;
                }
                text.Append(Width);
            }

            if (Format != null)
            {
                text.Append(':').Write(Format);
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return TextBuilder.Build(this, (text, msg) => msg.Write(text));
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <see cref="https://messagetemplates.org/"/>
    public class LogMessage
    {
        public static LogMessage Empty => new LogMessage(string.Empty, new List<LogMessageArgument>(0));
       
        
        public static LogMessage Parse(string? message)
        {
            if (message is null) return Empty;
            int pos = 0;
            int len = message.Length;
            char ch = '\0';

            var args = new List<LogMessageArgument>();
            
            // Processing loop
            while (true)
            {
                // While we have characters to process
                while (pos < len)
                {
                    ch = message[pos];
                    pos++;
                    // Closing brace?
                    if (ch == '}')
                    {
                        // Check if the next character (if there is one) to see if this is just an escape (eg: '}}')
                        if (pos < len && message[pos] == '}')
                        {
                            // append ch, skip the repeat
                            pos++;
                        }
                        // Otherwise, treat as an error (mismatched closing brace)
                        else
                        {
                            throw new FormatException($"Mismatched closing brace '}}' at {pos - 1}");
                        }
                    }
                    // Opening brace?
                    else if (ch == '{')
                    {
                        // Escaped?
                        if (pos < len && message[pos] == '{')
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
                    //Write(ch);
                }

                // Start parsing an Argument Hole
                // Hole ::= '{' ( '@' | '$' )? ( PropertyName | Index ) ( ',' Alignment )? ( ':' Format )? '}'

                // Ran out of characters?
                if (pos >= len) break;
                pos++;
                
                // If reached end of text then error (Unexpected end of text)
                if (pos == len)
                    throw new FormatException("Ran out of characters for Argument Hole");
                
                // Parse optional capture specifier
                bool? destructure = null;
                ch = message[pos];
                if (ch == '@')
                {
                    destructure = true;
                    pos++;
                    if (pos >= len)
                        throw new FormatException("Ran out of characters for Argument Hole");
                    ch = message[pos];
                }
                else if (ch == '$')
                {
                    destructure = false;
                    pos++;
                    if (pos >= len)
                        throw new FormatException("Ran out of characters for Argument Hole");
                    ch = message[pos];
                }
                
                // Parse PropertyName / Index
                // Name ::= [0-9A-z_]+
                string? name;
                if (!LogMessageArgument.IsValidNameChar(ch))
                    throw new FormatException("Invalid Name character '{ch}' for argument hole");
                int start = pos;
                do
                {
                    pos++;
                    if (pos == len)
                        throw new FormatException("Ran out of name characters for argument hole");
                    ch = message[pos];
                } while (LogMessageArgument.IsValidNameChar(ch));

                name = message.Substring(start, pos - start);
                // End of parsing hole name
                
                // Start of parsing optional Alignment
                // ( ',' Alignment )?
                // Alignment ::= (-+|)? [0-9]+

                Alignment alignment = Alignment.Left;
                int width = 0;
                
                 // Is the character a comma, which indicates the start of alignment parameter.
                if (ch == ',')
                {
                    pos++;
                    
                    // Consume Optional whitespace
                    while (pos < len && char.IsWhiteSpace(message[pos])) pos++;

                    // If reached the end of the text then error (Unexpected end of text)
                    if (pos == len)
                        throw new FormatException($"Unexpected end-of-text in Argument '{name}' Alignment");

                    // What alignment did they specify?
                    ch = message[pos];
                    if (ch == '-')
                    {
                        // Yes, then alignment is left justified.
                        alignment = Alignment.Left;
                        pos++;
                        // If reached end of text then error (Unexpected end of text)
                        if (pos == len)
                            throw new FormatException($"Unexpected end-of-text in Argument '{name}' Alignment");
                        ch = message[pos];
                    }
                    else if (ch == '+')
                    {
                        // Right-aligned
                        alignment = Alignment.Right;
                        pos++;
                        // If reached end of text then error (Unexpected end of text)
                        if (pos == len)
                            throw new FormatException($"Unexpected end-of-text in Argument '{name}' Alignment");
                        ch = message[pos];
                    }
                    else if (ch == '|')
                    {
                        // Center-Aligned
                        alignment = Alignment.Center;
                        pos++;
                        // If reached end of text then error (Unexpected end of text)
                        if (pos == len)
                            throw new FormatException($"Unexpected end-of-text in Argument '{name}' Alignment");
                        ch = message[pos];
                    }

                    // If current character is not a digit then error (Unexpected character)
                    if (ch < '0' || ch > '9')
                        throw new FormatException($"Non-digit character '{ch}' in Argument '{name}' Alignment Width");
                    
                    // Parse alignment digits.
                    do
                    {
                        width = ((width * 10) + ch) - '0';
                        pos++;
                        // If reached end of text then error. (Unexpected end of text)
                        if (pos == len)
                            throw new FormatException($"Unexpected end-of-text in Argument '{name}' Alignment Width");
                        ch = message[pos];
                        // So long a current character is a digit and the value of width is less than 100000 ( width limit )
                    }
                    while (ch >= '0' && ch <= '9');
                    // end of parsing Argument Alignment
                    
                    // Consume optional whitespace
                    while (pos < len && (ch = message[pos]) == ' ') pos++;
                }
                
                // Start of parsing optional formatting parameter
                // ( ':' Format )?
                // Format ::= [^\{]+
                string? format = null;
                
                // Is current character a colon? which indicates start of formatting parameter.
                if (ch == ':')
                {
                    pos++;
                    int startPos = pos;

                    while (true)
                    {
                        // If reached end of text then error. (Unexpected end of text)
                        if (pos == len)
                            throw new FormatException($"Unexpected end-of-text in Argument '{name}' Format");
                        ch = message[pos];

                        if (ch == '}')
                        {
                            // Argument hole closed
                            break;
                        }
                        if (ch == '{')
                        {
                            // Braces inside the argument hole are not supported
                            throw new FormatException($"Illegal {{ in Argument '{name}' Format");
                        }
                        pos++;
                    }
                    if (pos > startPos)
                    {
                        format = message.Substring(startPos, pos - startPos);
                    }
                }
                else if (ch != '}')
                {
                    // Unexpected character
                    throw new FormatException($"Unexpected character '{ch}' Argument '{name}'");
                }

                // Construct the output for this arg hole.
                

                var startLen = new StartLen(start, pos - start);
                var arg = new LogMessageArgument(startLen)
                {
                    Destructure = destructure,
                    Name = name,
                    Alignment = alignment,
                    Width = width,
                    Format = format,
                };
                args.Add(arg);

                // Continue to parse other characters.
                pos++;
            }

            return new LogMessage(message, args);
        }

        public static LogMessage Parse(Expression<Func<FormattableString>> expr)
        {
            var getFormattableString = expr.Compile();
            FormattableString formattableString = getFormattableString();
            var msg = Parse(formattableString.Format);
                
            var call = expr.Body as MethodCallExpression;
            Debug.Assert(call != null);
            //var formatter = call.Arguments[0];
            var converts = call.Arguments[1] as NewArrayExpression;
            Debug.Assert(converts != null);
            var argNames = converts.Expressions
                                   .SelectWhere<Expression, string>((Expression x, out string? name) =>
                                   {
                                       var member = x.GetMember();
                                       if (member is ConstructorInfo ctor)
                                       {
                                           // TODO: Fix Dumper.Dump(Member) to have decent options
                                           name = $"new {ctor.DeclaringType?.Name}";
                                           return true;
                                       }
                                       else if (member != null)
                                       {
                                           name = member.Name;
                                           return true;
                                       }

                                       if (x is ConstantExpression constantExpression)
                                       {
                                           name = constantExpression.Value?.ToString();
                                           if (!string.IsNullOrWhiteSpace(name))
                                               return true;
                                       }

                                       name = default;
                                       return false;
                                   }).ToList();
            Debug.Assert(argNames.Count == msg.Args.Count);
            for (var i = 0; i < argNames.Count; i++)
            {
                msg.Args[i].Name = argNames[i];
            }
            return msg;
        }
        
        public string Message { get; }
        public IReadOnlyList<LogMessageArgument> Args { get; }

        protected LogMessage(string message, List<LogMessageArgument> args)
        {
            this.Message = message;
            this.Args = args;
        }
    }

    public interface IArgumentRenderer
    {
        bool CanRender(Type argumentType);
        Result TryRender(TextBuilder textBuilder, object? argument);
    }

    internal sealed class DefaultArgumentRenderer : IArgumentRenderer
    {
        public static IArgumentRenderer Instance { get; } = new DefaultArgumentRenderer();

        /// <inheritdoc />
        public bool CanRender(Type argumentType)
        {
            return true;
        }

        /// <inheritdoc />
        public Result TryRender(TextBuilder textBuilder, object? argument)
        {
            textBuilder.Append(argument);
            return true;
        }
    }
    

    public interface IArgumentRenderer<in T> : IArgumentRenderer
    {
        Result TryRender(TextBuilder textBuilder, [AllowNull] T argument);
    }

    public interface IRenderer
    {
        List<IArgumentRenderer> ArgumentRenderers { get; }
        
        void Render(TextBuilder textBuilder, LogMessage message);
    }

    public abstract class Renderer : IRenderer
    {
        protected readonly TypeCache<IArgumentRenderer> _argRendererCache;
        
        /// <inheritdoc />
        public List<IArgumentRenderer> ArgumentRenderers { get; }

        protected Renderer()
        {
            this.ArgumentRenderers = new List<IArgumentRenderer>();
            _argRendererCache = new TypeCache<IArgumentRenderer>();
        }

        protected IArgumentRenderer GetRenderer(Type argType)
        {
            return _argRendererCache.GetOrAdd(argType, type =>
            {
                foreach (var r in ArgumentRenderers)
                {
                    if (r.CanRender(type))
                        return r;
                }
                return DefaultArgumentRenderer.Instance;
            });
        }
        protected IArgumentRenderer GetRenderer(object? arg)
        {
            if (arg is null) return DefaultArgumentRenderer.Instance;
            return _argRendererCache.GetOrAdd(arg.GetType(), type =>
            {
                foreach (var r in ArgumentRenderers)
                {
                    if (r.CanRender(type))
                        return r;
                }
                return DefaultArgumentRenderer.Instance;
            });
        }
        
        
        /// <inheritdoc />
        public void Render(TextBuilder textBuilder, LogMessage message)
        {
            var msg = message.Message;
            var args = message.Args;
            if (args.Count == 0)
            {
                textBuilder.Write(msg);
            }
            else
            {
                int start = 0;
                for (var a = 0; a < args.Count; a++)
                {
                    var arg = args[a];
                    var range = new Range(start, arg.MessageRange.Start);
                    textBuilder.Write(msg[range]);
                    start = arg.MessageRange.End;
                    throw new NotImplementedException();
                    // var renderer = GetRenderer(arg.Value);
                }
            }
        }
    }
}