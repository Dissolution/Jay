namespace Jay.Text;

public delegate void TextAction(ReadOnlySpan<char> text);

public delegate TResult TextFunction<out TResult>(ReadOnlySpan<char> text);