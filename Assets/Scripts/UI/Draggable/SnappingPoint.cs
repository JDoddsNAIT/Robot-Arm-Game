using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Game.UI
{
	public partial class SnappingPoint : MonoBehaviour, IDraggableListener
	{
		private SnappingPoint _target;

		[SerializeField] private Draggable _draggable;
		[Space]
		[Min(0)]
		[SerializeField] private float _snappingDistance;
		[EnumButtons, Tooltip("The type of this point.")]
		[SerializeField] private Type _type;
		[EnumButtons, Tooltip("Determines the type of points this point can snap to.")]
		[SerializeField] private Type _targetType;
		[Tooltip("List of points that this point can never snap to.")]
		[SerializeField] private List<SnappingPoint> _exclusions = new();
		[Space]
		[SerializeField] private UnityEvent _onEnterSnapRange = new();
		[SerializeField] private UnityEvent _onExitSnapRange = new();
		[SerializeField] private UnityEvent<SnappingPoint> _onSnapTo = new();

		public event UnityAction OnEnterSnapRange {
			add => _onEnterSnapRange.AddListener(value);
			remove => _onEnterSnapRange.RemoveListener(value);
		}
		public event UnityAction OnExitSnapRange {
			add => _onExitSnapRange.AddListener(value);
			remove => _onExitSnapRange.RemoveListener(value);
		}
		public event UnityAction<SnappingPoint> OnSnapTo {
			add => _onSnapTo.AddListener(value);
			remove => _onSnapTo.RemoveListener(value);
		}

		public void OnEnable()
		{
			_target = null;
			_registry.Register(this);

			if (_draggable != null)
			{
				_draggable.AddCallbacks(this);
			}
		}

		public void OnDrawGizmosSelected()
		{
			if (_targetType is Type.Disabled)
				return;

			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(transform.position, _snappingDistance);
		}

		public void OnDisable()
		{
			_registry.Deregister(this);

			if (_draggable != null)
			{
				_draggable.RemoveCallbacks(this);
			}
		}

		public void OnDragStart() { /*no-op*/ }

		public void WhileDragging(Vector2 delta)
		{
			if (_targetType is Type.Disabled)
				return;

			SetTarget(FindTarget());
		}

		public void OnDragEnd()
		{
			if (_targetType is Type.Disabled)
				return;

			if (_target == null)
				return;

			Debug.Log(gameObject.name);

			var snapDisplacement = _target.transform.position - transform.position;
			_draggable.transform.position += snapDisplacement with { z = 0 };
			_onSnapTo.Invoke(_target);
			_target._onSnapTo.Invoke(this);
		}

		private readonly List<float> _distancesCache = new();
		private SnappingPoint FindTarget()
		{
			_distancesCache.Clear();
			int pointId = 0;
			var candidates = _registry.GetSnappingPoints(_targetType, _exclusions);
			Debug.Assert(candidates.Distinct().Count() == candidates.Count()); // Ensure no dupes
			candidates = candidates.Where(p => getDistance(p) <= _snappingDistance);
			pointId = 0;
			candidates = candidates.OrderBy(getDistance);
			return candidates.FirstOrDefault();

			// All of this saves 1 (one) call to Vector2.Distance for each SnappingPoint in the filtered list.
			// Probably not necessary as the filtered list is usually very short, but I still had fun writing this.

			float getDistance(SnappingPoint p)
			{
				float distance;
				if (pointId >= _distancesCache.Count)
				{
					distance = Vector2.Distance(transform.position, p.transform.position);
					if (distance <= _snappingDistance)
					{
						_distancesCache.Add(distance);
						pointId++;
					}
				}
				else
				{
					distance = _distancesCache[pointId];
					pointId++;
				}
				return distance;
			}
		}

		private void SetTarget(SnappingPoint newTarget)
		{
			if (_target == newTarget)
				return;

			if (_target != null)
			{
				_target._onExitSnapRange.Invoke();
			}

			_target = newTarget;

			if (newTarget != null)
			{
				newTarget._onEnterSnapRange.Invoke();
			}
		}
	}
}
