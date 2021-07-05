using System;
using System.Globalization;
using System.Windows.Input;

namespace Jay.GhostInput.Native
{
    internal sealed class KeyboardPointer : IDisposable
    {
        private readonly IntPtr _ptr;

        public KeyboardPointer(int klid)
        {
            _ptr = NativeMethods.LoadKeyboardLayout(klid.ToString("X8"), 1);
        }
        public KeyboardPointer(CultureInfo culture)
            : this(culture.KeyboardLayoutId){}
        public KeyboardPointer()
            : this(CultureInfo.CurrentCulture){}

        ~KeyboardPointer()
        {
            NativeMethods.UnloadKeyboardLayout(_ptr);
        }

        public void Dispose()
        {
            NativeMethods.UnloadKeyboardLayout(_ptr);
            GC.SuppressFinalize(this);
        }

        public bool GetKey(char c, out Key key)
        {
            short keyNumber = NativeMethods.VkKeyScanEx(c, _ptr);
            if (keyNumber == -1)
            {
                key = default;
                return false;
            }

            key = KeyInterop.KeyFromVirtualKey(keyNumber);
            return key != default;
        }

        public bool GetVirtualKey(char c, out int virtualKey)
        {
            virtualKey = NativeMethods.VkKeyScanEx(c, _ptr);
            return virtualKey != -1;
        }

    }
}