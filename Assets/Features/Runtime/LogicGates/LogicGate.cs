using Features.Persistence;

namespace Features.LogicGates
{
	public abstract class LogicGate : MonoBehaviour
	{
		[SerializeField] private SerializableGuid _id;

		public SerializableGuid Id => _id;

		public abstract LogicNodeData GetLogicNodeData();
	}
}
