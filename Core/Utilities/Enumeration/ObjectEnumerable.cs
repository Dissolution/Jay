using System;
using System.Collections;
using System.Collections.Generic;

namespace Jay
{
    public sealed class ObjectEnumerable : IEnumerable<object?>, IEnumerable
    {
        private readonly IEnumerable _enumerable;

        public ObjectEnumerable(IEnumerable enumerable)
        {
            _enumerable = enumerable ?? throw new ArgumentNullException(nameof(enumerable));
        }

        public ObjectEnumerator GetEnumerator()
        {
            return new ObjectEnumerator(_enumerable.GetEnumerator());
        }

        IEnumerator<object?> IEnumerable<object?>.GetEnumerator()
        {
            return new ObjectEnumerator(_enumerable.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ObjectEnumerator(_enumerable.GetEnumerator());
        }
    }
}