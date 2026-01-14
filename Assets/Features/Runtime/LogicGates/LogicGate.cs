namespace Features.LogicGates
{
	public abstract class LogicGate : MonoBehaviour
	{
		private float[] _inputValues;
		private float[] _outputValues;
		private float[,] _outputValueBuffer;
		private int _bufferIndex;
		private int _bufferSize;

		protected virtual int Buffer => 0;

		public abstract GameObject Prefab { get; set; }
		public abstract IReadOnlyList<GateInput> Inputs { get; }
		public abstract IReadOnlyList<GateOutput> Outputs { get; }

		public IReadOnlyList<GateConnector> this[ConnectionPointData.PointType type] => type switch {
			ConnectionPointData.PointType.Input => Inputs,
			ConnectionPointData.PointType.Output => Outputs,
			_ => throw new NotImplementedException(),
		};

		public Simulation Simulation { get; private set; }

		public abstract GateConfigData[] GetConfigData();
		public abstract void SetConfigData(GateConfigData[] value);

		protected virtual void Start()
		{
			Simulation = GetComponentInParent<Simulation>();
			if (Simulation != null)
				Simulation.AddToSimulation(this);
		}

		protected virtual void OnDestroy()
		{
			if (Simulation != null)
				Simulation.RemoveFromSimulation(this);
		}

		public virtual void OnSimulationStart()
		{
			_inputValues = new float[Inputs.Count];
			_outputValues = new float[Outputs.Count];
			_bufferIndex = 0;
			_bufferSize = Buffer + 1;
			_outputValueBuffer = new float[_bufferSize, Outputs.Count];
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
				Outputs[i].Value = _outputValueBuffer[_bufferIndex, i];
				_outputValueBuffer[_bufferIndex, i] = _outputValues[i];
			}
			_bufferIndex = (_bufferIndex + 1) % _bufferSize;
		}

		protected abstract void ProcessInputs(ReadOnlySpan<float> input, Span<float> output);
	}
}
