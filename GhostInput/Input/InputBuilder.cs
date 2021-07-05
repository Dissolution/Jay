using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Jay.GhostInput.Native;
using Jay.GhostInput.Native.Structs;

namespace Jay.GhostInput.Input
{
	/// <summary>
	/// A builder of Input Operations
	/// </summary>
	public sealed class InputBuilder
	{
		private readonly InputSettings _settings;
		private readonly List<object> _commands = new List<object>(4);
		private Point _snapback;

		public InputBuilder Snapback
		{
			get
			{
				_snapback = Input.MousePosition;
				return this;
			}
		}

		public InputBuilder(InputSettings settings)
		{
			_settings = settings;
		}

		#region Public Methods
		#region Blocking
		/// <summary>
		/// Starts input blocking
		/// </summary>
		/// <returns></returns>
		public InputBuilder Block()
		{
			_commands.Add(typeof(InputBlocker));
			return this;
		}

		public InputBuilder Unblock()
		{
			_commands.Add(typeof(IDisposable));
			return this;
		}
		#endregion
		#region Mouse
		#region Movement
		/// <summary>
		/// Move the mouse cursor to the specified <see cref="POINT"/>.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public InputBuilder Move(Point p)
		{
			return Move(p.X, p.Y);
		}

		/// <summary>
		/// Move the mouse cursor to the specified x and y coordinates.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public InputBuilder Move(double x, double y)
		{
			_commands.Add(InputFactory.Mouse.Move(x, y));
			return this;
		}

		public InputBuilder Slide(Point p, TimeSpan duration, int increments = 10)
		{
			var startPos = NativeMethods.GetCursorPosition();
			var xDiff = Math.Abs(startPos.X - p.X);
			var xIncrement = xDiff / (double)increments;
			var yDiff = Math.Abs(startPos.Y - p.Y);
			var yIncrement = yDiff / (double)increments;
			var dIncrement = duration.MultiplyBy(1d / increments);
			for (var i = 0; i < increments-1; i++)
			{
				var newX = startPos.X + (xIncrement * i + 1);
				var newY = startPos.Y + (yIncrement * (i + 1));
				_commands.Add(InputFactory.Mouse.Move(newX, newY));
				_commands.Add(dIncrement);
			}
			_commands.Add(InputFactory.Mouse.Move(p.X, p.Y));
			return this;
		}

		#endregion
		#region Click
		public InputBuilder MouseDown(MouseButton button)
		{
			_commands.Add(InputFactory.Mouse.ButtonDown(button));
			return this;
		}

		public InputBuilder MouseUp(MouseButton button)
		{
			_commands.Add(InputFactory.Mouse.ButtonUp(button));
			return this;
		}

		public InputBuilder MouseClick(MouseButton button)
		{
			_commands.AddRange<object>(
				InputFactory.Mouse.ButtonDown(button),
				InputFactory.Mouse.ButtonUp(button));
			return this;
		}

		public InputBuilder MouseDoubleClick(MouseButton button) => MouseClick(button).MouseClick(button);
		public InputBuilder DoubleClick(MouseButton button) => MouseDoubleClick(button);
		public InputBuilder Click(MouseButton button) => MouseClick(button);
		public InputBuilder LeftClick() => MouseClick(MouseButton.Left);
		public InputBuilder RightClick() => MouseClick(MouseButton.Right);
		#endregion
		#region Scroll

		public InputBuilder ScrollVertical(int scrollAmount)
		{
			_commands.Add(InputFactory.Mouse.ScrollVertical(scrollAmount));
			return this;
		}
		public InputBuilder ScrollHorizontal(int scrollAmount)
		{
			_commands.Add(InputFactory.Mouse.ScrollHorizontal(scrollAmount));
			return this;
		}
		#endregion
		#endregion
		#region Keyboard
		/// <summary>
		/// Press the specified <see cref="Key"/> down.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public InputBuilder KeyDown(Key key)
		{
			_commands.Add(InputFactory.Keyboard.KeyDown(key));
			return this;
		}

		/// <summary>
		/// Press the specified key(s) down
		/// </summary>
		/// <param name="keys">The key(s) to press down</param>
		/// <returns></returns>
		public InputBuilder KeyDown(params Key[] keys)
		{
			if (keys is null || keys.Length == 0)
				return this;
			foreach (var key in keys)
			{
				_commands.Add(InputFactory.Keyboard.KeyDown(key));
			}
			return this;
		}

		/// <summary>
		/// Press the specified key(s) down
		/// </summary>
		/// <param name="keys">The key(s) to press down</param>
		/// <returns></returns>
		public InputBuilder KeyDown(IEnumerable<Key> keys)
		{
			if (keys is null)
				return this;
			foreach (var key in keys)
			{
				_commands.Add(InputFactory.Keyboard.KeyDown(key));
			}
			return this;
		}

		/// <summary>
		/// Press the specified key down
		/// </summary>
		/// <param name="c">The character key to press down</param>
		/// <returns></returns>
		public InputBuilder KeyDown(char c)
		{
			_commands.Add(InputFactory.Keyboard.KeyDown(c));
			return this;
		}

		/// <summary>
		/// Release the specified <see cref="Key"/>.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public InputBuilder KeyUp(Key key)
		{
			_commands.Add(InputFactory.Keyboard.KeyUp(key));
			return this;
		}

		/// <summary>
		/// Release the specified key(s)
		/// </summary>
		/// <param name="keys">The key(s) to release</param>
		/// <returns></returns>
		public InputBuilder KeyUp(params Key[] keys)
		{
			if (keys is null || keys.Length == 0)
				return this;
			foreach (var key in keys)
			{
				_commands.Add(InputFactory.Keyboard.KeyUp(key));
			}
			return this;
		}
		
		/// <summary>
		/// Release the specified key(s)
		/// </summary>
		/// <param name="keys">The key(s) to release</param>
		/// <returns></returns>
		public InputBuilder KeyUp(IEnumerable<Key> keys)
		{
			if (keys is null)
				return this;
			foreach (var key in keys)
			{
				_commands.Add(InputFactory.Keyboard.KeyUp(key));
			}
			return this;
		}

		/// <summary>
		/// Release the specified character key
		/// </summary>
		/// <param name="c">The character key to release</param>
		/// <returns></returns>
		public InputBuilder KeyUp(char c)
		{
			_commands.Add(InputFactory.Keyboard.KeyUp(c));
			return this;
		}

		/// <summary>
		/// Press the specified key(s)
		/// </summary>
		/// <param name="keys">The key(s) to press and release</param>
		/// <returns></returns>
		public InputBuilder KeyPress(params Key[] keys)
		{
			if (keys.NullOrNone())
				return this;
			foreach (var key in keys)
			{
				_commands.AddRange<object>(
					InputFactory.Keyboard.KeyDown(key),
					InputFactory.Keyboard.KeyUp(key));
			}
			return this;
		}

		/// <summary>
		/// Press the specified key(s)
		/// </summary>
		/// <param name="keys">The key(s) to press and release</param>
		/// <returns></returns>
		public InputBuilder KeyPress(IEnumerable<Key> keys)
		{
			if (keys is null)
				return this;
			foreach (var key in keys)
			{
				_commands.AddRange<object>(
				                           InputFactory.Keyboard.KeyDown(key),
				                           InputFactory.Keyboard.KeyUp(key));
			}
			return this;
		}

		/// <summary>
		/// Press the specified character
		/// </summary>
		/// <param name="c">The character whos key we will press and release</param>
		/// <returns></returns>
		public InputBuilder KeyPress(char c)
		{
			_commands.AddRange<object>(
			                           InputFactory.Keyboard.KeyDown(c),
			                           InputFactory.Keyboard.KeyUp(c));
			return this;
		}

		/// <summary>
		/// Type the specified text
		/// </summary>
		/// <param name="text">The text to type, one character at a time</param>
		/// <returns></returns>
		public InputBuilder Type(string text)
		{
			if (text.IsNullOrEmpty())
				return this;
			foreach (var c in text)
			{
				KeyPress(c);
			}
			return this;
		}

		/// <summary>
		/// Perform the specified keystroke
		/// </summary>
		/// <param name="modifierKey">Modifier key to hold down</param>
		/// <param name="key">Key to press while modifier is held down</param>
		/// <returns></returns>
		public InputBuilder KeyStroke(Key modifierKey, Key key)
		{
			return KeyDown(modifierKey).KeyPress(key).KeyUp(modifierKey);
		}

		/// <summary>
		/// Perform the specified keystroke
		/// </summary>
		/// <param name="modifierKeys">Modifier keys to hold down (in order)</param>
		/// <param name="key">Key to press while modifiers are held down</param>
		/// <returns></returns>
		public InputBuilder KeyStroke(IEnumerable<Key> modifierKeys, Key key)
		{
			var modArray = modifierKeys as Key[] ?? modifierKeys.ToArray();
			return KeyDown(modArray).KeyPress(key).KeyUp(modArray);
		}

		/// <summary>
		/// Perform the specified keystroke
		/// </summary>
		/// <param name="modiferKey">Modifier key to hold down</param>
		/// <param name="keys">Key to press (in order) while modifier is held down</param>
		/// <returns></returns>
		public InputBuilder KeyStroke(Key modiferKey, IEnumerable<Key> keys)
		{
			var keyArray = keys as Key[] ?? keys.ToArray();
			return KeyDown(modiferKey).KeyPress(keyArray).KeyUp(modiferKey);
		}

		/// <summary>
		/// Perform the specified keystroke
		/// </summary>
		/// <param name="modifierKeys">Modifier keys to hold down (in order)</param>
		/// <param name="keys">Key to press (in order) while modifier is held down</param>
		/// <returns></returns>
		public InputBuilder KeyStroke(IEnumerable<Key> modifierKeys, IEnumerable<Key> keys)
		{
			var modArray = modifierKeys as Key[] ?? modifierKeys.ToArray();
			var keyArray = keys as Key[] ?? keys.ToArray();
			return KeyDown(modArray).KeyPress(keyArray).KeyUp(modArray);
		}
		#endregion
		#region Wait
		/// <summary>
		/// Wait for the specified timespan
		/// </summary>
		/// <param name="timeSpan"></param>
		/// <returns></returns>
		public InputBuilder Wait(TimeSpan timeSpan)
		{
			if (timeSpan <= TimeSpan.Zero)
				return this;
			_commands.Add(timeSpan);
			return this;
		}

		/// <summary>
		/// Wait for the specified number of milliseconds
		/// </summary>
		/// <param name="milliseconds"></param>
		/// <returns></returns>
		public InputBuilder Wait(int milliseconds)
		{
			if (milliseconds <= 0)
				return this;
			_commands.Add(TimeSpan.FromMilliseconds(milliseconds));
			return this;
		}
		#endregion
		#region Clipboard
		/// <summary>
		/// Copies any selected text (Ctrl + C)
		/// </summary>
		/// <returns></returns>
		public InputBuilder Copy(out Box<string> text)
		{
			text = new Box<string>(null);
			_commands.Add(text);
			return this;
		}

		/// <summary>
		/// Paste the specified text (Ctrl + V)
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public InputBuilder Paste(string text)
		{
			_commands.Add(text);
			return this;
		}
		#endregion
		#region Actions + Functions
		/// <summary>
		/// Invoke the specified Action
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		public InputBuilder Action(Action action)
		{
			_commands.Add(action);
			return this;
		}
		#endregion

		/// <summary>
		/// Execute all input actions stored in this <see cref="InputBuilder"/>.
		/// </summary>
		/// <returns></returns>
		public Result Execute()
		{
			var exceptions = new List<Exception>(0);
			var inputs = new List<INPUT>(1);
			var blocks = new List<IDisposable>(0);
			void Flush()
			{
				var sent = NativeMethods.SendInput(inputs.ToArray());
				if (sent.TryGetError(out var ex))
				{
					exceptions.Add(ex);
				}
				inputs.Clear();
			}

			lock (Input.LOCK)
			{
				//Check settings

				//Block
				if (_settings.Block)
					blocks.Add(Input.Block);
				//Snapback
				if (_snapback.X != 0 || _snapback.Y != 0)
					Move(_snapback);

				//Process each command in a row
				for (var i = 0; i < _commands.Count; i++)
				{
					var command = _commands[i];

					//Delay between actions?
					if (i > 0 && _settings.InputDelay != TimeSpan.Zero)
					{
						Flush();
						//TODO: FIXME
						//var randomTime = Rand.Between(_settings.InputDelay.Minimum, _settings.InputDelay.Maximum);
						//Jay.Wait.For(randomTime);
					}

					//INPUT structure?
					if (command is INPUT input)
					{
						inputs.Add(input);
						continue;
					}

					//Flush any non-INPUTS
					Flush();

					//Wait
					if (command is TimeSpan delay)
					{
						Thread.Sleep(delay);
						continue;
					}
					//Block
					if (command is Type type)
					{
						if (type == typeof(InputBlocker))
						{
							blocks.Add(new InputBlocker());
						}
						else if (type == typeof(IDisposable))
						{
							var len = blocks.Count - 1;
							Result.Dispose(blocks[len]);
							blocks.RemoveAt(len);
						}
						continue;
					}
					//Dispose block
					if (command.Equals(typeof(IDisposable)))
					{
						Unblock();
						continue;
					}
					////Copy
					//if (command is Ref<string> rf)
					//{
					//	rf.Value = Input.Copy();
					//	continue;
					//}
					////Paste
					//if (command is string str)
					//{
					//	Input.Paste(str);
					//	continue;
					//}
					//Action
					if (command is Action action)
					{
						var result = Result.Try(action);
						if (result.TryGetError(out var exception))
							exceptions.Add(exception);
						continue;
					}

					//Unknown

					exceptions.Add(new InvalidOperationException($"Unknown Input Action: {command}"));
				}

				//Flush anything left
				Flush();

				//Cleanup?
				blocks.ForEach(b => Result.Dispose(b));

				//Return our result
				if (exceptions.Count == 0)
					return true;
				if (exceptions.Count == 1)
					return exceptions[0];
				return new AggregateException(exceptions);
			}
		}
		#endregion
	}
}
