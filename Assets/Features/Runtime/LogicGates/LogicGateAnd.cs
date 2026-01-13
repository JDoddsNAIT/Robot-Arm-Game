using System.Linq;
using UnityEngine;

namespace Features.LogicGates
{
	public class LogicGateAnd : LogicGate
	{
		[SerializeField] private GateInput[] _inputs;
		[SerializeField] private GateOutput[] _outputs;

		protected override IReadOnlyList<GateInput> Inputs => _inputs;
		protected override IReadOnlyList<GateOutput> Outputs => _outputs;

		protected override void ProcessInputs(float[] input, float[] output)
		{
			float value = 1;
			for (int i = 0; i < input.Length; i++)
			{
				value *= input[i];
			}
			output[0] = value;
		}
	}
}
