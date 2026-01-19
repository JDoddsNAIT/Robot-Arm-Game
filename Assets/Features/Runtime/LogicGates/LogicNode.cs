using Features.Persistence;

namespace Features.LogicGates
{
	public class LogicNode
	{
		private readonly SerializableGuid _id;
		private readonly int _bufferSize;
		private readonly LogicNodeIO[] _inputs, _outputs;
		private readonly float[,] _outputBuffer;
		private readonly ProcessInputsDelegate _behaviour;

		private int _bufferIndex;

		public SerializableGuid Id => _id;
		public LogicNodeIO this[int index] => index < 0 ? _outputs[~index] : _inputs[index];

		public LogicNode(SerializableGuid id, int buffer, LogicNodeIO[] inputs, LogicNodeIO[] outputs, ProcessInputsDelegate behaviour)
		{
			_id = id;
			_bufferSize = Mathf.Max(1, buffer + 1);
			_inputs = inputs;
			_outputs = outputs;
			_outputBuffer = new float[_bufferSize, _outputs.Length];
			_behaviour = behaviour;
			_bufferIndex = 0;
		}

		public void Update()
		{
			Span<float> inputValues = stackalloc float[_inputs.Length];
			for (int i = 0; i < _inputs.Length; i++)
			{
				inputValues[i] = _inputs[i].Value;
			}
			Span<float> outputValues = stackalloc float[_outputs.Length];
			_behaviour?.Invoke(inputValues, outputValues);
			for (int i = 0; i < _outputs.Length; i++)
			{
				_outputs[i].Value = _outputBuffer[_bufferIndex, i];
				_outputBuffer[_bufferIndex, i] = outputValues[i];
			}
			_bufferIndex = (_bufferIndex + 1) % _bufferSize;
		}
	}

	public class LogicNodeIO : IEquatable<LogicNodeIO>
	{
		private float _value;

		public int Id { get; init; }
		public bool Invert { get; set; }
		public float Scale { get; set; }
		public float Value {
			get => (Invert ? (Mathf.Approximately(0, _value) ? 1 : 0) : _value) * Scale;
			set => _value = value;
		}

		public LogicNodeIO() : this(false, 1.0f) { }
		public LogicNodeIO(IOData data) : this(data.Invert, data.Scale) { }
		public LogicNodeIO(bool invert, float scale) { Invert = invert; Scale = scale; }

		public override int GetHashCode() => Id.GetHashCode();
		public override bool Equals(object obj) => obj is LogicNodeIO other && Equals(other);
		public bool Equals(LogicNodeIO other) => Id == other.Id;
	}
}
