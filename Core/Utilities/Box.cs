using System;
using System.Diagnostics.CodeAnalysis;

namespace Jay
{
    public class Box
    {
        protected readonly object? _obj;
        protected readonly Type? _objType;

        public Type? ValueType => _objType;

        public bool IsNull => _obj is null;
        
        protected Box(object? obj, Type? objType)
        {
            _obj = obj;
            _objType = objType;
        }
        
        public virtual bool TryUnbox<T>([NotNullWhen(true)] out T? value)
        {
            if (_objType.Implements(typeof(T)) &&
                _obj.Is<T>(out value))
            {
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public override bool Equals(object? obj)
        {
            return Comparison.Comparison.DefaultEqualityComparer(_objType)
                             .Equals(_obj, obj);
        }

        public override int GetHashCode()
        {
            if (_obj is null) return 0;
            return _obj.GetHashCode();
        }

        public override string ToString()
        {
            return $"(object){_obj}";
        }
    }
}