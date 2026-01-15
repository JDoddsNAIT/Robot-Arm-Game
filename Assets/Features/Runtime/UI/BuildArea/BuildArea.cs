using System.Linq;
using Features.Persistence;

namespace Features.UI
{
	public class BuildArea : MonoBehaviour
	{
		private readonly IDataService<BuildAreaData> _dataService = new FileDataService<BuildAreaData>(new JsonSerializer(prettyPrint: true));
		private readonly Dictionary<SerializableGuid, LogicNodeUI> _nodes = new();

		[SerializeField] private Transform _spawnArea;
		[SerializeField] private GameObject _nodePrefab;

		public void SaveLevelAs(string levelName)
		{
			_dataService.Save(levelName, new() { nodes = _nodes.Values.Select(x => x.Data).ToArray() });
		}

		public void LoadLevel(string levelName)
		{
			if (!_dataService.Load(levelName, out var data))
				throw new Exception();
			_nodes.Clear();
			data.nodes.ForEach(n => AddToBuildArea(n));
		}

		public void AddToBuildArea(LogicNodeData node)
		{
			if (!_nodes.ContainsKey(node.gateId))
			{
				var parent = _spawnArea;
				if (parent == null)
					parent = transform;
				var clone = Instantiate(_nodePrefab, parent);
				_nodes.Add(node.gateId, clone.GetComponent<LogicNodeUI>());
			}
			_nodes[node.gateId].Bind(node);
		}
	}

	[Serializable]
	public class BuildAreaData
	{
		public LogicNodeData[] nodes;
	}
}
