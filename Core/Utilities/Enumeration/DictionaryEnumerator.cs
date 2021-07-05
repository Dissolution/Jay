using System;
using System.Collections;
using System.Collections.Generic;

namespace Jay
{
    public sealed class DictionaryEnumerator : IEnumerator<DictionaryEntry>, IEnumerator, IDisposable
    {
        private readonly IDictionaryEnumerator _dictionaryEnumerator;
        private int _index;

        DictionaryEntry IEnumerator<DictionaryEntry>.Current => _dictionaryEnumerator.Entry;
        object IEnumerator.Current => _dictionaryEnumerator.Entry;

        public int Index => _index;

        public object? Key => _dictionaryEnumerator.Key;
        public object? Value => _dictionaryEnumerator.Value;
        public DictionaryEntry Entry => _dictionaryEnumerator.Entry;
        
        public DictionaryEnumerator(IDictionaryEnumerator dictionaryEnumerator)
        {
            _dictionaryEnumerator = dictionaryEnumerator ?? throw new ArgumentNullException(nameof(dictionaryEnumerator));
            _index = -1;
        }

        public bool MoveNext()
        {
            if (_dictionaryEnumerator.MoveNext())
            {
                _index++;
                return true;
            }
            return false;
        }

        void IEnumerator.Reset()
        {
            _dictionaryEnumerator.Reset();
            _index = -1;
        }

        void IDisposable.Dispose()
        {
            if (_dictionaryEnumerator is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}