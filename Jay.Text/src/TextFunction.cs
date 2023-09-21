namespace Jay.Text;

public delegate TResult TextFunction<out TResult>(ReadOnlySpan<char> text);