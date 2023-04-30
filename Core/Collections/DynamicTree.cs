using System.Dynamic;
using Jay.Text;
using Jay.Utilities;

namespace Jay.Collections;

public sealed class DynamicTree : DynamicObject, IEnumerable<object?>
{
    public static dynamic Create()
    {
        return new DynamicTree();
    }

    private readonly Dictionary<object, DynamicTree> _branches;
    private Option<object> _branchValue;

    private DynamicTree()
    {
        _branches = new Dictionary<object, DynamicTree>(0);
        _branchValue = Option.None;
    }

    private bool SetValue(object? value)
    {
        _branchValue = Option.Create(value);
        return true;
    }

    private bool TryGetValue(out object? value)
    {
        return _branchValue.TryGetValue(out value);
    }

    private void WriteTo(StringBuilder builder, int indent)
    {
        if (_branchValue.TryGetValue(out var value))
        {
            builder.Append(value).AppendLine();
        }

        foreach (var item in _branches.Indexed())
        {
            item.Deconstruct(out var kvp);
            object name = kvp.Key;
            DynamicTree branch = kvp.Value;
            for (var i = 0; i < indent; i++)
            {
                builder.Append("  ");
            }
            builder.Append(item.IsLast ? "└" : "├")
                .Append('[').Append(name).Append(']');
            if (!branch._branchValue.IsNone)
            {
                builder.Append(": ");
            }
            else
            {
                builder.AppendLine();
            }
            branch.WriteTo(builder, indent + 1);
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
        if (_branchValue.TryGetValue(out var bv))
        {
            if (bv.GetType().Implements(binder.ReturnType))
            {
                result = bv;
                return true;
            }
        }

        result = null;
        return false;
    }

    IEnumerator IEnumerable.GetEnumerator() => _branchValue.GetEnumerator();
    public IEnumerator<object?> GetEnumerator() => _branchValue.GetEnumerator(); 

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return _branchValue.Equals(obj);
    }

    /// <inheritdoc />
    public override int GetHashCode() => throw new NotSupportedException();


    public override string ToString()
    {
        return StringBuilderPool.Shared.Use(sb => this.WriteTo(sb, 0));
    }
}