using System.Linq;
using Features.Persistence;

namespace Features.UI
{
	public abstract class LogicNodeUI : MonoBehaviour, IDataOwner<LogicNodeData>
	{
		bool _bound = false;

		[SerializeField] private SerializableGuid _gateId;
		[SerializeField] private GameObject _inputPrefab;
		[SerializeField] private Transform _inputParent;
		[SerializeField] private GameObject _outputPrefab;
		[SerializeField] private Transform _outputParent;
		[Space]
		[SerializeField] private List<LogicNodeConnector> _inputs;
		[SerializeField] private List<LogicNodeConnector> _outputs;

		public SerializableGuid Id { get => _gateId; set => _gateId = value; }
		public LogicNodeData Data => !_bound ? null : new LogicNodeData() {
			gateId = _gateId,
			position = transform.localPosition,
			type = this.GateType,
			name = this.name,
			configOptions = this.GetConfigValues()
				.Select(x => new LogicNodeData.ConfigOption() { name = x.Key, value = x.Value })
				.ToArray(),
			inputCount = _inputs.Count,
			outputCount = _outputs.Count,
			connectors = _inputs.Select(GetData).Concat(_outputs.Select(GetData)).ToArray()
		};

		public void Bind(in LogicNodeData data)
		{
			gameObject.name = data.name;
			transform.localPosition = data.position;

			Assert.Equals(GateType, data.type);

			for (int i = 0; i < data.inputCount; i++)
			{
				var clone = Instantiate(_inputPrefab, _inputParent);
				Assert.IsTrue(clone.TryGetComponent(out LogicNodeConnector connector));
				connector.Index = i;
				_inputs.Add(connector);
			}

			for (int i = 0; i < data.outputCount; i++)
			{
				var clone = Instantiate(_outputPrefab, _outputParent);
				Assert.IsTrue(clone.TryGetComponent(out LogicNodeConnector connector));
				connector.Index = ~i;
				_inputs.Add(connector);
			}

			for (int i = 0; i < data.connectors.Length; i++)
			{
				var obj = data.connectors[i];
				if (obj.index < 0)
				{
					int index = ~obj.index;
					_outputs[index].Invert = obj.invert;
					_outputs[index].Scale = obj.scale;
					_outputs[index].Connected = obj.connections;
				}
				else
				{
					int index = obj.index;
					_outputs[index].Invert = obj.invert;
					_outputs[index].Scale = obj.scale;
					_outputs[index].Connected = obj.connections;
				}
			}

			this.BindConfig(data.configOptions.ToDictionary(x => x.name, x => x.value));

			_bound = true;
		}

		public void Unbind()
		{
			_bound = false;
		}

		protected abstract GateType GateType { get; }
		protected abstract void BindConfig(IReadOnlyDictionary<string, string> config);
		protected abstract IReadOnlyDictionary<string, string> GetConfigValues();

		private static LogicNodeData.ConnectorData GetData(LogicNodeConnector x) => new() {
			index = x.Index,
			invert = x.Invert,
			scale = x.Scale,
			connections = x.Connected.ToArray(),
		};
	}
}
