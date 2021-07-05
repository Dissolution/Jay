using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using Jay.Collections;
using Jay.Consolas.Palette;
using Jay.Constraints;
using JetBrains.Annotations;

// ReSharper disable MethodOverloadWithOptionalParameter

namespace Jay.Consolas
{
	/// <summary>
	/// A replacement for System.Console with advanced options.
	/// </summary>
	public class Console
	{
		#region Static
		private static readonly ColorEqualityComparer _colorComparer = new ColorEqualityComparer();

		/// <summary>
		/// Get an instance of the <see cref="Console"/>.
		/// </summary>
		public static Console Instance = new Console();
		#endregion

		#region Fields
		private IPalette _palette;
		private readonly ColorMapper _colorMapper = new ColorMapper();
		#endregion
		
		#region Properties
		/// <summary>
		/// Gets or sets the foreground <see cref="System.Drawing.Color"/> of the <see cref="Console"/>.
		/// </summary>
		public Color ForegroundColor
		{
			get => _palette[System.Console.ForegroundColor];
			set => System.Console.ForegroundColor = _palette[value];
		}

		/// <summary>
		/// Gets or sets the background <see cref="System.Drawing.Color"/> of the <see cref="Console"/>.
		/// </summary>
		public Color BackgroundColor
		{
			get => _palette[System.Console.BackgroundColor];
			set => System.Console.BackgroundColor = _palette[value];
		}

		/// <summary>
		/// Gets or sets the <see cref="IPalette"/> in use for this <see cref="Console"/>.
		/// </summary>
		public IPalette Palette
		{
			get => _palette;
			set
			{
				_palette = value; 
				_colorMapper.SetBufferColors(_palette);
			}
		}
		
		/// <summary>
		/// Options related to the <see cref="Console"/>'s buffer area.
		/// </summary>
		public Buffer Buffer { get; }
		
		/// <summary>
		/// Options related to the <see cref="Console"/>'s cursor.
		/// </summary>
		public Cursor Cursor { get; }

		/// <summary>
		/// Options related to the <see cref="Console"/>'s input.
		/// </summary>
		public Input Input { get; }
		/// <summary>
		/// Options related to the <see cref="Console"/>'s output.
		/// </summary>
		public Output Output { get; }
		/// <summary>
		/// Options related to the <see cref="Console"/>'s error.
		/// </summary>
		public Error Error { get; }

		/// <summary>
		/// Options related to the <see cref="Console"/>'s inner window.
		/// </summary>
		public DisplayWindow DisplayWindow { get; }

		/// <summary>
		/// Options related to the actual <see cref="Console"/> window.
		/// </summary>
		public Window Window { get; }

		/// <summary>
		/// Gets a value indicating whether a key press is available in the input stream.
		/// </summary>
		public bool KeyAvailable => System.Console.KeyAvailable;

		/// <summary>
		/// Gets a value indicating whether CAPS LOCK is toggled on or off.
		/// </summary>
		public bool CapsLock => System.Console.CapsLock;

		/// <summary>
		/// Gets a value indicating whether NUMBER LOCK is toggled on or off.
		/// </summary>
		public bool NumberLock => System.Console.NumberLock;
		#endregion

		#region Events
		/// <summary>
		/// Occurs when the <see cref="ConsoleModifiers.Control"/> modifier key (ctrl) and
		/// either the <see cref="ConsoleKey.C"/> console key (C) or
		/// the Break key are pressed simultaneously (Ctrl+C or Ctrl+Break).
		/// </summary>
		public event ConsoleCancelEventHandler CancelKeyPress;
		#endregion

		#region Constructors
		private Console()
		{
			_palette = Palettes.Default;
			SetColors(_palette.DefaultForeColor, _palette.DefaultBackColor);

			Buffer = new Buffer(this);
			Cursor = new Cursor(this);
			Input = new Input(this);
			Output = new Output(this);
			Error = new Error(this);
			DisplayWindow = new DisplayWindow(this);
			Window = new Window(this);

			System.Console.CancelKeyPress += OnCancelKeyPress;

			Output.Encoding = Encoding.UTF8;
		}
		#endregion
		
		#region Private Methods
		private void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
		{
			CancelKeyPress?.Invoke(sender, e);
		}

		private Color? Resolve(object arg)
		{
			switch (arg)
			{
				case null:
					return null;
				case Color color:
					return color;
				case ConsoleColor consoleColor:
					return _palette[consoleColor];
			}
			return null;
		}

		private void Write(FormattableString text, out int length)
		{
			if (text is null)
			{
				length = 0;
				return;
			}
			var format = text.Format;
			var formatLength = format.Length;
			var args = text.GetArguments();
			var argCount = text.ArgumentCount;

			var startFore = System.Console.ForegroundColor;
			var startBack = System.Console.BackgroundColor;

			var segment = new StringBuilder(formatLength);
			length = 0;
			//Parse
			for (var i = 0; i < formatLength; i++)
			{
				char c = format[i];
				//Check for start of placeholder
				if (c == '{')
				{
					//New segment
					segment.Clear();
					//Find the closing bracket
					for (var j = i + 1; j < formatLength; j++)
					{
						char d = format[j];
						if (d == '}')
						{
							//Should be a number specifying an index
							if (!int.TryParse(segment.ToString(), out int argIndex) || argIndex < 0 || argIndex >= argCount)
								throw new FormatException("Format string is invalid (arguments)");
							var arg = args[argIndex];
							if (arg is Color color)
							{
								SetForeColor(color, startFore);
							}
							else if (arg is ConsoleColor consoleColor)
							{
								SetForeColor(consoleColor);
							}
							else if (arg is ITuple tuple)
							{
								if (tuple.Length >= 1)
								{
									var fore = Resolve(tuple[0]);
									SetForeColor(fore, startFore);
								}

								if (tuple.Length >= 2)
								{
									var back = Resolve(tuple[1]);
									SetBackColor(back, startBack);
								}
							}
							else
							{
								//Just write it, it's a format object
								var argString = arg?.ToString() ?? string.Empty;
								System.Console.Write(argString);
								length += argString.Length;
							}

							//Move ahead
							i = j;
							break;
						}
						else
						{
							//Add to segment
							segment.Append(d);
						}
					}
				}
				else
				{
					//Write this char
					System.Console.Write(c);
					length++;
				}
			}
		}

		private void SetForeColor(Color? color, ConsoleColor defaultColor)
		{
			if (color is null)
				return;
			if (_colorComparer.Equals(color, System.Drawing.Color.Empty))
			{
				System.Console.ForegroundColor = defaultColor;
			}
			else if (!_colorComparer.Equals(color, System.Drawing.Color.Transparent))
			{
				System.Console.ForegroundColor = _palette[color.Value];
			}
		}
		private void SetBackColor(Color? color, ConsoleColor defaultColor)
		{
			if (color is null)
				return;
			if (_colorComparer.Equals(color, System.Drawing.Color.Empty))
			{
				System.Console.BackgroundColor = defaultColor;
			}
			else if (!_colorComparer.Equals(color, System.Drawing.Color.Transparent))
			{
				System.Console.BackgroundColor = _palette[color.Value];
			}
		}
		#endregion

		#region Public Methods
		#region Beep
		/// <summary>
		/// Plays a beep through the console speaker.
		/// </summary>
		/// <returns></returns>
		public Console Beep()
		{
			System.Console.Beep();
			return this;
		}

		/// <summary>
		/// Plays a beep of specified frequency and duration through the console speaker.
		/// </summary>
		/// <param name="frequency"></param>
		/// <param name="duration"></param>
		/// <returns></returns>
		public Console Beep(int frequency, int duration)
		{
			System.Console.Beep(frequency, duration);
			return this;
		}

		/// <summary>
		/// Plays a beep of specified frequency and duration through the console speaker.
		/// </summary>
		/// <param name="frequency"></param>
		/// <param name="duration"></param>
		/// <returns></returns>
		public Console Beep(int frequency, TimeSpan duration)
		{
			System.Console.Beep(frequency, (int)duration.TotalMilliseconds);
			return this;
		}
		#endregion

		#region Clear
		/// <summary>
		/// Clears the <see cref="Console"/> buffer and corresponding <see cref="DisplayWindow"/> of display information.
		/// </summary>
		/// <returns></returns>
		public Console Clear()
		{
			System.Console.Clear();
			return this;
		}

		/// <summary>
		/// Clears the current line the <see cref="Console"/> <see cref="Cursor"/> is on.
		/// </summary>
		/// <returns></returns>
		public Console ClearLine()
		{
			var y = System.Console.CursorTop;
			var x = System.Console.CursorLeft;
			System.Console.SetCursorPosition(0, y);
			System.Console.Write(new string(' ', System.Console.WindowWidth));
			System.Console.SetCursorPosition(x, y);
			return this;
		}
		#endregion

		#region Read
		/// <summary>
		/// Reads the next <see cref="char"/>acter from the standard <see cref="Input"/> stream.
		/// </summary>
		/// <returns></returns>
		public char Read()
		{
			return (char) System.Console.Read();
		}

		/// <summary>
		/// Reads the next <see cref="char"/>acter from the standard <see cref="Input"/> stream.
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		public Console Read(out char c)
		{
			var i = System.Console.Read();
			c = (char) i;
			return this;
		}

		/// <summary>
		/// Reads the next <see cref="char"/>acter or function key pressed by the user.
		/// The pressed key is displayed in the <see cref="Console"/> window.
		/// </summary>
		/// <returns></returns>
		public ConsoleKeyInfo ReadKey()
		{
			return System.Console.ReadKey();
		}

		/// <summary>
		/// Reads the next <see cref="char"/>acter or function key pressed by the user.
		/// The pressed key is displayed in the <see cref="Console"/> window.
		/// </summary>
		/// <param name="consoleKeyInfo"></param>
		/// <returns></returns>
		public Console ReadKey(out ConsoleKeyInfo consoleKeyInfo)
		{
			consoleKeyInfo = System.Console.ReadKey();
			return this;
		}

		/// <summary>
		/// Reads the next <see cref="char"/>acter or function key pressed by the user.
		/// The pressed key is optionally displayed in the <see cref="Console"/> window.
		/// </summary>
		/// <param name="intercept"></param>
		/// <returns></returns>
		public ConsoleKeyInfo ReadKey(bool intercept)
		{
			return System.Console.ReadKey(intercept);
		}

		/// <summary>
		/// Reads the next <see cref="char"/>acter or function key pressed by the user.
		/// The pressed key is optionally displayed in the <see cref="Console"/> window.
		/// </summary>
		/// <param name="intercept"></param>
		/// <param name="consoleKeyInfo"></param>
		/// <returns></returns>
		public Console ReadKey(bool intercept, out ConsoleKeyInfo consoleKeyInfo)
		{
			consoleKeyInfo = System.Console.ReadKey(intercept);
			return this;
		}

		/// <summary>
		/// Reads the next line of characters from the standard <see cref="Input"/> stream.
		/// </summary>
		/// <returns></returns>
		public string ReadLine()
		{
			return System.Console.ReadLine();
		}

		/// <summary>
		/// Reads the next line of characters from the standard <see cref="Input"/> stream.
		/// </summary>
		/// <param name="line"></param>
		/// <returns></returns>
		public Console ReadLine(out string line)
		{
			line = System.Console.ReadLine();
			return this;
		}

		/// <summary>
		/// Reads the next line of characters from the standard <see cref="Input"/> stream.
		/// The pressed keys are optionally displayed in the <see cref="Console"/> window.
		/// </summary>
		/// <param name="intercept"></param>
		/// <param name="line"></param>
		/// <returns></returns>
		public Console ReadLine(bool intercept, out string line)
		{
			if (!intercept)
				return ReadLine(out line);
			var buffer = new StringBuilder();
			while (true)
			{
				var cki = System.Console.ReadKey(true);
				if (cki.Key == ConsoleKey.Enter)
					break;
				buffer.Append(cki.KeyChar);
			}

			line = buffer.ToString();
			return this;
		}

		/// <summary>
		/// Reads the next line of characters from the standard <see cref="Input"/> stream into a <see cref="SecureString"/>, displaying an asterisk (*) instead of the pressed characters.
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		public Console ReadPassword(out SecureString password)
		{
			password = new SecureString();
			while (true)
			{
				var cki = System.Console.ReadKey(true);
				if (cki.Key == ConsoleKey.Enter)
				{
					break;
				}

				if (cki.Key == ConsoleKey.Backspace)
				{
					password.RemoveAt(password.Length - 1);
					System.Console.CursorLeft -= 1;
					System.Console.Write(' ');
					System.Console.CursorLeft -= 1;
				}
				else
				{
					password.AppendChar(cki.KeyChar);
					System.Console.Write('*');
				}
			}
			//Done
			return this;
		}
		#endregion

		#region Color
		/// <summary>
		/// Resets the <see cref="ForegroundColor"/> and <see cref="BackgroundColor"/> to their default <see cref="System.Drawing.Color"/> values.
		/// </summary>
		/// <returns></returns>
		public Console ResetColor()
		{
			System.Console.ForegroundColor = _palette[_palette.DefaultForeColor];
			System.Console.BackgroundColor = _palette[_palette.DefaultBackColor];
			return this;
		}

		/// <summary>
		/// Sets the foreground <see cref="System.Drawing.Color"/> for the <see cref="Console"/>.
		/// </summary>
		/// <param name="foreColor"></param>
		/// <returns></returns>
		public Console SetForeColor(Color foreColor)
		{
			if (_colorComparer.Equals(foreColor, System.Drawing.Color.Empty))
				System.Console.ForegroundColor = _palette[_palette.DefaultForeColor];
			else if (_colorComparer.Equals(foreColor, System.Drawing.Color.Transparent))
				return this;
			else
				System.Console.ForegroundColor = _palette[foreColor];
			return this;
		}

		/// <summary>
		/// Sets the background <see cref="System.Drawing.Color"/> for the <see cref="Console"/>.
		/// </summary>
		/// <param name="backColor"></param>
		/// <returns></returns>
		public Console SetBackColor(Color backColor)
		{
			if (_colorComparer.Equals(backColor, System.Drawing.Color.Empty))
				System.Console.BackgroundColor = _palette[_palette.DefaultBackColor];
			else if (_colorComparer.Equals(backColor, System.Drawing.Color.Transparent))
				return this;
			else
				System.Console.BackgroundColor = _palette[backColor];
			return this;
		}

		/// <summary>
		/// Sets the foreground <see cref="ConsoleColor"/> for the <see cref="Console"/>.
		/// </summary>
		/// <param name="foreColor"></param>
		/// <returns></returns>
		public Console SetForeColor(ConsoleColor foreColor)
		{
			System.Console.ForegroundColor = _palette[_palette[foreColor]];
			return this;
		}

		/// <summary>
		/// Sets the background <see cref="ConsoleColor"/> for the <see cref="Console"/>.
		/// </summary>
		/// <param name="backColor"></param>
		/// <returns></returns>
		public Console SetBackColor(ConsoleColor backColor)
		{
			System.Console.BackgroundColor = _palette[_palette[backColor]];
			return this;
		}

		/// <summary>
		/// Sets the foreground and background <see cref="System.Drawing.Color"/>s for the <see cref="Console"/>.
		/// </summary>
		/// <param name="foreColor"></param>
		/// <param name="backColor"></param>
		/// <returns></returns>
		public Console SetColors(Color? foreColor = null, Color? backColor = null)
		{
			if (foreColor != null)
				SetForeColor(foreColor.Value);
			if (backColor != null)
				SetBackColor(backColor.Value);
			return this;
		}

		/// <summary>
		/// Sets the foreground and background colors for the <see cref="Console"/>.
		/// </summary>
		/// <param name="foreColor"></param>
		/// <param name="backColor"></param>
		/// <returns></returns>
		public Console SetColors(ConsoleColor? foreColor = null, ConsoleColor? backColor = null)
		{
			if (foreColor != null)
				SetForeColor(foreColor.Value);
			if (backColor != null)
				SetBackColor(backColor.Value);
			return this;
		}
		#endregion

		#region Write
		/// <summary>
		/// Writes the text representation of the specified <see cref="bool"/> value to the standard <see cref="Output"/> stream.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Console Write(bool value)
		{
			System.Console.Write(value);
			return this;
		}

		/// <summary>
		/// Writes the specified <see cref="Encoding.Unicode"/> <see cref="char"/> to the standard <see cref="Output"/> stream.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Console Write(char value)
		{
			System.Console.Write(value);
			return this;
		}

		/// <summary>
		/// Writes the specified array of Unicode characters to the standard output stream.
		/// </summary>
		/// <param name="buffer"></param>
		/// <returns></returns>
		public Console Write(char[] buffer)
		{
			System.Console.Write(buffer);
			return this;
		}
		
		/// <summary>
		/// Writes the text representation of the specified <see cref="object"/> to the standard <see cref="Output"/> stream.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Console Write(object value)
		{
			System.Console.Write(value);
			return this;
		}

		/// <summary>
		/// Writes the text representation of the specified <see cref="int"/> value to the standard <see cref="Output"/> stream.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Console Write(int value)
		{
			System.Console.Write(value);
			return this;
		}

		/// <summary>
		/// Writes the text representation of the specified <see cref="uint"/> value to the standard <see cref="Output"/> stream.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Console Write(uint value)
		{
			System.Console.Write(value);
			return this;
		}

		/// <summary>
		/// Writes the text representation of the specified <see cref="long"/> value to the standard <see cref="Output"/> stream.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Console Write(long value)
		{
			System.Console.Write(value);
			return this;
		}

		/// <summary>
		/// Writes the text representation of the specified <see cref="ulong"/> value to the standard <see cref="Output"/> stream.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Console Write(ulong value)
		{
			System.Console.Write(value);
			return this;
		}

		/// <summary>
		/// Writes the text representation of the specified <see cref="float"/> value to the standard <see cref="Output"/> stream.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Console Write(float value)
		{
			System.Console.Write(value);
			return this;
		}

		/// <summary>
		/// Writes the text representation of the specified <see cref="double"/> value to the standard <see cref="Output"/> stream.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Console Write(double value)
		{
			System.Console.Write(value);
			return this;
		}

		/// <summary>
		/// Writes the text representation of the specified <see cref="decimal"/> value to the standard <see cref="Output"/> stream.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Console Write(decimal value)
		{
			System.Console.Write(value);
			return this;
		}

		/// <summary>
		/// Writes the specified <see cref="string"/> value to the standard output stream.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Console Write(NonFormattableString value)
		{
			System.Console.Write(value.Value);
			return this;
		}

		/// <summary>
		/// Writes the specified <see cref="FormattableString"/> value to the standard output stream, using embedded <see cref="System.Drawing.Color"/> values to change the foreground color during the writing process.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public Console Write(FormattableString text)
		{
			Write(text, out _);
			return this;
		}

		/// <summary>
		/// Writes a formatted <see cref="string"/> to the standard output stream.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <returns></returns>
		[StringFormatMethod("format")]
		public Console Write(string format, object arg0)
		{
			System.Console.Write(format, arg0);
			return this;
		}

		/// <summary>
		/// Writes a formatted <see cref="string"/> to the standard output stream.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		/// <returns></returns>
		[StringFormatMethod("format")]
		public Console Write(string format, object arg0, object arg1)
		{
			System.Console.Write(format, arg0, arg1);
			return this;
		}

		/// <summary>
		/// Writes a formatted <see cref="string"/> to the standard output stream.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		/// <param name="arg2"></param>
		/// <returns></returns>
		[StringFormatMethod("format")]
		public Console Write(string format, object arg0, object arg1, object arg2)
		{
			System.Console.Write(format, arg0, arg1, arg2);
			return this;
		}

		/// <summary>
		/// Writes a formatted <see cref="string"/> to the standard output stream.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		/// <param name="arg2"></param>
		/// <param name="arg3"></param>
		/// <returns></returns>
		[StringFormatMethod("format")]
		public Console Write(string format, object arg0, object arg1, object arg2, object arg3)
		{
			System.Console.Write(format, arg0, arg1, arg2, arg3);
			return this;
		}

		/// <summary>
		/// Writes a formatted <see cref="string"/> to the standard output stream.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		[StringFormatMethod("format")]
		public Console Write(NonFormattableString format, params object[] args)
		{
			System.Console.Write(format.Value, args);
			return this;
		}
		#endregion

		#region WriteLine
		/// <summary>
		/// Writes the current line terminator (<see cref="Environment.NewLine"/>) to the standard output stream.
		/// </summary>
		/// <returns></returns>
		public Console WriteLine()
		{
			System.Console.WriteLine();
			return this;
		}

		/// <summary>
		/// Writes the text representation of the specified <see cref="object"/>, followed by the current line terminator, to the standard <see cref="Output"/> stream.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Console WriteLine(object value)
		{
			System.Console.WriteLine(value);
			return this;
		}

		/// <summary>
		/// Writes the text representation of the specified <see cref="bool"/> value, followed by the current line terminator, to the standard <see cref="Output"/> stream.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Console WriteLine(bool value)
		{
			System.Console.WriteLine(value);
			return this;
		}

		/// <summary>
		/// Writes the specified <see cref="Encoding.Unicode"/> <see cref="char"/>, followed by the current line terminator, to the standard <see cref="Output"/> stream.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Console WriteLine(char value)
		{
			System.Console.WriteLine(value);
			return this;
		}

		/// <summary>
		/// Writes the specified array of Unicode characters, followed by the current line terminator, to the standard output stream.
		/// </summary>
		/// <param name="buffer"></param>
		/// <returns></returns>
		public Console WriteLine(char[] buffer)
		{
			System.Console.WriteLine(buffer);
			return this;
		}

		/// <summary>
		/// Writes the text representation of the specified <see cref="int"/> value, followed by the current line terminator, to the standard <see cref="Output"/> stream.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Console WriteLine(int value)
		{
			System.Console.WriteLine(value);
			return this;
		}

		/// <summary>
		/// Writes the text representation of the specified <see cref="uint"/> value, followed by the current line terminator, to the standard <see cref="Output"/> stream.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Console WriteLine(uint value)
		{
			System.Console.WriteLine(value);
			return this;
		}

		/// <summary>
		/// Writes the text representation of the specified <see cref="long"/> value, followed by the current line terminator, to the standard <see cref="Output"/> stream.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Console WriteLine(long value)
		{
			System.Console.WriteLine(value);
			return this;
		}

		/// <summary>
		/// Writes the text representation of the specified <see cref="ulong"/> value, followed by the current line terminator, to the standard <see cref="Output"/> stream.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Console WriteLine(ulong value)
		{
			System.Console.WriteLine(value);
			return this;
		}

		/// <summary>
		/// Writes the text representation of the specified <see cref="float"/> value, followed by the current line terminator, to the standard <see cref="Output"/> stream.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Console WriteLine(float value)
		{
			System.Console.WriteLine(value);
			return this;
		}

		/// <summary>
		/// Writes the text representation of the specified <see cref="double"/> value, followed by the current line terminator, to the standard <see cref="Output"/> stream.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Console WriteLine(double value)
		{
			System.Console.WriteLine(value);
			return this;
		}

		/// <summary>
		/// Writes the text representation of the specified <see cref="decimal"/> value, followed by the current line terminator, to the standard <see cref="Output"/> stream.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Console WriteLine(decimal value)
		{
			System.Console.WriteLine(value);
			return this;
		}

		/// <summary>
		/// Writes the specified <see cref="string"/> value, followed by the current line terminator, to the standard output stream.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Console WriteLine(NonFormattableString value)
		{
			System.Console.WriteLine(value.Value);
			return this;
		}

		/// <summary>
		/// Writes the specified <see cref="FormattableString"/> value, followed by the current line terminator, to the standard output stream, using embedded <see cref="System.Drawing.Color"/> values to change the foreground color during the writing process.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public Console WriteLine(FormattableString text)
		{
			Write(text, out _);
			System.Console.WriteLine();
			return this;
		}

		/// <summary>
		/// Writes a formatted <see cref="string"/>, followed by the current line terminator, to the standard output stream.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <returns></returns>
		public Console WriteLine(string format, object arg0)
		{
			System.Console.WriteLine(format, arg0);
			return this;
		}

		/// <summary>
		/// Writes a formatted <see cref="string"/>, followed by the current line terminator, to the standard output stream.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		/// <returns></returns>
		public Console WriteLine(string format, object arg0, object arg1)
		{
			System.Console.WriteLine(format, arg0, arg1);
			return this;
		}

		/// <summary>
		/// Writes a formatted <see cref="string"/>, followed by the current line terminator, to the standard output stream.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		/// <param name="arg2"></param>
		/// <returns></returns>
		public Console WriteLine(string format, object arg0, object arg1, object arg2)
		{
			System.Console.WriteLine(format, arg0, arg1, arg2);
			return this;
		}

		/// <summary>
		/// Writes a formatted <see cref="string"/>, followed by the current line terminator, to the standard output stream.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		/// <param name="arg2"></param>
		/// <param name="arg3"></param>
		/// <returns></returns>
		public Console WriteLine(string format, object arg0, object arg1, object arg2, object arg3)
		{
			System.Console.WriteLine(format, arg0, arg1, arg2, arg3);
			return this;
		}

		/// <summary>
		/// Writes a formatted <see cref="string"/>, followed by the current line terminator, to the standard output stream.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public Console WriteLine(NonFormattableString format, params object[] args)
		{
			System.Console.WriteLine(format.Value, args);
			return this;
		}
		#endregion

		#region Temporary Actions
		/// <summary>
		/// Stores the current <see cref="Console"/> <see cref="Color"/>s and <see cref="Cursor"/> positions, performs an <see cref="Action"/> on this <see cref="Console"/>, then restores the <see cref="Color"/>s and <see cref="Cursor"/> position.
		/// </summary>
		/// <param name="tempAction"></param>
		/// <returns></returns>
		public Console Temp(Action<Console>? tempAction)
		{
			if (tempAction is null)
				return this;
			//Store temp vars
			var oldForeColor = System.Console.ForegroundColor;
			var oldBackColor = System.Console.BackgroundColor;
			var oldCursorLeft = System.Console.CursorLeft;
			var oldCursorTop = System.Console.CursorTop;
			//Do the temp action
			tempAction(this);
			//Reset temp vars
			System.Console.ForegroundColor = oldForeColor;
			System.Console.BackgroundColor = oldBackColor;
			System.Console.CursorLeft = oldCursorLeft;
			System.Console.CursorTop = oldCursorTop;
			//Fin
			return this;
		}

		/// <summary>
		/// Stores the current <see cref="Console"/> <see cref="Color"/>s, performs an <see cref="Action"/> on this <see cref="Console"/>, then restores the <see cref="Color"/>s.
		/// </summary>
		/// <param name="tempAction"></param>
		/// <returns></returns>
		public Console TempColor(Action<Console> tempAction)
		{
			if (tempAction is null)
				return this;
			//Store temp vars
			var oldForeColor = System.Console.ForegroundColor;
			var oldBackColor = System.Console.BackgroundColor;
			//Do the temp action
			tempAction(this);
			//Reset temp vars
			System.Console.ForegroundColor = oldForeColor;
			System.Console.BackgroundColor = oldBackColor;
			//Fin
			return this;
		}

		/// <summary>
		/// Stores the current <see cref="Console"/> <see cref="Cursor"/> position, performs an <see cref="Action"/> on this <see cref="Console"/>, then restores the <see cref="Cursor"/> position.
		/// </summary>
		/// <param name="tempAction"></param>
		/// <returns></returns>
		public Console TempPosition(Action<Console> tempAction)
		{
			if (tempAction is null)
				return this;
			//Store temp vars
			var oldCursorLeft = System.Console.CursorLeft;
			var oldCursorTop = System.Console.CursorTop;
			//Do the temp action
			tempAction(this);
			//Reset temp vars
			System.Console.CursorLeft = oldCursorLeft;
			System.Console.CursorTop = oldCursorTop;
			//Fin
			return this;
		}

		/// <summary>
		/// Gets an <see cref="IDisposable"/> that, when disposed, resets the <see cref="Console"/>'s colors back to what they were when the <see cref="ColorLock"/> was taken.
		/// </summary>
		public IDisposable ColorLock
		{
			get
			{
				//Store temp vars
				var oldForeColor = System.Console.ForegroundColor;
				var oldBackColor = System.Console.BackgroundColor;
				return Disposable.Action(() =>
				{
					//Reset temp vars
					System.Console.ForegroundColor = oldForeColor;
					System.Console.BackgroundColor = oldBackColor;
				});
			}
		}

		/// <summary>
		/// Gets an <see cref="IDisposable"/> that, when disposed, resets the <see cref="Console"/>'s cursor back to where it was when the <see cref="ColorLock"/> was taken.
		/// </summary>
		public IDisposable CursorLock
		{
			get
			{
				//Store temp vars
				var oldCursorLeft = System.Console.CursorLeft;
				var oldCursorTop = System.Console.CursorTop;
				return Disposable.Action(() =>
				{
					//Reset temp vars
					System.Console.CursorLeft = oldCursorLeft;
					System.Console.CursorTop = oldCursorTop;
				});
			}
		}
		#endregion
		#endregion
	}
}
