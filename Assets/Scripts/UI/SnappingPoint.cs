using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Game.UI
{
	[RequireComponent(typeof(RectTransform))]
	public partial class SnappingPoint : MonoBehaviour
	{
		private static readonly DB _pointsDB = new();

		[EnumButtons]
		[SerializeField] private Type _pointType;
		[Space]
		[SerializeField] private UnityEvent _onEnterSnapRange;
		[SerializeField] private UnityEvent _onExitSnapRange;

		public UnityEvent OnEnterSnapRange => _onEnterSnapRange;

		public UnityEvent OnExitSnapRange => _onExitSnapRange;

		private void OnEnable()
		{
			_pointsDB.Add(this);
		}

		public bool CanSnap(float snappingDistance, IReadOnlyList<SnappingPoint> exclusions, out SnappingPoint position)
		{
			position = _pointsDB.GetSnappingPoints(_pointType, exclusions)
				.Where(p => Vector2.Distance(p.transform.position, this.transform.position) < snappingDistance)
				.OrderBy(p => Vector2.Distance(p.transform.position, this.transform.position))
				.FirstOrDefault();

			Debug.Assert(position == null || position != this);
			return position != null;
		}

		private void OnDisable()
		{
			_pointsDB.Remove(this);
		}
	}
}
