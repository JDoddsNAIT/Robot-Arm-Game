using UnityEngine.EventSystems;

namespace Features.LogicGates
{
	public class LogicGateButton : LogicGate, IPointerDownHandler, IPointerUpHandler
	{
		[SerializeField] private bool _isPressed;
		[SerializeField] private bool _toggle;
		[SerializeField] private GateOutput[] _outputs;

		protected override IReadOnlyList<GateInput> Inputs => Array.Empty<GateInput>();
		protected override IReadOnlyList<GateOutput> Outputs => _outputs;

		protected override void ProcessInputs(ReadOnlySpan<float> input, Span<float> output)
		{
			for (int i = 0; i < output.Length; i++)
			{
				output[i] = _isPressed ? 1 : 0;
			}
		}

		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			_isPressed = _toggle ? !_isPressed : true;
		}

		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			if (!_toggle)
				_isPressed = false;
		}
	}
}
