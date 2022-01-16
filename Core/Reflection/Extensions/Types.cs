using System.Linq.Expressions;
using System.Runtime.InteropServices;
using Jay.Reflection.Exceptions;

namespace Jay.Reflection.Extensions;

public static class Types
{
    /// <summary>
    /// Represents a placeholder <see cref="Type"/> for accessing <see langword="static"/> methods
    /// </summary>
    public struct Static
    {
        private static Static _instance = default;

        /// <summary>
        /// Gets a <see langword="ref"/> to an instance of <see cref="Static"/> for use in accessing <see langword="static"/> methods
        /// </summary>
        public static ref Static Instance => ref _instance;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0)]
    public readonly struct Void
    {
        public static bool operator ==(Void v, Void x) => true;
        public static bool operator !=(Void v, Void x) => false;

        public override bool Equals(object? obj) => obj is null || obj is Void;

        public override int GetHashCode() => 0;

        public override string ToString() => "void";
    }

    static Types()
    {

    }

    public static TMember FindMember<TMember>(Type type, Expression<Func<Type, TMember?>> memberExpression)
    {
        ArgumentNullException.ThrowIfNull(type);
        var memberFind = memberExpression.Compile();
        TMember? member;
        try
        {
            member = memberFind(type);
        }
        catch (Exception ex)
        {
            throw new ReflectionException($"Cannot find {type}.{memberExpression}", ex);
        }
        if (member is null)
        {
            throw new ReflectionException($"Cannot find {type}.{memberExpression}");
        }
        return member;
    }
}