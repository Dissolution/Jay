using System.Runtime.InteropServices;

// ReSharper disable IdentifierTypo

namespace Jay.COM
{
    /// <summary>
    /// Utility to assist with COM objects
    /// </summary>
    public static class COMHelper
    {
        private const int S_OK = 0;                                        // from WinError.h
        private const int LOCAL_SYSTEM_DEFAULT = 2 << 10;                  // from WinNT.h == 2048 == 0x800
        private const int DISP_E_UNKNOWNNAME = unchecked((int)0x80020006); // from WinError.h
        private const int DISPID_UNKNOWN = -1;                             // from OAIdl.idl

        private static Type? GetType(IDispatchInfo dispatch, bool throwIfNotFound = true)
        {
            Type? type = null;
            
            int hr = dispatch.GetTypeInfoCount(out int typeInfoCount);
            if (hr == S_OK && typeInfoCount > 0)
            {
                dispatch.GetTypeInfo(0, LOCAL_SYSTEM_DEFAULT, out type);
            }

            if (type == null && throwIfNotFound)
            {
                // If the GetTypeInfoCount failed, throw an exception for that
                Marshal.ThrowExceptionForHR(hr);
                // Otherwise throw the same exception that Type.GetType() would throw
                throw new TypeLoadException();
            }

            return type;
        }

        private static bool TryGetDispId(IDispatchInfo dispatch, string name, out int dispId)
        {
            Guid iidNull = Guid.Empty;
            int hr = dispatch.GetDispId(ref iidNull, ref name, 1, LOCAL_SYSTEM_DEFAULT, out dispId);
            if (hr == S_OK)
                return true;
            if (hr == DISP_E_UNKNOWNNAME && dispId == DISPID_UNKNOWN)
                return false;
            Marshal.ThrowExceptionForHR(hr);
            return false;
        }
        
        public static bool ImplementsIDispatch(object? obj)
        {
            return obj is IDispatchInfo;
        }

        public static Type GetType(object obj)
        {
            if (obj is null)
                throw new ArgumentNullException(nameof(obj));
            if (obj is IDispatchInfo dispatch) 
                return GetType(dispatch, true)!;
            return obj.GetType();
        }

        public static bool TryGetDispatchId(object? obj, string name, out int dispatchId)
        {
            if (obj is not IDispatchInfo dispatch)
            {
                dispatchId = default;
                return false;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                dispatchId = default;
                return false;
            }

            return TryGetDispId(dispatch, name, out dispatchId);
        }
    }
}