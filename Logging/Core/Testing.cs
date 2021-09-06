using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Jay.Debugging;
using Jay.Text;

namespace Jay.Logging
{
    public enum LogLevel
    {
        Trace = 0,
        Debug = 1,
        Information = 2,
        Warning = 3,
        Error = 4,
        Fatal = 5,
    }
  

    public interface ILogWriter : IDisposable
    {
        void Write(LogLevel logLevel, Exception exception);
        void Write(LogLevel logLevel, string message, params object?[] args);
        void Write(LogLevel logLevel, Exception? exception, string? message, params object?[] args);

        void Write<T>(LogLevel logLevel, [AllowNull] T context, Expression<Func<T, FormattableString>> message);
        void Write<T>(LogLevel logLevel, Exception? exception, [AllowNull] T context, Expression<Func<T, FormattableString>> message);

        void Write(ILogMessage message);
    }

    internal abstract class LogWriter : ILogWriter
    {
        /// <inheritdoc />
        public void Write(LogLevel logLevel, Exception exception)
        {
            var message = new LogMessage(exception, logLevel);
        }

        /// <inheritdoc />
        public void Write(LogLevel logLevel, string message, params object?[] args)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Write(LogLevel logLevel, Exception? exception, string? message, params object?[] args)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Write<T>(LogLevel logLevel, T context, Expression<Func<T, FormattableString>> message)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Write<T>(LogLevel logLevel, Exception? exception, T context, Expression<Func<T, FormattableString>> message)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Write(ILogMessage message)
        {
            throw new NotImplementedException();
        }
        
        /// <inheritdoc />
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public interface IArgumentFormatter
    {
        bool TryFormat(object? argument, TextBuilder textBuilder);
    }

    public interface IArgumentFormatter<in T> : IArgumentFormatter
    {
        bool TryFormat([AllowNull] T argument, TextBuilder textBuilder);
    }

    internal class ArgumentFormatters
    {
        private readonly List<IArgumentFormatter> _argumentFormatters;

        public ArgumentFormatters()
        {
            _argumentFormatters = new List<IArgumentFormatter>();
        }

        public void AddFormatter<T>(Action<T, TextBuilder> writeValue)
        {
            throw new NotImplementedException();
        }
    }
    
    internal sealed class ParsedMessage
    {
        private readonly string _format;
        private readonly Range[] _argumentHoles;
        private readonly List<(string, object?)> _arguments;

        public List<(string, object?)> Arguments => _arguments;

        public ParsedMessage(string format, Range[] argumentHoles, IEnumerable<(string, object?)> arguments)
        {
            _format = format;
            _argumentHoles = argumentHoles;
            _arguments = new List<(string, object?)>(arguments);
        }

        public void WriteTo(TextBuilder textBuilder)
        {
        }
    }
    
    public sealed class LogMessage : ILogMessage
    {
        private IDictionary<string, object?>? _data;
        
        /// <inheritdoc />
        public DateTimeOffset Timestamp { get; }

        /// <inheritdoc />
        public LogLevel Level { get; set; }

        /// <inheritdoc />
        public Exception? Exception { get; set; }

        /// <inheritdoc />
        public string Message { get; }

        /// <inheritdoc />
        public IReadOnlyList<object?> Arguments { get; }

        /// <inheritdoc />
        public IDictionary<string, object?> Data => (_data ??= new Dictionary<string, object?>(0));

        public LogMessage(Exception exception, LogLevel logLevel = LogLevel.Error)
        {
            this.Timestamp = DateTimeOffset.Now;
            this.Level = logLevel;
            this.Exception = exception;
            this.Message = exception.Message;
            this.Arguments = Array.Empty<object?>();
            if (exception.Data.Count > 0)
            {
                _data = new Dictionary<string, object?>(exception.Data.Count, StringComparer.OrdinalIgnoreCase);
                foreach (DictionaryEntry entry in exception.Data)
                {
                    _data[(entry.Key.ToString() ?? string.Empty)] = entry.Value;
                }
            }
        }
        
        public LogMessage(string message, params object?[] args)
        {
            this.Timestamp = DateTimeOffset.Now;
            this.Level = default;
            this.Exception = null;
            this.Message = message;
            this.Arguments = args;
            _data = null;
        }
    }
    
    public interface ILogMessage
    {
        DateTimeOffset Timestamp { get; }
        LogLevel Level { get; set; }
        Exception? Exception { get; set; }
        string Message { get; }
        IReadOnlyList<object?> Arguments { get; }
        
        IDictionary<string, object?> Data { get; }
    }
    
    public class Testing
    {
        public sealed class StructuredMessageArgument
        {
            public StartLen MessageSpan { get; }
            public string Name { get; }
            public object? Value { get; }
            public string? Format { get; set; }
            public bool? Destructure { get; set; }

            public StructuredMessageArgument(StartLen messageSpan,
                                             string name,
                                             object? value)
            {
                this.MessageSpan = messageSpan;
                this.Name = name;
                this.Value = value;
                this.Format = null;
                this.Destructure = null;
            }
        }
        

        
        public class StructuredMessage
        {

            
            private static IEnumerable<StartLen> GetArgRanges(string format)
            {
                //if (format is null) throw new ArgumentNullException(nameof(format));
                int pos = 0;
                int len = format.Length;
                char ch = '\0';
    
                int start = 0;
                bool inHole = false;
                
                // While we have characters to process
                while (pos < len)
                {
                    ch = format[pos];
                    pos++;

                    // Opening brace?
                    if (ch == '{')
                    {
                        // Escaped?
                        if (pos < len && format[pos] == '{')
                        {
                            pos++;
                        }
                        // Already in a hole?
                        else if (inHole)
                        {
                            throw new FormatException($"Mismatched opening brace '{{' at {pos - 1}");
                        }
                        else
                        {
                            start = pos;
                            inHole = true;
                        }
                    }
                    // Closing brace?
                    else if (ch == '}')
                    {
                        // Check if the next character (if there is one) to see if this is just an escape (eg: '}}')
                        if (pos < len && format[pos] == '}')
                        {
                            pos++;
                        }
                        // Otherwise, in hole?
                        else if (inHole)
                        {
                            yield return new StartLen(start, pos - start - 1);
                            inHole = false;
                        }
                        else
                        {
                            throw new FormatException($"Mismatched closing brace '}}' at {pos - 1}");
                        }
                    }
                }

                if (inHole)
                {
                    throw new FormatException($"Mismatched opening brace '{{' at {start - 1}");
                }
            }

            
            public static StructuredMessage Create(string format, params object?[] args)
            {
                var ranges = GetArgRanges(format).ToList();
                if (ranges.Count != args.Length)
                    throw new InvalidOperationException();
                var msgArgs = new List<StructuredMessageArgument>(args.Length);
                for (var i = 0; i < args.Length; i++)
                {
                    var range = ranges[i];
                    // TODO: Format
                    msgArgs.Add(new StructuredMessageArgument(range,
                                                              format.Substring(range.Start, range.Length),
                                                              args[i]));
                }
                Hold.Debug(format, ranges, args, msgArgs);
                return new StructuredMessage(format, msgArgs);
            }
            
            public string Format { get; }
            public IReadOnlyList<StructuredMessageArgument> Args { get; }

            protected StructuredMessage(string format,
                                        List<StructuredMessageArgument> args)
            {
                this.Format = format;
                this.Args = args;
            }
        }
        
        internal sealed class Msg
        {
            public string Format { get; }
            public Range[] ArgSpans { get; }
            public object?[] Args { get; }
            public string[] ArgNames { get; }
            
            public Msg(Expression<Func<FormattableString>> getFormat)
            {
                var getFormattableString = getFormat.Compile();
                FormattableString formattableString = getFormattableString();
                this.Format = formattableString.Format;
                for (var i = 0; i < Format.Length; i++)
                {
                    // Look for start of argument hole
                }
                
                
                this.Args = formattableString.GetArguments();
                
                var call = getFormat.Body as MethodCallExpression;
                Debug.Assert(call != null);
                //var formatter = call.Arguments[0];
                var converts = call.Arguments[1] as NewArrayExpression;
                Debug.Assert(converts != null);
                this.ArgNames = converts.Expressions
                                    .SelectWhere<Expression, string>((Expression x, out string? name) =>
                                    {
                                        name = x.GetMember()?.Name;
                                        return !string.IsNullOrWhiteSpace(name);
                                    }).ToArray();
            }
        }
        
        public static void TestLog(LogLevel level, Expression<Func<FormattableString>> expression)
        {
            var msg = new Msg(expression);
            Hold.Debug(msg);
        }

        public static void TestLog(LogLevel level, string message, params object?[] args)
        {
            
        }
        
        public static void Test<T>(T instance, Expression<Func<T, FormattableString>> expr)
        {
            var func = expr.Compile();
            FormattableString fstr = func(instance);
            string format = fstr.Format;
            var args = fstr.GetArguments();

            Hold.Debug(expr, instance, func, format, args);

            var call = expr.Body as MethodCallExpression;
            var formatter = call.Arguments[0];
            var converts = call.Arguments[1] as NewArrayExpression;
            var names = converts.Expressions
                                .SelectWhere((Expression? x, out string? name) =>
                                {
                                    var member = x.GetMember();
                                    if (member != null)
                                    {
                                        name = member.Name;
                                        return true;
                                    }

                                    name = default;
                                    return false;
                                }).ToList();
            
            Hold.Debug(call, converts, names);
        }
    }
}