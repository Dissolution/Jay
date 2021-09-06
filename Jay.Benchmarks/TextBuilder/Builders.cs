using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using Jay.Text;

namespace Jay.Benchmarks.TextBuilder
{
    public class BuilderA : IDisposable
    {
        private static readonly ArrayPool<char> _pool = ArrayPool<char>.Create();
        
        private char[] _characters;
        private int _length;

        public BuilderA()
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
                var newArray = _pool.Rent(newLen);
                TextHelper.Copy(_characters, newArray, _length);
                _pool.Return(_characters, false);
                _characters = newArray;
            }

            _characters[index] = c;
            _length = newLen;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ReadOnlySpan<char> text)
        {
            int index = _length;
            int newLen = index + text.Length;
            if (newLen >= _characters.Length)
            {
                var newArray = _pool.Rent(newLen);
                TextHelper.Copy(_characters, newArray, _length);
                _pool.Return(_characters, false);
                _characters = newArray;
            }
            TextHelper.Copy(text, _characters.Slice(index, text.Length));
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
    
    // public class BuilderA2 : IDisposable
    // {
    //     private static readonly ArrayPool<char> _pool = ArrayPool<char>.Create();
    //     
    //     private char[] _characters;
    //     private int _length;
    //
    //     public BuilderA2()
    //     {
    //         _characters = _pool.Rent(1024);
    //         _length = 0;
    //     }
    //     
    //     
    //     public void Write(char c)
    //     {
    //         int index = _length;
    //         int newLen = index + 1;
    //         if (newLen >= _characters.Length)
    //         {
    //             var newArray = _pool.Rent(newLen);
    //             TextHelper.Copy(_characters, newArray, _length);
    //             _pool.Return(_characters, false);
    //             _characters = newArray;
    //         }
    //
    //         _characters[index] = c;
    //         _length = newLen;
    //     }
    //     
    //     public void Write(ReadOnlySpan<char> text)
    //     {
    //         int index = _length;
    //         int newLen = index + text.Length;
    //         if (newLen >= _characters.Length)
    //         {
    //             var newArray = _pool.Rent(newLen);
    //             TextHelper.Copy(_characters, newArray, _length);
    //             _pool.Return(_characters, false);
    //             _characters = newArray;
    //         }
    //         TextHelper.Copy(text, _characters.Slice(index, text.Length));
    //         _length = newLen;
    //     }
    //
    //     public void Dispose()
    //     {
    //         _pool.Return(_characters);
    //     }
    //
    //     /// <inheritdoc />
    //     public sealed override string ToString()
    //     {
    //         return new string(_characters, 0, _length);
    //     }
    // }
    
    public class BuilderB : IDisposable
    {
        private static readonly ArrayPool<char> _pool = ArrayPool<char>.Create();
        
        private char[] _characters;
        private int _length;

        public BuilderB()
        {
            _characters = _pool.Rent(1024);
            _length = 0;
        }


        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Expand(int newLen)
        {
            var newArray = _pool.Rent(newLen);
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ReadOnlySpan<char> text)
        {
            int index = _length;
            int newLen = index + text.Length;
            if (newLen >= _characters.Length)
            {
                Expand(newLen);
            }
            TextHelper.Copy(text, _characters.Slice(index, text.Length));
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
    
    // public class BuilderB2 : IDisposable
    // {
    //     private static readonly ArrayPool<char> _pool = ArrayPool<char>.Create();
    //     
    //     private char[] _characters;
    //     private int _length;
    //
    //     public BuilderB2()
    //     {
    //         _characters = _pool.Rent(1024);
    //         _length = 0;
    //     }
    //
    //     
    //     private void Expand(int newLen)
    //     {
    //         var newArray = _pool.Rent(newLen);
    //         TextHelper.Copy(_characters, newArray, _length);
    //         _pool.Return(_characters, false);
    //         _characters = newArray;
    //     }
    //     
    //
    //     public void Write(char c)
    //     {
    //         int index = _length;
    //         int newLen = index + 1;
    //         if (newLen >= _characters.Length)
    //         {
    //             Expand(newLen);
    //         }
    //
    //         _characters[index] = c;
    //         _length = newLen;
    //     }
    //     
    //
    //     public void Write(ReadOnlySpan<char> text)
    //     {
    //         int index = _length;
    //         int newLen = index + text.Length;
    //         if (newLen >= _characters.Length)
    //         {
    //             Expand(newLen);
    //         }
    //         TextHelper.Copy(text, _characters.Slice(index, text.Length));
    //         _length = newLen;
    //     }
    //
    //     public void Dispose()
    //     {
    //         _pool.Return(_characters);
    //     }
    //
    //     /// <inheritdoc />
    //     public sealed override string ToString()
    //     {
    //         return new string(_characters, 0, _length);
    //     }
    // }
    
    public class BuilderC : IDisposable
    {
        private static readonly ArrayPool<char> _pool = ArrayPool<char>.Create();
        
        private char[] _characters;
        private int _length;

        public BuilderC()
        {
            _characters = _pool.Rent(1024);
            _length = 0;
        }


        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Expand(int newLen)
        {
            var newArray = _pool.Rent(newLen);
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
                Expand(index + 1);
            }

            _characters[index] = c;
            _length = index + 1;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ReadOnlySpan<char> text)
        {
            int index = _length;
            int newLen = index + text.Length;
            if (newLen >= _characters.Length)
            {
                Expand(newLen);
            }
            TextHelper.Copy(text, _characters.Slice(index, text.Length));
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
    
    public class BuilderD : IDisposable
    {
        private static readonly ArrayPool<char> _pool = ArrayPool<char>.Create();
        
        private char[] _characters;
        private int _length;

        public BuilderD()
        {
            _characters = _pool.Rent(1024);
            _length = 0;
        }


        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Expand(int newLen)
        {
            var newArray = _pool.Rent(newLen);
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
                Expand(index + 1);
            }

            _characters[index] = c;
            _length = index + 1;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ReadOnlySpan<char> text)
        {
            int index = _length;
            int textLen = text.Length;
            int newLen = index + textLen;
            if (newLen >= _characters.Length)
            {
                Expand(newLen);
            }
            TextHelper.Copy(text, _characters.Slice(index, textLen));
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
    
    public class BuilderE : IDisposable
    {
        private static readonly ArrayPool<char> _pool = ArrayPool<char>.Create();
        
        private char[] _characters;
        private int _length;

        public BuilderE()
        {
            _characters = _pool.Rent(1024);
            _length = 0;
        }


        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Expand(int newLen)
        {
            var newArray = _pool.Rent(newLen);
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
                Expand(index + 1);
            }

            _characters[index] = c;
            _length = index + 1;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ReadOnlySpan<char> text)
        {
            int index = _length;
            int newLen = index + text.Length;
            if (newLen >= _characters.Length)
            {
                Expand(newLen * 2);
            }
            TextHelper.Copy(text, _characters.Slice(index));
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
    
    public class BuilderF : IDisposable
    {
        private static readonly ArrayPool<char> _pool = ArrayPool<char>.Create();
        
        private char[] _characters;
        private int _length;

        public BuilderF()
        {
            _characters = _pool.Rent(1024);
            _length = 0;
        }


        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Expand(int newLen)
        {
            var newArray = _pool.Rent(newLen);
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ReadOnlySpan<char> text)
        {
            int index = _length;
            int newLen = index + text.Length;
            if (newLen >= _characters.Length)
            {
                Expand(newLen * 2);
            }
            TextHelper.Copy(text, _characters.Slice(index));
            _length = newLen;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(string? text)
        {
            if (text != null)
            {
                int index = _length;
                int newLen = index + text.Length;
                if (newLen >= _characters.Length)
                {
                    Expand(newLen * 2);
                }
                TextHelper.Copy(text, _characters.Slice(index));
                _length = newLen;
            }
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
    
    /*
    public class Builder : IDisposable
    {
        private static readonly ArrayPool<char> _pool = ArrayPool<char>.Create();
        
        private char[] _characters;
        private int _length;

        public Builder()
        {
            _characters = _pool.Rent(1024);
            _length = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(char c)
        {
           
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ReadOnlySpan<char> text)
        {
            
        }

        public void Dispose()
        {
            _pool.Return(_characters);
        }
    }
    */
}