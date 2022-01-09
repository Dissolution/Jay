using System.Diagnostics.CodeAnalysis;

namespace Jay;

public static class EnumerableExtensions
{
    public delegate bool SelectWherePredicate<in TIn, TOut>(TIn input, [NotNullWhen(true)] out TOut output);

    public static IEnumerable<TOut> SelectWhere<TIn, TOut>(this IEnumerable<TIn> source, SelectWherePredicate<TIn, TOut> selectWhere)
    {
        foreach (var element in source)
        {
            if (selectWhere(element, out var output))
            {
                yield return output;
            }
        }
    }
}