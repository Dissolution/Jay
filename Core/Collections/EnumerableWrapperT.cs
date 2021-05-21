using Jay.Text;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Jay.Collections
{
    /* Idea:
     * foreach (var item in thing.Wrap())
     
     */

    public readonly struct EnumeratorValue<T>
    {
        public static implicit operator T?(EnumeratorValue<T?> enumeratorValue) => enumeratorValue.Value;
        public static implicit operator (int Index, T? Value)(EnumeratorValue<T?> enumeratorValue) => (enumeratorValue.Index,
                                                                                                       enumeratorValue.Value);
        
        public readonly int Index;
        public readonly int? SourceLength;
        public readonly bool IsFirst;
        public readonly bool IsLast;
        public readonly T? Value;

        public EnumeratorValue(int index, int? sourceLength, bool isFirst, bool isLast, T? value)
        {
            this.Index = index;
            this.SourceLength = sourceLength;
            this.IsFirst = isFirst;
            this.IsLast = isLast;
            this.Value = value;
        }

        public void Deconstruct(out T? value)
        {
            value = this.Value;
        }
        
        public void Deconstruct(out int index, out T? value)
        {
            index = this.Index;
            value = this.Value;
        }

        public override bool Equals(object? obj)
        {
            if (obj is T value)
                return EqualityComparer<T>.Default.Equals(Value, value);
            return false;
        }

        public override int GetHashCode()
        {
            return Hasher.Create(Index, SourceLength, IsFirst, IsLast, Value);
        }

        public override string ToString()
        {
            return TextBuilder.Build(this, (sb, value) =>
            {
                // Index
                sb.Append('[')
                  .Append(value.Index)
                  .Append('/');
                if (value.SourceLength.HasValue)
                    sb.Append(value.SourceLength.Value);
                else
                    sb.Append('?');
                sb.Append("] ")
                  .Append(value.Value);
            });
        }
    }


    public static class EnumerableWrapExtensions
    {
        public static IEnumerable<EnumeratorValue<T>> Wrap<T>(this IEnumerable<T>? enumerable)//,
                                                              //IDisposable? additionalDispose = null)
        {
            if (enumerable is null)
            {
                yield break;
            }
            //IList<T>
            else if (enumerable is IList<T> list)
            {
                var count = list.Count;
                //No items, exit immediately
                if (count == 0)
                    yield break;
                //For each item, yield the entry
                for (var i = 0; i < list.Count; i++)
                {
                    yield return new EnumeratorValue<T>(i, count, i == 0, i == count - 1, list[i]);
                }
            }
            //ICollection<T>
            else if (enumerable is ICollection<T> collection)
            {
                var count = collection.Count;
                if (count == 0)
                    yield break;
                using (var e = collection.GetEnumerator())
                {
                    var i = 0;
                    while (e.MoveNext())
                    {
                        yield return new EnumeratorValue<T>(i, count, i == 0, i == count - 1, e.Current);
                        i++;
                    }
                }
            }
            //IReadOnlyList<T>
            else if (enumerable is IReadOnlyList<T> roList)
            {
                var count = roList.Count;
                //No items, exit immediately
                if (count == 0)
                    yield break;
                //For each item, yield the entry
                for (var i = 0; i < roList.Count; i++)
                {
                    yield return new EnumeratorValue<T>(i, count, i == 0, i == count - 1, roList[i]);
                }
            }
            //IReadOnlyCollection<T>
            else if (enumerable is IReadOnlyCollection<T> roCollection)
            {
                var count = roCollection.Count;
                if (count == 0)
                    yield break;
                using (var e = roCollection.GetEnumerator())
                {
                    var i = 0;
                    while (e.MoveNext())
                    {
                        yield return new EnumeratorValue<T>(i, count, i == 0, i == count - 1, e.Current);
                        i++;
                    }
                }
            }
            //Have to enumerate
            else
            {
                using (var e = enumerable.GetEnumerator())
                {
                    //If we cannot move, we are done
                    if (!e.MoveNext())
                        yield break;

                    //Defaults to first, not last, with index 0
                    var last = false;
                    var i = 0;
                    while (!last)
                    {
                        //Get the current value
                        var current = e.Current;
                        //Move next now to check for last
                        last = !e.MoveNext();
                        //Return our entry
                        yield return new EnumeratorValue<T>(i, null, i == 0, last, current);
                        //increment index
                        i++;
                    }
                }
            }
        }
    }
}