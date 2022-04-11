// using System;
// using System.Buffers;
// using System.Collections.Generic;
// using System.Linq;
// using System.Runtime.CompilerServices;
// using System.Text;
// using System.Threading.Tasks;
// using BenchmarkDotNet.Attributes;
//
// namespace Jay.Benchmarking
// {
//     public ref struct Football //: IDisposable
//     {
//         internal char[] _array;
//         internal int _index;
//
//         internal int Capacity
//         {
//             [MethodImpl(MethodImplOptions.AggressiveInlining)]
//             get => _array.Length;
//         }
//
//         public int Index => _index;
//
//         public Football()
//         {
//             _array = ArrayPool<char>.Shared.Rent(1024);
//             _index = 0;
//         }
//
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         public void Write(char ch)
//         {
//             int i = _index;
//             int len = i + 1;
//             if (len < Capacity)
//             {
//                 _array![i] = ch;
//                 _index = len;
//             }
//             else
//             {
//
//             }
//         }
//
//         public void Dispose()
//         {
//             char[]? array = Interlocked.Exchange<char[]?>(ref _array, null);
//             if (array is not null)
//             {
//                 ArrayPool<char>.Shared.Return(array);
//             }
//         }
//
//         public override string ToString()
//         {
//             return new string(_array, 0, _index);
//         }
//     }
//
//
//     public class TBWriteBenchmarks
//     {
//         private static readonly char[] _sourceChars;
//         private static readonly string?[] _sourceStrings;
//
//         static TBWriteBenchmarks()
//         {
//             _sourceChars = new char[]
//             {
//                 '\0',
//                 '\t',
//                 ' ',
//                 char.MaxValue,
//             };
//             _sourceStrings = new string?[]
//             {
//                 null,
//                 string.Empty,
//                 ",",
//                 Environment.NewLine,
//                 Guid.NewGuid().ToString("B"),
//                 string.Create(1024, Random.Shared, (span, rand) =>
//                 {
//                     for (var i = 0; i < span.Length; i++)
//                     {
//                         span[i] = (char)rand.Next();
//                     }
//                 }),
//             };
//         }
//
//
//         public IEnumerable<char> SourceChars => _sourceChars;
//         public IEnumerable<string?> SourceStrings => _sourceStrings;
//
//
//         [Benchmark]
//         public void WriteA()
//         {
//
//         }
//     }
// }
