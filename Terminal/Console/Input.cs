using System.IO;
using System.Text;

namespace Jay.Consolas
{
	/// <summary>
	/// Options related to a <see cref="Console"/>'s Input.
	/// </summary>
	public sealed class Input
	{
		private readonly Console _console;

		/// <summary>
		/// Gets or sets the <see cref="TextReader"/> the input reads from.
		/// </summary>
		public TextReader Reader
		{
			get => System.Console.In;
			set => System.Console.SetIn(value);
		}

		/// <summary>
		/// Gets or sets the <see cref="System.Text.Encoding"/> the <see cref="Console"/> uses to read input.
		/// </summary>
		public Encoding Encoding
		{
			get => System.Console.InputEncoding;
			set => System.Console.InputEncoding = value;
		}

		/// <summary>
		/// Has the input stream been redirected from standard?
		/// </summary>
		public bool Redirected => System.Console.IsInputRedirected;

		/// <summary>
		/// Gets or sets whether Ctrl+C should be treated as input or as a break command.
		/// </summary>
		public bool TreatCtrlCAsInput
		{
			get => System.Console.TreatControlCAsInput;
			set => System.Console.TreatControlCAsInput = value;
		}

		internal Input(Console console)
		{
			_console = console;
		}

		/// <summary>
		/// Acquires the standard input <see cref="Stream"/>.
		/// </summary>
		/// <returns></returns>
		public Stream Open() => System.Console.OpenStandardInput();

		/// <summary>
		/// Acquires the standard input <see cref="Stream"/>, which is set to a specified buffer size.
		/// </summary>
		/// <param name="bufferSize"></param>
		/// <returns></returns>
		public Stream Open(int bufferSize) => System.Console.OpenStandardInput(bufferSize);

		/// <summary>
		/// Sets the input to the specified <see cref="TextReader"/>.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public Console SetReader(TextReader reader)
		{
			System.Console.SetIn(reader);
			return _console;
		}

		/// <summary>
		/// Sets the input <see cref="System.Text.Encoding"/>.
		/// </summary>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public Console SetEncoding(Encoding encoding)
		{
			System.Console.InputEncoding = encoding;
			return _console;
		}
	}
}