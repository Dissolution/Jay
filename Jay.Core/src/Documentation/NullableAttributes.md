# Nullable Analysis Attributes

- https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/attributes/nullable-analysis

## **Pre**-Conditions

`[AllowNull]`

- Specifies that `null` is allowed as an input even if the corresponding type disallows it.

`[DisallowNull]`

- Specifies that `null` is disallowed as an input even if the corresponding type allows it.

Can be applied upon:

- Fields, Properties, & Indexers
    - `[ATTR] private T _value`
- Parameter Types:
    - `T` generic parameters
    - `in`, `out`, `ref`

## **Post**-Conditions

`[MaybeNull]`

- Specifies that an output may be `null` even if the corresponding type disallows it.

`[NotNull]`

- Specifies that an output is not-`null` even if the corresponding type allows it.
- Specifies that an input argument was not null when the call returns.

Can be applied upon:

- Fields, Properties, & Indexers
    - `[ATTR] private T _value`
- Method return values
    - `[return: ATTR]`
    - place _before_ the method declaration
- `out` parameters
    - `[ATTR] out NAME`
    - Indicates the condition of the parameter value after the method is called
- `ref` parameters
    - `[ATTR] ref NAME`
    - Indicates the condition of the parameter value after the method is called

## Conditional **Post**-Conditions

`[MaybeNullWhen(bool)]`

- Specifies that when a method returns `true`/`false`, the parameter may be `null` even if the corresponding type disallows it.

`[NotNullWhen(bool)]`

- Specifies that when a method returns `true`/`false`, the parameter will **not** be `null` even if the corresponding type allows it.

Can be applied upon:

- Parameters
    - `bool Method([ATTR(true|false)] T arg`

## Nullness Dependence

`[NotNullIfNotNull(string)]`

- Specifies that the output will be non-null if the named parameter is non-null.

Can be applied upon:

- Method returns
    - `[return: NotNullIfNotNull("paramName")]`
