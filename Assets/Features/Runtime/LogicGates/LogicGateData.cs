namespace Features.LogicGates
{
	[Serializable]
	public class LogicGateData
	{
		public int gateId;
		public GameObject prefab;
		public Vector2 position;
		public ConnectorData[] inputData;
		public ConnectorData[] outputData;
		public GateConfigData[] configData;
	}

	[Serializable]
	public class ConnectorData
	{
		public bool invert;
		public float scale;
	}

	[Serializable]
	public class GateConfigData
	{
		public string name;
		public string value;
	}
}
