namespace HelpersCore;

public static partial class ConsoleEx
{
	/// <summary>
	/// Writes "<paramref name="name"/>:" to the console and then reads input line.
	/// If input is not valid, it will be repeated until valid input is received.
	/// </summary>
	/// <param name="name">Name to prefix reading value.</param>
	/// <param name="validator">Function to validate the input. Error message or <c>null</c> if value is valid.</param>
	public static string Read(string name, ValidateFn<string>? validator = null)
		=> Read<string>(name, validator);

	/// <summary>
	/// Writes "<paramref name="name"/>:" to the console and then reads input line.
	/// If input is not valid, it will be repeated until valid input is received.
	/// </summary>
	/// <typeparam name="T">Type to convert input string to.</typeparam>
	/// <param name="name">Name to prefix reading value.</param>
	/// <param name="validator">Function to validate the input. Error message or <c>null</c> if value is valid.</param>
	public static T Read<T>(string name, ValidateFn<T>? validator = null)
	{
		while (true)
		{
			Console.Write(name);
			Console.Write(@": ");
			string? str = Console.ReadLine();
			string? error = null;
			T? result = default;
			if (str == null || !str.TryConvertTo(out result))
				error = "Неверный формат";
			else if (validator != null)
				error = validator(result!);

			if (error == null)
				return result!;
			WriteLine(error, ConsoleColor.DarkYellow);
		}
	}

	/// <summary>
	/// Writes "<paramref name="name"/> (y/n):" to the console and then reads
	/// boolean value with "y" for <c>true</c> or "n" for <c>false</c>.
	/// If input is not valid, it will be repeated until valid input is received.
	/// </summary>
	/// <param name="name">Name to prefix reading value.</param>
	public static bool ReadBool(string name)
	{
		while (true)
		{
			Console.Write(name);
			Console.Write(@" (y/n)");
			Console.Write(@": ");
			char c = Console.ReadKey().KeyChar;
			Console.WriteLine();
			switch (c)
			{
				case 'y':
					return true;
				case 'n':
					return false;
				default:
					Console.WriteLine(Strings.EnterYesNo);
					break;
			}
		}
	}

	/// <summary>
	/// Console input validation delegate.
	/// </summary>
	/// <typeparam name="T">Type to validate.</typeparam>
	/// <param name="value">Value to validate.</param>
	/// <returns>Error message or <c>null</c> if <paramref name="value"/> is valid.</returns>
	public delegate string? ValidateFn<T>(T value);
}