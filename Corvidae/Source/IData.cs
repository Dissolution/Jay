using System;
using Jay.Comparison;

namespace Corvidae
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// : IDictionary(string, object?) that auto-casts object -> TValue
    /// </remarks>
    public interface IData : IReadOnlyData
    {
        bool TryAdd<TValue>(string? key, TValue? value);
        bool TryRemove(string? key);
        bool TryRemove<TValue>(string? key, out TValue? removedValue);

        TValue? GetOrAdd<TValue>(string? key, TValue? value);
        TValue? GetOrAdd<TValue>(string? key, Func<TValue?> valueFactory);
        TValue? GetOrAdd<TValue>(string? key, Func<string, TValue?> valueFactory);

        TValue? AddOrUpdate<TValue>(string? key, TValue? addValue, Func<TValue?, TValue?> updateValue);
        TValue? AddOrUpdate<TValue>(string? key, Func<string, TValue?> addValueFactory, Func<string, TValue?, TValue?> updateValue);
    }
}