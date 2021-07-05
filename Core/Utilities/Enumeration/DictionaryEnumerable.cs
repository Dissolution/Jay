using System;
using System.Collections;
using System.Collections.Generic;

namespace Jay
{
    public sealed class DictionaryEnumerable : IEnumerable<DictionaryEntry>, IEnumerable
    {
        private readonly IDictionary _dictionary;

        public DictionaryEnumerable(IDictionary dictionary)
        {
            _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        }

        public DictionaryEnumerator GetEnumerator()
        {
            return new DictionaryEnumerator(_dictionary.GetEnumerator());
        }

        IEnumerator<DictionaryEntry> IEnumerable<DictionaryEntry>.GetEnumerator()
        {
            return new DictionaryEnumerator(_dictionary.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new DictionaryEnumerator(_dictionary.GetEnumerator());
        }
    }
}