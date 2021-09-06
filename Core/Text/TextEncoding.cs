using System;
using System.Buffers.Text;
using System.Text;

namespace Jay.Text
{
    public static class TextEncoding
    {
        public static string Base64Encode(ReadOnlySpan<byte> bytes)
        {
            return Convert.ToBase64String(bytes);
        }
        public static string Base64Encode(ReadOnlySpan<char> text)
        {
            Span<byte> bytes = stackalloc byte[text.Length * 2];
            var len = Encoding.UTF8.GetBytes(text, bytes);
            return Convert.ToBase64String(bytes[..len]);
        }

        public static byte[] Base64Decode(char[] chars)
        {
            return Convert.FromBase64CharArray(chars, 0, chars.Length);
        }
        
        public static string Base64Decode(string text)
        {
            var bytes = Convert.FromBase64String(text);
            return Encoding.UTF8.GetString(bytes);
        }
     
    }
}