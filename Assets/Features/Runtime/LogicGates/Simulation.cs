using System.Linq;
using Features.Persistence;
using Features.UI;

namespace Features.LogicGates
{
	public class Simulation
	{
		private readonly Dictionary<SerializableGuid, LogicGate> _logicGates;
		private readonly Network[] _networks;

		public Simulation(IEnumerable<LogicData> nodes)
		{
			var gates = new Dictionary<SerializableGuid, LogicGate>();
			var connections = new List<(LogicData.Connection a, LogicData.Connection b)>();

			foreach (var data in nodes)
			{

				var inputs = new LogicConnector[data.inputCount];
				var outputs = new LogicConnector[data.outputCount];

				for (int c = 0; c < data.connectors.Length; c++)
				{
					var connector = data.connectors[c];
					if (connector.index >= 0)
					{
						inputs[connector.index] = new(ConnectorType.Input, connector.invert, connector.scale);
					}
					else
					{
						outputs[~connector.index] = new(ConnectorType.Output, connector.invert, connector.scale);
					}

					var connection
				}
			}
		}

		public void Step()
		{
			for (int n = 0; n < _networks.Length; n++)
			{
				var network = _networks[n];
				var value = network.outputs.Sum(static o => o.Value);
				for (int i = 0; i < network.inputs.Length; i++)
				{
					network.inputs[i].Value = value;
				}
			}

			foreach (var gate in _logicGates.Values)
			{
				gate.Process();
			}
		}

		public void Step(int steps)
		{
			for (int s = 0; s < steps; s++)
			{
				Step();
			}
		}

		private static IEnumerable<Network> GetNetworks(IEnumerable<Network.Connection> connections)
		{
			var explored = new HashSet<uint>();
			var remaining = connections.ToList();
			var toExplore = new Queue<LogicConnector>(capacity: 1);
			var inputs = new List<LogicConnector>();
			var outputs = new List<LogicConnector>();

			while (remaining.Count > 0)
			{
				toExplore.Enqueue(remaining[0].ItemA);
				while (toExplore.TryDequeue(out var node))
				{
					if (node is null || explored.Contains(node.Id))
						continue;
					explored.Add(node.Id);
					if (node.Type == ConnectorType.Input)
						inputs.Add(node);
					else if (node.Type == ConnectorType.Output)
						outputs.Add(node);

					for (int i = remaining.Count - 1; i >= 0; i--)
					{
						if (!remaining[i].Contains(node))
							continue;
						var next = remaining[i].Other(node);
						if (explored.Contains(next.Id))
							continue;

						remaining.RemoveAt(i);
						toExplore.Enqueue(next);
					}
				}

				yield return new Network() { inputs = inputs.ToArray(), outputs = outputs.ToArray() };
				inputs.Clear();
				outputs.Clear();
			}
			yield break;
		}
	}

	public class Network
	{
		public LogicConnector[] inputs, outputs;

		[Serializable]
		public struct Connection : IEquatable<Connection>
		{
			[SerializeField] private LogicConnector _a, _b;

			public LogicConnector ItemA { readonly get => _a; init => _a = value; }
			public LogicConnector ItemB { readonly get => _b; init => _b = value; }

			public Connection(LogicConnector a, LogicConnector b)
			{
				_a = a; _b = b;
			}

			public readonly bool Contains(LogicConnector obj)
			{
				EnsurePointsNotNull();
				ThrowHelper.IfNull(obj, nameof(obj));
				return _a.Equals(obj) || _b.Equals(obj);
			}

			public readonly LogicConnector Other(LogicConnector obj)
			{
				EnsurePointsNotNull();
				ThrowHelper.IfNull(obj, nameof(obj));
				if (obj.Equals(_a))
					return _b;
				else if (obj.Equals(_b))
					return _a;
				else
					throw new ArgumentException("The object is not part of this connection.", nameof(obj));
			}

			public override readonly int GetHashCode()
			{
				EnsurePointsNotNull();
				(var a, var b) = (_a.Id, _b.Id);
				if (b < a)
					(a, b) = (b, a);
				return HashCode.Combine(a, b);
			}
			public override readonly bool Equals([NotNullWhen(true)] object obj)
				=> obj is Connection other && Equals(other);
			public readonly bool Equals(Connection other)
				=> GetHashCode() == other.GetHashCode();

			private readonly void EnsurePointsNotNull()
			{
				if (_a is null || _b is null)
					throw new InvalidOperationException("One or more points in the connection are null.");
			}
		}
	}
}
