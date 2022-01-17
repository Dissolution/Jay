namespace Jay.Text;

public static class CharExtensions
{
    public static ReadOnlySpan<char> ToReadOnlySpan(ref this char ch)
    {
        unsafe
        {
            return new ReadOnlySpan<char>(Unsafe.AsPointer(in ch), 1);
        }
    }
}