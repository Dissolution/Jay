using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Jay.Debugging;
using Jay.Text;
using Microsoft.Extensions.Logging;

namespace Jay.Logging
{
    internal static class LogInfoCache<T>
    {
        
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
            this.Level = LogLevel.None;
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