using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities.Extensions;

namespace Game.UI
{
	[RequireComponent(typeof(RectTransform))]
	public class SnappingPoint : MonoBehaviour, IEquatable<SnappingPoint>
	{
		private static readonly List<SnappingPoint> _snappingPoints = new();

		public RectTransform RectTransform => transform as RectTransform;
		public Vector2 TotalAnchoredPosition => RectTransform.anchoredPosition + (transform.parent as RectTransform).anchoredPosition;

		private void OnEnable()
		{
			_snappingPoints.Add(this);
		}

		public bool CanSnap(float snappingDistance, out SnappingPoint position, IReadOnlyList<SnappingPoint> exclusions)
		{
			var nearest = _snappingPoints
				.Where(
					p => {
						bool result = p != null && p.enabled;
						result &= !exclusions.Contains(p);
						Debug.Log(result);
						var dist = Vector2.Distance(p.transform.position, this.transform.position);
						Debug.Log($"{dist} < {snappingDistance} is {dist < snappingDistance}");
						result &= dist < snappingDistance;
						return result;
					})
				.OrderBy(
					p => Vector2.Distance(p.transform.position, this.transform.position))
				.FirstOrDefault();

			if (nearest != null)
			{
				Debug.Assert(nearest != this);
				position = nearest;
				return true;
			}
			else
			{
				Debug.Log("No point was found within range.");
				position = null;
				return false;
			}
		}

		private void OnDisable()
		{
			_snappingPoints.Remove(this);
		}

		public bool Equals(SnappingPoint other)
		{
			return this.GetInstanceID() == other.GetInstanceID();
		}

		public Vector2 GetScreenPosition()
		{
			Vector2 result = Vector2.zero;
			var hierarchy = this.Get().Components<RectTransform>().InParent(includeInactive: true);
			for (int i = 0; i < hierarchy.Length; i++)
			{
				result += hierarchy[i].anchoredPosition;
			}
			return result;
		}
	}
}
