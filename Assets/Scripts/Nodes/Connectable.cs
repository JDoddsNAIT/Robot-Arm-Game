namespace Game.NodeCode
{
	public abstract class Connectable : MonoBehaviour, IEquatable<Connectable>
	{
		protected static int _nextId = 0;

		protected readonly int _id = _nextId++;

		[SerializeField] private List<Connectable> _connections;

		public int Id => _id;

		public override int GetHashCode() => _id;
		public bool Equals(Connectable other) => _id == other._id;

		public void Connect(Connectable other)
		{
			switch (_connections.Contains(other), other._connections.Contains(this))
			{
				case (true, true): return;
				case (false, true): this._connections.Add(other); return;
				case (true, false): other._connections.Add(this); return;
				case (false, false):
					this._connections.Add(other);
					other._connections.Add(this);
					return;
			}
		}

		public void Disconnect(Connectable other)
		{
			this._connections.Remove(other);
			other._connections.Remove(this);
		}

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

	public struct Connection : IEquatable<Connection>
	{
		private Connectable _itemA, _itemB;

		public Connectable ItemA { readonly get => _itemA; init => _itemA = value; }
		public Connectable ItemB { readonly get => _itemB; init => _itemB = value; }

		public Connection(Connectable a, Connectable b)
		{
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
			=> HashCode.Combine(Mathf.Min(_itemA.Id, _itemB.Id), Mathf.Max(_itemA.Id, _itemB.Id));
		public readonly bool Equals(Connection other)
			=> this.GetHashCode() == other.GetHashCode();

		public static bool operator ==(Connection lhs, Connection rhs) => lhs.Equals(rhs);
		public static bool operator !=(Connection lhs, Connection rhs) => !lhs.Equals(rhs);
	}
}
