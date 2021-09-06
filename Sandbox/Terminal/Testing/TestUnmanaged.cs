using System.Runtime.InteropServices;

namespace Jay.Sandbox
{
    [StructLayout(LayoutKind.Explicit)]
    public struct TestUnmanaged
    {
        [FieldOffset(0)] 
        public int FirstInt;

        // [FieldOffset(4)]
        // public int SecondInt;
        //
        // [FieldOffset(8)]
        // public int ThirdInt;
        //
        // [FieldOffset(12)]
        // public int FourthInt;
        //
        // [FieldOffset(16)]
        // public int FifthInt;
    }
}