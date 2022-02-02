using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Jay.Comparision;
using Jay.Text;

namespace Jay;

public readonly struct Options : IEquatable<Options>
{
    public static Options Create<T1>(T1 arg1)
    {
        return new Options(Box.Wrap(arg1));
    }
    public static Options Create<T1, T2>(T1 arg1, T2 arg2)
    {
        return new Options(Box.Wrap(arg1), Box.Wrap(arg2));
    }
    public static Options Create<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
    {
        return new Options(Box.Wrap(arg1), Box.Wrap(arg2), Box.Wrap(arg3));
    }
    public static Options Create<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        return new Options(Box.Wrap(arg1), Box.Wrap(arg2), Box.Wrap(arg3), Box.Wrap(arg4));
    }
    public static Options Create<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
        return new Options(Box.Wrap(arg1), Box.Wrap(arg2), Box.Wrap(arg3), Box.Wrap(arg4), Box.Wrap(arg5));
    }

    private readonly Box[] _args;

    internal Options(params Box[] args)
    {
        _args = args;
    }

    public bool TryGetOption<TOption>([NotNullWhen(true)] out TOption? option)
    {
        for (var i = 0; i < _args.Length; i++)
        {
            if (_args[i].Is<TOption>(out option))
                return true;
        }

        option = default;
        return false;
    }

    public TOption OptionOrDefault<TOption>(TOption defaultOption)
    {
        for (var i = 0; i < _args.Length; i++)
        {
            if (_args[i].Is<TOption>(out var opt))
                return opt;
        }
        return defaultOption;
    }

    public bool Equals<TOption>(TOption option)
    {
        return TryGetOption<TOption>(out var meOption) && 
               EqualityComparer<TOption>.Default.Equals(meOption, option);
    }

    public bool Equals(Options options)
    {
        return EnumerableEqualityComparer<Box>.Default.Equals(_args, options._args);
    }

    public bool Equals(Box box)
    {
        for (var i = 0; i < _args.Length; i++)
        {
            if (_args[i].Equals(box)) return true;
        }
        return false;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Options options)
            return Equals(options);
        return Equals(Box.Wrap(obj));
    }

    public override int GetHashCode()
    {
        return Hasher.Create<Box>(_args);
    }

    public override string ToString()
    {
        using var text = new TextBuilder();
        text.AppendDelimit(" | ", _args);
        return text.ToString();
    }
}