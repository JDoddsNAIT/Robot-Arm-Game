using System.Linq;

namespace Features.LogicGates
{
	public class Network
	{
		private readonly GateInput[] _inputs;
		private readonly GateOutput[] _outputs;

		public GateInput[] Inputs { get => _inputs; init => _inputs = value; }
		public GateOutput[] Outputs { get => _outputs; init => _outputs = value; }

		public void Update()
		{
			var value = 0f;
			for (int o = 0; o < _outputs.Length; o++)
			{
				value += _outputs[o].Value;
			}

			for (int i = 0; i < _inputs.Length; i++)
			{
				_inputs[i].Value = value;
			}
		}
	}

	public class Simulation : MonoBehaviour
	{
		private bool _started;
		private bool _running;
		private Network[] _networks;

		[SerializeField] private List<Connection> _connections = new();
		[SerializeField] private List<LogicGate> _gates = new();

		public void AddToSimulation(Connection connection)
		{
			if (!_connections.Contains(connection))
				_connections.Add(connection);
		}

		public void RemoveFromSimulation(Connection connection) => _connections.Remove(connection);

		public void AddToSimulation(LogicGate gate)
		{
			if (!_gates.Contains(gate))
				_gates.Add(gate);
		}

		public void RemoveFromSimulation(LogicGate gate) => _gates.Remove(gate);

		[ContextMenu(nameof(StartSimulation))]
		public void StartSimulation()
		{
			_networks = GetNetworks(_connections).ToArray();

			for (int i = 0; i < _gates.Count; i++)
			{
				_gates[i].OnSimulationStart();
			}

			_started = true;
			PauseSimulation();
		}

		public void Update()
		{
			if (!_running || !enabled)
				return;

			for (int n = 0; n < _networks.Length; n++)
			{
				_networks[n].Update();
			}

			for (int g = 0; g < _gates.Count; g++)
			{
				_gates[g].OnSimulationUpdate();
			}
		}

		[ContextMenu(nameof(PauseSimulation))]
		private void PauseSimulation() => PauseSimulation(null);

		public void PauseSimulation(bool? state = null)
		{
			if (!_started) return;
			_running = state switch {
				null => !_running,
				false => false,
				true => true,
			};
		}

		[ContextMenu(nameof(StopSimulation))]
		public void StopSimulation()
		{
			_started = false;
			_running = false;
		}

		private static IEnumerable<Network> GetNetworks(IEnumerable<Connection> connections)
		{
			var explored = new HashSet<GateConnector>();
			var unexplored = connections.ToList();
			var toExplore = new Queue<GateConnector>(capacity: 1);

			while (unexplored.Count > 0)
			{
				toExplore.Enqueue(unexplored[0].ItemA);
				var network = createNetwork();
				yield return new Network() {
					Inputs = network.OfType<GateInput>().ToArray(),
					Outputs = network.OfType<GateOutput>().ToArray(),
				};
			}

			IEnumerable<GateConnector> createNetwork()
			{
				while (toExplore.TryDequeue(out var node))
				{
					if (node == null || explored.Contains(node))
						continue;
					explored.Add(node);
					yield return node;

					for (int i = unexplored.Count - 1; i >= 0; i--)
					{
						if (!unexplored[i].Contains(node))
							continue;
						var next = unexplored[i].Other(node);
						if (next == null || explored.Contains(next))
							continue;

						toExplore.Enqueue(next);
						unexplored.RemoveAt(i);
					}
				}
			}
		}
	}
}
