// using Jay.Reflection.Builders;
//
// namespace Jay.Reflection.Maths;
//
// public static class Operators
// {
//     
// }
//
// public static class Operators<T>
// {
//     private static readonly Func<T, T> _negate;
//
//     static Operators()
//     {
//         _negate = RuntimeBuilder.BuildDelegate<Func<T,T>>($"negate_{typeof(T)}", build =>
//         {
//             // Search for primitive types that we can Emit.Neg()
//             // Then, search for op_Negate (name?)
//             
//         })
//     }
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public static void Negate(T value) => _negate(value);
// }
//
// public static class Operators<T, U>
// {
//     
// }
//
// public static class Operators<T, U, V>
// {
//     
// }