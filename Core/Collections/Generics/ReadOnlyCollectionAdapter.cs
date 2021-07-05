using System.Collections;
using System.Collections.Generic;

namespace Jay.Collections
{
    internal sealed class ReadOnlyCollectionAdapter<T> : IReadOnlyCollection<T>,
                                                         IEnumerable<T>,
                                                         IEnumerable
    {
        private readonly ICollection<T> _collection;

        public int Count => _collection.Count;
        
        public ReadOnlyCollectionAdapter(ICollection<T> collection)
        {
            _collection = collection;
        }


        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => _collection.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => _collection.GetEnumerator();
    }
}