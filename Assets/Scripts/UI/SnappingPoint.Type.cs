using System;

namespace Game.UI
{
	public partial class SnappingPoint
	{
		[Flags]
		public enum Type
		{
			None = 0,
			Block = 0b1,
			Parameter = 0b10,
		}
	}
}
