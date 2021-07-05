/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Jay.Reflection.Deep
{
    public class Members
    {
        static Members()
        {
            
        }
        
        public Members()
        {
            //Members.Of<int>.
        }

        public interface IMembers : IEnumerable<MemberInfo>
        {
            
        }

        [Flags]
        public enum NameMatchFlags
        {
            Exact = 0,
            IgnoreCase = 1 << 0,
            StartsWith = 1 << 1,
            EndsWith = 1 << 2,
            Contains = 1 << 3 | StartsWith | EndsWith,
            Any = IgnoreCase | Contains,
        }

        [Flags]
        public enum VisibilityFlags
        {
            Public = 1 << 0,
            Internal = 1 << 1,
            Protected = 1 << 2,
            Private = 1 << 3,
            NonPublic = Internal | Protected | Private,
            Static = 1 << 4,
            Instance = 1 << 5,
            Any = Public | NonPublic | Static | Instance,
        }
        
        public static class Of<T>
        {
            public static IEnumerable<MemberInfo> Where(string? name = null,
                                          NameMatchFlags nameMatch = NameMatchFlags.Any,
                                          VisibilityFlags visibility = VisibilityFlags.Any,
                                          MemberTypes memberTypes = MemberTypes.All)
            {
                throw new NotImplementedException();
            }

            public static FieldInfo FindField(string? name = null,
                                              NameMatchFlags nameMatch = NameMatchFlags.Any,
                                              VisibilityFlags visibility = VisibilityFlags.Any,
                                              Type? fieldType = null)
            {
                var type = typeof(T);
                var fields = type.GetFields(Reflect.AllFlags);
                if (!string.IsNullOrWhiteSpace(name))
                {
                    if (nameMatch == NameMatchFlags.Exact)
                    {
                        fields.Select(f => f.Name == name);
                    }
                }
            }
        }
    }
}*/