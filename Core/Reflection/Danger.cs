using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Jay.Reflection;

public static unsafe class Danger
{


    public static ref TOut CastRef<TIn, TOut>(ref TIn input)
        where TIn : unmanaged
        where TOut : unmanaged
    {
        return ref Unsafe.As<TIn, TOut>(ref input);
    }
}