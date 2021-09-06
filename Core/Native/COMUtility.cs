using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Jay.Native
{
	/// <summary>
	/// Provides helper methods for working with COM IDispatch objects that have a registered type library.
	/// </summary>
	/// <seealso cref="https://www.codeproject.com/Articles/523417/Reflection-with-IDispatch-based-COM-objects"/>
	public static class COMUtility
	{
		/// <summary>
		/// From WinError.h
		/// </summary>
		private const int S_OK = 0;

		/// <summary>
		/// From WinNT.h == 2048 == 0x800
		/// </summary>
		private const int LOCALE_SYSTEM_DEFAULT = 2 << 10;

	
		/// <summary>
		/// Gets a Type that can be used with reflection.
		/// </summary>
		/// <param name="dispatch">An object that implements IDispatch.</param>
		/// <param name="throwIfNotFound">Whether an exception should be thrown if a Type can't be obtained.</param>
		/// <returns>A .NET Type that can be used with reflection.</returns>
		/// <exception cref="TypeLoadException"></exception>
		private static Type? GetType(IDispatch? dispatch, bool throwIfNotFound = true)
		{
			if (dispatch is null) return null;

			try
			{
				Type? result = null;
				int hResult = dispatch.GetTypeInfoCount(out var typeInfoCount);
				if (hResult == S_OK && typeInfoCount > 0)
				{
					// Type info isn't usually culture-aware for IDispatch, so we might as well pass
					// the default locale instead of looking up the current thread's LCID each time
					// (via CultureInfo.CurrentCulture.LCID).
					dispatch.GetTypeInfo(0, LOCALE_SYSTEM_DEFAULT, out result);
				}

				if (result is null && throwIfNotFound)
				{
					// If the GetTypeInfoCount called failed, throw an exception for that.
					Marshal.ThrowExceptionForHR(hResult);

					// Otherwise, throw the same exception that Type.GetType would throw.
					throw new TypeLoadException();
				}

				return result;
			}
			catch (Exception)
			{
				if (throwIfNotFound)
					throw;
				return null;
			}
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
		//[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public static Type? GetType(object? obj, bool throwIfNotFound)
		{
			if (obj is IDispatch dispatchInfo)
			{
				return GetType(dispatchInfo, throwIfNotFound);
			}
			if (throwIfNotFound)
				throw new ArgumentException("Object is not IDispatch", nameof(obj));
			return null;
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
		//[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public static bool TryGetDispId(object? obj, string name, out int dispId)
		{
			if (obj is IDispatch dispatchInfo)
			{
				return TryGetDispId(dispatchInfo, name, out dispId);
			}
			dispId = default;
			return false;
		}

		/// <summary>
		/// Invokes a member by DISPID.
		/// </summary>
		/// <param name="obj">An object that implements IDispatch.</param>
		/// <param name="dispId">The DISPID of a member.  This can be obtained using
		/// <see cref="TryGetDispId(object, string, out int)"/>.</param>
		/// <param name="args">The arguments to pass to the member.</param>
		/// <returns>The member's return value.</returns>
		/// <remarks>
		/// This can invoke a method or a property get accessor.
		/// </remarks>
		public static object? Invoke(object? obj, int dispId, params object[] args)
		{
			string memberName = $"[DispId={dispId}]";
			object? result = Invoke(obj, memberName, args);
			return result;
		}

		/// <summary>
		/// Invokes a member by name.
		/// </summary>
		/// <param name="obj">An object.</param>
		/// <param name="memberName">The name of the member to invoke.</param>
		/// <param name="args">The arguments to pass to the member.</param>
		/// <returns>The member's return value.</returns>
		/// <remarks>
		/// This can invoke a method or a property get accessor.
		/// </remarks>
		public static object? Invoke(object? obj, string memberName, params object[] args)
		{
			Type type = obj?.GetType() ?? throw new ArgumentNullException(nameof(obj));
			object? result = type.InvokeMember(memberName, 
			                                   BindingFlags.InvokeMethod | BindingFlags.GetProperty | BindingFlags.IgnoreCase,
			                                   null, 
			                                   obj, 
			                                   args, 
			                                   null);
			return result;
		}


		#region Private Methods
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
			int hr = dispatch.GetDispId(ref iidNull, ref name, 1, LOCALE_SYSTEM_DEFAULT, out dispId);

			const int DISP_E_UNKNOWNNAME = unchecked((int)0x80020006);	//From WinError.h
			const int DISPID_UNKNOWN = -1;								//From OAIdl.idl
			
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
#endregion

		#region Interfaces
		/// <summary>
		/// A partial declaration of IDispatch used to lookup Type information and DISPIDs.
		/// </summary>
		/// <remarks>
		/// This interface only declares the first three methods of IDispatch.  It omits the
		/// fourth method (Invoke) because there are already plenty of ways to do dynamic
		/// invocation in .NET.  But the first three methods provide dynamic type metadata
		/// discovery, which .NET doesn't provide normally if you have a System.__ComObject
		/// RCW instead of a strongly-typed RCW.
		/// <para/>
		/// Note: The original declaration of IDispatch is in OAIdl.idl.
		/// </remarks>
		[ComImport]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[Guid("00020400-0000-0000-C000-000000000046")]
		private interface IDispatch
		{
			/// <summary>
			/// Gets the number of Types that the object provides (0 or 1).
			/// </summary>
			/// <param name="typeInfoCount">Returns 0 or 1 for the number of Types provided by <see cref="GetTypeInfo"/>.</param>
			/// <remarks>
			/// http://msdn.microsoft.com/en-us/library/da876d53-cb8a-465c-a43e-c0eb272e2a12(VS.85)
			/// </remarks>
			[PreserveSig]
			int GetTypeInfoCount(out int typeInfoCount);

			/// <summary>
			/// Gets the Type information for an object if <see cref="GetTypeInfoCount"/> returned 1.
			/// </summary>
			/// <param name="typeInfoIndex">Must be 0.</param>
			/// <param name="lcid">Typically, LOCALE_SYSTEM_DEFAULT (2048).</param>
			/// <param name="typeInfo">Returns the object's Type information.</param>
			/// <remarks>
			/// http://msdn.microsoft.com/en-us/library/cc1ec9aa-6c40-4e70-819c-a7c6dd6b8c99(VS.85)
			/// </remarks>
			void GetTypeInfo(int typeInfoIndex, int lcid, 
			                 [MarshalAs(UnmanagedType.CustomMarshaler,
			                            MarshalTypeRef = typeof(System.Runtime.InteropServices.CustomMarshalers.TypeToTypeInfoMarshaler))] 
			                 out Type typeInfo);

			/// <summary>
			/// Gets the DISPID of the specified member name.
			/// </summary>
			/// <param name="riid">Must be IID_NULL.  Pass a copy of Guid.Empty.</param>
			/// <param name="name">The name of the member to look up.</param>
			/// <param name="nameCount">Must be 1.</param>
			/// <param name="lcid">Typically, LOCALE_SYSTEM_DEFAULT (2048).</param>
			/// <param name="dispId">If a member with the requested <paramref name="name"/>
			/// is found, this returns its DISPID and the method's return value is 0.
			/// If the method returns a non-zero value, then this parameter's output value is
			/// undefined.</param>
			/// <returns>Zero for success. Non-zero for failure.</returns>
			/// <remarks>
			/// http://msdn.microsoft.com/en-us/library/6f6cf233-3481-436e-8d6a-51f93bf91619(VS.85)
			/// </remarks>
			[PreserveSig]
			int GetDispId(ref Guid riid, ref string name, int nameCount, int lcid, out int dispId);

			// NOTE: The real IDispatch also has an Invoke method next, but we don't need it.
			// We can invoke methods using .NET's Type.InvokeMember method with the special
			// [DISPID=n] syntax for member "names", or we can get a .NET Type using GetTypeInfo
			// and invoke methods on that through reflection.
			// Type.InvokeMember: http://msdn.microsoft.com/en-us/library/de3dhzwy.aspx
		}
#endregion
	}
}