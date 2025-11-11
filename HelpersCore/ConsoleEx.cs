using System.Text;

namespace HelpersCore;

/// <summary>
/// Provides additional functions to work with <see cref="Console"/>.
/// </summary>
public static partial class ConsoleEx
{
	extension(Console)
	{
		/// <summary>
		/// Enables specified console foreground color.
		/// </summary>
		/// <param name="foregroundColor">Foreground color.</param>
		/// <returns>Object that should be disposed to restore console color.</returns>
		public static IDisposable Color(ConsoleColor foregroundColor)
			=> new ConsoleConfigurator(foregroundColor);

		/// <summary>
		/// Enables specified console state.
		/// </summary>
		/// <param name="state">State to enable.</param>
		/// <returns>Object that should be disposed to restore console state.</returns>
		public static IDisposable State(ConsoleState state)
			=> Color(StateToColor(state));

		/// <summary>
		/// Enabled prefix addition to each line until result is disposed.
		/// </summary>
		/// <param name="prefix">Prefix to add to each line.</param>
		/// <returns>Object that should be disposed to remove prefix.</returns>
		public static IDisposable Prefix(string prefix)
			=> new ConsolePrefixer(prefix);

		/// <summary>
		/// Enabled label prefix addition to each line until result is disposed.
		/// Label prefix is 2 spaces.
		/// </summary>
		/// <returns>Object that should be disposed to remove prefix.</returns>
		public static IDisposable PrefixLevel()
			=> Prefix("  ");

		/// <summary>
		/// Writes line to console with specified foreground color.
		/// </summary>
		/// <param name="value">Value to write.</param>
		/// <param name="foregroundColor">Foreground color.</param>
		public static void WriteLine(string value, ConsoleColor foregroundColor)
		{
			using var _ = Color(foregroundColor);
			Console.WriteLine(value);
		}

		/// <summary>
		/// Writes line to console with specified state.
		/// </summary>
		/// <param name="value">Value to write.</param>
		/// <param name="state">Console state.</param>
		public static void WriteLine(string value, ConsoleState state)
			=> WriteLine(value, StateToColor(state));

		/// <summary>
		/// Writes to console with specified foreground color.
		/// </summary>
		/// <param name="value">Value to write.</param>
		/// <param name="foregroundColor">Foreground color.</param>
		public static void Write(string value, ConsoleColor foregroundColor)
		{
			using var _ = Color(foregroundColor);
			Console.Write(value);
		}

		/// <summary>
		/// Writes to console with specified state.
		/// </summary>
		/// <param name="value">Value to write.</param>
		/// <param name="state">Console state.</param>
		public static void Write(string value, ConsoleState state)
			=> Write(value, StateToColor(state));

		/// <summary>
		/// Writes exception information to the console.
		/// </summary>
		public static void WriteException(Exception ex)
		{
			using var _ = Color(ConsoleColor.Red);
			Console.Error.WriteLine($"{ex.GetType().Name}: {ex.Message.Trim()}");
			Exception? ex1 = ex.InnerException;
			while (ex1 != null)
			{
				Console.Error.WriteLine($"   {ex1.GetType().Name}: {ex1.Message.Trim()}");
				ex1 = ex1.InnerException;
			}
		}

		/// <summary>
		/// Returns console color from state.
		/// </summary>
		static ConsoleColor StateToColor(ConsoleState state) => state switch
		{
			ConsoleState.Success => ConsoleColor.Green,
			ConsoleState.Warning => ConsoleColor.Yellow,
			ConsoleState.Error => ConsoleColor.Red,
			_ => ConsoleColor.White
		};
	}

	class ConsoleConfigurator : IDisposable
	{
		public ConsoleConfigurator(ConsoleColor foregroundColor)
		{
			Console.ForegroundColor = foregroundColor;
		}

		public void Dispose()
		{
			Console.ResetColor();
		}
	}

	class ConsolePrefixer : IDisposable
	{
		readonly TextWriter _initialOut;

		public ConsolePrefixer(string prefix)
		{
			_initialOut = Console.Out;
			Console.SetOut(new PrefixerTextWriter(_initialOut, prefix));
		}

		public void Dispose()
		{
			Console.SetOut(_initialOut);
		}
	}

	class PrefixerTextWriter(TextWriter baseWriter, string prefix) : TextWriter
	{
		readonly TextWriter _baseWriter = baseWriter;
		readonly string _prefix = prefix;

		public override Encoding Encoding => _baseWriter.Encoding;

		public override IFormatProvider FormatProvider => _baseWriter.FormatProvider;

		public override string NewLine
		{
			get => _baseWriter.NewLine;
#pragma warning disable CS8765
			set => _baseWriter.NewLine = value;
#pragma warning restore CS8765
		}

		public override void WriteLine(string? value)
		{
			_baseWriter.Write(_prefix);
			_baseWriter.WriteLine(value);
		}

		public override void Write(string? value)
			=> _baseWriter.Write(value);

		public override void Flush()
			=> _baseWriter.Flush();

		public override void Close()
			=> _baseWriter.Close();
	}
}

/// <summary>
/// Represents console states.
/// </summary>
public enum ConsoleState
{
	Success,
	Warning,
	Error
}