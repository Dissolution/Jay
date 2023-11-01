using Jay.Utilities;
#if NET7_0_OR_GREATER
using System.Numerics;
#endif

namespace Jay.Reflection.Emitting;

public abstract class Emission :
    #if NET7_0_OR_GREATER
    IEqualityOperators<Emission,Emission,bool>,
 #endif
    IEquatable<Emission>
{
    public static bool operator ==(Emission? left, Emission? right) => Easy.FastEqual(left, right);
    public static bool operator !=(Emission? left, Emission? right) => !Easy.FastEqual(left, right);

    private object?[]? _arguments;
    
    public string Name { get; }
    public abstract int Size { get; }

    public bool HasArgs => _arguments is not null && _arguments.Length > 0;
    
    public object? Arg
    {
        get
        {
            var args = _arguments;
            if (args is null || args.Length == 0) return null;
            return args[0];
        }
        set
        {
            var args = _arguments;
            if (args is null || args.Length == 0)
            {
                _arguments = new object?[1] { value };
            }
            else
            {
                args[0] = value;
            }
        }
    }

    public object?[] Arguments
    {
        get
        {
            return _arguments ??= Array.Empty<object?>();
        }
    }

    protected Emission(string name)
    {
        this.Name = name;
        _arguments = null;
    }

    protected Emission(string name, object? arg)
    {
        this.Name = name;
        _arguments = new object?[1] { arg };
    }
    
    protected Emission(string name, params object?[]? arguments)
    {
        this.Name = name;
        _arguments = arguments;
    }

    public bool Equals(Emission? emission)
    {
        return emission is not null &&
            Easy.FastEqual(Name, emission.Name) &&
            Easy.SeqEqual(this.Arguments, emission.Arguments);
    }

    public override bool Equals(object? obj)
    {
        return obj is Emission emission && Equals(emission);
    }

    public override int GetHashCode()
    {
        var hasher = new Hasher();
        hasher.Add(Name);
        hasher.AddAll(Arguments);
        return hasher.ToHashCode();
    }
}