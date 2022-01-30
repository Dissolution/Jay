using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using InlineIL;
using Jay.Collections;
using Jay.Validation;

namespace Jay.Reflection.Building.Fulfilling;

public enum Group
{
    Arithmetic,
    Comparison,
    BooleanLogic,
    Bitwise,
    Equality,
}

public enum Targets
{
    Unary = 1,
    Binary = 2,
}

public sealed class Operator : EnumLike<Operator>
{
    public static readonly Operator Increment = new Operator(Group.Arithmetic, Targets.Unary, "++");
    public static readonly Operator Decrement = new Operator(Group.Arithmetic, Targets.Unary, "--");
    public static readonly Operator Plus = new Operator(Group.Arithmetic, Targets.Unary, "+");
    public static readonly Operator Minus = new Operator(Group.Arithmetic, Targets.Unary, "-");
    public static readonly Operator Multiplication = new Operator(Group.Arithmetic, Targets.Binary, "*");
    public static readonly Operator Division = new Operator(Group.Arithmetic, Targets.Binary, "/");
    public static readonly Operator Remainder = new Operator(Group.Arithmetic, Targets.Binary, "%");
    public static readonly Operator Addition = new Operator(Group.Arithmetic, Targets.Binary, "+");
    public static readonly Operator Subtraction = new Operator(Group.Arithmetic, Targets.Binary, "-");




    public Group Group { get; }
    public Targets Targets { get; }
    public string Symbol { get; }

    private Operator(Group group, Targets targets, string symbol, [CallerMemberName] string name = "")
        : base(name)
    {
        this.Group = group;
        this.Targets = targets;
        this.Symbol = symbol;
    }
}

public static class Operators
{
    private static readonly Operator[] _operators;

    static Operators()
    {

    }

    public static class Arithmetic
    {
        public static class Unary
        {
            public static readonly Operator Increment = new Operator("++");
            public static readonly Operator Decrement = new Operator("--");
            public static readonly Operator Plus = new Operator("+");
            public static readonly Operator Minus = new Operator("-");
        }

        public static class Binary
        {
            public static readonly Operator Multiplication = new Operator("*");
            public static readonly Operator Division = new Operator("/");
            public static readonly Operator Remainder = new Operator("%");
            public static readonly Operator Addition = new Operator("+");
            public static readonly Operator Subtraction = new Operator("-");
        }
    }

    public static class Comparision
    {
        public static readonly Operator LessThan = new Operator("<");
        public static readonly Operator LessThanOrEqual = new Operator("<=");
        public static readonly Operator GreaterThan = new Operator(">");
        public static readonly Operator GreaterThanOrEqual = new Operator(">=");
    }

    public static class Boolean
    {
        public static class Logical
        {
            public static class Unary
            {
                public static readonly Operator Negation = new Operator("!");
            }

            public static class Binary
            {
                public static readonly Operator And = new Operator("&");
                public static readonly Operator Or = new Operator("|");
                public static readonly Operator Xor = new Operator("^");
            }
        }

        public static class Conditional
        {

            public static class Binary
            {
                public static readonly Operator And = new Operator("&&");
                public static readonly Operator Or = new Operator("||");
            }
        }
    }
}


public static class OperatorCache
{
    private static readonly ConcurrentTypeDictionary<EqualityMethods> _equalsMethods;

    static OperatorCache()
    {
        _equalsMethods = new();
        _delegateCombineMethod = new Lazy<MethodInfo>(() =>
        {
            return typeof(Delegate).GetMethod(nameof(Delegate.Combine),
                                       BindingFlags.Public | BindingFlags.Static,
                                       new Type[2] { typeof(Delegate), typeof(Delegate) })
                                   .ThrowIfNull();
        });
        _delegateRemoveMethod = new Lazy<MethodInfo>(() =>
        {
            return typeof(Delegate).GetMethod(nameof(Delegate.Remove),
                                       BindingFlags.Public | BindingFlags.Static,
                                       new Type[2] { typeof(Delegate), typeof(Delegate) })
                                   .ThrowIfNull();
        });
    }

    internal static EqualityMethods GetEqualityMethods(Type type)
    {
        return _equalsMethods.GetOrAdd(type, FindEqualityMethods);
    }

    private static EqualityMethods FindEqualityMethods(Type type)
    {
        var ecType = typeof(EqualityComparer<>).MakeGenericType(type);
        var getDefaultMethod = ecType.GetMethod("get_Default", Reflect.StaticFlags)
                                     .ThrowIfNull();
        var equalsMethod = ecType.GetMethod("Equals", Reflect.InstanceFlags, new Type[2] { type, type })
                                 .ThrowIfNull();
        return new(getDefaultMethod, equalsMethod);
    }

    private static readonly Lazy<MethodInfo> _delegateCombineMethod;

    public static MethodInfo DelegateCombineMethod => _delegateCombineMethod.Value;

    private static readonly Lazy<MethodInfo> _delegateRemoveMethod;

    public static MethodInfo DelegateRemoveMethod => _delegateRemoveMethod.Value;




    public static MethodInfo InterlockedCompareExchange(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        if (type.IsClass || type.IsInterface)
        {
            return typeof(Interlocked)
                   .GetMethods(BindingFlags.Public | BindingFlags.Static)
                   .Where(method => method.Name == nameof(Interlocked.CompareExchange))
                   .FirstOrDefault(method =>
                   {
                       if (method.IsGenericMethod)
                       {
                           var methodGenericTypes = method.GetGenericArguments();
                           Debugger.Break();
                           // Have to find T : class method
                       }

                       throw new NotImplementedException();
                   })
                   .ThrowIfNull();
        }
        else
        {
            throw new NotImplementedException();
        }
    }
}