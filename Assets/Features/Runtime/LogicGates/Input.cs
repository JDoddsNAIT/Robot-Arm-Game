using System.Linq;
using UnityEngine.UIElements;

namespace Features.LogicGates
{
	public class Input : Connectable
	{
		[SerializeField] private bool _invert;
		[SerializeField] private float _scale;

		public float Value {
			get {
				var value = this.GetConnectedObjects().OfType<Output>().Sum(static x => x.Value);
				if (_invert)
					value = value == 0 ? 1 : 0;
				value *= _scale;
				return value;
			}
		}
	}
}
