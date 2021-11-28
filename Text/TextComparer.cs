
using Jay.Text.Extensions;

namespace Jay.Text
{
    public abstract class TextComparer : IEqualityComparer<char>, IComparer<char>,
                                         IEqualityComparer<string>, IComparer<string>, 
                                         IEqualityComparer<char[]>, IComparer<char[]>
    {


        public virtual int Compare(char x, char y)
        {
            return Compare(x.ToReadOnlySpan(), y.ToReadOnlySpan());
        }

        public virtual int Compare(string? x, string? y)
        {
            return Compare((text)x, (text)y);
        }

        public virtual int Compare(char[]? x, char[]? y)
        {
            return Compare((text)x, (text)y);
        }

        public abstract int Compare(text x, text y);

        public virtual bool Equals(char x, char y)
        {
            return Equals(x.ToReadOnlySpan(), y.ToReadOnlySpan());
        }

        public virtual bool Equals(string? x, string? y)
        {
            return Equals((text)x, (text)y);
        }

        public virtual bool Equals(char[]? x, char[]? y)
        {
            return Equals((text)x, (text)y);
        }

        public abstract bool Equals(text x, text y);

        public int GetHashCode(char ch)
        {
            return GetHashCode(ch.ToReadOnlySpan());
        }

        public int GetHashCode(string? text)
        {
            return GetHashCode((text)text);
        }
        
        public int GetHashCode(char[] charArray)
        {
            return GetHashCode((text)charArray);
        }

        public abstract int GetHashCode(text text);

    }
}
