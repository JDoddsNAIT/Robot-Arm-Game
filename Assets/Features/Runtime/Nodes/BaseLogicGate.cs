using System.Linq;

namespace Features.LogicGates
{
	public abstract class BaseLogicGate : MonoBehaviour
	{
		[SerializeField] protected int _buffer;
		[SerializeField] protected GateInput[] _inputs;
		[SerializeField] protected GateOutput[] _outputs;

		private float[] _inputValues;
		private float[] _outputValues;
		private float[,] _outputValueBuffer;
		private int _bufferIndex;
		private int _bufferSize;

		protected virtual void OnEnable()
		{
			transform.parent.GetComponentInParent<LogicSimulator>().AddToSimulation(this);
		}

		protected virtual void OnDisable()
		{
			transform.parent.GetComponentInParent<LogicSimulator>().RemoveFromSimulation(this);
		}

		public virtual void OnSimulationStart()
		{
			_inputValues = new float[_inputs.Length];
			_outputValues = new float[_outputs.Length];
			_bufferIndex = 0;
			_bufferSize = _buffer + 1;
			_outputValueBuffer = new float[_bufferSize, _outputs.Length];
		}

		public virtual void OnSimulationEarlyUpdate()
		{
			for (int i = 0; i < _outputs.Length; i++)
			{
				_outputs[i].Value = _outputValueBuffer[_bufferIndex, i];
			}
		}

		public virtual void OnSimulationUpdate()
		{
			for (int i = 0; i < _inputs.Length; i++)
			{
				_inputValues[i] = _inputs[i].Value;
			}

			this.GetOutputValues(_inputValues, _outputValues);

			for (int i = 0; i < _outputs.Length; i++)
			{
				_outputValueBuffer[_bufferIndex, i] = _outputValues[i];
			}
			_bufferIndex = (_bufferIndex + 1) % _bufferSize;
		}

		protected abstract void GetOutputValues(float[] input, float[] output);
	}

	[Obsolete]
	[Serializable]
	public struct GateInput
	{
		[SerializeField] private bool _invert;
		[SerializeField] private float _scale;
		[SerializeField] private Input _input;

		public bool Invert { readonly get => _invert; set => _invert = value; }
		public float Scale { readonly get => _scale; set => _scale = value; }
		public Input Input { readonly get => _input; set => _input = value; }

		public readonly float Value {
			get {
				var value = _input.GetConnectedObjects().OfType<Output>().Sum(static x => x.Value);
				if (_invert)
					value = value == 0 ? 1 : 0;
				value *= _scale;
				return value;
			}
		}
	}

	[Obsolete]
	[Serializable]
	public struct GateOutput
	{
		[SerializeField] private bool _invert;
		[SerializeField] private float _scale;
		[SerializeField] private Output _output;

		public bool Invert { readonly get => _invert; set => _invert = value; }
		public float Scale { readonly get => _scale; set => _scale = value; }
		public Output Output { readonly get => _output; set => _output = value; }

		public readonly float Value {
			get => _output.Value;
			set {
				if (_invert)
					value = value == 0 ? 1 : 0;
				value *= _scale;
				_output.Value = value;
			}
		}
	}
}
