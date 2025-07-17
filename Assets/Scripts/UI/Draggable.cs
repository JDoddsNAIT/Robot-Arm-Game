using System;
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
		[SerializeField] private SnappingPoint[] _snappingPoints = Array.Empty<SnappingPoint>();
		[SerializeField] private UnityEvent _onBeginDrag;
		[SerializeField] private UnityEvent<SnappingPoint, SnappingPoint> _onSnapTo;
		private SnappingPoint _source, _target;

		/// <summary>
		/// The transform to drag. If no value was assigned in the inspector or this property, this returns the <see cref="RectTransform"/> on this <see cref="MonoBehaviour"/>.
		/// </summary>
		public RectTransform DragTransform {
			get {
				if (_transform == null)
					return base.transform as RectTransform;
				else
					return _transform;
			}
			set => _transform = value;
		}

		public bool ConstrainBounds { get => _constrainBounds; set => _constrainBounds = value; }
		public RectTransform Container { get => _container == null ? DragTransform.parent as RectTransform : _container; set => _container = value; }

		public bool EnableSnapping { get => _enableSnapping; set => _enableSnapping = value; }
		/// <summary>
		/// The maximum distance in world space units between valid snapping points.
		/// </summary>
		public float SnappingDistance { get => _snappingDistance; set => _snappingDistance = value; }
		/// <summary>
		/// List of snapping points on this object.
		/// </summary>
		public System.Collections.Generic.IReadOnlyList<SnappingPoint> SnappingPoints => _snappingPoints;

		public event UnityAction OnBeginDrag {
			add => _onBeginDrag.AddListener(value);
			remove => _onBeginDrag.RemoveListener(value);
		}

		public event UnityAction<SnappingPoint, SnappingPoint> OnSnapTo {
			add => _onSnapTo.AddListener(value);
			remove => _onSnapTo.RemoveListener(value);
		}

		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
		{
			_dragOffset = (Vector2)DragTransform.position - eventData.position;
			_onBeginDrag.Invoke();
		}

		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			DragTransform.position = _dragOffset + eventData.position;

			if (_enableSnapping)
			{
				if (TryGetSnappingPoint(out var source, out var target))
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
				Snap(from: _source, to: _target);
			}

			if (_constrainBounds)
			{
				try
				{
					DragTransform.Constrain(within: Container);
				}
				catch (InvalidOperationException ex)
				{
					Debug.LogWarning(ex.Message);
				}
			}
		}

		#region Snapping Methods
		private bool TryGetSnappingPoint(out SnappingPoint source, out SnappingPoint target)
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
			_onSnapTo.Invoke(from, to);
			this.ClearSourceAndTarget();
		}
		#endregion

		public void OnDestroy()
		{
			_onSnapTo.RemoveAllListeners();
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
