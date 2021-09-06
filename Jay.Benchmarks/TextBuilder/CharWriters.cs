using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using Jay.Text;

namespace Jay.Benchmarks.TextBuilder
{
    public class CharWriterBase : IDisposable
    {
        private static readonly ArrayPool<char> _pool = ArrayPool<char>.Create();
        
        private char[] _characters;
        private int _length;

        public CharWriterBase()
        {
            _characters = _pool.Rent(1024);
            _length = 0;
        }
        
        
        public void Write(char c)
        {
            int index = _length;
            int newLen = index + 1;
            if (newLen >= _characters.Length)
            {
                var newArray = _pool.Rent(newLen * 2);
                TextHelper.Copy(_characters, newArray, _length);
                _pool.Return(_characters, false);
                _characters = newArray;
            }
            _characters[index] = c;
            _length = newLen;
        }

        public void Dispose()
        {
            _pool.Return(_characters);
        }

        /// <inheritdoc />
        public sealed override string ToString()
        {
            return new string(_characters, 0, _length);
        }
    }
    
    public class AggCharWriterBase : IDisposable
    {
        private static readonly ArrayPool<char> _pool = ArrayPool<char>.Create();
        
        private char[] _characters;
        private int _length;

        public AggCharWriterBase()
        {
            _characters = _pool.Rent(1024);
            _length = 0;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(char c)
        {
            int index = _length;
            int newLen = index + 1;
            if (newLen >= _characters.Length)
            {
                var newArray = _pool.Rent(newLen * 2);
                TextHelper.Copy(_characters, newArray, _length);
                _pool.Return(_characters, false);
                _characters = newArray;
            }
            _characters[index] = c;
            _length = newLen;
        }

        public void Dispose()
        {
            _pool.Return(_characters);
        }

        /// <inheritdoc />
        public sealed override string ToString()
        {
            return new string(_characters, 0, _length);
        }
    }
    
    public class AggTCharWriterBase : IDisposable
    {
        private static readonly ArrayPool<char> _pool = ArrayPool<char>.Create();
        
        private char[] _characters;
        private int _length;

        public AggTCharWriterBase()
        {
            _characters = _pool.Rent(1024);
            _length = 0;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(char c)
        {
            int index = _length;
            if (index == _characters.Length)
            {
                var newArray = _pool.Rent(index * 2);
                TextHelper.Copy(_characters, newArray, _length);
                _pool.Return(_characters, false);
                _characters = newArray;
            }
            _characters[index] = c;
            _length = index + 1;
        }

        public void Dispose()
        {
            _pool.Return(_characters);
        }

        /// <inheritdoc />
        public sealed override string ToString()
        {
            return new string(_characters, 0, _length);
        }
    }
    
    public class SplitCharWriterBase : IDisposable
    {
        private static readonly ArrayPool<char> _pool = ArrayPool<char>.Create();
        
        private char[] _characters;
        private int _length;

        public SplitCharWriterBase()
        {
            _characters = _pool.Rent(1024);
            _length = 0;
        }
        
        private void Expand(int newLen)
        {
            var newArray = _pool.Rent(newLen * 2);
            TextHelper.Copy(_characters, newArray, _length);
            _pool.Return(_characters, false);
            _characters = newArray;
        }

        public void Write(char c)
        {
            int index = _length;
            int newLen = index + 1;
            if (newLen >= _characters.Length)
            {
                Expand(newLen);
            }
            _characters[index] = c;
            _length = newLen;
        }

        public void Dispose()
        {
            _pool.Return(_characters);
        }

        /// <inheritdoc />
        public sealed override string ToString()
        {
            return new string(_characters, 0, _length);
        }
    }
    
    public class NoAggSplitCharWriterBase : IDisposable
    {
        private static readonly ArrayPool<char> _pool = ArrayPool<char>.Create();
        
        private char[] _characters;
        private int _length;

        public NoAggSplitCharWriterBase()
        {
            _characters = _pool.Rent(1024);
            _length = 0;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Expand(int newLen)
        {
            var newArray = _pool.Rent(newLen * 2);
            TextHelper.Copy(_characters, newArray, _length);
            _pool.Return(_characters, false);
            _characters = newArray;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(char c)
        {
            int index = _length;
            int newLen = index + 1;
            if (newLen >= _characters.Length)
            {
                Expand(newLen);
            }
            _characters[index] = c;
            _length = newLen;
        }

        public void Dispose()
        {
            _pool.Return(_characters);
        }

        /// <inheritdoc />
        public sealed override string ToString()
        {
            return new string(_characters, 0, _length);
        }
    }
    
    public class NoAggTSplitCharWriterBase : IDisposable
    {
        private static readonly ArrayPool<char> _pool = ArrayPool<char>.Create();
        
        private char[] _characters;
        private int _length;

        public NoAggTSplitCharWriterBase()
        {
            _characters = _pool.Rent(1024);
            _length = 0;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Expand(int newLen)
        {
            var newArray = _pool.Rent(newLen * 2);
            TextHelper.Copy(_characters, newArray, _length);
            _pool.Return(_characters, false);
            _characters = newArray;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(char c)
        {
            int index = _length;
            if (index == _characters.Length)
            {
                Expand(index * 2);
            }
            _characters[index] = c;
            _length = index + 1;
        }

        public void Dispose()
        {
            _pool.Return(_characters);
        }

        /// <inheritdoc />
        public sealed override string ToString()
        {
            return new string(_characters, 0, _length);
        }
    }
}