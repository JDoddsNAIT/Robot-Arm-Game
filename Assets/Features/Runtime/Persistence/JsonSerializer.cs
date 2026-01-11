#nullable enable

namespace Features.Persistence
{
	public sealed class JsonSerializer : IFileSerializer
	{
		private readonly bool _prettyPrint;
		public string FileExtension => "json";

		public JsonSerializer(bool prettyPrint = true) => _prettyPrint = prettyPrint;
		public string Serialize<T>(in T obj) => JsonUtility.ToJson(obj, _prettyPrint);
		public T Deserialize<T>(string obj) => JsonUtility.FromJson<T>(obj);
	}
}
