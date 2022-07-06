# Implementer Concepts

### Equality
- `EqualityAttribute`
  - Only on a Property, it specifies whether the Property does or doesn't participate in Equality operations
  - The default is non-participation, the same as not having the `Attribute` at all
  - Using this `Attribute` changes the functionality of `Equals(obj?) and `GetHashCode()`
- By default, any Implementation acts as a `class`
  - `Equals<TSelf>` (if defined) is reference-equality
  - `Equals(obj?)` is reference-equality
  - `GetHashCode()` calls `RuntimeHelpers.GetHashCode()` on itself
- If any `Property` has an `EqualityAttribute` defined, Property-specific comparision is enabled for all Properties where it is defined as `true`
  - `Equals<TSelf>` checks for `null`, checks for reference-equality, and then compares each property of each Implementation
  - `Equals(obj?)` checks that `obj is TSelf` and calls `Equals<TSelf>`
  - `GetHashCode()` uses `HashCode` to generate a hash from properties
- `IStructuralEquatable` forces Property-specific comparison
  - If no Properties have an `EqualityAttribute` defined, all will be used

### ToString
- The default of `GetType().ToString()` (aka the `Type.Name`) will be hardcoded as a response
- If Property-specific comparison has been activated, the output will include each key property's name and value
- `IFormattable`
  - Will use the given values to call `GetType().ToString()` by default
  - Will attempt to use each Prop~~~~erties's values as an IFormattable and pass the args forward
### Property Notification
- `INotifyPropertyChanged`
- `INotifyPropertyChanging`
- Either interface will alter the behavior of Property set accessors for all implemented Properties in order to implement the interface(s)

### Cloning
- `ICloneable`
- `ICloneable<T>` // custom interface
- We will always implement both if we have either, doing a field-wise deepclone of self (all Properties!)

### Disposal
- `IDisposable`
- While not necessary, dispose will clear any Event handlers (set underlying field to `null`)

Others that are interesting, but impossible?
- `IComparable<T> where T : self`
- `IComparable`
- `IStructuralComparable`