using Features.Persistence;

namespace Features.LogicGates
{
	[Serializable]
	public class SimulationData : IData
	{
		[field: SerializeField] public SerializableGuid LevelId { get; set; }
		[field: SerializeField] public LogicGateData[] LogicGates { get; set; }
		[field: SerializeField] public ConnectionData[] Connections { get; set; }

		SerializableGuid IData.Owner { get => LevelId; set => LevelId = value; }
	}

	[Serializable]
	public class ConnectionData
	{
		public ConnectionPointData pointA;
		public ConnectionPointData pointB;

		public Connection GetConnection(IReadOnlyList<LogicGate> gates)
			=> pointA.GetConnector(gates) + pointB.GetConnector(gates);
	}

	[Serializable]
	public struct ConnectionPointData
	{
		public enum PointType { Input, Output }

		public int gateId;
		public PointType pointType;
		public int pointId;

		public readonly GateConnector GetConnector(IReadOnlyList<LogicGate> gates)
			=> gates[gateId][pointType][pointId];
	}
}
