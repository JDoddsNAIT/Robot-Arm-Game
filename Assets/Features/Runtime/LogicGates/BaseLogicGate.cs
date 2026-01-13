namespace Features.LogicGates
{
	public abstract class BaseLogicGate : MonoBehaviour
	{
		private float[] _inputValues;
		private float[] _outputValues;
		private float[,] _outputValueBuffer;
		private int _bufferIndex;
		private int _bufferSize;

		protected virtual int BufferSize => 0;
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
			_bufferSize = BufferSize + 1;
			_outputValueBuffer = new float[_bufferSize, Outputs.Count];
		}

		public virtual void SetOutputValues()
		{
			for (int i = 0; i < Outputs.Count; i++)
			{
				Outputs[i].Value = _outputValueBuffer[_bufferIndex, i];
			}
		}

		public virtual void OnSimulationUpdate()
		{
			for (int i = 0; i < Inputs.Count; i++)
			{
				_inputValues[i] = Inputs[i].Value;
			}

			this.ProcessInputs(_inputValues, _outputValues);

			for (int i = 0; i < Outputs.Count; i++)
			{
				_outputValueBuffer[_bufferIndex, i] = _outputValues[i];
			}
			_bufferIndex = (_bufferIndex + 1) % _bufferSize;
		}

		protected abstract void ProcessInputs(float[] input, float[] output);
	}
}
