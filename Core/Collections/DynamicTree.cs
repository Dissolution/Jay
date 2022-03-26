using System.Dynamic;
using Jay.Comparision;
using Jay.Dumping;
using Jay.Exceptions;
using Jay.Text;

namespace Jay.Collections;

public sealed class DynamicTree : DynamicObject, IEnumerable<object?>
{
    public static dynamic Create()
    {
        return new DynamicTree();
    }
        
    private readonly Dictionary<object, DynamicTree> _branches;
    private object? _value;
    private bool _hasValue;

    private DynamicTree()
    {
        _branches = new Dictionary<object, DynamicTree>(0);
        _value = default;
        _hasValue = false;
    }
        
    IEnumerator IEnumerable.GetEnumerator()
    {
        if (_hasValue)
            yield return _value;
    }
        
    private bool SetValue(object? value)
    {
        _value = value;
        _hasValue = true;
        return true;
    }

    private bool TryGetValue(out object? value)
    {
        value = _value;
        return _hasValue;
    }

    private void Dump(TextBuilder text, DumpOptions options, int indent)
    {
        if (_hasValue)
        {
            text.AppendDump(_value, options).WriteLine();
        }

        foreach (var item in _branches.Indexed())
        {
            item.Deconstruct(out KeyValuePair<object, DynamicTree> branch);
            text.AppendRepeat(indent, "  ");
            if (item.IsLast)
            {
                text.Write('└');
            }
            else
            {
                text.Write('├');
            }
            text.Append('[')
                .AppendDump(branch.Key, options)
                .Write(']');
            if (branch.Value._hasValue)
            {
                text.Write(": ");
            }
            else
            {
                text.WriteLine();
            }
            branch.Value.Dump(text, options, indent + 1);
        }
    }
        
    public override bool TryGetIndex(GetIndexBinder binder, object?[] indexes, out object? result)
    {
        if (binder.CallInfo.ArgumentCount != 1 ||
            indexes.Length != 1)
        {
            throw new InvalidOperationException();
        }

        var key = indexes[0];
        if (key is null)
            return TryGetValue(out result);
        result = _branches.GetOrAdd(key, _ => new DynamicTree());
        return true;
    }
        
    public override bool TrySetIndex(SetIndexBinder binder, object?[] indexes, object? value)
    {
        if (binder.CallInfo.ArgumentCount != 1 ||
            indexes.Length != 1)
        {
            throw new InvalidOperationException();
        }
            
        var key = indexes[0];
        if (key is null)
            return SetValue(value);
        return _branches.GetOrAdd(key, _ => new DynamicTree())
                        .SetValue(value);
    }

    public override bool TryConvert(ConvertBinder binder, out object? result)
    {
        if (_hasValue)
        {
            if (_value is null)
            {
                result = null;
                return binder.ReturnType.CanBeNull();
            }

            if (_value.GetType().Implements(binder.ReturnType))
            {
                result = _value;
                return true;
            }
        }

        result = null;
        return false;
    }


    public IEnumerator<object?> GetEnumerator()
    {
        if (_hasValue)
            yield return _value;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return _hasValue && CommonMethodCache.Comparer.Equal(obj, _value);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return UnsuitableException.ThrowGetHashCode(this);
    }
        
    /// <inheritdoc />
    public override string ToString()
    {
        using var text = TextBuilder.Borrow();
        var options = new DumpOptions(true);
        text.Write("DynamicTree");
        if (_hasValue)
        {
            text.Write(": ");
        }
        else
        {
            text.WriteLine();
        }
        Dump(text, options, 0);
        return text.ToString();
    }
}