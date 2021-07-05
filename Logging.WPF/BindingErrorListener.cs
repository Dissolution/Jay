using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Jay.Logging.WPF
{
    /// <inheritdoc cref="TraceListener"/>
    /// <summary>
    /// A <see cref="TraceListener"/> specifically for Binding Errors.
    /// </summary>
    internal sealed class BindingErrorListener : TraceListener
    {
        private const string BindingErrorPattern = @"^BindingExpression path error(?:.+)'(.+)' property not found(?:.+)object[\s']+(.+?)'(?:.+)target element is '(.+?)'(?:.+)target property is '(.+?)'(?:.+)$";
        private static readonly Regex _bindingErrorRegex;

        static BindingErrorListener()
        {
            _bindingErrorRegex = new Regex(BindingErrorPattern, RegexOptions.Compiled);
        }

        private readonly UnhandledBindingExceptionWatcher _watcher;

        public BindingErrorListener(UnhandledBindingExceptionWatcher watcher)
        {
            _watcher = watcher;
        }

        public override void Write(string? message)
        {
            Write(message, null);
        }
        
        public void Write(string? message, Exception? innerException)
        {
            var ex = new BindingException(message, innerException);
            if (!string.IsNullOrWhiteSpace(message))
            {
                var match = _bindingErrorRegex.Match(message);
                if (match.Success && match.Groups.Count >= 5)
                {
                    ex.SourceObject = match.Groups[2].Value;
                    ex.SourceProperty = match.Groups[1].Value;
                    ex.TargetElement = match.Groups[3].Value;
                    ex.TargetProperty = match.Groups[4].Value;
                }
            }
            _watcher.OnWriteException(ex);
        }

        public override void WriteLine(string? message)
        {
            WriteLine(message, null);
        }
        
        public void WriteLine(string? message, Exception? innerException)
        {
            var ex = new BindingException(message, innerException);
            if (!string.IsNullOrWhiteSpace(message))
            {
                var match = _bindingErrorRegex.Match(message);
                if (match.Success && match.Groups.Count >= 5)
                {
                    ex.SourceObject = match.Groups[2].ToString();
                    ex.SourceProperty = match.Groups[1].ToString();
                    ex.TargetElement = match.Groups[3].ToString();
                    ex.TargetProperty = match.Groups[4].ToString();
                }
            }
            _watcher.OnWriteException(ex);
        }
    }
}