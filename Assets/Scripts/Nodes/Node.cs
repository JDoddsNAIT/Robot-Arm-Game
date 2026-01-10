using System.Linq;

namespace Game.NodeCode
{
	public abstract class Node : MonoBehaviour
	{
		[SerializeField] protected NodeInput[] _inputs;
		[SerializeField] protected NodeOutput[] _outputs;

		protected float[] _inputValues;

		protected virtual void Update()
		{
			_inputValues = _inputs.Select(GetInputValue).ToArray();
		}

		protected virtual void LateUpdate()
		{
			var outputValues = this.GetOutputValues();

			int length = Mathf.Min(_outputs.Length, outputValues.Length);
			for (int i = 0; i < length; i++)
			{
				_outputs[i].Output.Value = outputValues[i];
			}
		}

		protected abstract float[] GetOutputValues();

		private float GetInputValue(NodeInput input)
		{
			float value = input.Input.Value;
			if (input.Invert)
				return value == 0 ? 1 : 0;
			else
				return value;
		}
	}

	[Serializable]
	public struct NodeInput
	{
		[SerializeField] private bool _invert;
		[SerializeField] private Input _input;

		public bool Invert { readonly get => _invert; set => _invert = value; }
		public Input Input { readonly get => _input; set => _input = value; }
	}

	[Serializable]
	public struct NodeOutput
	{
		[SerializeField] private bool _invert;
		[SerializeField] private Output _output;

		public bool Invert { readonly get => _invert; set => _invert = value; }
		public Output Output { readonly get => _output; set => _output = value; }
	}
}
