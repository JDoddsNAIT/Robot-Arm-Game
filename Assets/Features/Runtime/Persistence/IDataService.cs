#nullable enable

using System.Linq;

namespace Features.Persistence
{
	public interface IDataService<TData>
	{
		/// <summary>
		/// Deletes the <typeparamref name="TData"/> with the given <paramref name="name"/>.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		bool Delete(string name);
		/// <summary>
		/// Checks if any <typeparamref name="TData"/> exists with the given <paramref name="name"/>.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		bool Exists(string name);
		/// <summary>
		/// Saves the given <typeparamref name="TData"/>.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="overwrite"></param>
		/// <returns></returns>
		bool Save(string name, TData data, bool overwrite = true);
		/// <summary>
		/// Loads the <typeparamref name="TData"/> with the given <paramref name="name"/>.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		bool Load(string name, [NotNullWhen(true)] out TData? data);
		/// <summary>
		/// Gets all <typeparamref name="TData"/> managed by this service.
		/// </summary>
		/// <returns></returns>
		IEnumerable<string> GetAll();
	}

	public partial class Messages
	{
		public const string DATA_SERVICE__DATA_DELETED = "{0}: Deleted {1} entries (out of {2}).";
	}

	public partial class Helpers
	{
		public static bool DeleteAll<TData>(this IDataService<TData> service)
		{
			var names = service.GetAll().ToArray();
			int total = names.Length;
			int count = names.Count(service.Delete);
			Debug.LogFormat(Messages.DATA_SERVICE__DATA_DELETED, service, count, total);
			return true;
		}
	}
}
