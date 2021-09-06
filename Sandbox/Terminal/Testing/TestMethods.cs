namespace Jay.Sandbox
{
    public static class TestMethods
    {
        public static int HalfRoundUp(int value)
        {
            return (value >> 1) + (value & 1);
        }
        
        public static int HalfRoundDown(int value)
        {
            return (value >> 1);
        }
    }
}