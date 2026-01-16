using System.Linq;
using Features.Persistence;
using Features.LogicGates;

namespace Features.UI
{
	public class LogicNodeConnector : MonoBehaviour
	{
		[SerializeField] private bool _invert;
		[SerializeField] private float _scale = 1f;
		[SerializeField] private List<LogicData.Connection> _connected;

		public SerializableGuid Node { get; set; }
		public int Index { get; set; }

		public bool Invert { get => _invert; set => _invert = value; }
		public float Scale { get => _scale; set => _scale = value; }
		public IEnumerable<LogicData.Connection> Connected { get => _connected; set => _connected = value.ToList(); }

		public void AddConnection(LogicNodeConnector other)
			=> _connected.Add(new() { node = other.Node, index = other.Index });
		public void RemoveConnection(LogicNodeConnector other)
			=> _connected.RemoveAll(x => x.node == other.Node && x.index == other.Index);
	}
}
