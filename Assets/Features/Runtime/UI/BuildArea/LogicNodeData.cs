using Features.Persistence;

namespace Features.UI
{
	public enum GateType
	{
		Custom,
		AND, OR, XOR,
		Display,
		Control,
	}

	[Serializable]
	public class LogicNodeData : IData
	{
		public SerializableGuid gateId;
		public Vector2 position;
		public GateType type;
		public string name;
		public ConfigOption[] configOptions;
		public int inputCount;
		public int outputCount;
		public ConnectorData[] connectors;

		SerializableGuid IData.Owner { get => gateId; set => gateId = value; }

		[Serializable]
		public struct ConfigOption
		{
			public string name;
			public string value;
		}

		[Serializable]
		public struct ConnectorData
		{
			public int index;
			public bool invert;
			public float scale;
			public Connection[] connections;
		}

		[Serializable]
		public struct Connection
		{
			public SerializableGuid node;
			public int index;
		}
	}
}
