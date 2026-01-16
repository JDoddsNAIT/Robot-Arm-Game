using System.Linq;
using Features.LogicGates;
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

		public void AddToBuildArea(LogicData node)
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

		public Simulation GetSimulation()
		{
			var logicGates = new Dictionary<SerializableGuid, LogicGate>(_nodes.Count);
			var connections = new List<IntermediateConnection>();
			foreach (var node in _nodes.Values)
			{
				var data = node.Data;

				var inputs = new LogicConnector[data.inputCount];
				var outputs = new LogicConnector[data.outputCount];
				foreach (var obj in data.connectors)
				{
					int index;
					if (obj.index >= 0)
					{
						index = obj.index;
						inputs[index] = new LogicConnector() {
							Type = ConnectorType.Input,
							Invert = obj.invert,
							Scale = obj.scale,
						};
					}
					else
					{
						index = ~obj.index;
						outputs[index] = new LogicConnector() {
							Type = ConnectorType.Output,
							Invert = obj.invert,
							Scale = obj.scale,
						};
					}

					var start = new LogicData.Connection() { node = node.Id, index = obj.index };
					foreach (var connected in obj.connections)
					{
						if (connections.Contains((start, connected)) || connections.Contains((connected, start)))
							continue;
						connections.Add((start, connected));
					}
				}

				var gate = new LogicGate(node.GetBehaviour(), inputs, outputs);
				logicGates.Add(node.Id, gate);
			}

			return new Simulation(logicGates.Values, connections.Select(c => c.ToConnection(logicGates)));
		}
	}

	[Serializable]
	public class BuildAreaData
	{
		public LogicData[] nodes;
	}

	internal record struct IntermediateConnection(LogicData.Connection A, LogicData.Connection B)
	{
		public static implicit operator (LogicData.Connection a, LogicData.Connection b)(IntermediateConnection value)
		{
			return (value.A, value.B);
		}

		public static implicit operator IntermediateConnection((LogicData.Connection a, LogicData.Connection b) value)
		{
			return new IntermediateConnection(value.a, value.b);
		}

		public readonly Connection ToConnection(IReadOnlyDictionary<SerializableGuid, LogicGate> gates)
		{
			LogicConnector getConnector(LogicData.Connection connection) => connection.index >= 0
				? gates[connection.node].Inputs[connection.index]
				: gates[connection.node].Outputs[~connection.index];
			return new Connection(getConnector(A), getConnector(B));
		}
	}
}
