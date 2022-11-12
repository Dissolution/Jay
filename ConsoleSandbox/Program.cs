using System.Buffers;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using ConsoleSandbox;
using Jay.Collections;
using Jay.Reflection;
using Jay.Reflection.Building;
using Jay.Reflection.Building.Deconstruction;
using Jay.Reflection.Implementation;
using Jay.Reflection.Search;
using Jay.Text;


//using TextBuilder = Jay.Text.Scratch.TextBuilder;

#if RELEASE
using Jay.BenchTests;
using Jay.BenchTests.Text;

    var result = Runner.RunAndOpenHtml<MathTests>();
    Console.WriteLine(result);

#else
using var text = TextBuilder.Borrow();
//
// var enumInfo = EnumInfo.For<BindingFlags>();
// var dict = new Dictionary<BindingFlags, string>();
// for (var i = 0; i < enumInfo.MemberCount; i++)
// {
//     dict[enumInfo.Members[i]] = enumInfo.Names[i];
// }
//
// Dictionary<BindingFlags, string> clone = Cloner.DeepClone(dict);
// var dump = Dumper.Dump(clone);
//

//new MontyHall().Test();

var str = "This is a test of an advanced encoding system";

var bytes = Encoding.ASCII.GetBytes(Dumb.Lorem);
var output = Dumb.Compress(bytes);
var encoded = Dumb.Encode(bytes);
var outputStr = Encoding.UTF8.GetString(output);
var encodedStr = Encoding.UTF8.GetString(encoded);

Debug.Assert(outputStr == encodedStr);

var input = Dumb.Decompress(output);
var inputStr = Encoding.ASCII.GetString(input);

Debug.Assert(inputStr == str);

var cdmText = string.Concat(str.Prepend('E').Select(ch => Dumb.ConvertToCDM(ch)));
var asciiText = string.Concat(cdmText.Skip(1).Select(ch => Dumb.ConvertToAscii(ch)));

Debug.Assert(asciiText == str);

var eq = outputStr == cdmText;

Debugger.Break();


//var member = MemberSearch.Find<FieldInfo>(() => typeof(MemberInfo).GetField("Blah", Reflect.InstanceFlags));


// text.Append("Writing").AppendNewLine()
//     .Append("{")
//     .Indent("    ", tb =>
//     {
//         tb.AppendNewLine()
//             .Append("Eat").AppendNewLine()
//             .Append("At").AppendNewLine()
//             .Append("Joes");
//     })
//     .AppendNewLine().Append("}").AppendNewLine();

// var backingType = InterfaceImplementer.CreateImplementationType<IKeyedEntity<int>>();
// var backingInstance = (Activator.CreateInstance(backingType) as IKeyedEntity<int>)!;
//
// var members = backingInstance.GetType().GetMembers(Reflect.AllFlags);
//
// var str = backingInstance.ToString();


var textString = str.ToString();
Debugger.Break();
Console.WriteLine(textString);
#endif

Console.WriteLine("Press Enter to close this window.");
Console.ReadLine();
return 0;


namespace ConsoleSandbox
{
    public class Dumb
    {
        public const string Lorem =
            @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed et bibendum erat, ut pellentesque velit. Duis pretium blandit velit, a finibus neque venenatis nec. Suspendisse convallis dictum neque at bibendum. Morbi porta ipsum eu enim sodales sagittis. Quisque et nibh blandit, semper velit sed, finibus felis. Suspendisse ut leo consectetur, lacinia magna a, condimentum orci. Curabitur id molestie nisi. Etiam eget nunc sodales, eleifend leo sit amet, dictum nisl. Fusce eleifend ligula libero, nec posuere diam consequat a. Nulla facilisi. Maecenas auctor erat quis arcu molestie, non sodales orci tincidunt.

In maximus rutrum diam at imperdiet. Quisque id lacus vulputate, porta lectus quis, convallis turpis. Proin aliquet rutrum arcu quis auctor. Interdum et malesuada fames ac ante ipsum primis in faucibus. Aenean luctus interdum elit, ut varius nibh feugiat in. Etiam a enim congue, tempor justo sit amet, porta felis. Nulla semper nunc sapien, sed interdum elit lacinia eleifend. Maecenas id semper dolor. Etiam suscipit sem id malesuada tristique. Aenean feugiat ultrices ligula, non elementum nunc feugiat vulputate. Nulla ipsum nulla, iaculis sit amet ornare in, semper finibus orci. Proin urna nibh, iaculis vitae enim in, lacinia auctor est. Vestibulum interdum rutrum urna, nec gravida nulla dictum ac. Etiam ultricies quam eget mi rutrum dictum. Sed eleifend ac orci at maximus.

Nullam rutrum quam enim, ac ullamcorper augue vestibulum sed. Nunc sollicitudin ligula a hendrerit egestas. In scelerisque tellus augue, et condimentum mauris finibus eget. Donec vitae sapien id dolor ultricies tempor dictum id mauris. Integer rutrum est vel velit tempor, vel tempor mi imperdiet. Proin pharetra urna sit amet purus sollicitudin rutrum. Etiam congue sem sed massa pretium dapibus. Maecenas tristique a ex in aliquet. Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Curabitur ligula ligula, efficitur sed pulvinar quis, rutrum sed enim.

Praesent tristique nibh et tellus volutpat rutrum. Pellentesque rhoncus hendrerit elit ut varius. Morbi hendrerit tortor ut massa scelerisque, pulvinar pharetra metus dictum. Integer gravida hendrerit leo. Phasellus sem erat, fringilla ut eros ut, fringilla rhoncus enim. Nulla maximus bibendum mollis. Donec blandit leo at mollis hendrerit. Donec cursus neque sed eros fringilla maximus. Donec a sem et felis sollicitudin blandit. Aenean dignissim sem a nulla laoreet porttitor.

Ut convallis tristique lacus, sed sagittis augue iaculis sed. In congue eleifend rutrum. Aenean sit amet enim lacinia, aliquam metus vel, varius quam. Nunc in dapibus arcu, gravida pharetra sapien. Aenean pellentesque nec ante efficitur mattis. Interdum et malesuada fames ac ante ipsum primis in faucibus. In finibus eros ac mi volutpat commodo. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin nisl dui, ullamcorper vel elementum quis, tincidunt eget turpis. Sed et turpis gravida, imperdiet magna nec, ultrices arcu. Cras ac odio non felis bibendum facilisis non in dui.";
        
        
        // https://unicode-table.com/en/blocks/combining-diacritical-marks/
        private const char cdmStart = (char)0b_00000011_00000000;
        private const char cdmEnd = (char)0b_00000011_01101111;

        private static byte[] cdmStartBytes = Encoding.UTF8.GetBytes(new char[1] { cdmStart });
        private static byte[] cdmEndBytes = Encoding.UTF8.GetBytes(new char[1] { cdmEnd });

        private const char asciiStart = (char)0b00100000;
        private const char asciiEnd = (char)0b01111110;

        static Dumb()
        {
            int len = (cdmEnd - cdmStart) + 1;
            Debug.Assert(len == 112);

            len = (asciiEnd - asciiStart) + 1;
            Debug.Assert(len == 95);
            //
            // var sStrs = cdmStartBytes.Select(b => Convert.ToString(b, 2)).ToList();
            // var eStrs = cdmEndBytes.Select(b => Convert.ToString(b, 2)).ToList();
            //
            // Debugger.Break();
        }

        public static byte[] Encode(ReadOnlySpan<byte> asciiBytes)
        {
            var count = asciiBytes.Length;
            
            byte[] output = new byte[1 + (count * 2)];
            output[0] = (byte)'E';    // seed char
            int o = 1;

            for (var i = 0; i < count; i++)
            {
                byte ch = asciiBytes[i];
                
                // Special handling for certain control chars
                if (ch == '\t')
                {
                    ch = asciiEnd + 1;
                }
                else if (ch == '\n')
                {
                    ch = asciiEnd + 2;
                }
                else if (ch == '\r')
                {
                    ch = asciiEnd + 3;
                }
                else if (ch < asciiStart || ch > asciiEnd)
                {
                    throw new ArgumentException($"The char '{(char)ch}' is not supported", nameof(asciiBytes));
                }

                // floor the ascii char to 0-94 (rather than 32-126)
                ch -= 32;

                // first UTF8 byte
                byte first = (byte)(((ch >> 6) & 0b00000001) | 0b11001100);
                output[o++] = first;
                // second UTF8 byte
                byte second = (byte)((ch & 0b00111111) | 0b10000000);
                output[o++] = second;
                
                var lh = Encoding.UTF8.GetString(new byte[2] { first, second });
                Debug.Assert(lh.Length == 1);
                Debug.Assert(lh[0] is >= cdmStart and <= cdmEnd);

                //Debugger.Break();
            }

            // fin
            Debug.Assert(o == output.Length);
            return output;
        }
        
        
        
        
        
        public static char ConvertToCDM(char asciiChar)
        {
            if (!TryConvertToCDM(asciiChar, out var cdmChar))
                throw new InvalidOperationException();
            return cdmChar;
        }

        // public static byte[] ConvertToCDMBytes(byte asciiChar)
        // {
        //     if (asciiChar == '\t') // 0x0009
        //     {
        //
        //
        //         cdmChar = (char)(asciiEnd + 1 + cdmStart);
        //     }
        //     else if (asciiChar == '\n') // 0x000A
        //     {
        //         cdmChar = (char)(asciiEnd + 2 + cdmStart);
        //     }
        //     else if (asciiChar == '\r') // 0x000D
        //     {
        //         cdmChar = (char)(asciiEnd + 3 + cdmStart);
        //     }
        //     else if (asciiChar >= asciiStart && asciiChar <= asciiEnd)
        //     {
        //         cdmChar = (char)((asciiChar - asciiStart) + cdmStart);
        //     }
        //     else
        //     {
        //         cdmChar = default;
        //         return false;
        //     }
        //
        //     return true;
        // }

        public static bool TryConvertToCDM(char asciiChar, out char cdmChar)
        {
            if (asciiChar == '\t') // 0x0009
            {
                cdmChar = (char)(asciiEnd + 1 + cdmStart);
            }
            else if (asciiChar == '\n') // 0x000A
            {
                cdmChar = (char)(asciiEnd + 2 + cdmStart);
            }
            else if (asciiChar == '\r') // 0x000D
            {
                cdmChar = (char)(asciiEnd + 3 + cdmStart);
            }
            else if (asciiChar >= asciiStart && asciiChar <= asciiEnd)
            {
                cdmChar = (char)((asciiChar - asciiStart) + cdmStart);
            }
            else
            {
                cdmChar = default;
                return false;
            }

            return true;
        }

        public static char ConvertToAscii(char cdmChar)
        {
            if (!TryConvertToAscii(cdmChar, out var asciiChar))
                throw new InvalidOperationException();
            return asciiChar;
        }

        public static bool TryConvertToAscii(char cdmChar, out char asciiChar)
        {
            if (cdmChar >= cdmStart && cdmChar <= cdmEnd)
            {
                asciiChar = (char)((cdmChar - cdmStart) + asciiStart);
                if (asciiChar >= asciiStart && asciiChar <= asciiEnd)
                    return true;
                if (asciiChar == asciiEnd + 1)
                {
                    asciiChar = '\t';
                    return true;
                }
                if (asciiChar == asciiEnd + 2)
                {
                    asciiChar = '\n';
                    return true;
                }
                if (asciiChar == asciiEnd + 3)
                {
                    asciiChar = '\r';
                    return true;
                }
            }

            asciiChar = default;
            return false;
        }



        //https: //github.com/DaCoolOne/DumbIdeas/blob/main/reddit_ph_compressor/compress.py
        //https://www.reddit.com/r/ProgrammerHumor/comments/yqof9f/the_most_upvoted_comment_picks_the_next_line_of/#ivrd9ur

        /* # Compress algorithm
    def unicode_compress(bytes):
        o = b'E'
        for c in bytes:
            # Skip carriage returns
            if c == 13:
                continue
            # Check for invalid code points
            if (c < 20 or c > 126) and c != 10:
                raise Exception("Cannot encode character with code point " + str(c))
            # Code point translation
            v = (c-11)%133-21
            o += ((v >> 6) & 1 | 0b11001100).to_bytes(1,'big')
            o += ((v & 63) | 0b10000000).to_bytes(1,'big')
        return o*/
        public static byte[] Compress(ReadOnlySpan<byte> asciiBytes)
        {
            List<byte> output = new(asciiBytes.Length * 2) { (byte)'E' };
            foreach (byte c in asciiBytes)
            {
                // Special handling
                if (c == '\t' || c == '\r') continue;

                // Check for invalid code points
                if (c != 10 && (c < 20 || c > 126))
                    throw new InvalidOperationException($"Cannot encode character with code point '{c}'");
                // code point translation
                int v = ((c - 11) % 133) - 21;

                int h = c - 32;
                Debug.Assert(v == h);

                byte lower = (byte)(((v >> 6) & 0b00000001) | 0b11001100);
                output.Add(lower);

                byte higher = (byte)((v & 0b00111111) | 0b10000000);
                output.Add(higher);

                var lh = Encoding.UTF8.GetString(new byte[2] { lower, higher });
                Debug.Assert(lh.Length == 1);
                Debug.Assert(lh[0] is >= cdmStart and <= cdmEnd);

                //Debugger.Break();
            }

            return output.ToArray();
        }

/*# Decompress algorithm (Code golfed)
def unicode_decompress(b):
    return ''.join([chr(((h<<6&64|c&63)+22)%133+10)for h,c in zip(b[1::2],b[2::2])])
*/
        public static byte[] Decompress(IReadOnlyList<byte> bytes)
        {
            var output = new List<byte>();
            var first = bytes.Skip(1).Where((b, i) => i % 2 == 0);
            var second = bytes.Skip(2).Where((b, i) => i % 2 == 0);
            foreach (var (h, c) in Enumerable.Zip(first, second))
            {
                byte x = (byte)(((((h << 6) & 0b01000000) | (c & 0b00111111)) + 22) % 133 + 10);
                output.Add(x);
            }

            return output.ToArray();
        }
    }



    public abstract class EnumLike<TEnum> : IEquatable<TEnum>, IComparable<TEnum>, IFormattable
        where TEnum : EnumLike<TEnum>
    {
        public static bool operator ==(EnumLike<TEnum> left, EnumLike<TEnum> right) => left.Equals(right);
        public static bool operator !=(EnumLike<TEnum> left, EnumLike<TEnum> right) => !left.Equals(right);
        public static bool operator <=(EnumLike<TEnum> left, EnumLike<TEnum> right) => left.CompareTo((TEnum)right) <= 0;
        public static bool operator <(EnumLike<TEnum> left, EnumLike<TEnum> right) => left.CompareTo((TEnum)right) < 0;
        public static bool operator >(EnumLike<TEnum> left, EnumLike<TEnum> right) => left.CompareTo((TEnum)right) > 0;
        public static bool operator >=(EnumLike<TEnum> left, EnumLike<TEnum> right) => left.CompareTo((TEnum)right) >= 0;

        private static readonly Func<string, TEnum> _ctorFunc;

        static EnumLike()
        {
            var ctor = MemberSearch.Find<ConstructorInfo>(() =>
                typeof(TEnum).GetConstructor(
                    Reflect.InstanceFlags,
                    new Type[1] { typeof(string) }));
            _ctorFunc = RuntimeBuilder.CreateDelegate<Func<string, TEnum>>(emitter =>
                emitter.Ldarg(0).Newobj(ctor).Ret());
        }
        protected static List<TEnum> _members = new();
        protected static TEnum Create([CallerMemberName] string name = "") => _ctorFunc(name);

        public static bool TryParse(ulong value, [NotNullWhen(true)] out TEnum? enumLike)
        {
            // All members are in order!
            foreach (var member in _members)
            {
                if (member.Value == value)
                {
                    enumLike = member;
                    return true;
                }

                if (member.Value > value) break;
            }

            enumLike = default;
            return false;
        }

        public string Name { get; }

        internal ulong Value { get; }

        protected EnumLike(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            this.Name = name;
            this.Value = GetNextValue();
            _members.Add((TEnum)this);
        }

        protected virtual ulong GetNextValue()
        {
            return (ulong)_members.Count;
        }

        public int CompareTo(TEnum? enumLike)
        {
            if (enumLike is null) return 1; // Null is before me
            return this.Value.CompareTo(enumLike.Value);
        }

        public bool Equals(TEnum? enumLike)
        {
            return enumLike is not null && enumLike.Value == this.Value;
        }

        public sealed override bool Equals(object? obj)
        {
            return obj is TEnum enumLike && enumLike.Value == this.Value;
        }

        public sealed override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return Name;
            //throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public abstract class FlagEnumLike<TEnum> : EnumLike<TEnum>
        where TEnum : FlagEnumLike<TEnum>
    {
        protected FlagEnumLike(string name) : base(name)
        {
        }

        protected override ulong GetNextValue()
        {
            // Each flag is a power of 2
            // From 1 << 0 to 1 << n
            return 1UL << _members.Count;
        }
    }

    public interface IEntity
    {
        string? Name { get; set; }

        string? ToString() => Name;
    }

    public interface IKeyedEntity<T> : IEntity,
                                       IEquatable<IKeyedEntity<T>>
    {
        [Equality]
        T Key { get; set; }

        string? IEntity.ToString() => $"{Key}: \"{Name}\"";
    }



    public static class Sandbox
    {
        public struct TestStruct
        {
        }

        public class TestClass : IKeyedEntity<int>
        {
            public string? Name { get; set; }

            public int Key { get; set; }

            public Guid Unique { get; }

            public TestClass()
            {
                Unique = Guid.NewGuid();
            }

            public bool Equals(IKeyedEntity<int>? other)
            {
                return other is TestClass otherTestClass &&
                       otherTestClass.Key == this.Key;
            }
        }

        public static ref TestStruct ToRefStruct(object obj)
        {
            return ref Unsafe.Unbox<TestStruct>(obj);
        }

        // public static ref TestClass ToRefClass(object obj)
        // {
        //     var tc = obj as TestClass;
        //     return ref tc;
        // }

        /*public static ref TestClass ToRefClass(TestClass testClass)
        {
            return ref testClass;
        }*/
    }

    // public class Dumpable : IDumpable
    // {
    //     public void DumpTo(TextBuilder textBuilder, DumpOptions? options = default)
    //     {
    //         textBuilder.Write("Dumpable Default Message");
    //     }
    // }

    public enum TestEnum : ulong
    {
        Zero = 0,
        One = 1,
        Just = 1L << 32,
        Q1 = (ulong)(ulong.MaxValue * (1d / 4d)),
        Q2 = (ulong)(ulong.MaxValue * (2d / 4d)),
        Q3 = (ulong)(ulong.MaxValue * (3d / 4d)),
        Q4 = ulong.MaxValue,
    }


    public static class Reflector
    {
        public static object Box<T>(T value)
        {
            return (object)value;
        }

        /*public static MemberInfo GetMember<TInstance>(TInstance instance, Expression<Func<TInstance, object?>> selectMemberExpression)
        {
            Func<TInstance, object?> selectMemberFunc;
            try
            {
                selectMemberFunc = selectMemberExpression.Compile();
            }
            catch (Exception ex)
            {
                Debugger.Break();
                throw new ArgumentException($"Could not compile the given {nameof(selectMemberExpression)}", nameof(selectMemberExpression), ex);
            }

            MemberInfo member;
            try
            {
                member = selectMemberFunc(instance);
            }
            catch (Exception ex)
            {
                Debugger.Break();
                throw new ArgumentException($"Could not find the given {nameof(selectMemberExpression)}", nameof(selectMemberExpression), ex);
            }

            return member;
        }*/

        public static TMember GetMember<TMember>(Type type, Expression<Func<Type, TMember>> selectMemberExpression)
            where TMember : MemberInfo
        {
            ArgumentNullException.ThrowIfNull(type);
            Func<Type, TMember> selectMemberFunc;
            try
            {
                selectMemberFunc = selectMemberExpression.Compile();
            }
            catch (Exception ex)
            {
                Debugger.Break();
                throw new ArgumentException($"Could not compile the given {nameof(selectMemberExpression)}", nameof(selectMemberExpression), ex);
            }

            TMember member;
            try
            {
                member = selectMemberFunc(type);
            }
            catch (Exception ex)
            {
                Debugger.Break();
                throw new ArgumentException($"Could not find the given {nameof(selectMemberExpression)}", nameof(selectMemberExpression), ex);
            }

            return member;
        }
    }

    [InterpolatedStringHandler]
    public ref struct Football //: IDisposable
    {
        internal char[]? _array;
        internal int _length;

        internal int Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _array!.Length;
        }

        public int Length => _length;

        public Span<char> Available
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Span<char>(_array, _length, Capacity - _length);
        }

        public Football()
        {
            _array = ArrayPool<char>.Shared.Rent(1024);
            _length = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(char ch)
        {
            _array![_length++] = ch;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ReadOnlySpan<char> text)
        {
            TextHelper.CopyTo(text, Available);
            _length += text.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(string? text)
        {
            if (text is not null)
            {
                TextHelper.CopyTo(text, Available);
                _length += text.Length;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write<T>(T value)
        {
            if (value is IFormattable)
            {
                if (value is ISpanFormattable)
                {
                    if (((ISpanFormattable)value).TryFormat(Available, out int charsWritten, default, default))
                    {
                        _length += charsWritten;
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                else
                {
                    Write(((IFormattable)value).ToString(default, default));
                }
            }
            else
            {
                Write(value?.ToString());
            }
        }

        public void Dispose()
        {
            var array = Interlocked.Exchange(ref _array, null);
            if (array is not null)
            {
                ArrayPool<char>.Shared.Return(array);
            }
        }

        public override string ToString()
        {
            return new string(_array!, 0, _length);
        }
    }

    public delegate void PerValue<in T>(ref Football football, T value);

    public static class FootballExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref Football Append(this ref Football football, char ch)
        {
            football.Write(ch);
            return ref football;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref Football AppendDelimit<T>(this ref Football football,
            ReadOnlySpan<char> delimiter,
            IEnumerable<T> values,
            PerValue<T> perValue)
        {
            using var e = values.GetEnumerator();
            if (e.MoveNext())
            {
                perValue(ref football, e.Current);
            }
            while (e.MoveNext())
            {
                football.Write(delimiter);
                perValue(ref football, e.Current);
            }
            return ref football;
        }
    }








    public interface IInstance : IFluent<IInstance>
    {
    }

    public interface IFluent<B> where B : IFluent<B>
    {
        B Chain();
        ITCF<B> Try(Delegate? del = default);
    }

    public interface ITCF<B>
        where B : IFluent<B>
    {
        B EndTry { get; }

        ITCF<B> Catch<TEx>(Delegate? del = default) where TEx : Exception;
        B Finally(Delegate? del = default);
    }

    public static class Extensions
    {
        public static THandler GetHandler<THandler>()
            where THandler : Delegate
        {
            throw new NotImplementedException();
        }

        public static string GetEvent<T, THandler>(this T? instance, Func<T?, THandler> eventInteraction)
            where THandler : Delegate
        {
            throw new NotImplementedException();
        }

        public static string GetEvent<T>(this T? instance, Func<T?, Delegate?> eventInteraction)
        {
            throw new NotImplementedException();
        }

        public class TestEventCapture<TInstance>
        {
            public string GetEvent<THandler>(Action<TInstance?, THandler?> eventInteraction)
                where THandler : Delegate
            {
                throw new NotImplementedException();
            }

            public THandler GetHandler<THandler>()
                where THandler : Delegate
            {
                throw new NotImplementedException();
            }
        }
    }

    public static class EventCapture<T> where T : class
    {
        public static string GetEvent<THandler>(Action<T, THandler> captureEvent)
            where THandler : Delegate
        {
            var instructions = new RuntimeDeconstructor(captureEvent.Method).GetInstructions();

            Debugger.Break();
            return "";


            // var type = typeof(T);
            // var interfaces = type.GetInterfaces();
            // var iface = interfaces.First();
            // var obj = typeof(EventCaptureProxy<>).MakeGenericType(iface)
            //                                      .GetMethod("Decorate", Reflect.StaticFlags, new Type[1]{iface}).ThrowIfNull()
            //                                      .Invoke(null, new object?[1]{null});
            // T instance = obj as T;
            //
            // Debugger.Break();
            //
            //
            // captureEvent(instance);
            //
            // Debugger.Break();
            // return "";
        }
    }

    public class EventCaptureProxy<T> : DispatchProxy
        where T : class
    {
        private readonly List<string> _eventNames;

        internal T? Instance { get; private set; }

        public EventCaptureProxy()
        {
            _eventNames = new List<string>(1);
        }

        /// <inheritdoc />
        protected override object? Invoke(MethodInfo? targetMethod, params object?[]? args)
        {
            Debugger.Break();
            return null;
        }

        public static T Decorate(T? instance = default)
        {
            // DispatchProxy.Create creates proxy objects
            var proxy = (Create<T, EventCaptureProxy<T>>() as EventCaptureProxy<T>)!;

            // If the proxy wraps an underlying object, it must be supplied after creating
            // the proxy.
            proxy.Instance = instance ?? Activator.CreateInstance<T>();

            return (proxy as T)!;
        }
    }


    public class Local
    {
        // public static void Capture<T, THandler>(T thing, Action<T, THandler> func)
        //     where THandler : Delegate
        // {
        //
        // }

        public static void Capture<T, THandler>(T thing, Expression<Action<T, THandler>> func)
            where THandler : Delegate
        {

        }

        private static class S<T>
        {
            public static T Default = default!;
        }

        public static ref T GetRef<T>()
        {
            return ref S<T>.Default;
        }

        public static void Me<T>(out T value)
        {
            value = GetRef<T>();
        }

        public static T GenericType<T>(T value)
        {
            return value;
        }




        public static string CaptureOutParamName(out Label lbl, [CallerArgumentExpression("lbl")] string? labelName = null)
        {
            lbl = default;
            return labelName!;
        }
    }

    public class Thing : INotifyPropertyChanged
    {
        private int _id;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public sealed class DynamicWrapper : DynamicObject
    {
        private static readonly ConcurrentTypeDictionary<ConcurrentDictionary<ExpressionType, Delegate>> _cache;

        static DynamicWrapper()
        {
            _cache = new ConcurrentTypeDictionary<ConcurrentDictionary<ExpressionType, Delegate>>();
        }




        private readonly object? _obj;
        private readonly Type? _objType;

        public DynamicWrapper(object? obj)
        {
            _obj = obj;
            _objType = obj?.GetType();
        }

        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object? result)
        {
            throw new NotImplementedException();

            //return base.TryBinaryOperation(binder, arg, out result);
        }

        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object? result)
        {
            throw new NotImplementedException();

            //return base.TryUnaryOperation(binder, out result);
        }
    }



    internal class Scope
    {
        public static string GetType(object? obj)
        {
            if (obj is null)
                return "null";
            if (obj is int i)
                return "int";

            return obj.GetType().Name;
        }

        public static string GetGenType<T>(T value) => typeof(T).Name;

        public static string GetGenValueType<T>(T value)
        {
            if (value is null) return "null";
            if (value is int i) return "int";
            return value.GetType().Name;
        }
    }
}