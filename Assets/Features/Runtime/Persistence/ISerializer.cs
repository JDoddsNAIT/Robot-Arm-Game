#nullable enable

namespace Features.Persistence
{
	public interface ISerializer
	{
		string Serialize<T>(in T obj);
		T Deserialize<T>(string obj);
	}

	public interface IFileSerializer : ISerializer
	{
		string FileExtension { get; }
	}

	public static partial class Helpers
	{
		public static bool TrySerialize<T>(this ISerializer serializer, in T obj, [NotNullWhen(true)] out string? result)
		{
			result = null;
			try
			{
				result = serializer.Serialize(obj);
				return result is not null;
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				return false;
			}
		}

		public static bool TryDeserialize<T>(this ISerializer serializer, in string obj, [NotNullWhen(true)] out T? result)
		{
			result = default;
			try
			{
				result = serializer.Deserialize<T>(obj);
				return result is not null;
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				return false;
			}
		}
	}
}
