using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
	public static class Utils
	{
		public static IEnumerable<TEnum> GetFlags<TEnum>(this TEnum flags) where TEnum : struct, Enum
		{
			var values = Enum.GetValues(typeof(TEnum));
			foreach (var value in values)
			{
				if (flags.HasFlag((TEnum)value))
				{
					yield return (TEnum)value;
				}
			}
		}

		public static IEnumerable<TEnum> GetFlagsNonZero<TEnum>(this TEnum flags) where TEnum : struct, Enum
		{
			var values = Enum.GetValues(typeof(TEnum));
			foreach (var value in values)
			{
				// zero check
				if ((int)(object)flags is 0 && (int)value is 0)
				{
					yield return (TEnum)value;
				}
				else if (flags.HasFlag((TEnum)value))
				{
					yield return (TEnum)value;
				}
			}
		}


		/// <summary>
		/// Constrains the this object <see cref="RectTransform"/>'s bounds to be <paramref name="within"/> another.
		/// </summary>
		/// <remarks>
		/// Throws an <see cref="InvalidOperationException"/> if <paramref name="within"/> has dimensions smaller than the <paramref name="target"/>.
		/// </remarks>
		/// <param name="target"></param>
		/// <param name="within"></param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public static void Constrain(this RectTransform target, RectTransform within)
		{
			if (target == null)
				throw new ArgumentNullException(nameof(target));
			if (within == null)
				throw new ArgumentNullException(nameof(within));

			var worldCorners = new Vector3[4];

			const int topLeft = 1, bottomRight = 3;

			target.GetWorldCorners(fourCornersArray: worldCorners);
			float targetTop = worldCorners[topLeft].y,
				targetLeft = worldCorners[topLeft].x,
				targetBottom = worldCorners[bottomRight].y,
				targetRight = worldCorners[bottomRight].x;

			within.GetWorldCorners(fourCornersArray: worldCorners);
			float limitTop = worldCorners[topLeft].y,
				limitLeft = worldCorners[topLeft].x,
				limitBottom = worldCorners[bottomRight].y,
				limitRight = worldCorners[bottomRight].x;

			// Container is smaller than target
			if ((limitLeft > targetLeft && limitRight < targetRight) ||
				(limitBottom > targetBottom && limitTop < targetTop))
			{
				string message = $"{target} is being constrained to an area that is smaller than it's own dimensions.";
				throw new InvalidOperationException(message);
			}

			var translation = Vector2.zero;

			if (targetLeft < limitLeft)
				translation.x = limitLeft - targetLeft;
			else if (targetRight > limitRight)
				translation.x = limitRight - targetRight;
			else
				translation.x = 0;

			if (targetBottom < limitBottom)
				translation.y = limitBottom - targetBottom;
			else if (targetTop > limitTop)
				translation.y = limitTop - targetTop;
			else
				translation.y = 0;

			target.position += (Vector3)translation;
		}
	}
}
