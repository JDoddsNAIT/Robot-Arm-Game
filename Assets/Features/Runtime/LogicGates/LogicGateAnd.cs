namespace Features.LogicGates
{
	public class LogicGateAND : LogicGate
	{
		[SerializeField] private GateInput[] _inputs;
		[SerializeField] private GateOutput[] _outputs;

		protected override IReadOnlyList<GateInput> Inputs => _inputs;
		protected override IReadOnlyList<GateOutput> Outputs => _outputs;

		protected override void ProcessInputs(ReadOnlySpan<float> input, Span<float> output)
		{
			float value = 1;
			for (int i = 0; i < input.Length; i++)
			{
				value *= input[i];
			}

			for (int i = 0; i < output.Length; i++)
			{
				output[i] = value;
			}
		}
	}
}
