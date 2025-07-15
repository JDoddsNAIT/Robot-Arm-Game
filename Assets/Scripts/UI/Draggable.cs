using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Game.UI
{
	[RequireComponent(typeof(RectTransform))]
	public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		[Tooltip("The object to drag. Defaults to this object.")]
		[SerializeField] private RectTransform _transform;
		private Vector2 _dragOffset;

		[Header("Constraints")]
		[SerializeField] private bool _constrainBounds = true;
		[Tooltip("The dragged object will be constrained within this " + nameof(RectTransform) + ". Defaults to the parent.")]
		[SerializeField] private RectTransform _container;

		[Header("Snapping")]
		[SerializeField] private bool _enableSnapping = true;
		[SerializeField] private float _snappingDistance = 1;
		[SerializeField] private SnappingPoint[] _snappingPoints;
		[SerializeField] private UnityEvent<SnappingPoint> _onSnapTo;
		private SnappingPoint _source, _target;

		/// <summary>
		/// The transform to drag.
		/// </summary>
		public RectTransform DragTransform {
			get {
				if (_transform == null)
					return base.transform as RectTransform;
				else
					return _transform;
			}
		}

		public bool ConstrainBounds { get => _constrainBounds; set => _constrainBounds = value; }
		public RectTransform Container { get => _container == null ? DragTransform.parent as RectTransform : _container; set => _container = value; }

		public bool EnableSnapping { get => _enableSnapping; set => _enableSnapping = value; }
		public float SnappingDistance { get => _snappingDistance; set => _snappingDistance = value; }
		public System.Collections.Generic.IReadOnlyList<SnappingPoint> SnappingPoints => _snappingPoints;

		public event UnityAction<SnappingPoint> OnSnapTo {
			add => _onSnapTo.AddListener(value);
			remove => _onSnapTo.RemoveListener(value);
		}

		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
		{
			_dragOffset = (Vector2)DragTransform.position - eventData.position;
		}

		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			DragTransform.position = _dragOffset + eventData.position;

			if (_enableSnapping)
			{
				if (CanSnapToAny(out var source, out var target))
				{
					SetAndSourceTarget(source, target);
				}
				else
				{
					ClearSourceAndTarget();
				}
			}
		}

		void IEndDragHandler.OnEndDrag(PointerEventData eventData)
		{
			if (_enableSnapping)
			{
				Snap(_source, _target);
			}

			if (_constrainBounds)
			{
				ConstrainTransformBounds(DragTransform, Container);
			}
		}

		#region Snapping Methods
		private bool CanSnapToAny(out SnappingPoint source, out SnappingPoint target)
		{
			bool canSnap;
			target = null;

			for (int i = 0; i < _snappingPoints.Length; i++)
			{
				if (_snappingPoints[i] == null) continue;

				canSnap = _snappingPoints[i].CanSnap(_snappingDistance, _snappingPoints, out target);

				if (canSnap)
				{
					source = _snappingPoints[i];
					return true;
				}
			}

			source = null;
			return false;
		}

		private void ClearSourceAndTarget() => this.SetAndSourceTarget(null, null);

		private void SetAndSourceTarget(SnappingPoint newSource, SnappingPoint newTarget)
		{
			if (newSource != _source)
			{
				if (_source != null)
					_source.OnExitSnapRange.Invoke();

				_source = newSource;

				if (newSource != null)
					newSource.OnEnterSnapRange.Invoke();
			}

			if (newTarget != _target)
			{
				if (_target != null)
					_target.OnExitSnapRange.Invoke();

				_target = newTarget;

				if (newTarget != null)
					newTarget.OnEnterSnapRange.Invoke();
			}
		}

		private void Snap(SnappingPoint from, SnappingPoint to)
		{
			if (from == null || to == null) return;

			Debug.Log($"I want to snap {from} to {to}");

			DragTransform.position += to.transform.position - from.transform.position;
			_onSnapTo.Invoke(to);
			this.ClearSourceAndTarget();
		}
		#endregion

		public static void ConstrainTransformBounds(RectTransform target, RectTransform container)
		{
			if (target == null)
				throw new System.ArgumentNullException(nameof(target));
			if (container == null)
				throw new System.ArgumentNullException(nameof(container));

			var myCorners = new Vector3[4];
			var limitCorners = new Vector3[4];

			target.GetWorldCorners(fourCornersArray: myCorners);
			container.GetWorldCorners(fourCornersArray: limitCorners);

			const int topLeft = 1, bottomRight = 3;

			float myTop = myCorners[topLeft].y,
				myLeft = myCorners[topLeft].x,
				myBottom = myCorners[bottomRight].y,
				myRight = myCorners[bottomRight].x;

			float topLimit = limitCorners[topLeft].y,
				leftLimit = limitCorners[topLeft].x,
				bottomLimit = limitCorners[bottomRight].y,
				rightLimit = limitCorners[bottomRight].x;

			// Constraint is smaller than me
			if (leftLimit > myLeft && rightLimit < myRight || bottomLimit > myBottom && topLimit < myTop)
			{
				string message = $"{target} is being constrained to an area that is smaller than it's own dimensions.";
				throw new System.InvalidOperationException(message);
			}

			var translation = Vector2.zero;

			if (myLeft < leftLimit)
				translation.x = leftLimit - myLeft;
			else if (myRight > rightLimit)
				translation.x = rightLimit - myRight;
			else
				translation.x = 0;

			if (myBottom < bottomLimit)
				translation.y = bottomLimit - myBottom;
			else if (myTop > topLimit)
				translation.y = topLimit - myTop;
			else
				translation.y = 0;

			target.position += (Vector3)translation;
		}

		public void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.yellow;

			foreach (var point in _snappingPoints)
			{
				if (point == null || !point.enabled) continue;

				Gizmos.DrawWireSphere(point.transform.position, _snappingDistance);
			}
		}
	}
}
