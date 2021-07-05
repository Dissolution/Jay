using System;

namespace Jay.Collections
{
    internal sealed class StandardArrayEnumerator : ArrayEnumerator
    {
        public StandardArrayEnumerator(Array array)
            : base(array)
        {
            
        }

        /// <inheritdoc />
        public override bool MoveNext()
        {
            var newIndex = _index + 1;
            if (newIndex < _array.Length)
            {
                _index = newIndex;
                _current = _array.GetValue(newIndex);
                return true;
            }
            else
            {
                _index = _array.Length;
                _current = null;
                return false;
            }
        }
    }
}