using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;

namespace Jay.Collections
{
    public abstract class ArrayEnumerator : IEnumerator<object?>, IEnumerator, IDisposable
    {
        public static ArrayEnumerator Create(Array array)
        {
            if (array.Rank == 1)
                return new StandardArrayEnumerator(array);
            return new MultiArrayEnumerator(array);
        }
        
        protected readonly Array _array;
        protected int _index;
        protected object? _current;
        
        public int Index => _index;
        public object? Current => _current;
        
        protected ArrayEnumerator(Array array)
        {
            _array = array ?? throw new ArgumentNullException(nameof(array));
            _index = -1;
            _current = null;
        }

        public abstract bool MoveNext();

        public virtual void Reset()
        {
            _index = -1;
            _current = null;
        }

        void IDisposable.Dispose()
        {
            // Do nothing, just for ease-of-use with 'using'
        }
    }
}