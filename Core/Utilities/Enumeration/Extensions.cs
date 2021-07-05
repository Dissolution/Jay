using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Jay
{
    public static class Extensions
    {
        [return: NotNullIfNotNull("enumerable")]
        public static ObjectEnumerable AsObjectEnumerable(this IEnumerable? enumerable)
        {
            if (enumerable is null) return null!;
            return new ObjectEnumerable(enumerable);
        }
        
        [return: NotNullIfNotNull("enumerable")]
        public static DictionaryEnumerable AsDictionaryEnumerable(this IDictionary? dictionary)
        {
            if (dictionary is null) return null!;
            return new DictionaryEnumerable(dictionary);
        }
    }
}