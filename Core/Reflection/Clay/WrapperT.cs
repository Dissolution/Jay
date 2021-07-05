using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Jay.Debugging;
using Jay.Reflection.Emission;

namespace Jay.Reflection
{
   
    internal sealed class Wrapper<T> : DynamicObject
    {
        private T _value;
        private readonly BindingFlags _memberFlags;

        public Wrapper(T value, BindingFlags memberFlags = BindingFlags.Instance | BindingFlags.Public)
        {
            _value = value ?? throw new ArgumentNullException(nameof(value));
            _memberFlags = memberFlags;
        }

        /// <inheritdoc />
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return base.GetDynamicMemberNames();
        }

        /// <inheritdoc />
        public override DynamicMetaObject GetMetaObject(Expression parameter)
        {
            if (parameter is ParameterExpression parameterExpression)
            {
                Hold.Debug(parameterExpression);
            }
            return base.GetMetaObject(parameter);
        }

        /// <inheritdoc />
        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object? result)
        {
            return base.TryBinaryOperation(binder, arg, out result);
        }

        /// <inheritdoc />
        public override bool TryConvert(ConvertBinder binder, out object? result)
        {
            return base.TryConvert(binder, out result);
        }

        /// <inheritdoc />
        public override bool TryCreateInstance(CreateInstanceBinder binder, object?[]? args, [NotNullWhen(true)] out object? result)
        {
            return base.TryCreateInstance(binder, args, out result);
        }

        /// <inheritdoc />
        public override bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes)
        {
            return base.TryDeleteIndex(binder, indexes);
        }

        /// <inheritdoc />
        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            return base.TryDeleteMember(binder);
        }

        /// <inheritdoc />
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object? result)
        {
            return base.TryGetIndex(binder, indexes, out result);
        }

        /// <inheritdoc />
        public override bool TryGetMember(GetMemberBinder binder, out object? result)
        {
            var memberName = binder.Name;
            var returnType = binder.ReturnType;
            var nameComparison = binder.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            var member = typeof(T).GetMembers(_memberFlags)
                                  .Where(m => string.Equals(m.Name, memberName, nameComparison))
                                  .OneOrDefault();
            if (member is null)
            {
                Debugger.Break();
            }

            if (member is FieldInfo fieldInfo)
            {
                result = fieldInfo.GetValue<T, object>(ref _value);
                return true;
            }

            if (member is PropertyInfo propertyInfo)
            {
                result = propertyInfo.GetValue<T, object>(ref _value);
                return true;
            }
            
            Debugger.Break();
            
            return base.TryGetMember(binder, out result);
        }

        /// <inheritdoc />
        public override bool TryInvoke(InvokeBinder binder, object?[]? args, out object? result)
        {
            return base.TryInvoke(binder, args, out result);
        }

        /// <inheritdoc />
        public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
        {
            string name = binder.Name;
            BindingFlags flags;
            StringComparison nameComparison;
            args ??= Array.Empty<object>();
            if (binder.IgnoreCase)
            {
                flags = _memberFlags | BindingFlags.IgnoreCase;
                nameComparison = StringComparison.OrdinalIgnoreCase;
            }
            else
            {
                flags = _memberFlags;
                nameComparison = StringComparison.Ordinal;
            }

            var method = typeof(T).GetMethods(flags)
                                   .Where(m => string.Equals(m.Name, name, nameComparison))
                                   .Where(m => m.GetParameters().Length == args?.Length)
                                   .OneOrDefault();

            if (method is null)
            {
                Debugger.Break();
            }
            else
            {
                result = method.Adapt<InstanceFunc<T, object>>(Safety.AllowReturnDefaultFromVoid)
                               .Invoke(ref _value, args);
                return true;
            }
            
            return base.TryInvokeMember(binder, args, out result);
        }

        /// <inheritdoc />
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object? value)
        {
            return base.TrySetIndex(binder, indexes, value);
        }

        /// <inheritdoc />
        public override bool TrySetMember(SetMemberBinder binder, object? value)
        {
            return base.TrySetMember(binder, value);
        }

        /// <inheritdoc />
        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object? result)
        {
            return base.TryUnaryOperation(binder, out result);
        }

        public bool Equals([AllowNull] T value)
        {
            return EqualityComparer<T>.Default.Equals(value, _value);
        }
        
        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is T value)
                return Equals(value);
            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _value?.GetHashCode() ?? 0;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return _value?.ToString() ?? "null";
        }
    }
}