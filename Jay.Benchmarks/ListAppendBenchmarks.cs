using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using Jay.Text;

namespace Jay.Benchmarks
{
    public class TestTextBuilder
    {
        private char[] _characters;
        private int _length;

        internal Span<char> Written
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Span<char>(_characters, 0, _length);
        }

        internal Span<char> Available
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Span<char>(_characters, _length, _characters.Length - _length);
        }
        
        public TestTextBuilder()
        {
            _characters = Array.Empty<char>();
            _length = 0;
        }

        public TestTextBuilder(int capacity)
        {
            if (capacity > 0)
            {
                _characters = new char[capacity];
            }
            else
            {
                _characters = Array.Empty<char>();
            }
            _length = 0;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ExpandA(int size)
        {
            var newCharacters = new char[size];
            TextHelper.Copy(new ReadOnlySpan<char>(_characters, 0, _length),
                            newCharacters);
            _characters = newCharacters;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendA(ReadOnlySpan<char> text)
        {
            if (text.Length + _length > _characters.Length)
            {
                ExpandA((text.Length + _length) * 2);
            }
            TextHelper.Copy(text, new Span<char>(_characters, _length, text.Length));
            _length += text.Length;
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ExpandB(int size)
        {
            var newCharacters = new char[size];
            TextHelper.Copy(Written, newCharacters);
            _characters = newCharacters;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendB(ReadOnlySpan<char> text)
        {
            if (text.Length + _length > _characters.Length)
            {
                ExpandB((text.Length + _length) * 2);
            }
            TextHelper.Copy(text, Available);
            _length += text.Length;
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ExpandC(int size)
        {
            var newCharacters = new char[size];
            TextHelper.Copy(Written, newCharacters);
            _characters = newCharacters;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendC(ReadOnlySpan<char> text)
        {
            var newLength = text.Length + _length;
            if (newLength > _characters.Length)
            {
                ExpandC(newLength * 2);
            }
            TextHelper.Copy(text, Available);
            _length = newLength;
        }

        /// <inheritdoc />
        public override string ToString() => new string(_characters, 0, _length);
    }
    
    
    [ShortRunJob]
    public class ListAppendBenchmarks
    {
        [ParamsSource(nameof(Capacities))]
        public int Capacity { get; set; }
        
        [ParamsSource(nameof(Texts))]
        public string Text { get; set; }
        
        public IEnumerable<int> Capacities => new int[3]
        {
            0, 64, 20_000,
        };
        
        public IEnumerable<string> Texts => new string[3]
        {
            string.Empty, 
            Guid.NewGuid().ToString(), 
            new string('x', 1024)
        };

        [Benchmark]
        public string AppendA()
        {
            var text = new TestTextBuilder(Capacity);
            text.AppendA(Text);
            return text.ToString();
        }
        [Benchmark]
        public string AppendB()
        {
            var text = new TestTextBuilder(Capacity);
            text.AppendB(Text);
            return text.ToString();
        }
        
        [Benchmark]
        public string AppendC()
        {
            var text = new TestTextBuilder(Capacity);
            text.AppendC(Text);
            return text.ToString();
        }
    }
}