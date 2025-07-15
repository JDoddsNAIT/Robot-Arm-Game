using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Utilities.Extensions;

namespace Game.UI
{
	[RequireComponent(typeof(RectTransform))]
	public class SnappingPoint : MonoBehaviour
	{
		private static readonly List<SnappingPoint> _snappingPoints = new();

		[SerializeField] private UnityEvent _onEnterSnapRange;
		[SerializeField] private UnityEvent _onExitSnapRange;

		public UnityEvent OnExitSnapRange => _onExitSnapRange;

		public UnityEvent OnEnterSnapRange => _onEnterSnapRange;

		private void OnEnable()
		{
			_snappingPoints.Add(this);
		}

		public bool CanSnap(
			float snappingDistance,
			IReadOnlyList<SnappingPoint> exclusions,
			[System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out SnappingPoint position)
		{
			position = _snappingPoints
				.Where(p => {
					bool result = p != null && p.enabled;
					result &= !exclusions.Contains(p);
					result &= Vector2.Distance(p.transform.position, this.transform.position) < snappingDistance;
					return result;
				})
				.OrderBy(p => Vector2.Distance(p.transform.position, this.transform.position))
				.FirstOrDefault();

			if (position != null)
			{
				Debug.Assert(position != this);
				return true;
			}
			else
			{
				return false;
			}
		}

		private void OnDisable()
		{
			_snappingPoints.Remove(this);
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
