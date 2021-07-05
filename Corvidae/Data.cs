using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using Jay;

namespace Corvidae
{
    internal sealed class Data : IData, IReadOnlyData
    {
        private readonly ConcurrentDictionary<string, object?> _data;

        public Data(IEqualityComparer<string?>? keyComparer = default)
        {
            _data = new ConcurrentDictionary<string, object?>(keyComparer);
        }

        /// <inheritdoc />
        public bool Contains(string? key)
        {
            return _data.ContainsKey(key ?? string.Empty);
        }

        /// <inheritdoc />
        public bool TryGetValue<TValue>(string? key, out TValue? value)
        {
            if (_data.TryGetValue(key ?? string.Empty, out object? obj))
            {
                return obj.Is<TValue>(out value);
            }
            value = default;
            return false;
        }

        /// <inheritdoc />
        public TValue? GetOrDefault<TValue>(string? key, TValue? @default = default(TValue))
        {
            if (_data.TryGetValue(key ?? string.Empty, out object? obj) && obj is TValue value)
                return value;
            return @default;
        }

        /// <inheritdoc />
        public bool TryAdd<TValue>(string? key, TValue? value)
        {
            return _data.TryAdd(key ?? string.Empty, (object?)value);
        }

        /// <inheritdoc />
        public bool TryRemove(string? key)
        {
            return _data.TryRemove(key ?? string.Empty, out _);
        }

        /// <inheritdoc />
        public bool TryRemove<TValue>(string? key, out TValue? removedValue)
        {
            key ??= string.Empty;
            if (_data.TryGetValue(key, out object? obj))
            {
                if (obj.Is<TValue>(out removedValue))
                {
                    var removed = _data.TryRemove(key, out obj);
                    Debug.Assert(removed);
                    return true;
                }
            }
            removedValue = default;
            return false;
        }

        /// <inheritdoc />
        public TValue? GetOrAdd<TValue>(string? key, TValue? value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TValue? GetOrAdd<TValue>(string? key, Func<TValue?> valueFactory)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TValue? GetOrAdd<TValue>(string? key, Func<string, TValue?> valueFactory)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TValue? AddOrUpdate<TValue>(string? key, TValue? addValue, Func<TValue?, TValue?> updateValue)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TValue? AddOrUpdate<TValue>(string? key, Func<string, TValue?> addValueFactory, Func<string, TValue?, TValue?> updateValue)
        {
            throw new NotImplementedException();
        }
    }
}