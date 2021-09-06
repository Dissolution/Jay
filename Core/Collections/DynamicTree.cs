using System;
using System.Collections.Generic;
using System.Dynamic;
using Jay.Exceptions;
using Jay.Text;

namespace Jay.Collections
{
    public sealed class DynamicTree : DynamicObject
    {
        private readonly Dictionary<object, DynamicTree> _branches;
        private readonly Box<object?> _value;

        private DynamicTree(object? value)
        {
            _branches = new Dictionary<object, DynamicTree>(0);
            _value = new Box<object?>(value);
        }
        
        public DynamicTree()
        {
            _branches = new Dictionary<object, DynamicTree>(0);
            _value = Box<object?>.Empty;
        }

        private void SetValue(object? value)
        {
            _value.SetValue(value);
        }
        
        public override bool TryGetIndex(GetIndexBinder binder, object?[] indexes, out object? result)
        {
            if (binder.CallInfo.ArgumentCount != 1 ||
                indexes.Length != 1)
            {
                throw new InvalidOperationException();
            }

            var key = indexes[0];
            key ??= Box.Null;
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
            key ??= Box.Null;
            var subTree = _branches.GetOrAdd(key, _ => new DynamicTree());
            subTree.SetValue(value);
            return true;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj) => false;

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return GetHashCodeException.Throw(this);
        }
        
        /// <inheritdoc />
        public override string ToString()
        {
            // TODO: All of this all pretty
            return string.Empty;
        }
    }

}