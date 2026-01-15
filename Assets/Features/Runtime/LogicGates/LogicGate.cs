namespace Features.LogicGates
{
	[Serializable]
	public abstract class LogicGate
	{
		private readonly ILogicBehaviour _behaviour;
		[SerializeField] private LogicGateConnector[] _inputs, _outputs;
		[SerializeField] private float[] _inputValues, _outputValues;
		[SerializeField] private int _bufferSize;
		private readonly float[,] _outputBuffer;

		private int _bufferIndex;

		public IReadOnlyList<LogicGateConnector> Inputs => _inputs;
		public IReadOnlyList<LogicGateConnector> Outputs => _outputs;

		public LogicGate(ILogicBehaviour behaviour, LogicGateConnector[] inputs, LogicGateConnector[] outputs, int buffer = 0)
		{
			_behaviour = behaviour;
			_inputs = inputs;
			_outputs = outputs;

			_inputValues = new float[inputs.Length];
			_outputValues = new float[outputs.Length];

			_bufferSize = buffer + 1;
			_outputBuffer = new float[_bufferSize, outputs.Length];
			_bufferIndex = 0;
		}

		public void UpdateInputs()
		{
			for (int i = 0; i < _inputs.Length; i++)
			{
				_inputValues[i] = _inputs[i].Value;
			}
		}

		public void SetOutputs()
		{
			_behaviour.ProcessInputValues(_inputValues, _outputValues);
			for (int i = 0; i < _outputs.Length; i++)
			{
				_outputs[i].Value = _outputBuffer[_bufferIndex, i];
				_outputBuffer[_bufferIndex, i] = _outputValues[i];
			}
			_bufferIndex = (_bufferIndex + 1) % _bufferSize;
		}
	}

	public interface ILogicBehaviour
	{
		void ProcessInputValues(ReadOnlySpan<float> inputs, Span<float> outputs);
	}
}
