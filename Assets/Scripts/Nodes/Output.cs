namespace Game.NodeCode
{
	public class Output : Connectable
	{
		[SerializeField] private float _value;

		public float Value { get => _value; set => _value = value; }
	}
}
