using System.Runtime.InteropServices;

// ReSharper disable IdentifierTypo

namespace Jay.COM;

/// <summary>
/// Utility to assist with COM objects
/// </summary>
public static class COMHelper
{
    private const int S_OK = 0;                                        // from WinError.h
    private const int LOCAL_SYSTEM_DEFAULT = 2 << 10;                  // from WinNT.h == 2048 == 0x800
    private const int DISP_E_UNKNOWNNAME = unchecked((int)0x80020006); // from WinError.h
    private const int DISPID_UNKNOWN = -1;                             // from OAIdl.idl

    /// <summary>
    /// Gets a Type that can be used with reflection.
    /// </summary>
    /// <param name="dispatch">An object that implements IDispatch.</param>
    /// <param name="throwIfNotFound">Whether an exception should be thrown if a Type can't be obtained.</param>
    /// <returns>A .NET Type that can be used with reflection.</returns>
    /// <exception cref="TypeLoadException"></exception>
    private static Type? GetType(IDispatch dispatch, 
        bool throwIfNotFound = true)
    {
        Type? type = null;
            
        int hResult = dispatch.GetTypeInfoCount(out int typeInfoCount);
        if (hResult == S_OK && typeInfoCount > 0)
        {
            // Type info isn't usually culture-aware for IDispatch, so we might as well pass
            // the default locale instead of looking up the current thread's LCID each time
            // (via CultureInfo.CurrentCulture.LCID).
            dispatch.GetTypeInfo(0, LOCAL_SYSTEM_DEFAULT, out type);
        }

        if (type is null && throwIfNotFound)
        {
            // If the GetTypeInfoCount failed, throw an exception for that
            Marshal.ThrowExceptionForHR(hResult);
            // Otherwise throw the same exception that Type.GetType() would throw
            throw new TypeLoadException();
        }

        return type;
    }

    /// <summary>
    /// Tries to get the DISPID for the requested member name.
    /// </summary>
    /// <param name="dispatch">An object that implements IDispatch.</param>
    /// <param name="name">The name of a member to lookup.</param>
    /// <param name="dispId">If the method returns true, this holds the DISPID on output.
    /// If the method returns false, this value should be ignored.</param>
    /// <returns>True if the member was found and resolved to a DISPID.  False otherwise.</returns>
    private static bool TryGetDispId(IDispatch dispatch, string name, out int dispId)
    {
        if (dispatch is null)
            throw new ArgumentNullException(nameof(dispatch));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        bool result = false;

        // Members names aren't usually culture-aware for IDispatch, so we might as well
        // pass the default locale instead of looking up the current thread's LCID each time
        // (via CultureInfo.CurrentCulture.LCID).
        Guid iidNull = Guid.Empty;
        int hr = dispatch.GetDispId(ref iidNull, ref name, 1, LOCAL_SYSTEM_DEFAULT, out dispId);

        if (hr == S_OK)
        {
            result = true;
        }
        else if (hr == DISP_E_UNKNOWNNAME && dispId == DISPID_UNKNOWN)
        {
            // This is the only supported "error" case because it means IDispatch
            // is saying it doesn't know the member we asked about.
            result = false;
        }
        else
        {
            // The other documented result codes are all errors.
            Marshal.ThrowExceptionForHR(hr);
        }

        return result;
    }
        
    /// <summary>
    /// Gets whether the specified object implements <see cref="IDispatch"/>.
    /// </summary>
    /// <param name="obj">An <see cref="object"/> to check.</param>
    /// <returns><see langword="true"/> if the object implements <see cref="IDispatch"/>; otherwise, <see langword="false"/>.</returns>
    public static bool ImplementsIDispatch(object? obj)
    {
        return obj is IDispatch;
    }

    /// <summary>
    /// Gets a <see cref="Type"/> that can be used with reflection.
    /// </summary>
    /// <param name="obj">An <see cref="object"/> that implements <see cref="IDispatch"/>.</param>
    /// <param name="throwIfNotFound">Whether an <see cref="InvalidCastException"/> should be thrown if a <see cref="Type"/> can't be obtained.</param>
    /// <returns>A .NET <see cref="Type"/> that can be used with reflection.</returns>
    /// <exception cref="InvalidCastException">If <paramref name="obj"/> doesn't implement <see cref="IDispatch"/>.</exception>
    public static Type GetType(object obj)
    {
        if (obj is null)
            throw new ArgumentNullException(nameof(obj));
        if (obj is IDispatch dispatch) 
            return GetType(dispatch, true)!;
        return obj.GetType();
    }

    /// <summary>
    /// Tries to get the DISPID for the requested member name.
    /// </summary>
    /// <param name="obj">An object that implements IDispatch.</param>
    /// <param name="name">The name of a member to lookup.</param>
    /// <param name="dispId">If the method returns true, this holds the DISPID on output.
    /// If the method returns false, this value should be ignored.</param>
    /// <returns>True if the member was found and resolved to a DISPID.  False otherwise.</returns>
    /// <exception cref="InvalidCastException">If <paramref name="obj"/> doesn't implement IDispatch.</exception>
    public static bool TryGetDispatchId(object? obj, string name, out int dispatchId)
    {
        if (obj is not IDispatch dispatch)
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