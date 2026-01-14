using System.Linq;
using Features.Persistence;

namespace Features.LogicGates
{
	[Serializable]
	public class Network
	{
		[SerializeField]
		private GateInput[] _inputs;
		[SerializeField]
		private GateOutput[] _outputs;

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

	public class Simulation : MonoBehaviour, IDataOwner<SimulationData>
	{
		private Network[] _networks;
		private bool _running;

		[SerializeField] private SerializableGuid _levelId = SerializableGuid.NewGuid();
		[Space]
		[SerializeField] private List<Connection> _connections = new();
		[SerializeField] private List<LogicGate> _gates = new();
		[Space]
		[SerializeField] private UnityEvent _onSimulationStart = new();
		[SerializeField] private UnityEvent _onSimulationUpdate = new();
		[SerializeField] private UnityEvent _onSimulationStop = new();

		public SerializableGuid Id => _levelId;
		public SimulationData Data => this.GetSimulationData();

		public event UnityAction OnSimulationStart {
			add => _onSimulationStart.AddListener(value);
			remove => _onSimulationStart.RemoveListener(value);
		}
		public event UnityAction OnSimulationUpdate {
			add => _onSimulationUpdate.AddListener(value);
			remove => _onSimulationUpdate.RemoveListener(value);
		}
		public event UnityAction OnSimulationStop {
			add => _onSimulationStop.AddListener(value);
			remove => _onSimulationStop.RemoveListener(value);
		}

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
			_onSimulationStart.Invoke();
			_networks = GetNetworks(_connections).ToArray();

			for (int i = 0; i < _gates.Count; i++)
			{
				_gates[i].OnSimulationStart();
			}

			PauseSimulation(state: true);
		}

		public void Update()
		{
			if (!_running || !enabled)
				return;
			Tick();
		}

		/// <summary>
		/// Advances the simulation by one.
		/// </summary>
		public void Tick()
		{
			if (_networks is null)
				throw new InvalidOperationException("Cannot tick a simulation that has not been started.");

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
			if (_networks is null) return;
			_running = state switch {
				null => !_running,
				false => false,
				true => true,
			};
		}

		[ContextMenu(nameof(StopSimulation))]
		public void StopSimulation()
		{
			_networks = null;
			_running = false;
			_onSimulationStop.Invoke();
		}

		public void StartStopSimulation()
		{
			if (_networks is null)
				StartSimulation();
			else
				StopSimulation();
		}

		private static IEnumerable<Network> GetNetworks(IEnumerable<Connection> connections)
		{
			var explored = new HashSet<int>();
			var remaining = connections.ToList();
			var toExplore = new Queue<GateConnector>(capacity: 1);
			var inputs = new List<GateInput>();
			var outputs = new List<GateOutput>();

			while (remaining.Count > 0)
			{
				toExplore.Enqueue(remaining[0].ItemA);
				while (toExplore.TryDequeue(out var node))
				{
					if (node == null || explored.Contains(node.Id))
						continue;
					explored.Add(node.Id);
					if (node is GateInput input)
						inputs.Add(input);
					else if (node is GateOutput output)
						outputs.Add(output);

					for (int i = remaining.Count - 1; i >= 0; i--)
					{
						if (!remaining[i].Contains(node))
							continue;
						var next = remaining[i].Other(node);
						if (next == null || explored.Contains(next.Id))
							continue;

						toExplore.Enqueue(next);
						remaining.RemoveAt(i);
					}
				}

				yield return new Network() { Inputs = inputs.ToArray(), Outputs = outputs.ToArray() };
				inputs.Clear();
				outputs.Clear();
			}
			yield break;
		}

		public void Bind(in SimulationData data)
		{
			for (int i = 0; i < data.LogicGates.Length; i++)
			{
				var gateData = data.LogicGates[i];

				var clone = Instantiate(gateData.prefab, transform);
				clone.transform.position = gateData.position;
				var gate = clone.GetComponent<LogicGate>();
				Assert.NotNull(gate);

				var length = Mathf.Min(gate.Inputs.Count, gateData.inputData.Length);
				for (int c = 0; c < length; c++)
					gate.Inputs[c].SetConnectorData(gateData.inputData[c]);

				length = Mathf.Min(gate.Outputs.Count, gateData.outputData.Length);
				for (int c = 0; c < length; c++)
					gate.Outputs[c].SetConnectorData(gateData.outputData[c]);

				gate.SetConfigData(gateData.configData);

				_gates.Add(gate);
			}

			for (int i = 0; i < data.Connections.Length; i++)
			{
				_connections.Add(data.Connections[i].GetConnection(_gates));
			}
		}

		private SimulationData GetSimulationData()
		{
			var gateData = new LogicGateData[_gates.Count];
			var connectionData = new ConnectionData[_connections.Count];

			for (int gateId = 0; gateId < _gates.Count; gateId++)
			{
				var gate = _gates[gateId];

				gateData[gateId] = new LogicGateData() {
					gateId = gateId,
					prefab = gate.Prefab,
					// LogicGate component is expected to be a direct child of the simulation.
					position = gate.transform.localPosition,
					configData = gate.GetConfigData(),
					inputData = getConnectorData(gateId, gate.Inputs, ConnectionPointData.PointType.Input),
					outputData = getConnectorData(gateId, gate.Outputs, ConnectionPointData.PointType.Output),
				};
			}

			return new SimulationData() {
				LevelId = _levelId,
				LogicGates = gateData,
				Connections = connectionData,
			};

			ConnectorData[] getConnectorData(int gateId, IReadOnlyList<GateConnector> points, ConnectionPointData.PointType pointType)
			{
				var result = new ConnectorData[points.Count];
				for (int pointId = 0; pointId < points.Count; pointId++)
				{
					result[pointId] = points[pointId].GetConnectorData();
					var point = points[pointId];
					for (int c = 0; c < _connections.Count; c++)
					{
						connectionData[c] ??= new();
						var data = new ConnectionPointData() {
							gateId = gateId,
							pointId = pointId,
							pointType = pointType
						};

						if (_connections[c].ItemA.Equals(point))
							connectionData[c].pointA = data;
						else if (_connections[c].ItemB.Equals(point))
							connectionData[c].pointB = data;
					}
				}
				return result;
			}
		}

		public void Unbind()
		{
			_connections.Clear();
			_gates.Clear();
		}
	}
}
