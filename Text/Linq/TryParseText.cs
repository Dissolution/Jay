namespace Jay.Text.Linq;

public delegate bool TryParseText<T>(text text, [NotNullWhen(true)] out T value);