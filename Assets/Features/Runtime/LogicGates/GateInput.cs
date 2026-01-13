using System.Linq;

namespace Features.LogicGates
{
	public class GateInput : GateConnector
	{
		[SerializeField] private float _value;

		public override float Value { get => _value; set => _value = value; }
	}
}
