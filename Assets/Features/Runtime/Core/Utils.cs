#nullable enable

using System.Linq;

namespace Features
{
	/// <summary>
	/// Static helper class with generic helper methods.
	/// </summary>
	public static partial class Utils
	{
		/// <summary>
		/// Safe version of <see cref="string.Format(string, object[])"/>. If formatting fails, the resulting message will be the same as <paramref name="format"/>.
		/// </summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <returns>A copy of <paramref name="format"/> in which the format items have been replaced by the string representation of the corresponding objects in <paramref name="args"/>.</returns>
		public static string FormatSafe(string format, params object?[] args)
		{
			string message;
			try
			{
				int argCount = GetArgCount(format);
				message = string.Format(format, args[..Mathf.Min(argCount, args.Length)]);
			}
			catch
			{
				message = format;
			}
			return message;
		}

		/// <summary>
		/// Safe version of <see cref="string.Format(IFormatProvider, string, object[])"/>. If formatting fails, the resulting message will be the same as <paramref name="format"/>.
		/// </summary>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <returns>A copy of <paramref name="format"/> in which the format items have been replaced by the string representation of the corresponding objects in <paramref name="args"/>.</returns>
		public static string FormatSafe(IFormatProvider provider, string format, params object?[] args)
		{
			string message;
			try
			{
				int argCount = GetArgCount(format);
				message = argCount == 0
					? format
					: string.Format(provider, format, args[..Mathf.Min(argCount, args.Length)]);
			}
			catch
			{
				message = format;
			}
			return message;
		}

		/// <summary>
		/// Counts the number  of format arguments int the given <paramref name="format"/> string.
		/// </summary>
		/// <param name="format"></param>
		/// <returns></returns>
		public static int GetArgCount(ReadOnlySpan<char> format)
		{
			int args = 0, length = 0, index = -1;
			for (int i = 0; i < format.Length; i++)
			{
				switch (format[i], index)
				{
					case ('{', < 0):
						index = i + 1; length = 0;
						break;
					case ('{', >= 0):
						index = -1;
						break;
					case (>= '0' and <= '9', >= 0):
						length++;
						break;
					case (':' or '}', >= 0):
						if (int.TryParse(format.Slice(index, length), out var arg))
							args = Mathf.Max(args, arg + 1);
						index = -1;
						break;
				}
				continue;
			}
			return args;
		}

		public static bool IsAssignableTo(this Type t, Type type) => type.IsAssignableFrom(t);

		public static bool IsNull<T>(this T? value) => value switch {
			null => true,
			Object obj when obj == null => true,
			_ => false,
		};

		public static bool NotNull<T>(this T? value) => !IsNull(value);
	}
}
