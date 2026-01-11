#nullable enable

using System.Linq;
using System.IO;
using static Features.Utils;

namespace Features
{
	/// <summary>
	/// Static helper class for validation and throwing exceptions.
	/// </summary>
	public static class ThrowHelper
	{
		/// <summary>
		/// Throws a <see cref="System.ArgumentException"/> with a formatted message.
		/// </summary>
		/// <remarks>
		/// The first 2 format arguments can be used to define a parameter name and inner exception, in that order.
		/// </remarks>
		/// <param name="message"></param>
		/// <param name="formatArgs"></param>
		public static ArgumentException ArgumentException(string message, params object?[] formatArgs)
			=> formatArgs.Length switch {
				> 0 when formatArgs[0] is Exception inner => new ArgumentException(FormatSafe(message, formatArgs), inner),
				> 1 when formatArgs[0] is string paramName && formatArgs[1] is Exception inner => new ArgumentException(FormatSafe(message, formatArgs), paramName, inner),
				> 0 when formatArgs[0] is string paramName => new ArgumentException(FormatSafe(message, formatArgs), paramName),
				> 0 => new ArgumentException(FormatSafe(message, formatArgs)),
				<= 0 => new ArgumentException(message),
			};

		/// <summary>
		/// Throws a <see cref="System.ArgumentNullException"/> with a formatted message.
		/// </summary>
		/// <remarks>
		/// The first format argument can be used to define an inner exception.
		/// </remarks>
		/// <param name="paramName"></param>
		/// <param name="message"></param>
		/// <param name="formatArgs"></param>
		public static ArgumentNullException ArgumentNullException(string paramName, string? message = null, params object?[] formatArgs)
			=> formatArgs.Length switch {
				> 0 when formatArgs[0] is Exception inner => new ArgumentNullException(paramName, inner),
				> 0 when message is not null => new ArgumentNullException(paramName, FormatSafe(message, formatArgs)),
				_ when message is not null => new ArgumentNullException(paramName, message),
				_ => new ArgumentNullException(paramName),
			};

		/// <summary>
		/// Throws a <see cref="System.ArgumentOutOfRangeException"/> with a formatted message.
		/// </summary>
		/// <remarks>
		/// The first format argument can be used to define an inner exception.
		/// </remarks>
		/// <param name="paramName"></param>
		/// <param name="message"></param>
		/// <param name="formatArgs"></param>
		public static ArgumentOutOfRangeException ArgumentOutOfRangeException(string paramName, string? message = null, params object?[] formatArgs)
			=> formatArgs.Length switch {
				> 0 when formatArgs[0] is Exception inner => new ArgumentOutOfRangeException(paramName, inner),
				> 0 when message is not null => new ArgumentOutOfRangeException(paramName, FormatSafe(message, formatArgs)),
				_ when message is not null => new ArgumentOutOfRangeException(paramName, message),
				_ => new ArgumentOutOfRangeException(paramName),
			};

		/// <summary>
		/// Throws a <see cref="System.IO.FileNotFoundException"/> with a formatted message.
		/// </summary>
		/// <remarks>
		/// The first 2 format arguments can be used to define a file name and inner exception, in that order.
		/// </remarks>
		/// <param name="message"></param>
		/// <param name="formatArgs"></param>
		public static FileNotFoundException FileNotFoundException(string message, params object?[] formatArgs)
			=> formatArgs.Length switch {
				> 0 when formatArgs[0] is Exception inner => new FileNotFoundException(FormatSafe(message, formatArgs), inner),
				> 1 when formatArgs[0] is string fileName && formatArgs[1] is Exception inner => new FileNotFoundException(FormatSafe(message, formatArgs), fileName, inner),
				> 0 when formatArgs[0] is string fileName => new FileNotFoundException(FormatSafe(message, formatArgs), fileName),
				> 0 => new FileNotFoundException(FormatSafe(message)),
				<= 0 => new FileNotFoundException(message),
			};

		/// <summary>
		/// Throws a <see cref="System.IO.FileLoadException"/> with a formatted <paramref name="message"/>.
		/// </summary>
		/// <remarks>
		/// The first 2 format arguments can be used to define a file name and inner exception, in that order.
		/// </remarks>
		/// <param name="message"></param>
		/// <param name="formatArgs"></param>
		public static FileLoadException FileLoadException(string message, params object?[] formatArgs)
			=> formatArgs.Length switch {
				> 0 when formatArgs[0] is Exception inner => new FileLoadException(FormatSafe(message, formatArgs), inner),
				> 1 when formatArgs[0] is string fileName && formatArgs[1] is Exception inner => new FileLoadException(FormatSafe(message, formatArgs), fileName, inner),
				> 0 when formatArgs[0] is string fileName => new FileLoadException(FormatSafe(message, formatArgs), fileName),
				> 0 => new FileLoadException(FormatSafe(message)),
				<= 0 => new FileLoadException(message),
			};

		// Conditional throws

		/// <summary>
		/// Throws a <see cref="System.IO.FileNotFoundException"/> if no file exists at the given <paramref name="path"/>.
		/// </summary>
		/// <param name="path"></param>
		/// <exception cref="System.IO.FileNotFoundException"></exception>
		public static void IfFileNotExists(string path)
			=> IfFileNotExists(path, "The file '{0}' was not found at path '{1}'.", Path.GetFileName(path), Path.GetDirectoryName(path));
		
		/// <summary>
		/// Throws a <see cref="System.IO.FileNotFoundException"/> if no file exists at the given <paramref name="path"/>.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="messageFormat"></param>
		/// <param name="args"></param>
		/// <exception cref="System.IO.FileNotFoundException"></exception>
		public static void IfFileNotExists(string path, string messageFormat, params object?[] args)
		{
			if (!File.Exists(path))
			{
				throw new FileNotFoundException(FormatSafe(messageFormat, args), Path.GetFileName(path));
			}
		}
		/// <summary>
		/// Throws a <see cref="DirectoryNotFoundException"/> if no directory exists at the given <paramref name="path"/>.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="messageFormat"></param>
		/// <param name="args"></param>
		/// <exception cref="DirectoryNotFoundException"></exception>
		public static void IfDirectoryNotExists(string path, string messageFormat, params object?[] args)
		{
			if (!Directory.Exists(path))
			{
				throw new DirectoryNotFoundException(FormatSafe(messageFormat, args));
			}
		}

		/// <summary>
		/// Throws a <see cref="System.ArgumentNullException"/> if <paramref name="obj"/> is null.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="paramName"></param>
		/// <param name="message"></param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public static void IfNull([NotNull] object? obj, string paramName, string? message = null)
		{
			switch (obj)
			{
				case null:
				case Object uObj when uObj == null:
					if (string.IsNullOrWhiteSpace(message))
					{
						throw new ArgumentNullException(paramName);
					}
					else
					{
						throw new ArgumentNullException(paramName, message);
					}
			}
		}

		/// <summary>
		/// Throws an <see cref="System.ArgumentNullException"/> if the given <paramref name="value"/> is null or an empty string.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="paramName"></param>
		/// <param name="message"></param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public static void IfNullOrEmpty([NotNull] string? value, string paramName, string? message = null)
		{
			if (string.IsNullOrEmpty(value))
			{
				if (string.IsNullOrWhiteSpace(message))
					throw new ArgumentNullException(paramName);
				else
					throw new ArgumentNullException(paramName, message);
			}
		}

		/// <summary>
		/// Throws an <see cref="System.ArgumentNullException"/> if the given <paramref name="value"/> is null, an empty string, or contains only whitespace characters.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="paramName"></param>
		/// <param name="message"></param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public static void IfNullOrWhiteSpace([NotNull] string? value, string paramName, string? message = null)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				if (string.IsNullOrWhiteSpace(message))
					throw new ArgumentNullException(paramName);
				else
					throw new ArgumentNullException(paramName, message);
			}
		}

		/// <summary>
		/// Throws an <see cref="System.ArgumentNullException"/> when the given <paramref name="enumerable"/> is null or has no values.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <param name="paramName"></param>
		/// <param name="message"></param>
		/// <exception cref="System.ArgumentNullException"></exception>
		public static void IfNullOrEmpty<T>([NotNull] IEnumerable<T>? enumerable, string paramName, string? message = null)
		{
			switch (enumerable)
			{
				case null:
				case IReadOnlyCollection<T> c when c.Count is 0:
				case not null when enumerable.Any():
					if (string.IsNullOrWhiteSpace(message))
						throw new ArgumentNullException(paramName);
					else
						throw new ArgumentNullException(paramName, message);
			}
		}

		/// <summary>
		/// Throws an <see cref="System.ArgumentOutOfRangeException"/> if <paramref name="value"/> is less than <paramref name="min"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <param name="min"></param>
		/// <param name="paramName"></param>
		/// <param name="message"></param>
		/// <exception cref="System.ArgumentOutOfRangeException"/>
		public static void IfLessThan<T>(T value, T min, string paramName, string? message = null)
			where T : IComparable<T>
			=> IfLessThan(value, min, static (x, y) => x.CompareTo(y), paramName, message);
		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is less than <paramref name="min"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <param name="min"></param>
		/// <param name="comparer"></param>
		/// <param name="paramName"></param>
		/// <param name="message"></param>
		/// <exception cref="System.ArgumentOutOfRangeException"/>
		public static void IfLessThan<T>(T value, T min, IComparer<T> comparer, string paramName, string? message = null)
			=> IfLessThan(value, min, comparer.Compare, paramName, message);
		/// <summary>
		/// Throws an <see cref="System.ArgumentOutOfRangeException"/> if <paramref name="value"/> is less than <paramref name="min"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <param name="min"></param>
		/// <param name="comparison"></param>
		/// <param name="paramName"></param>
		/// <param name="message"></param>
		/// <exception cref="System.ArgumentOutOfRangeException"></exception>
		public static void IfLessThan<T>(T value, T min, Comparison<T> comparison, string paramName, string? message = null)
		{
			if (comparison(value, min) < 0)
			{
				if (string.IsNullOrWhiteSpace(message))
					throw new ArgumentOutOfRangeException(paramName);
				else
					throw new ArgumentOutOfRangeException(paramName, message);
			}
		}

		/// <summary>
		/// Throws an <see cref="System.ArgumentOutOfRangeException"/> if <paramref name="value"/> is less than or equal to <paramref name="min"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <param name="min"></param>
		/// <param name="paramName"></param>
		/// <param name="message"></param>
		/// <exception cref="System.ArgumentOutOfRangeException"/>
		public static void IfLessOrEqualTo<T>(T value, T min, string paramName, string? message = null)
			where T : IComparable<T>
			=> IfLessOrEqualTo(value, min, static (x, y) => x.CompareTo(y), paramName, message);
		/// <summary>
		/// Throws an <see cref="System.ArgumentOutOfRangeException"/> if <paramref name="value"/> is less than or equal to <paramref name="min"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <param name="min"></param>
		/// <param name="comparer"></param>
		/// <param name="paramName"></param>
		/// <param name="message"></param>
		/// <exception cref="System.ArgumentOutOfRangeException"/>
		public static void IfLessOrEqualTo<T>(T value, T min, IComparer<T> comparer, string paramName, string? message = null)
			=> IfLessOrEqualTo(value, min, comparer.Compare, paramName, message);
		/// <summary>
		/// Throws an <see cref="System.ArgumentOutOfRangeException"/> if <paramref name="value"/> is less than or equal to <paramref name="min"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <param name="min"></param>
		/// <param name="comparison"></param>
		/// <param name="paramName"></param>
		/// <param name="message"></param>
		/// <exception cref="System.ArgumentOutOfRangeException"/>
		public static void IfLessOrEqualTo<T>(T value, T min, Comparison<T> comparison, string paramName, string? message = null)
		{
			if (comparison(value, min) <= 0)
			{
				if (string.IsNullOrWhiteSpace(message))
					throw new ArgumentOutOfRangeException(paramName);
				else
					throw new ArgumentOutOfRangeException(paramName, message);
			}
		}

		/// <summary>
		/// Throws an <see cref="System.ArgumentOutOfRangeException"/> if <paramref name="value"/> is greater than <paramref name="max"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <param name="max"></param>
		/// <param name="paramName"></param>
		/// <param name="message"></param>
		/// <exception cref="System.ArgumentOutOfRangeException"/>
		public static void IfGreaterThan<T>(T value, T max, string paramName, string? message = null)
			where T : IComparable<T>
			=> IfGreaterThan(value, max, static (x, y) => x.CompareTo(y), paramName, message);
		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is greater than <paramref name="max"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <param name="max"></param>
		/// <param name="comparer"></param>
		/// <param name="paramName"></param>
		/// <param name="message"></param>
		public static void IfGreaterThan<T>(T value, T max, IComparer<T> comparer, string paramName, string? message = null)
			=> IfGreaterThan(value, max, comparer.Compare, paramName, message);
		/// <summary>
		/// Throws an <see cref="System.ArgumentOutOfRangeException"/> if <paramref name="value"/> is greater than <paramref name="max"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <param name="max"></param>
		/// <param name="comparison"></param>
		/// <param name="paramName"></param>
		/// <param name="message"></param>
		/// <exception cref="System.ArgumentOutOfRangeException"></exception>
		public static void IfGreaterThan<T>(T value, T max, Comparison<T> comparison, string paramName, string? message = null)
		{
			if (comparison(value, max) > 0)
			{
				if (string.IsNullOrWhiteSpace(message))
					throw new ArgumentOutOfRangeException(paramName);
				else
					throw new ArgumentOutOfRangeException(paramName, message);
			}
		}

		/// <summary>
		/// Throws an <see cref="System.ArgumentOutOfRangeException"/> if <paramref name="value"/> is greater than or equal to <paramref name="max"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <param name="max"></param>
		/// <param name="paramName"></param>
		/// <param name="message"></param>
		/// <exception cref="System.ArgumentOutOfRangeException"/>
		public static void IfGreaterOrEqualTo<T>(T value, T max, string paramName, string? message = null)
			where T : IComparable<T>
			=> IfGreaterOrEqualTo(value, max, static (x, y) => x.CompareTo(y), paramName, message);
		/// <summary>
		/// Throws an <see cref="System.ArgumentOutOfRangeException"/> if <paramref name="value"/> is greater than or equal to <paramref name="max"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <param name="max"></param>
		/// <param name="comparer"></param>
		/// <param name="paramName"></param>
		/// <param name="message"></param>
		/// <exception cref="System.ArgumentOutOfRangeException"/>
		public static void IfGreaterOrEqualTo<T>(T value, T max, IComparer<T> comparer, string paramName, string? message = null)
			=> IfGreaterOrEqualTo(value, max, comparer.Compare, paramName, message);
		/// <summary>
		/// Throws an <see cref="System.ArgumentOutOfRangeException"/> if <paramref name="value"/> is greater than or equal to <paramref name="max"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <param name="max"></param>
		/// <param name="comparison"></param>
		/// <param name="paramName"></param>
		/// <param name="message"></param>
		/// <exception cref="System.ArgumentOutOfRangeException"></exception>
		public static void IfGreaterOrEqualTo<T>(T value, T max, Comparison<T> comparison, string paramName, string? message = null)
		{
			if (comparison(value, max) >= 0)
			{
				if (string.IsNullOrWhiteSpace(message))
					throw new ArgumentOutOfRangeException(paramName);
				else
					throw new ArgumentOutOfRangeException(paramName, message);
			}
		}
	}
}
