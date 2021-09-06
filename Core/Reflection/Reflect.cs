using System.Reflection;
using Jay.Reflection.Emission;


namespace Jay.Reflection
{
    public static partial class Reflect
    {
        /// <summary>
        /// A filler for a <see cref="Getter{TInstance,TValue}"/> or <see cref="Setter{TInstance,TValue}"/> instance type.
        /// </summary>
        public sealed class Static
        {
            private static Static _instance = new Static();

            /// <summary>
            /// An instance of <see cref="Static"/> to be used for a `ref Static instance`
            /// </summary>
            public static ref Static Instance => ref _instance;
        }
    }
    
    public static partial class Reflect
    {
        public const BindingFlags AllFlags = BindingFlags.Public | BindingFlags.NonPublic |
                                             BindingFlags.Instance | BindingFlags.Static |
                                             BindingFlags.IgnoreCase;
        
        public const BindingFlags PublicFlags = BindingFlags.Public |
                                             BindingFlags.Instance | BindingFlags.Static |
                                             BindingFlags.IgnoreCase;
        
        public const BindingFlags NonPublicFlags = BindingFlags.NonPublic |
                                             BindingFlags.Instance | BindingFlags.Static |
                                             BindingFlags.IgnoreCase;
        
        public const BindingFlags InstanceFlags = BindingFlags.Public | BindingFlags.NonPublic |
                                             BindingFlags.Instance | 
                                             BindingFlags.IgnoreCase;
        
        public const BindingFlags StaticFlags = BindingFlags.Public | BindingFlags.NonPublic |
                                             BindingFlags.Static |
                                             BindingFlags.IgnoreCase;
    }
}