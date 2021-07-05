using System;
using System.Collections;
using System.Collections.Generic;

namespace Jay.Collections
{
    /// <summary>
    /// An IEnumerable&lt;<see cref="object"/>&gt; wrapper for <see cref="Array"/>
    /// </summary>
    public sealed class ArrayEnumerable : IReadOnlyList<object?>,
                                          IReadOnlyCollection<object?>,
                                          IEnumerable<object?>
    {
        private readonly Array _array;

        /// <summary>
        /// Gets or sets the <see cref="object"/> at the given <paramref name="index"/>
        /// </summary>
        /// <param name="index"></param>
        public object? this[int index]
        {
            get => _array.GetValue(index);
            set => _array.SetValue(value, index);
        }
        
        /// <summary>
        /// Gets the number of items in the <see cref="Array"/>.
        /// </summary>
        public int Count => _array.Length;


        public ArrayEnumerable(Array array)
        {
            _array = array ?? throw new ArgumentNullException(nameof(array));
        }
      
        
        public ArrayEnumerator GetEnumerator()
        {
            return ArrayEnumerator.Create(_array);
        }
        
        IEnumerator<object?> IEnumerable<object?>.GetEnumerator()
        {
            return ArrayEnumerator.Create(_array);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ArrayEnumerator.Create(_array);
        }
    }
}