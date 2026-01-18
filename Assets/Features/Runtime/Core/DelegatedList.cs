namespace Features
{
	public class DelegatedList<T> : IList<T>
	{
		public Func<int> Count { get; init; }
		public Func<int, T> IndexerGet { get; init; }
		public Action<int, T> IndexerSet { get; init; }
		public Func<bool> IsReadOnly { get; init; }

		public Action<T> Add { get; init; }
		public Action<int, T> Insert { get; init; }
		public Func<T, bool> Remove { get; init; }
		public Action<int> RemoveAt { get; init; }
		public Action Clear { get; init; }
		public Func<T, bool> Contains { get; init; }
		public Func<T, int> IndexOf { get; init; }

		public DelegatedList() { }

		public DelegatedList(IList<T> list)
		{
			this.Count = () => list.Count;
			this.IsReadOnly = () => list.IsReadOnly;
			this.IndexerGet = index => list[index];
			this.IndexerSet = (index, value) => list[index] = value;
			this.Add = list.Add;
			this.Insert = list.Insert;
			this.Remove = list.Remove;
			this.RemoveAt = list.RemoveAt;
			this.Clear = list.Clear;
			this.Contains = list.Contains;
			this.IndexOf = list.IndexOf;
		}

		public DelegatedList(IReadOnlyList<T> readOnlyList)
		{
			this.Count = () => readOnlyList.Count;
			this.IndexerGet = index => readOnlyList[index];

		}

		bool ICollection<T>.IsReadOnly => (IsReadOnly ?? throw new NotImplementedException())();
		int ICollection<T>.Count => (this.Count ?? throw new NotImplementedException())();
		T IList<T>.this[int index] {
			get => (this.IndexerGet ?? throw new NotImplementedException())(index);
			set => (this.IndexerSet ?? throw new NotImplementedException())(index, value);
		}

		void ICollection<T>.Add(T item) => (Add ?? throw new NotImplementedException())(item);
		void IList<T>.Insert(int index, T item) => (Insert ?? throw new NotImplementedException())(index, item);

		bool ICollection<T>.Remove(T item) => (Remove ?? throw new NotImplementedException())(item);
		void IList<T>.RemoveAt(int index) => (RemoveAt ?? throw new NotImplementedException())(index);
		void ICollection<T>.Clear() => (Clear ?? throw new NotImplementedException())();

		bool ICollection<T>.Contains(T item) => (Contains ?? throw new NotImplementedException())(item);
		int IList<T>.IndexOf(T item) => (IndexOf ?? throw new NotImplementedException())(item);

		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			int length = (this as IList<T>).Count;
			for (int i = 0; i < length; i++)
			{
				array[i + arrayIndex] = this.IndexerGet(i);
			}
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			int length = (this as IList<T>).Count;
			for (int i = 0; i < length; i++)
			{
				yield return (this as IList<T>)[i];
			}
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			int length = (this as IList<T>).Count;
			for (int i = 0; i < length; i++)
			{
				yield return (this as IList<T>)[i];
			}
		}

		public static implicit operator DelegatedList<T>(T[] array) => new(list: array);
		public static implicit operator DelegatedList<T>(List<T> list) => new(list: list);
	}
}
