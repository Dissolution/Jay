using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Jay.GhostInput.Hotkey
{
	/// <summary>
	/// Q global hotkey hook utility
	/// </summary>
	public sealed class GlobalHotkey : IDisposable
	{
		private const int WM_HOTKEY = 0x0312;
		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool RegisterHotKey(IntPtr hWnd, int id, int keyModifier, int key);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		private Application _app;
		private readonly Dictionary<Hotkey, Action> _hotkeyActions;

		public GlobalHotkey()
		{
			_hotkeyActions = new Dictionary<Hotkey, Action>();
			using (var waitForStartup = new ManualResetEvent(false))
			{

				Task.Run(() =>
				{
					ComponentDispatcher.ThreadPreprocessMessage += OnThreadPreProcessMessage;

					_app = new Application();
					_app.Startup += (s, e) => waitForStartup.Set();
					_app.Run();
				});
				waitForStartup.WaitOne();
			}
		}

		private void OnThreadPreProcessMessage(ref MSG msg, ref bool handled)
		{
			//Only looking for HOTKEY messages
			if (msg.message != WM_HOTKEY)
				return;

			//Get our Hotkey
			var key = KeyInterop.KeyFromVirtualKey(((int)msg.lParam >> 16) & 0xFFFF);
			var keyModifier = (ModifierKeys)((int)msg.lParam & 0xFFFF);
			var hotKey = new Hotkey(keyModifier, key);
			//If it is registered, fire the action
			if (_hotkeyActions.TryGetValue(hotKey, out Action action))
				action();
		}

		/// <summary>
		/// Registers a <see cref="Hotkey"/> press to perform the specified <see cref="Action"/>.
		/// </summary>
		/// <param name="hotkey"></param>
		/// <param name="action"></param>
		public void Add(Hotkey hotkey, Action action)
		{
			//Convert to our int values
			var keyModifier = (int)hotkey.KeyModifier;
			var key = KeyInterop.VirtualKeyFromKey(hotkey.Key);

			//Register via dispatcher
			_app.Dispatcher.Invoke(() =>
			{
				if (!RegisterHotKey(IntPtr.Zero, hotkey.GetHashCode(), keyModifier, key))
					throw new Win32Exception(Marshal.GetLastWin32Error());
			});

			//Assuming it worked, now we have it registered
			_hotkeyActions.Add(hotkey, action);
		}

		/// <summary>
		/// Remove a registered <see cref="Hotkey"/> <see cref="Action"/>.
		/// </summary>
		/// <param name="hotkey"></param>
		public void Remove(Hotkey hotkey)
		{
			if (_hotkeyActions.Remove(hotkey))
			{
				_app.Dispatcher.Invoke(() =>
				{
					if (!UnregisterHotKey(IntPtr.Zero, hotkey.GetHashCode()))
						throw new Win32Exception(Marshal.GetLastWin32Error());
				});
			}
		}

		///<inheritdoc/>
		public void Dispose()
		{
			var registeredHotKeys = _hotkeyActions.Keys;
			foreach (var hotkey in registeredHotKeys)
				Remove(hotkey);
			_app.Dispatcher.InvokeShutdown();
			_app.Shutdown();

		}

		
	}
}
