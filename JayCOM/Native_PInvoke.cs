using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Jay.Reflection.Building;
using Jay.Reflection.Building.Adapting;
using Jay.Validation;

namespace Jay.COM;

public static class NativeHelper
{
    private static readonly Func<int, string> _getErrorMessage;

    static NativeHelper()
    {
        // We want access to Win32Exception.GetErrorMessage(int errorCode), but it is private
        var method = typeof(Win32Exception)
            .GetMethod("GetErrorMessage",
                BindingFlags.NonPublic | BindingFlags.Static,
                new Type[1] { typeof(int) })
            .ThrowIfNull("Could not find Win32Exception.GetErrorMessage(int)");
        _getErrorMessage = RuntimeBuilder.CreateDelegate<Func<int, string>>(emitter => 
            emitter.Ldarg_0().Call(method).Ret());
    }
        
    public static Result PInvoke(Action action)
    {
        try
        {
            action();
            return true;
        }
        catch (Win32Exception win32Exception)
        {
            return win32Exception;
        }
        catch (RuntimeWrappedException nonCLSException)
        {
            return new Win32Exception("PInvoke Failure", nonCLSException);
        }
        catch (Exception ex)
        {
            var errorId = Marshal.GetLastWin32Error();
            // if (errorId <= 0)
            //     return ex;
            return new Win32Exception(_getErrorMessage(errorId), ex);
        }
    }
        
    public static Result<T> PInvoke<T>(Func<T> function)
    {
        try
        {
            return function()!;
        }
        catch (Win32Exception win32Exception)
        {
            return win32Exception;
        }
        catch (RuntimeWrappedException nonCLSException)
        {
            return new Win32Exception("PInvoke Failure", nonCLSException);
        }
        catch (Exception ex)
        {
            var errorId = Marshal.GetLastWin32Error();
            // if (errorId <= 0)
            //     return ex;
            return new Win32Exception(_getErrorMessage(errorId), ex);
        }
    }


    public static bool IsError() => Marshal.GetLastWin32Error() != 0;

    public static Win32Exception? GetLastError()
    {
        var errorCode = Marshal.GetLastWin32Error();
        if (errorCode == 0)
            return null;
        return new Win32Exception(errorCode);
    }

    public static bool TryGetLastError([NotNullWhen(true)] out Win32Exception? error)
    {
        var errorCode = Marshal.GetLastWin32Error();
        if (errorCode == 0)
        {
            error = null;
            return false;
        }
        error = new Win32Exception(errorCode);
        return true;
    }
        
    public static void ThrowLastError()
    {
        if (TryGetLastError(out var error))
            throw error;
    }
        
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern void SetLastError(uint dwErrorCode);

    /// <summary>
    /// Sets the last error code for the calling thread.
    /// </summary>
    /// <returns></returns>
    public static Result ClearLastError() => Result.TryInvoke(() => SetLastError(0));
}