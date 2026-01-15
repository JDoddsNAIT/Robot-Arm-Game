using Features.Persistence;

namespace Features.UI
{
	public class LogicNodeUI : MonoBehaviour, IDataOwner<LogicNodeData>
	{
		bool _bound = false;

		[SerializeField] private SerializableGuid _gateId;

		public SerializableGuid Id { get => _gateId; set => _gateId = value; }
		public LogicNodeData Data {
			get {
				if (!_bound)
					return null;
				throw new NotImplementedException();
			}
		}

		public void Bind(in LogicNodeData data)
		{
			_bound = true;
		}

		public void Unbind()
		{
			_bound = false;
		}
	}
}
