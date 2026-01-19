using System.Linq;
using Features.Persistence;

namespace Features.LogicGates
{
	public class Simulation
	{
		private readonly List<IOGroup> _groups;
		private readonly Dictionary<SerializableGuid, LogicNode> _nodes;

		public Simulation(IEnumerable<LogicNodeData> nodeData)
		{
			_nodes = new();
			var connections = new List<Connection>();
			foreach (var data in nodeData)
			{
				var inputs = new LogicNodeIO[data.Inputs.Length];
				for (int i = 0; i < inputs.Length; i++)
				{
					var point = new Connection.Point() { NodeId = data.Id, Index = i };
					inputs[i] = new LogicNodeIO(data.Inputs[i]) { Id = point.GetHashCode() };
					foreach (Connection.Point connected in data.Inputs[i].Connected)
					{
						var connection = new Connection(point, connected);
						if (!connections.Contains(connection))
							connections.Add(connection);
					}
				}

				var outputs = new LogicNodeIO[data.Outputs.Length];
				for (int o = 0; o < outputs.Length; o++)
				{
					var point = new Connection.Point() { NodeId = data.Id, Index = ~o };
					outputs[o] = new LogicNodeIO(data.Outputs[o]) { Id = point.GetHashCode() };
					foreach (var connected in data.Outputs[o].Connected)
					{
						var connection = new Connection(point, connected);
						if (!connections.Contains(connection))
							connections.Add(connection);
					}
				}

				_nodes.Add(data.Id, new LogicNode(data.Id, data.Buffer, inputs, outputs, data.Behaviour));
			}

			_groups = new();
			var explored = new HashSet<Connection.Point>();
			var toExplore = new Queue<Connection.Point>(capacity: 1);
			while (connections.Count > 0)
			{
				var groupInputs = new List<LogicNodeIO>();
				var groupOutputs = new List<LogicNodeIO>();
				toExplore.Enqueue(connections[0].Start);
				while (toExplore.TryDequeue(out var point))
				{
					if (explored.Contains(point))
						continue;
					explored.Add(point);
					LogicNodeIO nodeIO = _nodes[point.NodeId][point.Index];
					if (point.Index >= 0) 
						groupInputs.Add(nodeIO);
					else
						groupOutputs.Add(nodeIO);

					for (int i = connections.Count - 1; i >= 0; i--)
					{
						if (!connections[i].Contains(point))
							continue;
						var next = connections[i].Other(point);
						if (explored.Contains(next))
							continue;
						connections.RemoveAt(i);
						toExplore.Enqueue(next);
					}
				}

				_groups.Add(new IOGroup(groupInputs, groupOutputs));
			}
			return;
		}

		/// <summary>
		/// Ticks the simulation once.
		/// </summary>
		public void Tick()
		{
			foreach (var group in _groups)
			{
				float value = group.Outputs.Sum(static o => o.Value);
				for (int i = 0; i < group.Inputs.Count; i++)
				{
					group.Inputs[i].Value = value;
				}
			}

			foreach (var node in _nodes.Values)
			{
				node.Update();
			}
		}
	}

	internal record class IOGroup(IReadOnlyList<LogicNodeIO> Inputs, IReadOnlyList<LogicNodeIO> Outputs)
	{
		public static implicit operator (IReadOnlyList<LogicNodeIO> inputs, IReadOnlyList<LogicNodeIO> outputs)(IOGroup value)
		{
			return (value.Inputs, value.Outputs);
		}

		public static implicit operator IOGroup((IReadOnlyList<LogicNodeIO> inputs, IReadOnlyList<LogicNodeIO> outputs) value)
		{
			return new IOGroup(value.inputs, value.outputs);
		}
	}
}
