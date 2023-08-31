namespace Jay.Text.Utilities;

/// <summary>
/// Provides a way for methods capturing <see cref="FormattableString"/> to exist alongside
/// methods that only care about <see cref="string"/>
/// </summary>
/// <remarks>
/// This allows for these two methods to exist side-by-side with proper calling support:<br/>
/// <c>void DoThing(FormattableString formatString);</c><br/>
/// <c>void DoThing(NonFormattableString str);</c><br/>
///
/// So if you call <c>DoThing($"...")</c> it will pass along the <see cref="FormattableString"/><br/>
/// and if you call <c>DoThing("...")</c> it will capture the <see cref="string"/> in a <see cref="NonFormattableString"/><br/>
///
/// This is used for backwards compatability, as InterpolatedStringHandlers have largely replaced its necessity
/// </remarks>
public readonly struct NonFormattableString : 
    IEquatable<NonFormattableString>,
    IEquatable<string>
{
    public static implicit operator NonFormattableString(string? str) => new NonFormattableString(str);
    public static implicit operator NonFormattableString(FormattableString _) => throw new InvalidOperationException();
    public static implicit operator NonFormattableString(ReadOnlySpan<char> _) => throw new InvalidOperationException();

    public static bool operator ==(NonFormattableString left, NonFormattableString right) => left.Equals(right);
    public static bool operator !=(NonFormattableString left, NonFormattableString right) => !left.Equals(right);

    public static bool operator ==(NonFormattableString nfs, string? str) => nfs.Equals(str);
    public static bool operator !=(NonFormattableString nfs, string? str) => !nfs.Equals(str);

    private readonly string? _str;

    /// <summary>
    /// Gets the contained <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see>
    /// </summary>
    public ReadOnlySpan<char> Text => _str.AsSpan();
    
    /// <summary>
    /// Gets the contained <see cref="string"/>
    /// </summary>
    public string String => _str ?? "";

    private NonFormattableString(string? str)
    {
        _str = str;
    }

    public bool Equals(NonFormattableString nfs)
    {
        return string.Equals(_str, nfs._str);
    }
    
    public bool Equals(string? str)
    {
        return string.Equals(_str, str);
    }

    public override bool Equals(object? obj)
    {
        if (obj is string str) return Equals(str);
        if (obj is NonFormattableString nfs) return Equals(nfs);
        return false;
    }

    public override int GetHashCode()
    {
        if (_str is null) return 0;
        return _str.GetHashCode();
    }

    public override string ToString()
    {
        return _str ?? "";
    }
}