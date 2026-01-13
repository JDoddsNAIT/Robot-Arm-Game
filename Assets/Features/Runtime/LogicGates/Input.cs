using System.Linq;

namespace Features.LogicGates
{
	public class Input : Connectable
	{
		private ConfigOptions _options = new();

		public override float Value {
			get {
				float value = this.GetConnectedObjects().OfType<Output>().Sum(static x => x.Value);
				return _options.GetValue(value);
			}
			set => throw new NotSupportedException();
		}
	}
}
