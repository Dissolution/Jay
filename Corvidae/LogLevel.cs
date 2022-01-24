using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Jay.Text;

namespace Jay.Corvidae
{
    /* https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/interpolated-string-handler
     *
     *
     */


    public enum LogLevel
    {
        Off = 0,
        Trace,
        Debug,
        Information,
        Warning,
        Error,
        Fatal
    }

    public interface ILogMessageResolver
    {

    }

    public interface ILogEvent
    {
        DateTimeOffset Timestamp { get; }
        LogLevel Level { get; }
        
        string ResolveMessage(ILogMessageResolver? resolver = null);
    }

    public interface ILogger
    {
        bool IsEnabled(LogLevel level);
        void Log(ILogEvent logEvent);

        void Log(LogLevel level, [InterpolatedStringHandlerArgument("", "level")] LogInterpolatedStringHandler message);
        void Log(Exception? exception, LogLevel level, [InterpolatedStringHandlerArgument("", "level")] LogInterpolatedStringHandler message);

        // void LogThrow<TException>(LogLevel level,
        //                           [InterpolatedStringHandlerArgument("", "level")] LogInterpolatedStringHandler message)
        //     where TException : Exception;
        //
        // void LogThrow<TException>(Exception? innerException, 
        //                           LogLevel level,
        //                           [InterpolatedStringHandlerArgument("", "level")] LogInterpolatedStringHandler message)
        //     where TException : Exception;

    }

    public interface ILogEventParameter
    {
        string? Name { get; }
        object? Value { get; }
    }

    public interface ILogMessageParameter : ILogEventParameter
    {
        string? Format { get; }
    }

    public record class LogEventParameter(string Name, object? Value) : ILogEventParameter
    {

    }

    public record class LogMessageParameter(string? Name, object? Value, string? Format = null) : ILogMessageParameter
    {

    }

    public interface ILogMessage
    {
        string Template { get; }
        ILogMessageParams Parameters { get; }

        string Render(ILogMessageResolver resolver);
    }

    public interface ILogMessageParams : IList<ILogEventParameter>
    {

    }

    [InterpolatedStringHandler]
    public ref struct LogInterpolatedStringHandler
    {
        private readonly List<object?>? _parts;

        public LogInterpolatedStringHandler(int literalLength, int formattedCount,
                                            ILogger logger,
                                            LogLevel level,
                                            out bool isEnabled)
        {
            isEnabled = logger.IsEnabled(level);
            if (isEnabled)
            {
                _parts = new(4);
            }
            else
            {
                _parts = null;
            }
        }

        public void AppendLiteral(string? str)
        {
            _parts!.Add(str);
        }

        public void AppendFormatted<T>(T value, [CallerArgumentExpression("value")] string? valueName = null)
        {
            var p = new LogMessageParameter(valueName, value);
            _parts!.Add(p);
        }

        public void AppendFormatted<T>(T value, string? format, [CallerArgumentExpression("value")] string? valueName = null)
        {
            var p = new LogMessageParameter(valueName, value, format);
            _parts!.Add(p);
        }

        public void Render(TextBuilder textBuilder)
        {
            if (_parts is null) return;

            foreach (var part in _parts)
            {
                if (part.Is(out string? str))
                {
                    textBuilder.Write(str);
                }
                else if (part.Is(out ILogMessageParameter? logMsgParam))
                {
                    textBuilder.WriteFormat(logMsgParam.Value, logMsgParam.Format);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}
