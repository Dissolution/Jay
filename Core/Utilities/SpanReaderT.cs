namespace Jay.Utilities
{
    public ref struct SpanReader<T>
    {
        private readonly ReadOnlySpan<T> _span;
        private int _index;
        private T? _currentValue;

        public int Index => _index;

        public T? Current => _currentValue;

        public ReadOnlySpan<T> PastSpan
        {
            get
            {
                if (_index <= 0) return ReadOnlySpan<T>.Empty;
                if (_index >= _span.Length) return _span;
                return _span[0.._index];
            }
        }

        public ReadOnlySpan<T> FutureSpan
        {
            get
            {
                if (_index <= 0) return _span;
                if (_index >= _span.Length) return ReadOnlySpan<T>.Empty;
                return _span[(_index+1)..];
            }
        }

        public SpanReader(ReadOnlySpan<T> roSpan)
        {
            _span = roSpan;
            _index = -1;
            _currentValue = default;
        }

        public bool MoveNext()
        {
            int i = _index + 1;
            if (i < _span.Length)
            {
                _index = i;
                _currentValue = _span[i];
                return true;
            }
            else
            {
                _currentValue = default;
                return false;
            }
        }

        public void SkipUntil(Func<T, bool> predicate)
        {
            int i = _index;
            var span = _span;
            if ((uint)i >= span.Length) return;
            while (i < span.Length)
            {
                if (predicate(span[i]))
                {
                    _index = i;
                    return;
                }
                i++;
            }
            _index = span.Length;
        }
    }
}
