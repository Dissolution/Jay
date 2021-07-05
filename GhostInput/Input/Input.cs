using System;
using System.Windows;
using System.Windows.Input;
using Jay.GhostInput.Native;
using Point = System.Drawing.Point;

namespace Jay.GhostInput.Input
{
	/// <summary>
	/// Utility for all Input operations.
	/// </summary>
	public static class Input
	{
		/// <summary>
		/// Lock for all Input operations, keeps all inputs thread-safe
		/// </summary>
		internal static readonly object LOCK = new object();

		/// <summary>
		/// Get an <see cref="IDisposable"/> object that blocks input until the last Block has been disposed.
		/// </summary>
		public static IDisposable Block => new InputBlocker();

		/// <summary>
		/// Default <see cref="InputSettings"/> that all <see cref="InputBuilder"/>s inherit.
		/// </summary>
		public static InputSettings DefaultSettings { get; }

		/// <summary>
		/// Gets the current Mouse Position.
		/// </summary>
		public static Point MousePosition => NativeMethods.GetCursorPosition();
		
		/// <summary>
		/// Gets a new <see cref="InputBuilder"/> inheriting our <see cref="InputSettings"/>.
		/// </summary>
		public static InputBuilder Builder => new InputBuilder(DefaultSettings);

		static Input()
		{
			DefaultSettings = new InputSettings();
		}

		/// <summary>
		/// Execute a Copy Command (Control+C) and return whatever text is on the <see cref="Clipboard"/>.
		/// </summary>
		/// <returns></returns>
		public static string Copy()
		{
			lock (LOCK)
			{
				Result.Try(Clipboard.Clear);
				var executed = Builder.KeyStroke(Key.LeftCtrl, Key.C).Execute();
				if (!executed)
					return string.Empty;
				return Result.Swallow(() => Clipboard.GetText(TextDataFormat.Text), string.Empty)!;
			}
		}

		/// <summary>
		/// Puts the specified text onto the <see cref="Clipboard"/> and execute the Pase Command (Control+V)
		/// </summary>
		/// <param name="text"></param>
		public static Result Paste(string text)
		{
			lock (LOCK)
			{
				var set = Result.Try(() => Clipboard.SetText(text));
				if (set)
					return Builder.KeyStroke(Key.LeftCtrl, Key.V).Execute();
				return set;

			}
		}

		/// <summary>
		/// Sets the current mouse cursor and returns an <see cref="IDisposable"/> that, when disposed, returns the cursor to its original state.
		/// </summary>
		/// <param name="cursor"></param>
		/// <returns></returns>
		public static IDisposable SetCursor(Cursor cursor)
		{
			var oldCursor = Mouse.OverrideCursor;
			if (Mouse.SetCursor(cursor))
				return Disposable.Action(() => Mouse.SetCursor(oldCursor));
			return Disposable.None;
		}

		/// <summary>
		/// Execute an <see cref="InputBuilder"/> fluently.
		/// </summary>
		/// <param name="build"></param>
		/// <returns></returns>
		public static Result Execute(Action<InputBuilder> build)
		{
			var builder = new InputBuilder(DefaultSettings);
			build(builder);
			return builder.Execute();
		}
	}
}
