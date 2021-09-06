using System.Reflection.Emit;

namespace Jay.Reflection.Emission
{
    internal static class EmitExtensions
    {
        public static bool IsShort(this Label label)
        {
            int index = label.GetHashCode();
            return index <= sbyte.MaxValue && index >= sbyte.MinValue;
        }
        
        public static bool IsShort(this LocalBuilder local)
        {
            int index = local.LocalIndex;
            return index <= 65534 && index >= 0;
        }

    }
}