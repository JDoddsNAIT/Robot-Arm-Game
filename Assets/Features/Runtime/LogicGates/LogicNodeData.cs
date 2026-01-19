using Features.Persistence;

namespace Features.LogicGates
{
	public delegate void ProcessInputsDelegate(ReadOnlySpan<float> inputValues, Span<float> outputValues);

	public readonly struct LogicNodeData
	{
		public SerializableGuid Id { get; init; }
		public int Buffer { get; init; }
		public IOData[] Inputs { get; init; }
		public IOData[] Outputs { get; init; }
		public ProcessInputsDelegate Behaviour { get; init; }
	}

	public readonly struct IOData
	{
		public bool Invert { get; init; }
		public float Scale { get; init; }
		public Connection.Point[] Connected { get; init; }
	}

	public readonly struct Connection : IEquatable<Connection>
	{
		public Point Start { get; init; }
		public Point End { get; init; }

		public Connection(Point start, Point end)
		{
			this.Start = start;
			this.End = end;
		}

		public bool Contains(Point point) => Start == point || End == point;
		public Point Other(Point point)
		{
			if (point == Start)
				return End;
			else if (point == End)
				return Start;
			else
				throw new ArgumentOutOfRangeException(nameof(point));
		}

		public bool Equals(Connection other) => GetHashCode() == other.GetHashCode();
		public override bool Equals([NotNullWhen(true)] object obj)
			=> obj is Connection other && Equals(other);
		public override int GetHashCode()
		{
			(int a, int b) = (Start.GetHashCode(), End.GetHashCode());
			if (b < a) (a, b) = (b, a);
			return HashCode.Combine(a, b);
		}

		public static bool operator ==(Connection lhs, Connection rhs) => lhs.Equals(rhs);
		public static bool operator !=(Connection lhs, Connection rhs) => !(lhs == rhs);

		public readonly struct Point : IEquatable<Point>
		{
			public SerializableGuid NodeId { get; init; }
			public int Index { get; init; }

			public override int GetHashCode() => HashCode.Combine(NodeId, Index);
			public override bool Equals([NotNullWhen(true)] object obj)
				=> obj is Point other && Equals(other);
			public bool Equals(Point other)
				=> NodeId == other.NodeId && Index == other.Index;

			public static bool operator ==(Point a, Point b) => a.Equals(b);
			public static bool operator !=(Point a, Point b) => !(a == b);
		}
	}
}
