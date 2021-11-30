using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using static InlineIL.IL;

namespace Jay.Text;

internal  static unsafe class UnsafeText
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteTwoChars(text source, Span<char> dest)
    {
        Unsafe.WriteUnaligned(
                              ref Unsafe.As<char, byte>(ref MemoryMarshal.GetReference(source)),
                              Unsafe.ReadUnaligned<int>(ref Unsafe.As<char, byte>(ref MemoryMarshal.GetReference(dest))));
    }
}