using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace Jay.Utilities;

public static class Radix
{
    internal static readonly Dictionary<ulong, string> _baseChars = new()
    {
        {  2UL, "01" },
        {  8UL, "01234567" },
        { 10UL, "0123456789" },
        { 16UL, "0123456789ABCDEF" },
        { 62UL, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz" },
        { 64UL, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/" },
        { 174UL, "ぁあぃいぅうぇえぉおかがきぎくぐけげこごさざしじすずせぜそぞただちぢっつづてでとどなにぬねのはばぱひびぴふぶぷへべぺほぼぽまみむめもゃやゅゆょよらりるれろゎわゐゑをんゔァアィイゥウェエォオカガキギクグケゲコゴサザシジスズセゼソゾタダチヂッツヅテデトドナニヌネノハバパヒビピフブプヘベペホボポマミムメモャヤュユョヨラリルレロヮワヰヱヲンヴヵヶヷヸヹヺ" },
    };

    public static bool TryGetDefaultChars(ulong radix, [NotNullWhen(true)] out string? chars)
    {
        return _baseChars.TryGetValue(radix, out chars);
    }
    
    public static void SetDefaultChars(ulong radix, string chars)
    {
        if (string.IsNullOrWhiteSpace(chars))
            throw new ArgumentException();
        if (chars.Distinct().Count() != chars.Length)
            throw new ArgumentException();
        _baseChars[radix] = chars;
    }

    public readonly ref struct RadixData
    {
        public readonly ReadOnlySpan<char> Chars;
        public readonly ulong Radix;

        public RadixData(ReadOnlySpan<char> chars, ulong radix)
        {
            Chars = chars;
            Radix = radix;
        }

        public RadixData(ulong base10Number)
        {
            Chars = base10Number.ToString();
            Radix = 10UL;
        }
    }

    public static bool TryGetRadixChars(RadixData radix, [NotNullWhen(true)] out string? radixChars)
    {
        return _baseChars.TryGetValue(radix.Radix, out radixChars);
    }

    internal static bool Validate(RadixData input, out ReadOnlySpan<char> chars, out ulong radix, out ReadOnlySpan<char> radixChars)
    {
        chars = input.Chars;
        radix = input.Radix;
        if (chars.Length == 0 || !_baseChars.TryGetValue(radix, out var rchars))
        {
            radixChars = default;
            return false;
        }
        radixChars = rchars;
        return true;
    }
    

    public static bool TryConvert(RadixData input, out BigInteger base10Output)
    {
        // fast exit
        base10Output = default;
        
        if (!Validate(input, out var inputChars, out var inputRadix, out var inputRadixChars)) return false;
        
        BigInteger total = BigInteger.Zero;
        BigInteger multiplier = BigInteger.One;
        for (var i = inputChars.Length - 1; i >= 0; i--)
        {
            char ch = inputChars[i];
            int chIndex = inputRadixChars.IndexOf(ch);
            if (chIndex == -1) return false;

            total += (chIndex * multiplier);
            multiplier *= inputRadix;
        }
        
        base10Output = total;
        return true;
    }
    
    public static bool TryConvert(BigInteger base10Input, ulong outputBase, [NotNullWhen(true)] out string? outputChars)
    {
        if (!_baseChars.TryGetValue(outputBase, out var outputBaseChars))
        {
            outputChars = null;
            return false;
        }

        var outputText = new List<char>();

        BigInteger oBase = new BigInteger(outputBase);
        
        while (base10Input > 0UL)
        {
            BigInteger remainder = base10Input % oBase;
            base10Input /= outputBase;
            outputText.Add(outputBaseChars[(int)remainder]);
        }

        outputText.Reverse();
        outputChars = new string(outputText.ToArray());
        return true;
    }

    public static bool TryConvert(RadixData input, ulong outputBase, [NotNullWhen(true)] out string? outputChars)
    {
        if (!TryConvert(input, out var base10Output))
        {
            outputChars = default;
            return false;
        }
        if (!TryConvert(base10Output, outputBase, out outputChars))
        {
            outputChars = default;
            return false;
        }
        return true;
    }
}