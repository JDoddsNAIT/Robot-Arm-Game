using UnityEngine.EventSystems;

namespace Features.LogicGates
{
	public class LogicGateButton : LogicGate, IPointerDownHandler, IPointerUpHandler
	{
		[SerializeField] private bool _isPressed;
		[SerializeField] private GateOutput[] _outputs;

		protected override IReadOnlyList<GateInput> Inputs => Array.Empty<GateInput>();
		protected override IReadOnlyList<GateOutput> Outputs => _outputs;

		protected override void ProcessInputs(float[] input, float[] output)
		{
			for (int i = 0; i < output.Length; i++)
			{
				output[i] = _isPressed ? 1 : 0;
			}
		}

		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			_isPressed = true;
		}

		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			_isPressed = false;
		}
	}
}
