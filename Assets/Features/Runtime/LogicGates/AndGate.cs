using System.Linq;
using UnityEngine;

namespace Features.LogicGates
{
	public class AndGate : BaseLogicGate
	{
		[SerializeField] private Input[] _inputs;
		[SerializeField] private Output[] _outputs;

		protected override IReadOnlyList<Input> Inputs => _inputs;
		protected override IReadOnlyList<Output> Outputs => _outputs;

		protected override void GetOutputValues(float[] input, float[] output)
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
