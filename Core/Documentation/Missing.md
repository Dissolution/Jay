# Missing Classes

- `ConcurrentHashMapT.cs`: A commonly created class in old `Jay` libraries, ultimately any personal implementation that's not just `: ConcurrentDictionary<T, _>` are inefficient.
- `Uber****`: Extended Concurrent|Dictionary|List implementations that need a complete ground-up rewrite as a copy of `Dictionary<T,T>` if they ever _need_ to exist.
- `HashList`: Has to be a better implementation