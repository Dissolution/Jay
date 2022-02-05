using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Jay.Collections;
using Jay.Validation;
using ExprType = System.Linq.Expressions.ExpressionType;

namespace Jay.Reflection.Building.Fulfilling;

public enum Group
{
    Arithmetic,
    Comparison,
    BooleanLogic,
    Bitwise,
    Equality,
    Other,
}

public enum Targets
{
    None = 0,
    Unary = 1,
    Binary = 2,
}

/// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/"/>
/// <see cref="https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/operator-overloads"/>
public sealed class Operator : IEquatable<Operator>, IComparable<Operator>
{
    private static readonly List<Operator> _operators;
   
    public static readonly Operator Not = new Operator
    {
        Priority = 0,
        Group = Group.BooleanLogic,
        Targets = Targets.Unary,
        Symbol = "!",
        MethodName = "op_LogicalNot",
        Signature = DelegateSig.Of(typeof(Func<bool,bool>)),
        ExpressionType = ExprType.Not,
    };

    public static readonly Operator LogicalAnd = new Operator
    {
        Priority = 6,
        Group = Group.BooleanLogic,
        Targets = Targets.Binary,
        Symbol = "&",
        MethodName = "op_LogicalAnd",
        Signature = DelegateSig.Of(typeof(Func<,,bool>)),
        ExpressionType = ExprType.And,
    };

    public static readonly Operator LogicalOr = new Operator
    {
        Priority = 8,
        Group = Group.BooleanLogic,
        Targets = Targets.Binary,
        Symbol = "|",
        MethodName = "op_LogicalOr",
        Signature = DelegateSig.Of(typeof(Func<,,>)),
        ExpressionType = ExprType.Or,
    };

    public static readonly Operator Xor = new Operator
    {
        Priority = 7,
        Group = Group.BooleanLogic,
        Targets = Targets.Binary,
        Symbol = "^",
        MethodName = "op_ExclusiveOr",
        Signature = DelegateSig.Of(typeof(Func<,,>)),
        ExpressionType = ExprType.ExclusiveOr,
    };

    public static readonly Operator ConditionalAnd = new Operator
    {
        Priority = 9,
        Group = Group.BooleanLogic,
        Targets = Targets.Binary,
        Symbol = "&&",
        MethodName = "op_LogicalAnd",
        Signature = DelegateSig.Of(typeof(Func<,,bool>)),
        ExpressionType = ExprType.AndAlso,
    };


    public static readonly Operator ConditionalOr = new Operator
    {
        Priority = 10,
        Group = Group.BooleanLogic,
        Targets = Targets.Binary,
        Symbol = "||",
        MethodName = "op_LogicalOr",
        Signature = DelegateSig.Of(typeof(Func<,,>)),
        ExpressionType = ExprType.OrElse,
    };


    static Operator()
    {
        _operators = new List<Operator>(85);    // ExpressionType.Count
    }

    public static bool TryParse(string? text, out Operator? @operator)
    {
        throw new NotImplementedException();
    }


    private int Priority { get; init; }
    
    public string Name { get; }

    public Group Group { get; init; }
    public Targets Targets { get; init; }
    public string Symbol { get; init; }
    
    public string MethodName { get; init; }
    public DelegateSig Signature { get; init; }

    public ExpressionType? ExpressionType { get; init; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Operator([CallerMemberName] string name = "")
    {
        this.Name = name;
        _operators.Add(this);
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public int CompareTo(Operator? @operator)
    {
        if (@operator is null) return 1;
        return Priority.CompareTo(@operator.Priority);
    }

    public bool Equals(Operator? @operator)
    {
        return ReferenceEquals(this, @operator);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj);
    }

    public override int GetHashCode()
    {
        return Hasher.Create(Name);
    }

    public override string ToString()
    {
        return $"Name ({Symbol})";
    }
}


/*

    public static class Bitwise
    {
        public static readonly Operator Complement = new Operator(Group.Bitwise, Targets.Unary, "~", "op_OnesComplement", 0);
        public static readonly Operator LeftShift = new Operator(Group.Bitwise, Targets.Binary, "<<", "op_LeftShift", 3);
        public static readonly Operator RightShift = new Operator(Group.Bitwise, Targets.Binary, ">>", "op_RightShift", 3);
        public static readonly Operator And = new Operator(Group.Bitwise, Targets.Binary, "&", "op_BitwiseAnd", 6);
        public static readonly Operator Or = new Operator(Group.Bitwise, Targets.Binary, "|", "op_BitwiseOr", 8);
        public static readonly Operator Xor = new Operator(Group.Bitwise, Targets.Binary, "^", "op_ExclusiveOr", 7);
    }

    public static readonly Operator Increment = new Operator(Group.Arithmetic, Targets.Unary, "++", "op_Increment", 0);
    public static readonly Operator Decrement = new Operator(Group.Arithmetic, Targets.Unary, "--", "op_Decrement", 0);
    public static readonly Operator Plus = new Operator(Group.Arithmetic, Targets.Unary, "+", "op_UnaryPlus", 0);
    public static readonly Operator Minus = new Operator(Group.Arithmetic, Targets.Unary, "-", "op_UnaryNegation", 0);
    public static readonly Operator Multiplication = new Operator(Group.Arithmetic, Targets.Binary, "*", "op_Multiply", 1);
    public static readonly Operator Division = new Operator(Group.Arithmetic, Targets.Binary, "/", "op_Division", 1);
    public static readonly Operator Remainder = new Operator(Group.Arithmetic, Targets.Binary, "%", "op_Modulus", 1);
    public static readonly Operator Addition = new Operator(Group.Arithmetic, Targets.Binary, "+", "op_Addition", 2);
    public static readonly Operator Subtraction = new Operator(Group.Arithmetic, Targets.Binary, "-", "op_Subtraction", 2);

    public static readonly Operator LessThan = new Operator(Group.Comparison, Targets.Binary, "<", "op_LessThan", 4);
    public static readonly Operator GreaterThan = new Operator(Group.Comparison, Targets.Binary, ">", "op_GreaterThan", 4);
    public static readonly Operator LessThanOrEqual = new Operator(Group.Comparison, Targets.Binary, "<=", "op_LessThanOrEqual", 4);
    public static readonly Operator GreaterThanOrEqual = new Operator(Group.Comparison, Targets.Binary, ">", "op_GreaterThanOrEqual", 4);

    public static readonly Operator Equality = new Operator(Group.Equality, Targets.Binary, "==", "op_Equality", 5);
    public static readonly Operator Inequality = new Operator(Group.Equality, Targets.Binary, "!=", "op_Inequality", 5);

    public static readonly Operator Implicit =
        new Operator(Fulfilling.Group.Other, Targets.Binary, "(implicit)", "op_Implicit", -1);
    public static readonly Operator Explicit =
        new Operator(Fulfilling.Group.Other, Targets.Binary, "(explicit)", "op_Explicit", -1);

    /* op_True
     * op_False
     #2#

    private readonly int _priority;

    public Group Group { get; }
    public Targets Targets { get; }
    public string Symbol { get; }
    public string MetadataName { get; }
    public DelegateSig Signature { get; }

    private Operator(Group group, Targets targets, string symbol, 
                     string metadataName,
                     int priority, 
                     DelegateSig signature,
                     [CallerMemberName] string name = "")
        : base(name)
    {
        this.Group = group;
        this.Targets = targets;
        this.Symbol = symbol;
        this.MetadataName = metadataName;
        this.Signature = signature;
        _priority = priority;
    }

    public override int CompareTo(Operator? op)
    {
        if (op is null) return 1;
        return _priority.CompareTo(op._priority);
    }
}

public static class TypeOperatorCache
{
    public sealed class OperatorCache
    {
        private readonly ConcurrentDictionary<Operator, Delegate?> _operatorCache;

        public Type Type { get; }

        internal OperatorCache(Type type)
        {
            this.Type = type;
            _operatorCache = new ConcurrentDictionary<Operator, Delegate?>();
        }

        internal Result TryEmit(IILGeneratorEmitter emitter,
                                Operator @operator,
                                Type argType)
        {
            // Func<T,T>
            if (@operator.Targets == Targets.Unary)
            {
                // Special

            }
        }

        internal Result TryGetDelegate<TDelegate>(Operator @operator)
            where TDelegate : Delegate
        {
            var delSig = DelegateSig.Of<TDelegate>();
            if (@operator.Targets == Targets.Unary)
            {
                if (delSig.ParameterCount != 1)
                {
                    return Dump.GetException<ArgumentException>($"{typeof(TDelegate)} is not valid for Unary Operand {@operator}", nameof(@operator));
                }
                var returnType = delSig.ReturnType;
                if (returnType != delSig.ParameterTypes[0])
                {
                    return Dump.GetException<ArgumentException>($"{typeof(TDelegate)} is not valid for Unary Operand {@operator}", nameof(@operator));
                }
                
            }
        }
    }

    private static readonly ConcurrentTypeDictionary<ConcurrentDictionary<Operator, Delegate?>> _operatorCache;

    static TypeOperatorCache()
    {

    }
}
*/

using System.Diagnostics;
using System.Reflection;
using Jay.Collections;
using Jay.Reflection;
using Jay.Reflection.Building.Fulfilling;
using Jay.Validation;

public static class MethodCache
{
    private static readonly ConcurrentTypeDictionary<EqualityMethods> _equalsMethods;

    static MethodCache()
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