using Jay.Debugging;

namespace Jay.Sandbox.Reflection
{
    public static class Methods
    {
        public unsafe static void Modifiers(int a, in int b, ref int c, out int d,
                                            int* e, in int* f, ref int* g, out int* h)
        {
            d = default;
            h = default;
        }
        
        public static void Action()
        {
            Hold.Debug();
        }

        public static void Action<T>(T? value)
        {
            Hold.Debug(value);
        }
        
        public static void InAction<T>(in T? value)
        {
            Hold.Debug(value);
        }
        
        public static void RefAction<T>(ref T? value)
        {
            Hold.Debug(value);
        }
        
        public static void OutAction<T>(out T? value)
        {
            value = default;
            Hold.Debug(value);
        }
        
        public static void StructAction<T>(T value)
            where T : struct
        {
            Hold.Debug(value);
        }
        
        public static void InStructAction<T>(in T value)
            where T : struct
        {
            Hold.Debug(value);
        }
        
        public static void RefStructAction<T>(ref T value)
            where T : struct
        {
            Hold.Debug(value);
        }
        
        public static void OutStructAction<T>(out T value)
            where T : struct
        {
            value = default;
            Hold.Debug(value);
        }
    }
}