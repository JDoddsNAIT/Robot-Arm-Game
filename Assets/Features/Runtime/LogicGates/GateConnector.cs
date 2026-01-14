namespace Features.LogicGates
{
	public abstract partial class GateConnector : MonoBehaviour,
		IEquatable<GateConnector>
	{
		protected int _id = _nextId++;
		protected Simulation _simulation;

		public int Id => _id;
		public abstract float Value { get; set; }

		public abstract ConnectorData GetConnectorData();
		public abstract void SetConnectorData(ConnectorData value);

		protected virtual void Start()
		{
			_simulation = transform.parent.GetComponentInParent<Simulation>();
		}

		public virtual void OnClick() => Select(this);

		public void Connect(GateConnector other)
		{
			if (_simulation == null)
				throw new InvalidOperationException("Object is not part of a simulation.");
			_simulation.AddToSimulation(this + other);
		}

		public void Disconnect(GateConnector other)
		{
			if (_simulation == null)
				throw new InvalidOperationException("Object is not part of a simulation.");
			_simulation.RemoveFromSimulation(this + other);
		}

		public bool Equals(GateConnector other) => _id == other._id;
		public override int GetHashCode() => _id.GetHashCode();

		public static Connection operator +(GateConnector a, GateConnector b) => new(a, b);

		private static int _nextId = 0;
		private static GateConnector _selected;

		public static void Select(GateConnector obj)
		{
			if (_selected == null)
			{
				_selected = obj;
			}
			else
			{
				if (obj != null)
					_selected.Connect(obj);
				_selected = null;
			}
		}
	}

	[Serializable]
	public struct Connection : IEquatable<Connection>, IEnumerable<GateConnector>
	{
		[SerializeField]
		private GateConnector _itemA, _itemB;

		public GateConnector ItemA { readonly get => _itemA; init => _itemA = value; }
		public GateConnector ItemB { readonly get => _itemB; init => _itemB = value; }

		public Connection(GateConnector a, GateConnector b)
		{
			if (a == null) throw new ArgumentNullException(nameof(a));
			if (b == null) throw new ArgumentNullException(nameof(b));
			if (a == b) throw new ArgumentException("Cannot connect an object to itself.");
			_itemA = a;
			_itemB = b;
		}

		public readonly bool Contains(GateConnector obj)
			=> _itemA.Equals(obj) || _itemB.Equals(obj);

		public readonly GateConnector Other(GateConnector obj)
			=> obj.Equals(_itemA) ? _itemB : _itemA;

		public readonly override bool Equals([NotNullWhen(true)] object obj)
			=> obj is Connection other && Equals(other);
		public readonly bool Equals(Connection other)
			=> this.GetHashCode() == other.GetHashCode();
		public override readonly int GetHashCode()
		{
			(int a, int b) = (_itemA.Id, _itemB.Id);
			if (b < a) (a, b) = (b, a);
			return HashCode.Combine(a, b);
		}

		public readonly IEnumerator<GateConnector> GetEnumerator()
		{
			yield return _itemA;
			yield return _itemB;
		}
		readonly IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		public static bool operator ==(Connection lhs, Connection rhs) => lhs.Equals(rhs);
		public static bool operator !=(Connection lhs, Connection rhs) => !lhs.Equals(rhs);
	}
}
