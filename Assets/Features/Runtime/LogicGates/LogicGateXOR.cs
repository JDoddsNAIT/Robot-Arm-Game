namespace Features.LogicGates
{
	public class LogicGateXOR : LogicGate
	{
		protected override IReadOnlyList<GateInput> Inputs { get; }
		protected override IReadOnlyList<GateOutput> Outputs { get; }

		protected override void ProcessInputs(ReadOnlySpan<float> inputs, Span<float> outputs)
		{
			float? value = null;
			for (int i = 0; i < inputs.Length; i++)
			{
				if (inputs[i] != 0)
				{
					if (value == null)
						value = inputs[i];
					else
						value = 0;
				}
			}

			for (int i = 0; i < outputs.Length; i++)
			{
				outputs[i] = value ?? 0;
			}
		}
	}
}
