using System;
using System.Diagnostics;

namespace Jay.Logging.WPF
{
    public sealed class UnhandledBindingExceptionWatcher : UnhandledExceptionWatcher
    {
        private readonly BindingErrorListener _bindingErrorListener;
        
        public UnhandledBindingExceptionWatcher(SourceLevels sourceLevels = SourceLevels.Warning)
        {
            //Refresh
            PresentationTraceSources.Refresh();
            //Set our level
            PresentationTraceSources.DataBindingSource.Switch.Level = sourceLevels;
            //Add our listener
            _bindingErrorListener = new BindingErrorListener(this);
            PresentationTraceSources.DataBindingSource.Listeners.Add(_bindingErrorListener);
        }

        internal void OnWriteException(Exception exception)
        {
            OnException(this, "DataBindingSource Listener", exception);
        }
        
        /// <inheritdoc />
        public override void Dispose()
        {
            PresentationTraceSources.Refresh();
            PresentationTraceSources.DataBindingSource.Listeners.Remove(_bindingErrorListener);
        }
    }
}