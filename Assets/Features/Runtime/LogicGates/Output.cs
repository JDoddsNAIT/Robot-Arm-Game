namespace Features.LogicGates
{
	public class Output : Connectable
	{
		private ConfigOptions _options = new();

		[SerializeField] private float _value;

		public override float Value { get => _value; set => _value = _options.GetValue(value); }
	}
}
