using System.Runtime.CompilerServices;

namespace HelpersCore;

/// <summary>
/// Provides extension useful for nullable types.
/// </summary>
public static class CodeExtensions
{
	extension<T>(T self)
	{
		/// <summary>
		/// Performs the specified action on the object and returns the object itself.
		/// </summary>
		/// <param name="action">Action to invoke.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T Also(Action<T> action)
		{
			action(self);
			return self;
		}

		/// <summary>
		/// Performs the specified function on the object and returns function result.
		/// </summary>
		/// <param name="func">Function to invoke.</param>
		/// <typeparam name="TResult">New result type.</typeparam>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TResult Let<TResult>(Func<T, TResult> func)
			=> func(self);
	}
}