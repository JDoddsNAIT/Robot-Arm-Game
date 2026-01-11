#nullable enable

namespace Features.Persistence
{
	public interface IData
	{
		SerializableGuid Owner { get; set; }
	}

	public interface IDataOwner<TData> where TData : IData, new()
	{
		SerializableGuid Id { get; }
		TData? Data { get; }
		void Bind(in TData data);
		void Unbind();
	}

	public partial class Helpers
	{
		public static bool IsBound<TData>(this IDataOwner<TData> owner) where TData : IData, new()
			=> owner.Data is not null && owner.Data.Owner == owner.Id;
	}
}
