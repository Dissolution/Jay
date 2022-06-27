using System.Buffers;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Jay;
using Jay.Collections;
using Jay.Dumping;
using Jay.Reflection.Building.Deconstruction;
using Jay.Text;

//using TextBuilder = Jay.Text.Scratch.TextBuilder;

#if RELEASE
using Jay.BenchTests;
using Jay.BenchTests.Text;

    var result = Runner.RunAndOpenHtml<TextBuilderWriteCharBenchTests>();
    Console.WriteLine(result);

#else
using var text = TextBuilder.Borrow();


var type = typeof(Dictionary<string, object?>);
var def = type.GetDefaultValue();


Debugger.Break();

Console.WriteLine(text.ToString());
#endif
    
Console.WriteLine("Press Enter to close this window.");
Console.ReadLine();
return 0;


namespace ConsoleSandbox
{
    public static class Sandbox
    {
        public struct TestStruct
        {
            
        }

        public class TestClass
        {
            
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
        internal char[] _array;
        internal int _length;

        internal int Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _array.Length;
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
            _array[_length++] = ch;
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
            return new string(_array, 0, _length);
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





    public interface IEntity : INotifyPropertyChanged,
                               INotifyPropertyChanging
    {
        DateTimeOffset TimeStamp { get; set; }
        string Name { get; set; }
    }

    public interface IKeyEntity<TKey> : IEntity,
                                        IEquatable<IKeyEntity<TKey>>
                                        
        where TKey : IComparable<TKey>
    {
        TKey Key { get; }
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
        where T: class
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