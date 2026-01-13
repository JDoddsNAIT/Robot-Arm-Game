using System.Linq;
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

		protected override void ProcessInputs(float[] input, float[] output)
		{
			if (_targetGraphic != null)
			{
				_targetGraphic.color = input.Any(static x => !Mathf.Approximately(x, 0)) ? _onColor : _offColor;
			}
		}
	}
}
