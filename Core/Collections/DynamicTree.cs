using System.Dynamic;
using System.Collections;
using Jay.Text;
using Jay.Extensions;
using Jay.Exceptions;

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

    #if NET7_0_OR_GREATER
    private void ToString(ref DefaultInterpolatedStringHandler stringHandler, int indent)
    {
        if (_hasValue)
        {
            stringHandler.AppendFormatted(_value);
            stringHandler.AppendLiteral(Environment.NewLine);
        }

        foreach (var item in _branches.Indexed())
        {
            item.Deconstruct(out KeyValuePair<object, DynamicTree> branch);
            for (var i = 0; i < indent; i++)
                stringHandler.AppendLiteral("  ");
            if (item.IsLast)
            {
                stringHandler.AppendLiteral("└");
            }
            else
            {
                stringHandler.AppendLiteral("├");
            }
            stringHandler.AppendLiteral("[");
            stringHandler.AppendFormatted(branch.Key);
            stringHandler.AppendLiteral("]");
            if (branch.Value._hasValue)
            {
                stringHandler.AppendLiteral(": ");
            }
            else
            {
                stringHandler.AppendLiteral(Environment.NewLine);
            }
            branch.Value.ToString(ref stringHandler, indent + 1);
        }
    }
    #endif
        
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
                return binder.ReturnType.CanContainNull();
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
        return _hasValue && Equals(_value, obj);
    }

    /// <inheritdoc />
    public override int GetHashCode() => UnsupportedException.ThrowForGetHashCode(this);

    
    public override string ToString()
    {
        var stringHandler = new DefaultInterpolatedStringHandler();
        ToString(ref stringHandler, 0);
        return stringHandler.ToStringAndClear();
    }
}