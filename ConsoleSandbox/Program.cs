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
using System.Runtime.InteropServices;
using System.Text;
using ConsoleSandbox;
using Jay;
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

// var cdmstart = "̀";
// var cdmstartbytes = Encoding.UTF8.GetBytes(cdmstart);
//
// var oneway = ((cdmstartbytes[0] << 8) | cdmstartbytes[1]);
// var theother = ((cdmstartbytes[1] << 8) | cdmstartbytes[0]);
//
// var cdmend = "ͯ";
// var cdmendbytes = Encoding.UTF8.GetBytes(cdmend);
//
// var thisway = ((cdmendbytes[0] << 8) | cdmendbytes[1]);
// var thatway = ((cdmendbytes[1] << 8) | cdmendbytes[0]);
//
// Debugger.Break();
//


var bytes = Encoding.ASCII.GetBytes(str);
var encoded = CDM.Encode(bytes);
var encodedStr = Encoding.UTF8.GetString(encoded);

var decoded = CDM.Decode(encoded);
var decodedStr = Encoding.ASCII.GetString(decoded);

Debug.Assert(decodedStr == str);

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


string? textString = str.ToString();
Debugger.Break();
Console.WriteLine(textString);
#endif

Console.WriteLine("Press Enter to close this window.");
Console.ReadLine();
return 0;


namespace ConsoleSandbox
{
    /// <summary>
    /// Combining Diacritical Marks
    /// </summary>
    /// <see href="https://www.reddit.com/r/ProgrammerHumor/comments/yqof9f/the_most_upvoted_comment_picks_the_next_line_of/#ivrd9ur"/>
    /// <see href="https://unicode-table.com/en/blocks/combining-diacritical-marks/"/>
    /// <see href="https://github.com/DaCoolOne/DumbIdeas/blob/main/reddit_ph_compressor/compress.py"/>
    public static class CDM
    {
        /* ASCII is usually 0-127
         * But we want to skip all the control characters at the beginning
         * and the `delete` at the end
         */
        private const byte ASCII_START = 0x0020; // space

        private const byte ASCII_END = 0x007E; // tilde
        // 95 total possible chars

        // Combining Diacritical Marks exist in this UTF8 code block:
        private static readonly ushort CDM_START = 0b_00000011_00000000;

        private static readonly ushort CDM_END = 0b_00000011_01101111;

        // 112 total possible chars
        // We also need a seed ASCII char, something all the CDMs attach to
        private const byte SEED = (byte)'~';


        private static readonly byte[] _specialAsciiChars = { (byte)'\t', (byte)'\n', (byte)'\r' };

        private static ReadOnlySpan<byte> SpecialAsciiChars => _specialAsciiChars;

        public static byte[] Encode(ReadOnlySpan<byte> asciiBytes)
        {
            var count = asciiBytes.Length;

            // Seed, plus two UTF8 bytes per ASCII byte
            byte[] output = new byte[1 + (count * 2)];
            output[0] = SEED;
            int o = 1;

            for (var i = 0; i < count; i++)
            {
                byte ch = asciiBytes[i];

                // Outside normal?
                if (ch < ASCII_START || ch > ASCII_END)
                {
                    // Check special handling
                    int j = SpecialAsciiChars.IndexOf(ch);
                    // If we found one, add it as an offset to ASCII_END (which will easily fit in CDM's total size)
                    if (j >= 0)
                    {
                        ch = (byte)(ASCII_END + j +
                            1); // Has to be +1 or the first special char will be at ASCII_END, an overlap
                    }
                    else // No special handler, fail
                    {
                        throw new ArgumentException($"The char '{(char)ch}' is not supported", nameof(asciiBytes));
                    }
                }

                // floor the ascii char to 0-94 (rather than 32-126)
                ch -= ASCII_START;

                // first UTF8 byte
                byte first = (byte)(((ch >> 6) & 0b00000001) | 0b11001100);
                output[o++] = first;

                // second UTF8 byte
                byte second = (byte)((ch & 0b00111111) | 0b10000000);
                output[o++] = second;
            }

            // fin
            Debug.Assert(o == output.Length);
            return output;
        }

        [DoesNotReturn]
        private static void ThrowDecodeException(
            string message,
            ReadOnlySpan<byte> utf8Bytes,
            [CallerArgumentExpression(nameof(utf8Bytes))]
            string? bytesName = null)
        {
            string utf8Text = Encoding.UTF8.GetString(utf8Bytes);
            throw new ArgumentException($"The given UTF8 text \"{utf8Text}\" cannot be decoded: {message}", bytesName);
        }

        public static byte[] Decode(ReadOnlySpan<byte> utf8Bytes)
        {
            int len = utf8Bytes.Length;
            if (len == 0)
                ThrowDecodeException("You must pass at least one byte", utf8Bytes);
            if (utf8Bytes[0] != SEED)
                ThrowDecodeException($"Bad seed '{utf8Bytes[0]}'", utf8Bytes);
            int u = 1;

            // output is len - 1 (for seed), / 2 (two utf8 bytes per ascii byte)
            int outputLen = len - 1;
            if (outputLen % 2 != 0)
                ThrowDecodeException("Incorrect byte pairing", utf8Bytes);
            outputLen /= 2;

            byte[] asciiBytes = new byte[outputLen];
            int a = 0;
            while (a < outputLen)
            {
                byte firstUtf8Byte = utf8Bytes[u];
                byte secondUtf8Byte = utf8Bytes[u + 1];

                byte asciiByte = (byte)(((firstUtf8Byte << 6) & 0b01000000) | (secondUtf8Byte & 0b00111111));

                // Have to get it back to between ASCII_START and ASCII_END
                asciiByte += ASCII_START;

                // Might be a special char?
                if (asciiByte > ASCII_END)
                {
                    int offset = asciiByte - ASCII_END - 1; // remove the 1-base we added originally
                    if (offset >= 0 && offset < SpecialAsciiChars.Length)
                    {
                        asciiByte = SpecialAsciiChars[offset];
                    }
                    else
                    {
                        string utf8 = Encoding.UTF8.GetString(new byte[2] { firstUtf8Byte, secondUtf8Byte });
                        ThrowDecodeException($"Invalid UTF8 char '{utf8}'", utf8Bytes);
                    }
                }

                // We have a valid ascii char
                asciiBytes[a] = asciiByte;

                a += 1; // Next ascii char (1 byte)
                u += 2; // Next utf8 char (2 bytes)
            }

            // fin
            Debug.Assert(a == ((len - 1) / 2));
            Debug.Assert(u == utf8Bytes.Length);
            return asciiBytes;
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