using System;
using Jay.Debugging.Dumping;
using Jay.Text;

namespace Jay.Logging
{
    public class ExceptionEventArgs : EventArgs
    {
        public string Watcher { get; }
        public Exception? Exception { get; }

        public ExceptionEventArgs(string watcher, Exception exception)
        {
            this.Watcher = watcher ?? "UnhandledException";
            this.Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return TextBuilder.Build(this, (tb, args) => tb.Append(args.Watcher).Append(": ").AppendLine()
                                                           .AppendDump(Exception));
        }
    }
}