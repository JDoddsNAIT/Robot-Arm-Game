namespace Features.LogicGates
{
	public abstract class LogicGate : MonoBehaviour
	{
		private float[] _inputValues;
		private float[] _outputValues;
		private float[,] _outputValueBuffer;
		private int _bufferIndex;
		private int _bufferSize;

		protected virtual int BufferSize => 0;
		protected abstract IReadOnlyList<GateInput> Inputs { get; }
		protected abstract IReadOnlyList<GateOutput> Outputs { get; }

		public Simulation Simulation { get; private set; }

		protected virtual void Start()
		{
			Simulation = GetComponentInParent<Simulation>();
			Simulation.AddToSimulation(this);
		}

		protected virtual void OnDestroy()
		{
			Simulation.RemoveFromSimulation(this);
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
