namespace HelpersCore;

public static partial class ConsoleEx
{
	extension(Console)
	{
		/// <summary>
		/// Selects item from the list.
		/// </summary>
		/// <param name="name">Text to write before selection.</param>
		/// <param name="items">Items to select from.</param>
		/// <param name="displayFn">Function used to display an item in the console.</param>
		/// <returns>Selected index and item.</returns>
		public static async Task<(int Index, T Item)> SelectAsync<T>(string name, IList<T> items, DisplayFn<T>? displayFn = null)
		{
			displayFn ??= x => x is null ? Strings.Null : x.ToString();
			Console.Write(name);
			Console.WriteLine(@":");
			foreach (var (i, item) in items.Index())
				Console.WriteLine($@"{i + 1}. {displayFn(item)}");
			Console.WriteLine();

			int value = Read<int>(Strings.EnterSelection,
				v => 0 < v && v <= items.Count ? null : Strings.EnterValidSelection
			);
			int index = value - 1;
			var selected = items[index];
			Console.WriteLine($@"{Strings.Selected}: {displayFn(selected)}");
			Console.WriteLine();
			await Task.Delay(1000);
			return (index, selected);
		}

		/// <summary>
		/// Selects item from the list by key.
		/// </summary>
		/// <param name="name">Text to write before selection.</param>
		/// <param name="items">Items to select from.</param>
		/// <param name="keyFn">Key is displayed before an item. User inputs key into console to select the item. Key must be unique.</param>
		/// <param name="displayFn">Function used to display an item in the console.</param>
		/// <returns>Selected item.</returns>
		public static async Task<T> SelectByKeyAsync<T, TKey>(string name, IEnumerable<T> items, Func<T, TKey> keyFn, DisplayFn<T>? displayFn = null)
			where TKey : notnull
		{
			displayFn ??= x => x is null ? Strings.Null : x.ToString();
			Console.Write(name);
			Console.WriteLine(@":");
			Dictionary<TKey, T> dictionary = new();
			foreach (var item in items)
			{
				var key = keyFn(item);
				dictionary[key] = item;
				Console.WriteLine($@"{key}. {displayFn(item)}");
			}
			Console.WriteLine();

			var selectedKey = Read<TKey>(Strings.EnterSelection,
				key => dictionary.ContainsKey(key) ? null : Strings.EnterValidSelection
			);
			var selected = dictionary[selectedKey];
			Console.WriteLine($@"{Strings.Selected}: {displayFn(selected)}");
			Console.WriteLine();
			await Task.Delay(1000);
			return selected;
		}

		/// <summary>
		/// Selects item from the list by key.
		/// </summary>
		/// <param name="name">Text to write before selection.</param>
		/// <param name="items">Items to select from.</param>
		/// <returns>Selected item.</returns>
		public static Task<T> SelectByKeyAsync<T>(string name, IEnumerable<T> items)
			where T : IKeyEntity<int>
			=> SelectByKeyAsync(name, items, item => item.Id);

		/// <summary>
		/// Selects item or <c>null</c> from the list by key.
		/// </summary>
		/// <param name="name">Text to write before selection.</param>
		/// <param name="items">Items to select from.</param>
		/// <param name="keyFn">Key is displayed before an item. User inputs key into console to select the item. Key must be unique.</param>
		/// <param name="displayFn">Function used to display an item in the console.</param>
		/// <returns>Selected item.</returns>
		public static Task<T?> SelectByKeyOrNullAsync<T, TKey>(string name, IEnumerable<T> items, Func<T, TKey> keyFn, DisplayFn<T?>? displayFn = null)
			where T : class
			where TKey : notnull
			=> SelectByKeyAsync(name,
				items: items.Prepend(null),
				keyFn: item => item == null ? default! : keyFn(item),
				displayFn: item => displayFn != null ? displayFn(item)
					: item == null ? Strings.Null
					: item.ToString()
			);

		/// <summary>
		/// Selects item or <c>null</c> from the list by key.
		/// </summary>
		/// <param name="name">Text to write before selection.</param>
		/// <param name="items">Items to select from.</param>
		/// <param name="nullText">Text to display for <c>null</c> item.</param>
		/// <returns>Selected item.</returns>
		public static Task<T?> SelectByKeyOrNullAsync<T>(string name, IEnumerable<T> items, string? nullText = null)
			where T : class, IKeyEntity<int>
			=> SelectByKeyOrNullAsync(name, items,
				item => item.Id,
				item => item != null ? item.ToString() : nullText
			);
	}

	/// <summary>
	/// Returns string to display in console for specified item.
	/// </summary>
	public delegate string? DisplayFn<in T>(T item);
}