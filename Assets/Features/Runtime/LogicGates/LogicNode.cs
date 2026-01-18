using Features.Persistence;

namespace Features.LogicGates
{
	public class LogicNode
	{
		private int _bufferIndex;
		private readonly int _bufferSize;
		private readonly IO[] _inputs, _outputs;
		private readonly float[,] _outputBuffer;
		private readonly ProcessInputsDelegate _processInputs;

		public LogicNode(ProcessInputsDelegate processInputs, IO[] inputs, IO[] outputs, int buffer = 0)
		{
			_bufferIndex = 0;
			_bufferSize = buffer + 1;
			_inputs = inputs;
			_outputs = outputs;
			_outputBuffer = new float[_bufferSize, outputs.Length];
			_processInputs = processInputs;
		}

		public void Update()
		{
			Span<float> inputValues = stackalloc float[_inputs.Length];
			Span<float> outputValues = stackalloc float[_outputs.Length];
			for (int i = 0; i < inputValues.Length; i++)
			{
				inputValues[i] = _inputs[i].Value;
			}
			_processInputs?.Invoke(inputValues, outputValues);
			for (int i = 0; i < outputValues.Length; i++)
			{
				_outputs[i].Value = _outputBuffer[_bufferIndex, i];
				_outputBuffer[_bufferIndex, i] = outputValues[i];
			}
			_bufferIndex = (_bufferIndex + 1) % _bufferSize;
		}

		public class IO
		{
			private readonly bool _invert = false;
			private readonly float _scale = 1.0f;
			private float _value;

			public float Value {
				get => (!_invert ? _value : (Mathf.Approximately(_value, 0f) ? 1f : 0f)) * _scale;
				set => _value = value;
			}

			public IO() : this(false, 1.0f) { }
			public IO(bool invert, float scale)
			{
				_invert = invert;
				_scale = scale;
				_value = 0;
			}
		}

		public record struct Connection(SerializableGuid NodeId, int Index)
		{
			public static implicit operator (SerializableGuid nodeId, int index)(Connection value)
				=> (value.NodeId, value.Index);

			public static implicit operator Connection((SerializableGuid nodeId, int index) value)
				=> new(value.nodeId, value.index);
		}
	}

	public delegate void ProcessInputsDelegate(ReadOnlySpan<float> inputValues, Span<float> outputValues);

	public class LogicNodeData
	{
		public SerializableGuid Id { get; init; }
		public int Buffer { get; init; }
		public LogicNode.IO[] Inputs { get; init; }
		public LogicNode.IO[] Outputs { get; init; }
		public ProcessInputsDelegate ProcessInputs { get; init; }
		public LogicNode.Connection[] Connections { get; init; }
	}
}
