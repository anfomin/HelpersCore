using System.Diagnostics.CodeAnalysis;

namespace HelpersCore;

[SuppressMessage("ReSharper", "InvokeAsExtensionMemberFromSameClass")]
public static partial class ConsoleExtensions
{
	extension(Console)
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
		/// <typeparam name="T">Parsable type to convert input string to.</typeparam>
		/// <param name="name">Name to prefix reading value.</param>
		/// <param name="validator">Function to validate the input. Error message or <c>null</c> if value is valid.</param>
		public static T Read<T>(string name, ValidateFn<T>? validator = null)
			where T : IParsable<T>
			=> Read(name, (s, out result) => T.TryParse(s, null, out result!), validator);

		/// <summary>
		/// Writes "<paramref name="name"/>:" to the console and then reads input line.
		/// If input is not valid, it will be repeated until valid input is received.
		/// </summary>
		/// <typeparam name="T">Type to convert input string to.</typeparam>
		/// <param name="name">Name to prefix reading value.</param>
		/// <param name="tryParse">Function to parse the input.</param>
		/// <param name="validator">Function to validate the input. Error message or <c>null</c> if value is valid.</param>
		public static T Read<T>(string name, TryParseFn<T> tryParse, ValidateFn<T>? validator = null)
		{
			while (true)
			{
				Console.Write(name);
				Console.Write(@": ");
				string? str = Console.ReadLine();
				string? error = null;
				T? result = default;
				if (str is null || !tryParse(str, out result))
					error = "Неверный формат";
				else if (validator is not null)
					error = validator(result!);

				if (error is null)
					return result!;
				WriteLine(ConsoleColor.DarkYellow, error);
			}
		}

		/// <summary>
		/// Writes "<paramref name="name"/> (y/n):" to the console and then reads
		/// boolean value with "y" for <c>true</c> or "n" for <c>false</c>.
		/// If input is not valid, it will be repeated until valid input is received.
		/// </summary>
		/// <param name="name">Name to prefix reading value.</param>
		/// <param name="defaultResult">Default result to use then <see cref="ConsoleKey.Enter"/> is pressed.</param>
		public static bool ReadBool(string name, bool? defaultResult = null)
		{
			while (true)
			{
				Console.Write(name);
				Console.Write(defaultResult switch
				{
					true => @" (Y/n)",
					false => @" (y/N)",
					_ => @" (y/n)"
				});
				Console.Write(@": ");
				var key = Console.ReadKey();
				Console.WriteLine();
				switch (key)
				{
					case { Key: ConsoleKey.Enter } when defaultResult == true:
					case { KeyChar: 'y' or 'н' }:
						return true;
					case { Key: ConsoleKey.Enter } when defaultResult == false:
					case { KeyChar: 'n' or 'т' }:
						return false;
					default:
						Console.WriteLine(Strings.EnterYesNo);
						break;
				}
			}
		}
	}

	/// <summary>
	/// Tries to parse a string into a <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">Type to parse to.</typeparam>
	/// <returns><see langword="true" /> if <paramref name="s" /> was successfully parsed; otherwise, <see langword="false" />.</returns>
	public delegate bool TryParseFn<T>(string s, [MaybeNullWhen(false)] out T result);

	/// <summary>
	/// Validates console input.
	/// </summary>
	/// <typeparam name="T">Type to validate.</typeparam>
	/// <param name="value">Value to validate.</param>
	/// <returns>Error message or <c>null</c> if <paramref name="value"/> is valid.</returns>
	public delegate string? ValidateFn<T>(T value);
}