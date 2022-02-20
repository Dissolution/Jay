using System.Runtime.InteropServices;
using System.Runtime.InteropServices.CustomMarshalers;

// ReSharper disable IdentifierTypo

namespace Jay.COM
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("00020400-0000-0000-C000-000000000046")]
    internal interface IDispatchInfo
    {
        [PreserveSig]
        int GetTypeInfoCount(out int typeInfoCount);

        void GetTypeInfo(int typeInfoIndex, 
                         int lcid, 
                         [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(TypeToTypeInfoMarshaler))] out Type typeInfo);

        [PreserveSig]
        int GetDispId(ref Guid riid, ref string name, int nameCount, int lcid, out int dispId);

        // NOTE: The real IDispatch also has an Invoke method next, but we don't need it.            
    }
}