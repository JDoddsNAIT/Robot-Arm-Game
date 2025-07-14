using UnityEngine;
using Utilities.Extensions;

namespace Game.UI
{
	public static class Utils
	{
		public static Vector2 GetScreenPosition(this RectTransform rectTransform)
		{
			Vector2 result = Vector2.zero;
			var hierarchy = rectTransform.Get().Components<RectTransform>().InParent(includeInactive: true);
			for (int i = 0; i < hierarchy.Length; i++)
			{
				result += hierarchy[i].anchoredPosition;
			}
			return result;
		}
	}
}
