namespace Features.LogicGates
{
	public abstract class BaseLogicGate : MonoBehaviour
	{
		[SerializeField] protected int _buffer;

		private float[] _inputValues;
		private float[] _outputValues;
		private float[,] _outputValueBuffer;
		private int _bufferIndex;
		private int _bufferSize;

		protected abstract IReadOnlyList<Input> Inputs { get; }
		protected abstract IReadOnlyList<Output> Outputs { get; }

		protected virtual void OnEnable()
		{
			GetComponentInParent<LogicSimulator>().AddToSimulation(this);
		}

		protected virtual void OnDisable()
		{
			GetComponentInParent<LogicSimulator>().RemoveFromSimulation(this);
		}

		public virtual void OnSimulationStart()
		{
			_inputValues = new float[Inputs.Count];
			_outputValues = new float[Outputs.Count];
			_bufferIndex = 0;
			_bufferSize = _buffer + 1;
			_outputValueBuffer = new float[_bufferSize, Outputs.Count];
		}

		public virtual void SetOutputValues()
		{
			for (int i = 0; i < Outputs.Count; i++)
			{
				Outputs[i].Value = _outputValueBuffer[_bufferIndex, i];
			}
		}

		public virtual void UpdateOutputValues()
		{
			for (int i = 0; i < Inputs.Count; i++)
			{
				_inputValues[i] = Inputs[i].Value;
			}

			this.GetOutputValues(_inputValues, _outputValues);

			for (int i = 0; i < Outputs.Count; i++)
			{
				_outputValueBuffer[_bufferIndex, i] = _outputValues[i];
			}
			_bufferIndex = (_bufferIndex + 1) % _bufferSize;
		}

		protected abstract void GetOutputValues(float[] input, float[] output);
	}
}
