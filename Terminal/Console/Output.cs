using System.IO;
using System.Text;

namespace Jay.Consolas
{
	/// <summary>
	/// Options related to a <see cref="Console"/>'s Output.
	/// </summary>
	public sealed class Output
	{
		private readonly Console _console;

		/// <summary>
		/// Gets or sets the <see cref="TextWriter"/> used for output.
		/// </summary>
		public TextWriter Writer
		{
			get => System.Console.Out;
			set => System.Console.SetOut(value);
		}

		/// <summary>
		/// Gets or sets the <see cref="System.Text.Encoding"/> the <see cref="Console"/> uses to output.
		/// </summary>
		public Encoding Encoding
		{
			get => System.Console.OutputEncoding;
			set => System.Console.OutputEncoding = value;
		}

		/// <summary>
		/// Has the output stream been redirected from standard?
		/// </summary>
		public bool Redirected => System.Console.IsOutputRedirected;

		internal Output(Console console)
		{
			_console = console;
		}

		/// <summary>
		/// Acquires the standard output <see cref="Stream"/>.
		/// </summary>
		/// <returns></returns>
		public static Stream Open() => System.Console.OpenStandardOutput();

		/// <summary>
		/// Acquires the standard output <see cref="Stream"/>, which is set to a specified buffer size.
		/// </summary>
		/// <param name="bufferSize"></param>
		/// <returns></returns>
		public static Stream Open(int bufferSize) => System.Console.OpenStandardOutput(bufferSize);

		/// <summary>
		/// Sets the output to the specified <see cref="TextWriter"/>.
		/// </summary>
		/// <param name="writer"></param>
		/// <returns></returns>
		public Console SetWriter(TextWriter writer)
		{
			System.Console.SetOut(writer);
			return _console;
		}

		/// <summary>
		/// Sets the output <see cref="System.Text.Encoding"/>.
		/// </summary>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public Console SetEncoding(Encoding encoding)
		{
			System.Console.OutputEncoding = encoding;
			return _console;
		}
	}
}