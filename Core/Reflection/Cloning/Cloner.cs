using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Jay.Collections;
using Jay.Reflection.Building;
using Jay.Reflection.Building.Adapting;
using Jay.Reflection.Search;

namespace Jay.Reflection.Cloning
{
    public static class Cloner
    {
        [return: NotNullIfNotNull("value")]
        public delegate T CloneDelegate<T>(in T value);

        private static readonly DelegateMemberCache _cloneDelegateCache;

        static Cloner()
        {
            _cloneDelegateCache = new DelegateMemberCache();
        }


        private static CloneDelegate<T> CreateCloneDelegate<T>(Type type)
        {
            var dm = RuntimeBuilder.CreateDynamicMethod<CloneDelegate<T>>($"clone_{type.Name}");
            var emitter = dm.Emitter;
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                // Start with a blank clone
                emitter.DeclareLocal<T>(out var clone);
                if (type.IsValueType)
                {
                    emitter.Ldloca(clone)
                           .Initobj(type);
                }
                else if (type.HasDefaultConstructor(out var ctor))
                {
                    emitter.Newobj(ctor)
                           .Stloc(clone);
                }
                else
                {
                    emitter.LoadUninitialized(type)
                           .Stloc(clone);
                }

                // Copy each field in turn
            }
            else
            {
                // Plain value type, will be copied anyways
                emitter.Ldarg(0)
                       .Ldind<T>()
                       .Ret();
            }
            return dm.CreateDelegate();
        }


        public static CloneDelegate<T> CreateCloneDelegate<T>() => CreateCloneDelegate<T>(typeof(T));

            [return: NotNullIfNotNull("value")]
        public static T Clone<T>(in T value)
        {
            if (value is null) return default;
            var del = _cloneDelegateCache.GetOrAdd(typeof(T), type => CreateCloneDelegate<T>(type));
            return del(in value);
        }
    }
}
