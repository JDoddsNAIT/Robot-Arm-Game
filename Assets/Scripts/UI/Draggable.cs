using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Game.UI
{
	public class Draggable : MonoBehaviour, IDragHandler, IEndDragHandler
	{
		[SerializeField] private float _snappingDistance = 1;
		[SerializeField] private SnappingPoint[] _snappingPoints;

		public RectTransform RectTransform => transform as RectTransform;

		public void OnDrag(PointerEventData eventData)
		{
			RectTransform.position = eventData.position;
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			TrySnap();
		}

		public bool TrySnap()
		{
			bool snapped = false;
			for (int i = 0; i < _snappingPoints.Length && !snapped; i++)
			{
				snapped = _snappingPoints[i].CanSnap(_snappingDistance, out var snapPoint, _snappingPoints);
				if (snapped)
				{
					RectTransform.anchoredPosition += snapPoint.GetScreenPosition() - _snappingPoints[i].GetScreenPosition();
					Debug.Log($"Snapped to position of {snapPoint.name}.");
				}
			}
			return snapped;
		}

		public void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.yellow;

			foreach (var point in _snappingPoints)
			{
				Gizmos.DrawWireSphere(point.transform.position, _snappingDistance);
			}
		}
	}
}
