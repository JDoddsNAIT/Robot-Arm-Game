using System;
using System.Collections.Generic;

namespace Game.UI
{
	public static class Utils
	{
		public static IEnumerable<TEnum> GetFlags<TEnum>(this TEnum flags) where TEnum : struct, Enum
		{
			var values = Enum.GetValues(typeof(TEnum));
			foreach (TEnum value in values)
			{
				if (flags.HasFlag(value))
				{
					yield return value;
				}
			}
		}
	}
}
