#nullable enable

using Features;

namespace Features.Persistence
{
	[Serializable]
	public struct SerializableGuid : IFormattable, IEquatable<SerializableGuid>, IEquatable<Guid>
	{
		[SerializeField, HideInInspector]
		public uint Part1, Part2, Part3, Part4;

		public static SerializableGuid Empty { get; } = new();

		public SerializableGuid(uint part1, uint part2, uint part3, uint part4)
		{
			Part1 = part1;
			Part2 = part2;
			Part3 = part3;
			Part4 = part4;
		}

		public SerializableGuid(Guid guid)
		{
			var bytes = guid.ToByteArray();
			Part1 = BitConverter.ToUInt32(bytes, 00);
			Part2 = BitConverter.ToUInt32(bytes, 04);
			Part3 = BitConverter.ToUInt32(bytes, 08);
			Part4 = BitConverter.ToUInt32(bytes, 12);
		}

		public override readonly string ToString() => this.ToString("X8", null);

		public readonly string ToString(string? format, IFormatProvider? formatProvider)
			=> string.Join('-',
				Part1.ToString(format, formatProvider),
				Part2.ToString(format, formatProvider),
				Part3.ToString(format, formatProvider),
				Part4.ToString(format, formatProvider));

		public readonly string ToHexString() => new System.Text.StringBuilder(capacity: 4)
			.AppendFormat(Helpers.HEX_FORMAT, Part1)
			.AppendFormat(Helpers.HEX_FORMAT, Part2)
			.AppendFormat(Helpers.HEX_FORMAT, Part3)
			.AppendFormat(Helpers.HEX_FORMAT, Part4)
			.ToString();

		public readonly Guid ToGuid()
		{
			Span<byte> bytes = stackalloc byte[16];
			BitConverter.GetBytes(Part1).CopyTo(bytes, 00);
			BitConverter.GetBytes(Part2).CopyTo(bytes, 04);
			BitConverter.GetBytes(Part3).CopyTo(bytes, 08);
			BitConverter.GetBytes(Part4).CopyTo(bytes, 12);
			return new Guid(bytes);
		}

		public override readonly int GetHashCode() => HashCode.Combine(Part1, Part2, Part3, Part4);

		public override readonly bool Equals([NotNullWhen(true)] object obj) => obj switch {
			Guid guid => this.Equals(guid),
			SerializableGuid sGuid => this.Equals(sGuid),
			_ => false,
		};
		public readonly bool Equals(Guid other) => this.Equals((SerializableGuid)other);
		public readonly bool Equals(SerializableGuid other)
			=> Part1 == other.Part1 && Part2 == other.Part2 && Part3 == other.Part3 && Part4 == other.Part4;

		/// <summary>
		/// Creates a new and unique <see cref="SerializableGuid"/>.
		/// </summary>
		/// <returns></returns>
		public static SerializableGuid NewGuid() => Guid.NewGuid().ToSerializableGuid();

		public static SerializableGuid FromHexString(string hexString)
		{
			const ushort hexStringLength = 32, hexBase = 16, partLength =  8, partCount = 4;

			ThrowHelper.IfNullOrWhiteSpace(hexString, nameof(hexString));

			if (hexString.Length != hexStringLength)
				throw ThrowHelper.ArgumentException(Messages.SERIALIZABLE_GUID__IMPROPER_HEX_STRING, nameof(hexString), hexStringLength);

			Span<uint> parts = stackalloc uint[partCount];
			for (int i = 0; i < partCount; i++)
			{
				parts[i] = Convert.ToUInt32(hexString.Substring(startIndex: i * partLength, length: partLength), fromBase: hexBase);
			}
			return new SerializableGuid(parts[0], parts[1], parts[2], parts[3]);
		}

		public static implicit operator Guid(SerializableGuid guid) => guid.ToGuid();
		public static implicit operator SerializableGuid(Guid guid) => new(guid);

		public static bool operator ==(SerializableGuid lhs, SerializableGuid rhs) => lhs.Equals(rhs);
		public static bool operator !=(SerializableGuid lhs, SerializableGuid rhs) => !lhs.Equals(rhs);
	}

	public partial class Messages
	{
		public const string SERIALIZABLE_GUID__IMPROPER_HEX_STRING = "Hex string must be {1} characters long.";
	}

	public partial class Helpers
	{
		public const string HEX_FORMAT = "{0:X8}";

		public static SerializableGuid ToSerializableGuid(this Guid guid) => new(guid);

		public static void CopyTo<T>(this ICollection<T> values, Span<T> span, int index)
		{
			foreach (var item in values)
			{
				span[index] = item;
				index++;
			}
		}

		public static void CopyTo<T>(this IList<T> values, Span<T> span, int index)
		{
			for (int i = 0; i < values.Count; i++)
			{
				span[index + i] = values[i];
			}
		}
	}
}
