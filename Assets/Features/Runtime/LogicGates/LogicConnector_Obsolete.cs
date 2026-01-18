namespace Features.LogicGates
{
	public enum ConnectorType { None, Input, Output }

	[Obsolete]
	public class LogicConnector_Obsolete : IEquatable<LogicConnector_Obsolete>
	{
		private static uint _nextId = 0;
		private float _value;

		public uint Id { get; } = _nextId++;
		public ConnectorType Type { get; init; }
		public bool Invert { get; init; }
		public float Scale { get; init; }

		public float Value {
			get => (Invert ? Mathf.Approximately(_value, 0f) ? 1 : 0 : _value) * Scale;
			set => _value = value;
		}

		public LogicConnector_Obsolete(ConnectorType type, bool invert = false, float scale = 1f)
		{
			Type = type;
			Invert = invert;
			Scale = scale;
			Value = 0f;
		}

		public override int GetHashCode() => this.Id.GetHashCode();
		public override bool Equals(object obj) => obj is LogicConnector_Obsolete other && other.Id == this.Id;
		public bool Equals(LogicConnector_Obsolete other) => this.Id == other.Id;

		public static Network_Obsolete.Connection operator +(LogicConnector_Obsolete lhs, LogicConnector_Obsolete rhs) => new(lhs, rhs);
	}
}
