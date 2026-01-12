namespace Features.LogicGates
{
	public class Output : Connectable
	{
		[SerializeField] private float _value;
		[SerializeField] private bool _invert;
		[SerializeField] private float _scale;

		public float Value {
			get {
				var value = _value;
				if (_invert)
					value = value == 0 ? 1 : 0;
				value *= _scale;
				return value;
			}
			set => _value = value;
		}
	}
}
