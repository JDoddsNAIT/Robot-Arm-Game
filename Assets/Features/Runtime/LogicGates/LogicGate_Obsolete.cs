namespace Features.LogicGates
{
	[Obsolete]
	[Serializable]
	public class LogicGate_Obsolete
	{
		private readonly ILogicBehaviour_Obsolete _behaviour;
		[SerializeField] private LogicConnector_Obsolete[] _inputs, _outputs;
		[SerializeField] private float[] _inputValues, _outputValues;
		[SerializeField] private int _bufferSize;
		private readonly float[,] _outputBuffer;

		private int _bufferIndex;

		public IReadOnlyList<LogicConnector_Obsolete> Inputs => _inputs;
		public IReadOnlyList<LogicConnector_Obsolete> Outputs => _outputs;

		public LogicGate_Obsolete(ILogicBehaviour_Obsolete behaviour, LogicConnector_Obsolete[] inputs, LogicConnector_Obsolete[] outputs)
		{
			_behaviour = behaviour ?? throw new ArgumentNullException(nameof(behaviour));
			_inputs = inputs ?? throw new ArgumentNullException(nameof(inputs));
			_outputs = outputs ?? throw new ArgumentNullException(nameof(outputs));

			_inputValues = new float[inputs.Length];
			_outputValues = new float[outputs.Length];

			_bufferSize = behaviour.Buffer + 1;
			_outputBuffer = new float[_bufferSize, outputs.Length];
			_bufferIndex = 0;
		}

		public void Process()
		{
			for (int i = 0; i < _inputs.Length; i++)
			{
				_inputValues[i] = _inputs[i].Value;
			}

			_behaviour.ProcessInputValues(_inputValues, _outputValues);

			for (int i = 0; i < _outputs.Length; i++)
			{
				_outputs[i].Value = _outputBuffer[_bufferIndex, i];
				_outputBuffer[_bufferIndex, i] = _outputValues[i];
			}
			_bufferIndex = (_bufferIndex + 1) % _bufferSize;
		}
	}

	[Obsolete]
	public interface ILogicBehaviour_Obsolete
	{
		int Buffer { get; }
		void ProcessInputValues(ReadOnlySpan<float> inputs, Span<float> outputs);

		public static ILogicBehaviour_Obsolete GetBehaviourForType(GateType type)
		{
			return type switch {

				_ => null,
			};
		}
	}
}
