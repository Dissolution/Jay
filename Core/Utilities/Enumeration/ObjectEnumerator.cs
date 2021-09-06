using System;
using System.Collections;
using System.Collections.Generic;

namespace Jay
{
    public sealed class ObjectEnumerator : IEnumerator<object?>, IEnumerator, IDisposable
    {
        private readonly IEnumerator _enumerator;
        private int _index;

        public object? Current => _enumerator.Current;
        object? IEnumerator.Current => _enumerator.Current;

        public int Index => _index;
        
        public ObjectEnumerator(IEnumerator enumerator)
        {
            _enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
            _index = -1;
        }

        public bool MoveNext()
        {
            if (_enumerator.MoveNext())
            {
                _index++;
                return true;
            }
            return false;
        }

        void IEnumerator.Reset()
        {
            _enumerator.Reset();
            _index = -1;
        }

        void IDisposable.Dispose()
        {
            if (_enumerator is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}