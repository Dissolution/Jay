using System.Linq.Expressions;

namespace Jay.Expressions;

public static class PredicateBuilder
{
  



   
    /// <summary>
    /// Creates a predicate that evaluates to true.
    /// </summary>
    public static Expression<Func<T, bool>> True<T>() => Predicate<T>.True;

    /// <summary>
    /// Creates a predicate that evaluates to false.
    /// </summary>
    public static Expression<Func<T, bool>> False<T>() => Predicate<T>.False;

    /// <summary>
    /// Creates a predicate expression from the specified lambda expression.
    /// </summary>
    public static Expression<Func<T, bool>> Create<T>(Expression<Func<T, bool>> predicate) => predicate;

    public static Expression<Func<T, bool>> Create<T>(Func<T, bool> predicate) => (t => predicate(t));

  
}