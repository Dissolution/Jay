/*
    #region Properties
    /// <summary>
    /// Gets or sets the foreground <see cref="System.Drawing.Color"/> of the <see cref="Terminal"/>.
    /// </summary>
    public static Color ForegroundColor
    {
        get => _instance.ForegroundColor;
        set => _instance.BackgroundColor = value;
    }

    /// <summary>
    /// Gets or sets the background <see cref="System.Drawing.Color"/> of the <see cref="Terminal"/>.
    /// </summary>
    public static Color BackgroundColor
    {
        get => _instance.BackgroundColor;
        set => _instance.BackgroundColor = value;
    }

    /// <summary>
    /// Gets or sets the <see cref="IPalette"/> in use for this <see cref="Terminal"/>.
    /// </summary>
    public static IPalette Palette
    {
        get => _instance.Palette;
        set => _instance.Palette = value;
    }

    /// <summary>
    /// Options related to the <see cref="Terminal"/>'s buffer area.
    /// </summary>
    public static TerminalBuffer Buffer => _instance.Buffer;

    /// <summary>
    /// Options related to the <see cref="Terminal"/>'s cursor.
    /// </summary>
    public static TerminalCursor Cursor => _instance.Cursor;

    /// <summary>
    /// Options related to the <see cref="Terminal"/>'s input.
    /// </summary>
    public static TerminalInput Input => _instance.Input;

    /// <summary>
    /// Options related to the <see cref="Terminal"/>'s output.
    /// </summary>
    public static TerminalOutput Output => _instance.Output;

    /// <summary>
    /// Options related to the <see cref="Terminal"/>'s error.
    /// </summary>
    public static TerminalError Error => _instance.Error;

    /// <summary>
    /// Options related to the <see cref="Terminal"/>'s inner window.
    /// </summary>
    public static TerminalDisplayWindow DisplayWindow => _instance.DisplayWindow;

    /// <summary>
    /// Options related to the actual <see cref="Terminal"/> window.
    /// </summary>
    public static TerminalWindow Window => _instance.Window;

    /// <summary>
    /// Gets a value indicating whether a key press is available in the input stream.
    /// </summary>
    public static bool KeyAvailable => _instance.KeyAvailable;

    /// <summary>
    /// Gets a value indicating whether CAPS LOCK is toggled on or off.
    /// </summary>
    public static bool CapsLock => _instance.CapsLock;

    /// <summary>
    /// Gets a value indicating whether NUMBER LOCK is toggled on or off.
    /// </summary>
    public static bool NumberLock => _instance.NumberLock;
    #endregion

    #region Events
    /// <summary>
    /// Occurs when the <see cref="ConsoleModifiers.Control"/> modifier key (ctrl) and
    /// either the <see cref="ConsoleKey.C"/> console key (C) or
    /// the Break key are pressed simultaneously (Ctrl+C or Ctrl+Break).
    /// </summary>
    public static event ConsoleCancelEventHandler? CancelKeyPress;
    #endregion

    static Terminal()
    {
        _instance = new TerminalInstance();
        _instance.CancelKeyPress += (sender, args) => CancelKeyPress?.Invoke(sender, args);
    }

    #region Public Methods
    #region Beep

    /// <summary>
    /// Plays a beep through the console speaker.
    /// </summary>
    /// <returns></returns>
    public static TerminalInstance Beep() => _instance.Beep();

    /// <summary>
    /// Plays a beep of specified frequency and duration through the console speaker.
    /// </summary>
    /// <param name="frequency"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public static TerminalInstance Beep(int frequency, int duration) => _instance.Beep(frequency, duration);

    /// <summary>
    /// Plays a beep of specified frequency and duration through the console speaker.
    /// </summary>
    /// <param name="frequency"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public static TerminalInstance Beep(int frequency, TimeSpan duration) => _instance.Beep(frequency, duration);
    #endregion

    #region Clear

    /// <summary>
    /// Clears the <see cref="Terminal"/> buffer and corresponding <see cref="DisplayWindow"/> of display information.
    /// </summary>
    /// <returns></returns>
    public static TerminalInstance Clear() => _instance.Clear();

    /// <summary>
    /// Clears the current line the <see cref="Terminal"/> <see cref="Cursor"/> is on.
    /// </summary>
    /// <returns></returns>
    public static TerminalInstance ClearLine() => _instance.ClearLine();
    #endregion

    #region Read

    /// <summary>
    /// Reads the next <see cref="char"/>acter from the standard <see cref="Input"/> stream.
    /// </summary>
    /// <returns></returns>
    public static char Read() => _instance.Read();

    /// <summary>
    /// Reads the next <see cref="char"/>acter from the standard <see cref="Input"/> stream.
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static TerminalInstance Read(out char c) => _instance.Read(out c);

    /// <summary>
    /// Reads the next <see cref="char"/>acter or function key pressed by the user.
    /// The pressed key is displayed in the <see cref="Terminal"/> window.
    /// </summary>
    /// <returns></returns>
    public static ConsoleKeyInfo ReadKey() => _instance.ReadKey();

    /// <summary>
    /// Reads the next <see cref="char"/>acter or function key pressed by the user.
    /// The pressed key is displayed in the <see cref="Terminal"/> window.
    /// </summary>
    /// <param name="consoleKeyInfo"></param>
    /// <returns></returns>
    public static TerminalInstance ReadKey(out ConsoleKeyInfo consoleKeyInfo) => 
        _instance.ReadKey(out consoleKeyInfo);

    /// <summary>
    /// Reads the next <see cref="char"/>acter or function key pressed by the user.
    /// The pressed key is optionally displayed in the <see cref="Terminal"/> window.
    /// </summary>
    /// <param name="intercept"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo ReadKey(bool intercept) => _instance.ReadKey(intercept);

    /// <summary>
    /// Reads the next <see cref="char"/>acter or function key pressed by the user.
    /// The pressed key is optionally displayed in the <see cref="Terminal"/> window.
    /// </summary>
    /// <param name="intercept"></param>
    /// <param name="consoleKeyInfo"></param>
    /// <returns></returns>
    public static TerminalInstance ReadKey(bool intercept, out ConsoleKeyInfo consoleKeyInfo) =>
        _instance.ReadKey(intercept, out consoleKeyInfo);

    /// <summary>
    /// Reads the next line of characters from the standard <see cref="Input"/> stream.
    /// </summary>
    /// <returns></returns>
    public static string ReadLine() => _instance.ReadLine();

    /// <summary>
    /// Reads the next line of characters from the standard <see cref="Input"/> stream.
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    public static TerminalInstance ReadLine(out string line) => _instance.ReadLine(out line);

    /// <summary>
    /// Reads the next line of characters from the standard <see cref="Input"/> stream.
    /// The pressed keys are optionally displayed in the <see cref="Terminal"/> window.
    /// </summary>
    /// <param name="intercept"></param>
    /// <param name="line"></param>
    /// <returns></returns>
    public static TerminalInstance ReadLine(bool intercept, out string line) =>
        _instance.ReadLine(intercept, out line);

    /// <summary>
    /// Reads the next line of characters from the standard <see cref="Input"/> stream into a <see cref="SecureString"/>, displaying an asterisk (*) instead of the pressed characters.
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public static TerminalInstance ReadPassword(out SecureString password) => _instance.ReadPassword(out password);
    #endregion

    #region Color

    /// <summary>
    /// Resets the <see cref="ForegroundColor"/> and <see cref="BackgroundColor"/> to their default <see cref="System.Drawing.Color"/> values.
    /// </summary>
    /// <returns></returns>
    public static TerminalInstance ResetColor() => _instance.ResetColor();

    /// <summary>
    /// Sets the foreground <see cref="System.Drawing.Color"/> for the <see cref="Terminal"/>.
    /// </summary>
    /// <param name="foreColor"></param>
    /// <returns></returns>
    public static TerminalInstance SetForeColor(Color foreColor) => _instance.SetForeColor(foreColor);

    /// <summary>
    /// Sets the background <see cref="System.Drawing.Color"/> for the <see cref="Terminal"/>.
    /// </summary>
    /// <param name="backColor"></param>
    /// <returns></returns>
    public static TerminalInstance SetBackColor(Color backColor) => _instance.SetBackColor(backColor);

    /// <summary>
    /// Sets the foreground <see cref="ConsoleColor"/> for the <see cref="Terminal"/>.
    /// </summary>
    /// <param name="foreColor"></param>
    /// <returns></returns>
    public static TerminalInstance SetForeColor(ConsoleColor foreColor) => _instance.SetForeColor(foreColor);

    /// <summary>
    /// Sets the background <see cref="ConsoleColor"/> for the <see cref="Terminal"/>.
    /// </summary>
    /// <param name="backColor"></param>
    /// <returns></returns>
    public static TerminalInstance SetBackColor(ConsoleColor backColor) => _instance.SetBackColor(backColor);

    /// <summary>
    /// Sets the foreground and background <see cref="System.Drawing.Color"/>s for the <see cref="Terminal"/>.
    /// </summary>
    /// <param name="foreColor"></param>
    /// <param name="backColor"></param>
    /// <returns></returns>
    public static TerminalInstance SetColors(Color? foreColor = null, Color? backColor = null) =>
        _instance.SetColors(foreColor, backColor);

    /// <summary>
    /// Sets the foreground and background colors for the <see cref="Terminal"/>.
    /// </summary>
    /// <param name="foreColor"></param>
    /// <param name="backColor"></param>
    /// <returns></returns>
    public static TerminalInstance SetColors(ConsoleColor? foreColor = null, ConsoleColor? backColor = null) =>
        _instance.SetColors(foreColor, backColor);
    #endregion

    #region Write

    /// <summary>
    /// Writes the text representation of the specified <see cref="bool"/> value to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TerminalInstance Write(bool value) => _instance.Write(value);

    /// <summary>
    /// Writes the specified <see cref="Encoding.Unicode"/> <see cref="char"/> to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TerminalInstance Write(char value) => _instance.Write(value);

    /// <summary>
    /// Writes the specified array of Unicode characters to the standard output stream.
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static TerminalInstance Write(char[] buffer) => _instance.Write(buffer);

    /// <summary>
    /// Writes the a segement of an array of Unicode characters to the standard output stream.
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="startIndex"></param>
    /// <param name="length"></param>
    /// <param name="endIndex"></param>
    /// <returns></returns>
    public static TerminalInstance Write(char[] buffer, int? startIndex = null, int? length = null, int? endIndex = null) => 
        _instance.Write(buffer, startIndex, length, endIndex);

    /// <summary>
    /// Writes the text representation of the specified <see cref="object"/> to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TerminalInstance Write(object value) => _instance.Write(value);

    /// <summary>
    /// Writes the text representation of the specified <see cref="int"/> value to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TerminalInstance Write(int value) => _instance.Write(value);

    /// <summary>
    /// Writes the text representation of the specified <see cref="uint"/> value to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TerminalInstance Write(uint value) => _instance.Write(value);

    /// <summary>
    /// Writes the text representation of the specified <see cref="long"/> value to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TerminalInstance Write(long value) => _instance.Write(value);

    /// <summary>
    /// Writes the text representation of the specified <see cref="ulong"/> value to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TerminalInstance Write(ulong value) => _instance.Write(value);

    /// <summary>
    /// Writes the text representation of the specified <see cref="float"/> value to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TerminalInstance Write(float value) => _instance.Write(value);

    /// <summary>
    /// Writes the text representation of the specified <see cref="double"/> value to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TerminalInstance Write(double value) => _instance.Write(value);

    /// <summary>
    /// Writes the text representation of the specified <see cref="decimal"/> value to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TerminalInstance Write(decimal value) => _instance.Write(value);

    /// <summary>
    /// Writes the specified <see cref="string"/> value to the standard output stream.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static TerminalInstance Write(string text) => _instance.Write(text);

    /// <summary>
    /// Writes the specified <see cref="FormattableString"/> value to the standard output stream, using embedded <see cref="System.Drawing.Color"/> values to change the foreground color during the writing process.
    /// </summary>
    /// <param name="interpolatedText"></param>
    /// <returns></returns>
    public static TerminalInstance Write(ref DefaultInterpolatedStringHandler interpolatedText) => _instance.Write(ref interpolatedText);

    /// <summary>
    /// Writes a formatted <see cref="string"/> to the standard output stream.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="arg0"></param>
    /// <returns></returns>
    public static TerminalInstance Write(string format, object arg0) => _instance.Write(format, arg0);

    /// <summary>
    /// Writes a formatted <see cref="string"/> to the standard output stream.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    /// <returns></returns>
    public static TerminalInstance Write(string format, object arg0, object arg1) => _instance.Write(format, arg0, arg1);

    /// <summary>
    /// Writes a formatted <see cref="string"/> to the standard output stream.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <returns></returns>
    public static TerminalInstance Write(string format, object arg0, object arg1, object arg2) => _instance.Write(format, arg0, arg1, arg2);

    /// <summary>
    /// Writes a formatted <see cref="string"/> to the standard output stream.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <param name="arg3"></param>
    /// <returns></returns>
    public static TerminalInstance Write(string format, object arg0, object arg1, object arg2, object arg3) => _instance.Write(format, arg0, arg1, arg2, arg3);

    /// <summary>
    /// Writes a formatted <see cref="string"/> to the standard output stream.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static TerminalInstance Write(string format, params object[] args) => _instance.Write(format, args);
    #endregion

    #region WriteLine

    /// <summary>
    /// Writes the current line terminator (<see cref="Environment.NewLine"/>) to the standard output stream.
    /// </summary>
    /// <returns></returns>
    public static TerminalInstance WriteLine() => _instance.WriteLine();

    /// <summary>
    /// Writes the text representation of the specified <see cref="object"/>, followed by the current line terminator, to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TerminalInstance WriteLine(object value) => _instance.WriteLine(value);

    /// <summary>
    /// Writes the text representation of the specified <see cref="bool"/> value, followed by the current line terminator, to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TerminalInstance WriteLine(bool value) => _instance.WriteLine(value);

    /// <summary>
    /// Writes the specified <see cref="Encoding.Unicode"/> <see cref="char"/>, followed by the current line terminator, to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TerminalInstance WriteLine(char value) => _instance.WriteLine(value);

    /// <summary>
    /// Writes the specified array of Unicode characters, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static TerminalInstance WriteLine(char[] buffer) => _instance.WriteLine(buffer);

    /// <summary>
    /// Writes the a segement of an array of Unicode characters, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="startIndex"></param>
    /// <param name="length"></param>
    /// <param name="endIndex"></param>
    /// <returns></returns>
    public static TerminalInstance WriteLine(char[] buffer, int? startIndex = null, int? length = null, int? endIndex = null) => 
        _instance.WriteLine(buffer, startIndex, length, endIndex);

    /// <summary>
    /// Writes the text representation of the specified <see cref="int"/> value, followed by the current line terminator, to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TerminalInstance WriteLine(int value) => _instance.WriteLine(value);

    /// <summary>
    /// Writes the text representation of the specified <see cref="uint"/> value, followed by the current line terminator, to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TerminalInstance WriteLine(uint value) => _instance.WriteLine(value);

    /// <summary>
    /// Writes the text representation of the specified <see cref="long"/> value, followed by the current line terminator, to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TerminalInstance WriteLine(long value) => _instance.WriteLine(value);

    /// <summary>
    /// Writes the text representation of the specified <see cref="ulong"/> value, followed by the current line terminator, to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TerminalInstance WriteLine(ulong value) => _instance.WriteLine(value);

    /// <summary>
    /// Writes the text representation of the specified <see cref="float"/> value, followed by the current line terminator, to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TerminalInstance WriteLine(float value) => _instance.WriteLine(value);

    /// <summary>
    /// Writes the text representation of the specified <see cref="double"/> value, followed by the current line terminator, to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TerminalInstance WriteLine(double value) => _instance.WriteLine(value);

    /// <summary>
    /// Writes the text representation of the specified <see cref="decimal"/> value, followed by the current line terminator, to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TerminalInstance WriteLine(decimal value) => _instance.WriteLine(value);

    /// <summary>
    /// Writes the specified <see cref="string"/> value, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static TerminalInstance WriteLine(NonFormattableString text) => _instance.WriteLine(text);

    /// <summary>
    /// Writes the specified <see cref="FormattableString"/> value, followed by the current line terminator, to the standard output stream, using embedded <see cref="System.Drawing.Color"/> values to change the foreground color during the writing process.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static TerminalInstance WriteLine(FormattableString text) => _instance.WriteLine(text);

    /// <summary>
    /// Writes a formatted <see cref="string"/>, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="arg0"></param>
    /// <returns></returns>
    public static TerminalInstance WriteLine(string format, object arg0) => _instance.WriteLine(format, arg0);

    /// <summary>
    /// Writes a formatted <see cref="string"/>, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    /// <returns></returns>
    public static TerminalInstance WriteLine(string format, object arg0, object arg1) => _instance.WriteLine(format, arg0, arg1);

    /// <summary>
    /// Writes a formatted <see cref="string"/>, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <returns></returns>
    public static TerminalInstance WriteLine(string format, object arg0, object arg1, object arg2) => _instance.WriteLine(format, arg0, arg1, arg2);

    /// <summary>
    /// Writes a formatted <see cref="string"/>, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <param name="arg3"></param>
    /// <returns></returns>
    public static TerminalInstance WriteLine(string format, object arg0, object arg1, object arg2, object arg3) => _instance.WriteLine(format, arg0, arg1, arg2, arg3);

    /// <summary>
    /// Writes a formatted <see cref="string"/>, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static TerminalInstance WriteLine(NonFormattableString format, params object[] args) => _instance.WriteLine(format, args);
    #endregion

    #region Temporary Actions

    /// <summary>
    /// Stores the current <see cref="Terminal"/> <see cref="Color"/>s and <see cref="Cursor"/> positions, performs an <see cref="Action"/> on this <see cref="Terminal"/>, then restores the <see cref="Color"/>s and <see cref="Cursor"/> position.
    /// </summary>
    /// <param name="tempAction"></param>
    /// <returns></returns>
    public static TerminalInstance Temp(Action<TerminalInstance> tempAction) => _instance.Temp(tempAction);

    /// <summary>
    /// Stores the current <see cref="Terminal"/> <see cref="Color"/>s, performs an <see cref="Action"/> on this <see cref="Terminal"/>, then restores the <see cref="Color"/>s.
    /// </summary>
    /// <param name="tempAction"></param>
    /// <returns></returns>
    public static TerminalInstance TempColor(Action<TerminalInstance> tempAction) =>
        _instance.TempColor(tempAction);

    /// <summary>
    /// Stores the current <see cref="Terminal"/> <see cref="Cursor"/> position, performs an <see cref="Action"/> on this <see cref="Terminal"/>, then restores the <see cref="Cursor"/> position.
    /// </summary>
    /// <param name="tempAction"></param>
    /// <returns></returns>
    public static TerminalInstance TempPosition(Action<TerminalInstance> tempAction) =>
        _instance.TempPosition(tempAction);

    /// <summary>
    /// Gets an <see cref="IDisposable"/> that, when disposed, resets the <see cref="Terminal"/>'s colors back to what they were when the <see cref="ColorLock"/> was taken.
    /// </summary>
    public static IDisposable ColorLock => _instance.ColorLock;

    /// <summary>
    /// Gets an <see cref="IDisposable"/> that, when disposed, resets the <see cref="Terminal"/>'s cursor back to where it was when the <see cref="ColorLock"/> was taken.
    /// </summary>
    public static IDisposable ResetCursor => _instance.ResetCursor;
    #endregion
    #endregion
}*/