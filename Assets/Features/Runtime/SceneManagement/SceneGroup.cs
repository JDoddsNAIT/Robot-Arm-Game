using System.Linq;
using Eflatun.SceneReference;

namespace Features.SceneManagement
{
	public enum SceneType : byte
	{
		None = 0,
		ActiveScene = 1,
	}

	[Serializable]
	public struct SceneData
	{
		[SerializeField] private SceneType _type;
		[SerializeField] private int _loadOrder;
		[SerializeField] private SceneReference _scene;

		public readonly string Name => _scene.Name;

		public SceneType Type { readonly get => _type; init => _type = value; }
		public int LoadOrder { readonly get => _loadOrder; init => _loadOrder = value; }
		public SceneReference Scene { readonly get => _scene; init => _scene = value; }

		public override readonly string ToString() => Name;
	}

	[CreateAssetMenu(fileName = "NewSceneGroup", menuName = "Scene/Scene Group", order = 1000)]
	public class SceneGroup : ScriptableObject, IList<SceneData>
	{
		[SerializeField] private List<SceneData> _scenes;

		public int Count => _scenes.Count;
		public SceneData this[int index] { get => _scenes[index]; set => _scenes[index] = value; }

		bool ICollection<SceneData>.IsReadOnly => (_scenes as ICollection<SceneData>).IsReadOnly;

		public int IndexOf(SceneData item) => _scenes.IndexOf(item);
		public bool Contains(SceneData item) => _scenes.Contains(item);

		public void Add(SceneData item) => _scenes.Add(item);
		public void Insert(int index, SceneData item) => _scenes.Insert(index, item);
		public bool Remove(SceneData item) => _scenes.Remove(item);
		public void RemoveAt(int index) => _scenes.RemoveAt(index);
		public void Clear() => _scenes.Clear();

		public void CopyTo(SceneData[] array, int arrayIndex) => _scenes.CopyTo(array, arrayIndex);

		public string FindSceneNameByType(SceneType sceneType)
			=> _scenes.FirstOrDefault(s => s.Type == sceneType).Scene.Name;

		public IGrouping<int, SceneData>[] GetLoadGroups(bool descending = false)
			=> _scenes.OrderBy(static sg => sg.LoadOrder).GroupBy(static sg => sg.LoadOrder).ToArray();

		public IEnumerator<SceneData> GetEnumerator() => _scenes.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
