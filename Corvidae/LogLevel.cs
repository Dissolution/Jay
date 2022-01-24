using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Jay.Text;

namespace Jay.Corvidae
{
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
    }

    [InterpolatedStringHandler]
    public ref struct LogInterpolatedStringHandler
    {
        // Storage for the built-up string
        private readonly TextBuilder? _textBuilder;

        public LogInterpolatedStringHandler(int literalLength, int formattedCount,
                                            ILogger logger,
                                            LogLevel level,
                                            out bool isEnabled)
        {
            isEnabled = logger.IsEnabled(level);
            _textBuilder = isEnabled ? new TextBuilder() : null;
        }

        public void AppendLiteral(string? str)
        {
            _textBuilder!.Write(str);
        }

        public void AppendFormatted<T>(T value, [CallerArgumentExpression("value")] string? valueName)
        {
            _textBuilder!.WriteFormat<T>(value);
        }

        public void AppendFormatted<T>(T value, string? format)
        {
            _textBuilder!.WriteFormat<T>(value, format);
        }

        public string ToStringAndClear()
        {
            if (_textBuilder is not null)
            {
                var str = _textBuilder.ToString();
                _textBuilder.Dispose();
                return str;
            }
            return string.Empty;
        }
    }
}
