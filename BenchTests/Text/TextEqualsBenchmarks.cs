using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using Jay.Reflection;

namespace Jay.BenchTests.Text;

[ShortRunJob]
public class TextEqualsBenchmarks
{
    public IEnumerable<object[]> Args()
    {
        yield return new object[2] { "Admin", "Admin" };
        yield return new object[2] { "abcdefghijklmnopqrstuvwxyz", "abcdefghijklmnopqrstuvwxyZ" };
        //yield return new object[2] { "{C5986730-0603-495A-877E-689196538B92}", "{709A3557-0146-44A2-BAEE-3FA25D023744}" };
        yield return new object[2] { "/r/n", "0123456789012345678901234567890123456789012345678901234567890123456789" };
        yield return new object[2] { "0123456789012345678901234567890123456789012345678901234567890123456789", "/r/n" };

    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Args))]
    public bool Equals_Baseline(string? x, string? y)
    {
        return string.Equals(x, y);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Args))]
    public bool Equals_SequenceEqual(string? x, string? y)
    {
        return MemoryExtensions.SequenceEqual<char>(x, y);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Args))]
    public bool Equals_MemCmp(string? x, string? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null || x.Length != y.Length) return false;
        return Compare(in x.GetPinnableReference(),
            in y.GetPinnableReference(),
            x.Length) == 0;

        

    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe int memcmp(void* ptr1, void* ptr2, nuint byteCount);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare(in char x, in char y, int charCount)
    {
        unsafe
        {
            return memcmp(Danger.InToVoidPointer(in x),
                          Danger.InToVoidPointer(in y),
                (nuint)(charCount * 2));
        }
    }

}