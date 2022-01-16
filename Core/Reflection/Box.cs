// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using InlineIL;
// using static InlineIL.IL;
//
//
// namespace Jay.Reflection
// {
//     public struct Box
//     {
//         private object? _box;
//         private Type? _valueType;
//
//         public ref T Ref<T>()
//         {
//             Emit.Ldarg(0);
//             Emit.Ldfld(FieldRef.Field(typeof(Box), nameof(_box)));
//             Emit.Unbox<T>();
//             return ref ReturnRef<T>();
//         }
//     }
// }
