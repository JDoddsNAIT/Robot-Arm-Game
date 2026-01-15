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
		public IOSettings[] inputSettings;
		public int outputCount;
		public IOSettings[] outputSettings;
		public Connection[] connections;

		SerializableGuid IData.Owner { get => gateId; set => gateId = value; }

		[Serializable]
		public struct IOSettings
		{
			public bool invert;
			public float scale;
		}

		[Serializable]
		public struct ConfigOption
		{
			public string name;
			public string value;
		}

		[Serializable]
		public struct Connection
		{
			public SerializableGuid node;
			public int index;
		}
	}
}
