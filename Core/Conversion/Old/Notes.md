# Notes:

- Perfect world:
    - `TIn.ConvertTo<TOut>()`
    - `bool TIn.TryConvertTo<TOut>(out TOut output)`

- No voodoo, so no need to instance or overwrite
    - Implicit / explicit conversions
    - Methods that return the type and take no params
    - Constructors for the target type that take only the source type
    - Actual `:` implements
    - Low level IL `unmanaged` conversions
    - Common conversions are spelled out:
        - `object`, `string`, `ReadOnlySpan<char>`