## How `Interlocked.CompareExchange` works

### `T? Interlocked.CompareExchange<T>(ref T? location, T? value, T? comparand) where T : class?`

All comparison is _Reference_ comparison, not `Equals()` nor `==`

- **location** is compared with **comparand**
    - if they are the same reference, **value** is stored in **location**
- in any case, the _original_ `T` in **location** is returned