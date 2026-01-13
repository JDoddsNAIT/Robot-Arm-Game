using System.Linq;

namespace Features.SceneManagement
{
	public readonly struct AsyncOperationGroup : IReadOnlyList<AsyncOperation>, ICollection<AsyncOperation>
	{
		private readonly List<AsyncOperation> _operations;

		public float Progress => _operations.Count == 0 ? 0 : _operations.Average(static o => o.progress);
		public bool IsDone => _operations.All(static o => o.isDone);

		public int Count => _operations.Count;

		bool ICollection<AsyncOperation>.IsReadOnly => (_operations as ICollection<AsyncOperation>).IsReadOnly;

		public AsyncOperation this[int index] => _operations[index];

		public AsyncOperationGroup() : this(0) { }
		public AsyncOperationGroup(int capacity) => _operations = new List<AsyncOperation>(capacity);

		public void Add(AsyncOperation operation) => _operations.Add(operation);
		public bool Remove(AsyncOperation operation) => _operations.Remove(operation);
		public bool Contains(AsyncOperation operation) => _operations.Contains(operation);
		public void Clear() => _operations.Clear();
		public void CopyTo(AsyncOperation[] array, int arrayIndex) => _operations.CopyTo(array, arrayIndex);

		public IEnumerator<AsyncOperation> GetEnumerator() => _operations.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
