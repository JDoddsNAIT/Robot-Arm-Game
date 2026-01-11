namespace Features.LogicGates
{
	public abstract class Connectable : MonoBehaviour, IEquatable<Connectable>
	{
		protected static int _nextId = 0;

		protected readonly int _id = _nextId++;

		[SerializeField] private List<Connectable> _connections = new();

		public int Id => _id;

		public override int GetHashCode() => _id;
		public bool Equals(Connectable other) => _id == other._id;

		public bool ContainsConnection(Connectable other) => _connections.Contains(other);

		protected virtual void OnConnectedTo(Connectable other) { }
		protected virtual void OnDisconnectedFrom(Connectable other) { }

		public static void ToggleConnection(Connectable a, Connectable b)
		{
			if (a.ContainsConnection(b) && b.ContainsConnection(a))
				Disconnect(a, b);
			else
				Connect(a, b);
		}

		public static Connection Connect(Connectable a, Connectable b)
		{
			var result = a + b;
			if (!a.ContainsConnection(b))
			{
				a._connections.Add(b);
				a.OnConnectedTo(b);
			}

			if (!b.ContainsConnection(a))
			{
				b._connections.Add(a);
				b.OnConnectedTo(a);
			}

			return result;
		}

		public static void Disconnect(Connectable a, Connectable b)
		{
			if (a.ContainsConnection(b))
			{
				a._connections.Remove(b);
				a.OnDisconnectedFrom(b);
			}

			if (b.ContainsConnection(a))
			{
				b._connections.Remove(a);
				b.OnDisconnectedFrom(a);
			}
		}

		/// <summary>
		/// Uses breadth-first search to find all objects this object is connected to.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Connectable> GetConnectedObjects()
		{
			HashSet<int> explored = new(capacity: 1);
			Queue<Connectable> queue = new(capacity: 1);
			queue.Enqueue(this);
			while (queue.Count > 0)
			{
				var obj = queue.Dequeue();
				if (explored.Contains(obj.Id))
					continue;

				yield return obj;
				explored.Add(obj.Id);
				foreach (var next in obj._connections)
				{
					if (next != null)
						queue.Enqueue(next);
				}
			}
		}

		public static Connection operator +(Connectable lhs, Connectable rhs) => new(lhs, rhs);
	}

	[Serializable]
	public struct Connection : IEquatable<Connection>
	{
		[SerializeField]
		private Connectable _itemA, _itemB;

		public Connectable ItemA { readonly get => _itemA; init => _itemA = value; }
		public Connectable ItemB { readonly get => _itemB; init => _itemB = value; }

		public Connection(Connectable a, Connectable b)
		{
			if (a == null) throw new ArgumentNullException(nameof(a));
			if (b == null) throw new ArgumentNullException(nameof(b));
			if (a == b) throw new ArgumentException("Cannot connect an object to itself.");
			_itemA = a;
			_itemB = b;
		}

		public readonly bool Contains(Connectable obj)
			=> _itemA.Equals(obj) || _itemB.Equals(obj);

		public readonly Connectable Other(Connectable obj)
			=> obj.Equals(_itemA) ? _itemB : _itemA;

		public readonly override bool Equals([NotNullWhen(true)] object obj)
			=> obj is Connection other && Equals(other);
		public override readonly int GetHashCode()
		{
			(int a, int b) = (_itemA.Id, _itemB.Id);
			if (b < a) (a, b) = (b, a);
			return HashCode.Combine(a, b);
		}

		public readonly bool Equals(Connection other)
			=> this.GetHashCode() == other.GetHashCode();

		public static bool operator ==(Connection lhs, Connection rhs) => lhs.Equals(rhs);
		public static bool operator !=(Connection lhs, Connection rhs) => !lhs.Equals(rhs);
	}
}
