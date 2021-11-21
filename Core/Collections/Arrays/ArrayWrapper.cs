using System;
using System.Collections;
using System.Collections.Generic;

namespace Jay.Collections
{
    /// <summary>
    /// A list-like wrapper around <see cref="Array"/>
    /// </summary>
    public sealed class ArrayWrapper : IList<object?>, IReadOnlyList<object?>,
                                       ICollection<object?>, IReadOnlyCollection<object?>,
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

        /// <inheritdoc />
        bool ICollection<object?>.IsReadOnly => false;

        public ArrayWrapper(Array array)
        {
            _array = array ?? throw new ArgumentNullException(nameof(array));
        }

        /// <inheritdoc />
        void ICollection<object?>.Add(object? item)
        {
            throw new InvalidOperationException("Cannot Add to an ArrayWrapper");
        }
        /// <inheritdoc />
        void IList<object?>.Insert(int index, object? item)
        {
            throw new InvalidOperationException("Cannot Insert to an ArrayWrapper");
        }


        /// <inheritdoc />
        public bool Contains(object? item)
        {
            for (var i = 0; i < Count; i++)
            {
                if (Comparison.Comparison.Equals(item, _array.GetValue(i)))
                    return true;
            }
            return false;
        }
        
        
        /// <inheritdoc />
        public int IndexOf(object? item)
        {
            for (var i = 0; i < Count; i++)
            {
                if (Comparison.Comparison.Equals(item, _array.GetValue(i)))
                    return i;
            }
            return -1;
        }

        /// <inheritdoc />
        public void CopyTo(object?[] array, int arrayIndex = 0)
        {
            if (arrayIndex < 0 || arrayIndex + Count > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            for (var i = 0; i < Count; i++)
            {
                array[i] = _array.GetValue(i);
            }
        }

        
        /// <inheritdoc />
        void IList<object?>.RemoveAt(int index)
        {
            throw new InvalidOperationException("Cannot Remove from an ArrayWrapper");
        }
        
        /// <inheritdoc />
        bool ICollection<object?>.Remove(object? item)
        {
            throw new InvalidOperationException("Cannot Remove from an ArrayWrapper");
        }

        /// <inheritdoc />
        void ICollection<object?>.Clear()
        {
            _array.Initialize();
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

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is Array array) return ReferenceEquals(_array, array);
            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Hasher.Create(_array);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return TextBuilder.Build(_array, (text, array) => text.Append('[')
                                                                  .AppendDelimit(',', array)
                                                                  .Append(']'));
        }
    }
}