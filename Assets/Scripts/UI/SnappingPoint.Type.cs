using System;

namespace Game.UI
{
	public partial class SnappingPoint
	{
		[Flags]
		public enum Type
		{
			Any = -0b1,
			Disable = 0,
			BlockTop = 0b1,
			BlockBottom = 0b10,
			ParameterSlot = 0b100,
			ParameterBlock = 0b1000,
		}
	}
}
