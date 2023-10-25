# Nullability Analysis Attributes
- https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/attributes/nullable-analysis

## _Pre_-Conditions
- `[AllowNull]`
  - Specifies that `null` is allowed as an input even if the corresponding type disallows it.
- `[DisallowNull]`
  - Specifies that `null` is disallowed as an input even if the corresponding type allows it.
- Can be applied to Fields, Properties, Indexers, and Parameter Types:
  ```c#
  // You may not assign null to _value
  [DisallowNull]
  private string _value;
  
  // You may assign null to Name
  [AllowNull]
  public string Name { get; set; }
  
  // You may not assign null to the indexer
  [DisallowNull]
  public string this[int index] { get; set; }
  
  // You cannot pass null to name
  public void DoTheThing([DisallowNull] string name);
  
  // You may pass null to userId
  public User GetUser([AllowNull] string userId = null);
  ```

## _Post_-Conditions
- `[MaybeNull]`   
  - Specifies that an output may be `null` even if the corresponding type disallows it.
- `[NotNull]`
  - Specifies that an output is not-`null` even if the corresponding type allows it.
  - Specifies that an input argument will not be null once a function returns.
- Can be applied to Fields, Properties, Indexers, Method return values, `out` + `ref` parameters:
  ```c#
  // _value will never be null when read
  [NotNull]
  private string _value;
  
  // Thing might be null when read
  [MaybeNull]
  public object Thing { get; set; }
  
  // The return value will never be null
  [return: NotNull]
  public string DoTheThing(int id);
  
  // The out value may be null
  public bool TryParse([MaybeNull] out object value);
  
  // After Parse completes, input will not be null
  public T Parse([NotNull] string input);
  ```

## Conditional **Post**-Conditions
- `[MaybeNullWhen(bool)]`
  
  - Specifies that when a method returns `true`/`false`, the parameter may be `null` even if the corresponding type disallows it.
- `[NotNullWhen(bool)]`
  - Specifies that when a method returns `true`/`false`, the parameter will **not** be `null` even if the corresponding type allows it.
- Can be applied to Parameters:
  ```c#
  // If TryParse returns true, value will not be null
  public bool TryParse([NotNullWhen(true)] out T value);
  
  // If TryParse returns false, value might be null
  public bool TryParse([MaybeNullWhen(false)] out T value);
  ```

## Nullness Dependence
- `[NotNullIfNotNull(string)]`  
  - Specifies that the output will be non-null if the named parameter is non-null.
- Can be applied to Method returns:
  ```c#
  // If fallback is not null, then the return value will also be non-null
  [return: NotNullIfNotNull(nameof(fallback))]
  public T OneOrDefault<T>(this IEnumerable<T> source, [AllowNull] T fallback = default)
  ```

## Nullability Skipping
- `[DoesNotReturn]`
  - Specifies that the method never returns (it always throws an `Exception` or terminates the application)
  - May be applied to Methods:
  ```c#
  // We never return after calling ThrowException
  [DoesNotReturn]
  public void ThrowException();  
  ```
- `[DoesNotReturnIf(bool)]`
  - Specifies that the method never returns if the corresponding value is the same as the boolean
  - Can be applied to Method Parameters:
  ```c#
  // If we pass true to isnull, then we never return from this function
  public void FailFastIf([DoesNotReturnIf(true)] bool isNull);
  ```
  
## Combinations
```c#
// You may pass null to input, but if Parse returns, input will not be null
public T Parse([AllowNull, NotNull] string input);
```


# Nullable Variable Annotations
- https://learn.microsoft.com/en-us/dotnet/csharp/nullable-references
- https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/nullable-reference-types
- https://learn.microsoft.com/en-us/dotnet/csharp/nullable-migration-strategies

- EF Specific:
  - https://learn.microsoft.com/en-us/ef/core/miscellaneous/nullable-reference-types  `(required)`
  - 

## `!` + `?` --  Your best friends:exclamation:
- `!` indicates that a value should be treated as non-`null`, even if we know it is.
  - The 'null-forgiving' operator
  - Usually used to suppress warnings for DTO/POCO classes
  - Also finds use when storing a value that we know we'll overwrite immediately
  ```c#
  public string Name { get; set; } = default!;
  ```
- `?` indicates that a value _might_ be `null`
  - Similar to `Nullable<T>`, except it doesn't automatically make `T` nullable
  ```c#
  bool? == Nullable<bool>
  string? == string
  string == string
  T? == T
  ```
  - It is the more recent and accepted way of indicating nullability -- although the above attributes still must be used in certain circumstances.
  - Generally, `T?` means that the value might be null, `T` means that it is not.
  - This _cannot_ be fully enforced by the compiler, but can do a 95% job.
  ```c#
  // _value should not ever be null
  private string _value;
  
  // _thing might be null
  private object? _thing;
  
  // Name might be null
  public string? Name { get; set; }
  
  // the indexer never returns null
  public string this[int index] { get; }
  
  // name must not be null
  public void DoTheThing(string name);
  
  // userId can be null
  public User GetUser(string? userId = null);
  
  // value might be null even if parsing succeeds
  public bool TryParse(out object? value);
  
  // T will not be null and input must not be null
  public T Parse(string input);
  
  // if this method returns true, value will not be null; otherwise, it might be
  public bool TryParse([NotNullWhen(true)] out T? value);
  
  // if this method returns false, value might be null
  public bool TryParse([MaybeNullWhen(false)] out T? value);
  
  // so long as fallback is not null, the return will also be non-null
  [return: NotNullIfNotNull(nameof(fallback))]
  public T? OneOrDefault<T>(this IEnumerable<T> source, T? fallback = default)
  ```
  - It may also be used for nullabilty check chaining:
  ```c#
  // old way
  public string GetFirstName(Person person)
  {
    if (person == null)			// have to check for null before we can call .Name
    	return null;
    if (person.Name == null)		// have to check for null before we can call .First
	    return null;
	  return person.Name.First;		// this might return null    
  }
  
  // new way
  // immediately indicates that we can take nulls and return them
  public string? GetFirstName(Person? person)
  {
    return person?.Name?.First;
  }
  
  // by using [NotNull], we indicate to the caller that they can pass in null,
  // but if we do manage to return from Parse, input will definitly be non-null
  // and it returns a non-null value
  public Person Parse([NotNull] string? input)
  {
  	if (input is null)
          throw new ArgumentNullException(nameof(input));
      // ... code to get the person
      return person;
  }
  ```





MyExtensions.cs in ui_api -- as an example