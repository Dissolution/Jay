using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Jay.Text
{
    public sealed partial class TextBuilder
    {
        public static implicit operator string(TextBuilder? textBuilder)
        {
            if (textBuilder is null)
                return string.Empty;
            return new string(textBuilder._characters, 0, textBuilder._length);
        }
        
        private const int DefaultCapacity = 1024;

        private static readonly ArrayPool<char> _charArrayPool;

        static TextBuilder()
        {
            _charArrayPool = ArrayPool<char>.Create();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void SwapExpand(ref char[] characters, int minCapacity)
        {
            var newArray = _charArrayPool.Rent(minCapacity);
            TextHelper.Copy(characters, newArray);
            _charArrayPool.Return(characters);
            characters = newArray;
        }

        public static string Build(WriteText<TextBuilder> buildText)
        {
            using (var builder = new TextBuilder())
            {
                buildText(builder);
                return builder.ToString();
            }
        }
        
        public static string Build<TState>(TState state, WriteStateText<TextBuilder, TState> stateBuildText)
        {
            using (var builder = new TextBuilder())
            {
                stateBuildText(builder, state);
                return builder.ToString();
            }
        }

        public static string Build<T>(ReadOnlySpan<T> readOnlySpan, WriteSpanText<TextBuilder, T> spanBuildText)
        {
            using (var builder = new TextBuilder())
            {
                spanBuildText(builder, readOnlySpan);
                return builder.ToString();
            }
        }

        public static TextBuilder Rent() => new TextBuilder();

        public static string Start(ReadOnlySpan<char> text, WriteText<TextBuilder> buildText)
        {
            using (var builder = new TextBuilder())
            {
                builder.Write(text);
                buildText(builder);
                return builder.ToString();
            }
        }
    }
}