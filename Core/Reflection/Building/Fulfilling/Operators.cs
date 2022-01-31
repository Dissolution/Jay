using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Jay.Reflection.Building.Fulfilling
{
    public static class Operators
    {
    }

    public static class Operators<T>
    {
        public static T? Default { get; }

        private static readonly Lazy<Func<T, bool>> _unaryTrue = new Lazy<Func<T, bool>>();

        private static TDelegate CreateDelegate( sa)
    }
}
