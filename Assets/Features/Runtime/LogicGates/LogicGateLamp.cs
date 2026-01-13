using UnityEngine.UI;

namespace Features.LogicGates
{
	public class LogicGateLamp : LogicGate
	{
		[SerializeField] private Graphic _targetGraphic;
		[SerializeField] private Color _onColor = Color.white, _offColor = Color.black;
		[SerializeField] private GateInput[] _inputs;

		protected override IReadOnlyList<GateInput> Inputs => _inputs;
		protected override IReadOnlyList<GateOutput> Outputs => Array.Empty<GateOutput>();

		protected override void ProcessInputs(ReadOnlySpan<float> input, Span<float> output)
		{
			if (_targetGraphic != null)
			{
				bool on = false;
				for (int i = 0; on is false && i < input.Length; i++)
				{
					on = input[i] != 0;
				}
				_targetGraphic.color = on ? _onColor : _offColor;
			}
		}
	}
}
