using System.Linq;

namespace Game.NodeCode
{
	public class Input : Connectable
	{
		public float Value => this.GetConnectedObjects().OfType<Output>().Sum(static output => output.Value);
	}
}
