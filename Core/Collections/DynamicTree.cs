using System.Dynamic;
using Jay.Exceptions;
using Jay.Reflection;

namespace Jay.Collections
{
    public sealed class DynamicTree : DynamicObject
    {
        private readonly Dictionary<object, DynamicTree> _branches;
        private Box _value;

        private DynamicTree(object? value)
        {
            _branches = new Dictionary<object, DynamicTree>(0);
            _value = Box.Wrap(value);
        }
        
        public DynamicTree()
        {
            _branches = new Dictionary<object, DynamicTree>(0);
            _value = default;
        }

        private void SetValue(object? value)
        {
            _value = Box.Wrap(value);
        }
        
        public override bool TryGetIndex(GetIndexBinder binder, object?[] indexes, out object? result)
        {
            if (binder.CallInfo.ArgumentCount != 1 ||
                indexes.Length != 1)
            {
                throw new InvalidOperationException();
            }

            var key = indexes[0];
            if (key is null) throw new NullReferenceException();
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
            if (key is null) throw new NullReferenceException();
            var subTree = _branches.GetOrAdd(key, _ => new DynamicTree());
            subTree.SetValue(value);
            return true;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj) => false;

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return UnsuitableException.ThrowGetHashCode(this);
        }
        
        /// <inheritdoc />
        public override string ToString()
        {
            // TODO: All of this all pretty
            return string.Empty;
        }
    }

}