using System;
using System.Threading;
using Jay.GhostInput.Native;

namespace Jay.GhostInput
{
    /// <summary>
    /// A class that manages input blocks
    /// </summary>
    internal sealed class InputBlocker : IDisposable
    {
        /// <summary>
        /// The number of different locks that have been placed upon our input blocker.
        /// </summary>
        private static int _lockCount;

        /// <summary>
        /// Get an <see cref="IDisposable"/> block on Input.
        /// </summary>
        public static IDisposable Block => new InputBlocker();

        /// <summary>
        /// Add a new Input Block.
        /// </summary>
        public static void Add()
        {
            //On the first increment (0->1), block input
            if (Interlocked.Increment(ref _lockCount) == 1)
                NativeMethods.BlockInput();
        }

        /// <summary>
        /// Remove an Input Block.
        /// </summary>
        public static void Remove()
        {
            //If we decrement to 0 (no more locks), unblock input
            if (Interlocked.Decrement(ref _lockCount) == 0)
                NativeMethods.UnblockInput();
        }

        /// <summary>
        /// Clear all Input Blocks.
        /// </summary>
        public static void Clear()
        {
            Interlocked.Exchange(ref _lockCount, 0);
            NativeMethods.UnblockInput();
        }

        public InputBlocker()
        {
            //On the first increment (0->1), block input
            if (Interlocked.Increment(ref _lockCount) == 1)
                NativeMethods.BlockInput();
        }
        ~InputBlocker()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            //If we decrement to 0 (no more locks), unblock input
            if (Interlocked.Decrement(ref _lockCount) == 0)
                NativeMethods.UnblockInput();
            GC.SuppressFinalize(this);
        }
    }
}