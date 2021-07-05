using System.IO;

namespace Jay.Consolas
{
	/// <summary>
	/// Options related to a <see cref="Console"/>'s Error output.
	/// </summary>
	public sealed class Error
	{
		private readonly Console _console;

		/// <summary>
		/// Gets or sets the <see cref="TextWriter"/> the Error outputs to.
		/// </summary>
		public TextWriter Writer
		{
			get => System.Console.Error;
			set => System.Console.SetError(value);
		}

		/// <summary>
		/// Has the error stream been redirected from standard?
		/// </summary>
		public bool Redirected => System.Console.IsErrorRedirected;

		internal Error(Console console)
		{
			_console = console;
		}

		/// <summary>
		/// Acquires the standard error <see cref="Stream"/>.
		/// </summary>
		/// <returns></returns>
		public Stream Open() => System.Console.OpenStandardError();

		/// <summary>
		/// Acquires the standard error <see cref="Stream"/>, which is set to a specified buffer size.
		/// </summary>
		/// <param name="bufferSize"></param>
		/// <returns></returns>
		public Stream Open(int bufferSize) => System.Console.OpenStandardError(bufferSize);

		/// <summary>
		/// Sets the error output to the specified <see cref="TextWriter"/>.
		/// </summary>
		/// <param name="writer"></param>
		/// <returns></returns>
		public Console SetWriter(TextWriter writer)
		{
			System.Console.SetError(writer);
			return _console;
		}
	}
}