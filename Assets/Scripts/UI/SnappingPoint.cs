using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Game.UI
{
	[RequireComponent(typeof(RectTransform))]
	public partial class SnappingPoint : MonoBehaviour, IEquatable<SnappingPoint>
	{
		private static readonly DB _pointsDB = new();

		[EnumButtons, Tooltip("They type of this snapping point")]
		[SerializeField] private Type _pointType;
		[EnumButtons, Tooltip("The type of points this is allowed to snap to.")]
		[SerializeField] private Type _snapingType;
		[Space]
		[SerializeField] private UnityEvent _onEnterSnapRange;
		[SerializeField] private UnityEvent _onExitSnapRange;

		public UnityEvent OnEnterSnapRange => _onEnterSnapRange;

		public UnityEvent OnExitSnapRange => _onExitSnapRange;

		private void OnEnable()
		{
			Debug.Log($"Added {this} to the DB");
			_pointsDB.Add(this);
		}

		public bool CanSnap(float snappingDistance, IReadOnlyList<SnappingPoint> exclusions, out SnappingPoint position)
		{
			position = _pointsDB.GetSnappingPoints(_snapingType, exclusions)
				.Where(p => Vector2.Distance(p.transform.position, this.transform.position) < snappingDistance)
				.OrderBy(p => Vector2.Distance(p.transform.position, this.transform.position))
				.FirstOrDefault();
			Debug.Log(position);
			Debug.Assert(position == null || position != this);
			return position != null;
		}

		private void OnDisable()
		{
			Debug.Log($"Removed {this} from the DB");
			_pointsDB.Remove(this);
		}

		public bool Equals(SnappingPoint other)
		{
			return this.GetInstanceID() == other.GetInstanceID();
		}
	}
}
