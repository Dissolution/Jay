namespace Jay.Text
{
    public interface IRenderable : ISpanFormattable, IFormattable
    {
        static string Render(IRenderable? renderable)
        {
            if (renderable is null)
                return string.Empty;
            StringHandler handler = default;
            try
            {
                handler = new StringHandler();
                renderable.Render(ref handler);
                return handler.ToString();
            }
            finally
            {
                handler.Dispose();
            }
        }

        void Render(ref StringHandler handler);

        string IFormattable.ToString(string? format, IFormatProvider? formatProvider)
        {
            throw new NotImplementedException();
        }

        bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        {
            
        }
    }
}
