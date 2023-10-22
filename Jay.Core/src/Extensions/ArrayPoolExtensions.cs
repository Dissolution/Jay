using System.Buffers;

namespace Jay.Extensions;

public static class ArrayPoolExtensions
{
    public static T[] Rent<T>(this ArrayPool<T> arrayPool) => arrayPool.Rent(minimumLength: 64);

    public static IDisposable Rent<T>(this ArrayPool<T> arrayPool,
        int minimumLength,
        out T[] tempArray,
        bool clearArray = false)
    {
        tempArray = arrayPool.Rent(minimumLength);
        return new ArrayPoolReturner<T>(arrayPool, tempArray, clearArray);
    }

    private sealed class ArrayPoolReturner<T> : IDisposable
    {
        private readonly ArrayPool<T> _arrayPool;
        private readonly bool _clearArray;
        private T[]? _array;

        public ArrayPoolReturner(ArrayPool<T> arrayPool, T[] array, bool clearArray)
        {
            _arrayPool = arrayPool;
            _array = array;
            _clearArray = clearArray;
        }

        public void Dispose()
        {
            // Defensive
            var array = Interlocked.Exchange(ref _array, null);
            if (array is not null)
            {
                _arrayPool.Return(array, _clearArray);
            }
        }
    }
}