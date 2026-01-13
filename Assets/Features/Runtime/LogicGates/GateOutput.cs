namespace Features.LogicGates
{
	public class GateOutput : GateConnector
	{
		[SerializeField] private float _value;

		public override float Value { get => _value; set => _value = value; }
	}
}
