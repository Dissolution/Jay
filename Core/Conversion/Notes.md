# Notes:

- Perfect world:
    - `TOut ((TIn)value).ConvertTo<TOut>(options? = default)`
    - `bool ((TIn)value).TryConvertTo<TOut>(out TOut output, options? = default)`
    - Universal `options` class!
      - format/formatprovider, formats? TryParse/TryFormat stuff?

- No voodoo, so no need to instance or overwrite
    - Implicit / explicit conversions
    - Methods that return the type and take no params
    - Constructors for the target type that take only the source type
    - Actual `:` implements
    - Low level IL `unmanaged` conversions
    - Common conversions are spelled out:
        ` `object` and `string`