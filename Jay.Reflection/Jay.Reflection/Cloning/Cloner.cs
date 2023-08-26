// namespace Jay.Reflection.Cloning;
//

//
//
// public static class Cloner
// {
//     private static readonly ConcurrentTypeMap<Delegate> _valueCloneCache;
//     private static readonly ConcurrentTypeMap<DeepClone<object?>> _objectCloneCache;
//     
//     static Cloner()
//     {
//         _valueCloneCache = new()
//         {
//             [typeof(string)] = (DeepClone<string>)FastClone,
//         };
//         _objectCloneCache = new()
//         {
//             [typeof(object)] = (DeepClone<object?>)ObjectClone,
//         };
//     }
//     
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     private static T FastClone<T>(T value) => value;
//
//     [return: NotNullIfNotNull(nameof(obj))]
//     private static object? ObjectClone(object? obj)
//     {
//         if (obj is null) return null;
//         var type = obj.GetType();
//         return GetOrCreateObjectDeepClone(type).Invoke(obj);
//     }
//     
//    
//     internal static DeepClone<T> GetOrCreateDeepClone<T>()
//     {
//         var del = _valueCloneCache.GetOrAdd<T>(CreateDeepClone);
//         if (del is DeepClone<T> deepClone)
//             return deepClone;
//         throw new InvalidOperationException();
//     }
//     internal static Delegate GetOrCreateDeepClone(Type valueType)
//     {
//         var del = _valueCloneCache.GetOrAdd(valueType, CreateDeepClone);
//         if (del is not null)
//             return del;
//         throw new InvalidOperationException();
//     }
//     
//     internal static DeepClone<object?> GetOrCreateObjectDeepClone(Type type)
//     {
//         var dcd = _objectCloneCache.GetOrAdd(type,
//             t =>
//             {
//                 var valueClone = GetOrCreateDeepClone(t);
//                 // We can wrap this delegate!
//                 return WrapValueDeepClone(t, valueClone);
//             });
//         return dcd;
//     }
//     
//
//
//     [return: NotNullIfNotNull(nameof(array))]
//     public static Array? DeepCloneArray(Array? array)
//     {
//         if (array is null) return null;
//         var arrayWrapper = new ArrayWrapper(array);
//         Array clone = Array.CreateInstance(arrayWrapper.ElementType, arrayWrapper.RankLengths, arrayWrapper.LowerBounds);
//         var elementCloner = GetOrCreateObjectDeepClone(arrayWrapper.ElementType);
//         var cloneWrapper = new ArrayWrapper(clone);
//         using var e = arrayWrapper.GetEnumerator();
//         while (e.MoveNext())
//         {
//             int[] index = e.Indices;
//             cloneWrapper.SetValue(index, elementCloner(e.Current));
//         }
//         return clone;
//     }
//
//     private static DeepClone<object?> WrapValueDeepClone(Type type, Delegate valueDeepClone)
//     {
//         var builder = RuntimeBuilder.CreateRuntimeDelegateBuilder<DeepClone<object?>>(Dump($"clone_{type}_as_obj"));
//         var emitter = builder.Emitter;
//         // Null check
//         emitter
//             .Ldarg(0) // load object?
//             .Brtrue(out var notNull) // if (obj != null) goto: notnull;
//             .Ldnull()
//             .Ret() // return null;
//             .MarkLabel(notNull) //notnull:
//             // Attempt to cast object -> T
//             .Ldarg(0)
//             .Isinst(type)
//             .Brtrue(out var isT)
//             .ThrowException<ArgumentNullException>("value", $"Object is not a {type}")
//             .MarkLabel(isT)
//             // Cast to T, call valueDeepClone, box
//             .Ldarg(0)
//             .Unbox_Any(type)
//             .Call(valueDeepClone.Method)
//             .Box(type)
//             .Ret();
//
//         var il = emitter.ToString();
//         Debugger.Break();
//         
//         return builder.CreateDelegate();
//     }
//
//     private static Delegate CreateDeepClone(Type type)
//     {
//         var builder = RuntimeBuilder.CreateRuntimeDelegateBuilder(
//             typeof(DeepClone<>).MakeGenericType(type),
//             Dump($"clone_{type}"));
//         var emitter = builder.Emitter;
//
//         // Null check for non-value types
//         if (!type.IsValueType)
//         {
//             emitter.Ldarg(0)
//                 .Ldnull()
//                 .Ceq()
//                 .Brfalse(out var notNull)
//                 .Ldnull()
//                 .Ret()
//                 .MarkLabel(notNull);
//         }
//
//         // prevent infinite recursion loops
//         if (type == typeof(object))
//         {
//             var ctor = ExpressionExtensions.ExtractMember<ConstructorInfo>(() => new object());
//             Debugger.Break();
//             emitter.Newobj(ctor)
//                 .Ret();
//         }
//         // unmanaged or string we just dup + return
//         else if (type == typeof(string) || type.IsUnmanaged())
//         {
//             emitter.Ldarga(0).Ret();
//         }
//         // Special Array handling
//         else if (type.IsArray)
//         {
//             emitter.Ldarg(0)
//                 .EmitCast(type, typeof(Array))
//                 .Call(Searching.MemberSearch.FindMethod(typeof(Cloner),
//                     new(nameof(DeepCloneArray), Visibility.Public | Visibility.Static, typeof(Array), typeof(Array))))
//                 .EmitCast(typeof(Array), type)
//                 .Ret();
//         }
//         // Everything else has some sort of reference down the chain
//         else
//         {
//             // Create a raw value
//             emitter.DeclareLocal(type, out var copy);
//
//             if (type.IsValueType)
//             {
//                 // init the copy, we'll clone the fields
//                 emitter.Ldloca(copy)
//                     .Initobj(type);
//
//                 // copy each instance field
//                 var fields = type.GetFields(Reflect.Flags.Instance);
//                 foreach (var field in fields)
//                 {
//                     if (field.FieldType.Implements<FieldInfo>())
//                         Debugger.Break();
//
//                     var fieldDeepClone = GetDeepCloneMethod(field.FieldType);
//                     emitter.Ldloca(copy)
//                         .Ldarg(0)
//                         .Ldfld(field)
//                         .Call(fieldDeepClone)
//                         .Stfld(field);
//                 }
//             }
//             else
//             {
//                 // Uninitialized object
//                 // We don't want to call the constructor, that may have side effects
//                 emitter.LoadType(type)
//                     .Call(MemberCache.Methods.RuntimeHelpers_GetUninitializedObject)
//                     .Unbox_Any(type)
//                     .Stloc(copy);
//
//                 // copy each instance field
//                 var fields = type.GetFields(Reflect.Flags.Instance);
//                 foreach (var field in fields)
//                 {
//                     var fieldDeepClone = GetDeepCloneMethod(field.FieldType);
//                     emitter.Ldloc(copy)
//                         .Ldarg(0)
//                         .Ldfld(field)
//                         .Call(fieldDeepClone)
//                         .Stfld(field);
//                 }
//             }
//
//             // Load our clone and return!
//             emitter.Ldloc(copy)
//                 .Ret();
//         }
//
//         return builder.CreateDelegate();
//     }
//     
//     private static MethodInfo GetDeepCloneMethod(Type type)
//     {
//         return Searching.MemberSearch.FindMethod(typeof(Cloner),
//                 new(nameof(DeepClone), Visibility.Public | Visibility.Static))
//             .MakeGenericMethod(type);
//     }
//
//   
//
//     [return: NotNullIfNotNull(nameof(value))]
//     public static T DeepClone<T>(this T value)
//     {
//         if (value is null) return default!;
//         return GetOrCreateDeepClone<T>().Invoke(value);
//     }
//     
//     [return: NotNullIfNotNull(nameof(array))]
//     public static T[]? DeepClone<T>(this T[]? array)
//     {
//         if (array is null) return null;
//         int len = array.Length;
//         T[] clone = new T[len];
//         var itemCloner = GetOrCreateDeepClone<T>();
//         for (int i = 0; i < len; i++)
//         {
//             clone[i] = itemCloner(array[i])!;
//         }
//         return clone;
//     }
//     
//     [return: NotNullIfNotNull(nameof(array))]
//     public static T[,]? DeepClone<T>(this T[,]? array)
//     {
//         if (array is null) return null;
//         int arrayLen0 = array.GetLength(0);
//         int arrayLen1 = array.GetLength(1);
//         T[,] clone = new T[arrayLen0, arrayLen1];
//         var deepClone = GetOrCreateDeepClone<T>();
//         for (var x = 0; x < array.GetLength(0); x++)
//         {
//             for (var y = 0; y < array.GetLength(1); y++)
//             {
//                 clone[x, y] = deepClone(array[x, y])!;
//             }
//         }
//         return clone;
//     }
// }