using System;
using System.Reflection;
using System.Threading;

namespace Jay
{
    /// <summary>
    /// A helper class to ensure that only one instance of an application is run at a time.
    /// </summary>
    public sealed class SingleInstanceApplication : IDisposable
    {
        private static string GetAssemblyName()
        {
            return Assembly.GetExecutingAssembly()
                           .GetName()
                           .FullName;
        }
        
        private readonly string _name;
        private readonly Mutex _mutex;
        private bool _isFirst;

        /// <summary>
        /// Is this the first instance of this application?
        /// </summary>
        public bool IsFirstInstance => _isFirst;

        /// <summary>
        /// Gets the Name this Single Instance Mutex is registered under
        /// </summary>
        public string MutexName => _name;

        /// <summary>
        /// Construct a new <see cref="SingleInstanceApplication"/> with the specified name used for debugging purposes.
        /// </summary>
        /// <param name="name"></param>
        public SingleInstanceApplication(string? name)
        {
            _name = name ?? GetAssemblyName();
            _mutex = new Mutex(true, name, out _isFirst);
        }
        /// <inheritdoc />
        ~SingleInstanceApplication()
        {
            this.Dispose();
        }

        /// <summary>
        /// Wait until we can be the only instance running.
        /// </summary>
        /// <returns></returns>
        public bool WaitForSingleInstance()
        {
            var waited = _mutex.WaitOne();
            if (waited)
            {
                _isFirst = true;
            }
            return waited;
        }
        
        /// <summary>
        /// Wait until we can be the only instance running.
        /// </summary>
        /// <returns></returns>
        public bool WaitForSingleInstance(TimeSpan timeout)
        {
            var waited = _mutex.WaitOne(timeout);
            if (waited)
            {
                _isFirst = true;
            }
            return waited;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _mutex.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Single Instance Application '{_name}'";
        }
    }
}