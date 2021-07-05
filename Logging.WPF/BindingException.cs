using System;

namespace Jay.Logging.WPF
{
    /// <inheritdoc />
    /// <summary>
    /// An <see cref="Exception"/> thrown during WPF Data Binding/
    /// </summary>
    public class BindingException : Exception
    {
        /// <summary>
        /// The name of the source Object.
        /// </summary>
        public string? SourceObject { get; internal set; }
        /// <summary>
        /// The name of the source Property.
        /// </summary>
        public string? SourceProperty { get; internal set; }

        /// <summary>
        /// The name of the target Element.
        /// </summary>
        public string? TargetElement { get; internal set; }
        /// <summary>
        /// The name of the target Element's Property.
        /// </summary>
        public string? TargetProperty { get; internal set; }

        /// <summary>
        /// Create a new Binding Exception with a message.
        /// </summary>
        public BindingException(string? message = null, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}