using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jay.Reflection;

public static class MethodInfoExtensions
{
    public static TDelegate CreateDelegate<TDelegate>(this MethodInfo method)
    {
        ArgumentNullException.ThrowIfNull(method);

    }
}