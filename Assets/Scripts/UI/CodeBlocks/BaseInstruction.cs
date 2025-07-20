using System;
using System.Collections.Generic;
using Game.CodeBlocks;
using UnityEngine;
using Utilities.Extensions;

namespace Game.UI
{
	[DisallowMultipleComponent]
	public abstract class BaseInstruction : MonoBehaviour, IInstruction, IDraggableListener
	{
		protected SnappingPoint _topConnection;

		[SerializeField] protected Transform _defaultParent;
		[SerializeField] protected Draggable _draggable;
		[SerializeField] protected SnappingPoint _topPoint;

		protected bool _preventRecursion = false;
		protected IInstruction _parent;
		protected List<IInstruction> _children = new();

		#region IUIBlock Implementation
		public abstract string Name { get; }

		public abstract void Duplicate();
		public abstract void Delete();
		#endregion

		#region IInstruction Implementation
		Component IInstruction.Component => this;

		public abstract ICodeBlock CreateBlock();

		public virtual IInstruction GetParent() => _parent;
		public virtual void SetParent(IInstruction newParent)
		{
			if (_preventRecursion)
				return;

			if (_parent == newParent)
				return;

			_preventRecursion = true;

			if (_parent?.Component != null)
			{
				_parent.RemoveChild(this);

				if (newParent is null || newParent.Component == null)
					transform.SetParent(_defaultParent);
			}

			_parent = newParent;

			if (newParent?.Component != null)
			{
				newParent.AddChild(this);
				transform.SetParent(newParent.Component.transform);
			}

			_preventRecursion = false;
		}

		public virtual void AddChild(IInstruction child)
		{
			if (_preventRecursion)
				return;

			if (child is null || child.Component == null)
				return;

			if (_children.Contains(child))
				return;

			_preventRecursion = true;

			_children.Add(child);
			child.SetParent(this);

			_preventRecursion = false;
		}
		public virtual void RemoveChild(IInstruction child)
		{
			if (_preventRecursion)
				return;

			if (child is null || child.Component == null)
				return;

			if (!_children.Contains(child))
				return;

			_preventRecursion = true;

			_children.Remove(child);
			child.SetParent(null);

			_preventRecursion = false;
		}
		#endregion

		public void Start()
		{
			if (_draggable != null)
			{
				_draggable.AddCallbacks(this);
			}

			if (_topPoint != null)
			{
				_topPoint.OnSnapTo += this.TopPoint_OnSnapTo;
			}
		}

		private void TopPoint_OnSnapTo(SnappingPoint other)
		{
			Debug.Assert(other != null);
			if (other.Get().Component<IInstruction>().InParent(out var component))
			{
				Debug.Assert(component != null);
				this.SetParent(component);
				_topPoint.enabled = false;
				_topConnection = other;
				other.enabled = false;
			}
		}

		public void OnDestroy()
		{
			if (_draggable != null)
			{
				_draggable.RemoveCallbacks(this);
			}

			if (_topPoint != null)
			{
				_topPoint.OnSnapTo -= this.TopPoint_OnSnapTo;
			}
		}

		public virtual void OnDragStart()
		{
			SetParent(null);
			if (_topConnection != null)
			{
				_topConnection.enabled = true;
				_topConnection = null;
			}

			_topPoint.enabled = true;
		}

		public virtual void WhileDragging(Vector2 delta) { /* no-op */ }

		public virtual void OnDragEnd() { /* no-op */ }
	}
}
