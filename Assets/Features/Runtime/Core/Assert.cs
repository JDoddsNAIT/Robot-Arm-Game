#nullable enable

namespace Features
{
	/// <summary>Static helper class for negative-space programming patterns.</summary>
	/// <remarks>If any assertion fails, an <see cref="AssertionFailedException"/> will be thrown.</remarks>
	public static class Assert
	{
		/// <summary>
		/// Asserts that a <paramref name="condition"/> is <see langword="true"/>.
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="message"></param>
		/// <exception cref="AssertionFailedException"/>
		public static void IsTrue([DoesNotReturnIf(false)] bool? condition, string? message = null)
		{
			if (condition is not true) throw new AssertionFailedException(message);
		}

		/// <summary><inheritdoc cref=" IsTrue(bool?, string?)"/></summary>
		/// <param name="condition"></param>
		/// <param name="format"></param>
		/// <param name="args"></param>
		/// <exception cref="AssertionFailedException"></exception>
		public static void IsTrue([DoesNotReturnIf(false)] bool? condition, string format, params object[] args)
		{
			if (condition is not true) throw new AssertionFailedException(Utils.FormatSafe(format, args));
		}

		/// <summary>
		/// Asserts that a <paramref name="condition"/> is not <see langword="true"/>.
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="message"></param>
		/// <exception cref="AssertionFailedException"></exception>
		public static void NotTrue([DoesNotReturnIf(true)] bool? condition, string? message = null)
		{
			if (condition is true) throw new AssertionFailedException(message);
		}
		/// <summary><inheritdoc cref="NotTrue(bool?, string?)"/></summary>
		/// <param name="condition"></param>
		/// <param name="format"></param>
		/// <param name="args"></param>
		/// <exception cref="AssertionFailedException"></exception>
		public static void NotTrue([DoesNotReturnIf(true)] bool? condition, string format, params object?[] args)
		{
			if (condition is true) throw new AssertionFailedException(Utils.FormatSafe(format, args));
		}

		/// <summary>
		/// Asserts that a <paramref name="condition"/> is <see langword="false"/>.
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="message"></param>
		/// <exception cref="AssertionFailedException"></exception>
		public static void IsFalse([DoesNotReturnIf(true)] bool? condition, string? message = null)
		{
			if (condition is not false) throw new AssertionFailedException(message);
		}
		/// <summary><inheritdoc cref="IsFalse(bool?, string?)"/></summary>
		/// <param name="condition"></param>
		/// <param name="format"></param>
		/// <param name="args"></param>
		/// <exception cref="AssertionFailedException"></exception>
		public static void IsFalse([DoesNotReturnIf(true)] bool? condition, string format, params object[] args)
		{
			if (condition is not false) throw new AssertionFailedException(Utils.FormatSafe(format, args));
		}

		/// <summary>
		/// Asserts that a <paramref name="condition"/> is not <see langword="false"/>.
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="message"></param>
		/// <exception cref="AssertionFailedException"></exception>
		public static void NotFalse([DoesNotReturnIf(false)] bool? condition, string? message = null)
		{
			if (condition is false) throw new AssertionFailedException(message);
		}

		/// <summary><inheritdoc cref="NotFalse(bool?, string?)"/></summary>
		/// <param name="condition"></param>
		/// <param name="format"></param>
		/// <param name="args"></param>
		/// <exception cref="AssertionFailedException"></exception>
		public static void NotFalse([DoesNotReturnIf(false)] bool? condition, string format, params object?[] args)
		{
			if (condition is false) throw new AssertionFailedException(Utils.FormatSafe(format, args));
		}

		/// <summary>Asserts that an object is not <see langword="null"/>.</summary>
		/// <param name="obj"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		/// <exception cref="AssertionFailedException"></exception>
		public static void NotNull([NotNull] object? obj, string message = Messages.ASSERT_UNEXPECTED_NULL)
		{
			switch (obj)
			{
				case null:
				case Object uObj when uObj == null:
					throw new AssertionFailedException(message);
			}
		}
		/// <summary><inheritdoc cref="NotNull(object?, string)"/></summary>
		/// <param name="obj"></param>
		/// <param name="format"></param>
		/// <param name="args"></param>
		/// <exception cref="AssertionFailedException"></exception>
		public static void NotNull([NotNull] object? obj, string format, params object[] args)
		{
			switch (obj)
			{
				case null:
				case Object uObj when uObj == null:
					throw new AssertionFailedException(string.Format(format, args));
			}
		}

		/// <summary>Asserts that two objects are equal.</summary>
		/// <param name="objA"></param>
		/// <param name="objB"></param>
		/// <returns></returns>
		public static new void Equals(object? objA, object? objB)
			=> Equals(objA, objB, message: null);
		/// <summary><inheritdoc cref="Equals(object?, object?)"/></summary>
		/// <param name="objA"></param>
		/// <param name="objB"></param>
		/// <param name="message"></param>
		/// <exception cref="AssertionFailedException"></exception>
		public static void Equals(object? objA, object? objB, string? message = null)
		{
			switch (objA, objB)
			{
				case (null, null):
				case (Object a, null) when a == null:
				case (null, Object b) when b == null:
				case (Object a1, Object b1) when a1 == b1:
				case (not null, not null) when object.Equals(objA, objB):
					return;
				default:
					throw new AssertionFailedException(message);
			}
		}
		/// <summary><inheritdoc cref="Equals(object?, object?)"/></summary>
		/// <param name="objA"></param>
		/// <param name="objB"></param>
		/// <param name="format"></param>
		/// <param name="args"></param>
		/// <exception cref="AssertionFailedException"></exception>
		public static void Equals(object? objA, object? objB, string format, params object[] args)
		{
			switch (objA, objB)
			{
				case (null, null):
				case (Object a, null) when a == null:
				case (null, Object b) when b == null:
				case (Object a1, Object b1) when a1 == b1:
				case (not null, not null) when object.Equals(objA, objB):
					return;
				default:
					throw new AssertionFailedException(Utils.FormatSafe(format, args));
			}
		}

		/// <summary>Asserts that a given <paramref name="action"/> throws a certain type of <paramref name="exception"/>.</summary>
		/// <param name="exception">The expected exception type. Use <see langword="null"/> to expect no exception to be thrown.</param>
		/// <param name="action"></param>
		/// <param name="message"></param>
		/// <exception cref="AssertionFailedException"></exception>
		public static void Throws(Type? exception, Action action, string message = Messages.ASSERT_UNEXPECTED_EXCEPTION)
		{
			if (exception?.IsAssignableTo(typeof(Exception)) is false)
				exception = null;

			try
			{
				action?.Invoke();

				if (exception is not null)
					throw new AssertionFailedException(Utils.FormatSafe(message, exception, "no exception"));
			}
			catch (AssertionFailedException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new AssertionFailedException(Utils.FormatSafe(message, exception, ex.GetType()));
			}
		}
	}

	public partial class Messages
	{
		public const string ASSERT_UNEXPECTED_NULL = "Unexpected null object detected."
			, ASSERT_UNEXPECTED_NON_NULL = "Unexpected null object detected."
			, ASSERT_UNEXPECTED_EXCEPTION = "Expected exception '{0}', but '{1}' was thrown."
			;
	}

	[Serializable]
	public class AssertionFailedException : Exception
	{
		private const string FORMAT = "Assertion Failed: {0}",
			NO_INFO = "No additional information provided.";

		public AssertionFailedException() : this(message: null) { }
		public AssertionFailedException(string? message)
			: base(string.Format(FORMAT, message ?? NO_INFO))
		{ }
		protected AssertionFailedException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
