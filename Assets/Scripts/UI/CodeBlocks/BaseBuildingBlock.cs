using System.Collections.Generic;
using System.Linq;
using Game.CodeBlocks;
using UnityEngine;
using Utilities.Extensions;

namespace Game.UI
{
	[RequireComponent(typeof(Draggable))]
	public abstract class BaseBuildingBlock : MonoBehaviour, IBlock
	{
		[SerializeField] protected Transform _defaultParent;
		[SerializeField] protected SnappingPoint _top, _bottom;

		protected Draggable _draggable;

		protected bool _preventRecursion = false;
		protected IBlock _parent;
		protected readonly List<IBlock> _children = new();

		#region IBlock Implementation
		MonoBehaviour IBlock.Behaviour => this;

		public virtual void AddChild(IBlock child)
		{
			if (_preventRecursion)
				return;

			if (child is null || child.Equals(null))
				return;

			if (_children.Contains(child))
				return;

			_preventRecursion = true;

			child.SetParent(this);
			_children.Add(child);

			_preventRecursion = false;
		}

		public virtual void RemoveChild(IBlock child)
		{
			if (_preventRecursion)
				return;

			if (child is null || child.Equals(null))
				return;

			if (!_children.Contains(child))
				return;

			_preventRecursion = true;

			child.SetParent(null);
			_children.Remove(child);

			_preventRecursion = false;
		}

		public virtual void SetParent(IBlock newParent)
		{
			if (_preventRecursion)
				return;

			if (_parent is null && newParent is null)
				return;

			_preventRecursion = true;

			if (_parent is not null)
			{
				transform.SetParent(_defaultParent);
				_parent.RemoveChild(this);
			}

			_parent = newParent;

			if (newParent is not null)
			{
				if (newParent.Behaviour != null)
					transform.SetParent(newParent.Behaviour.transform);
				else
					transform.SetParent(_defaultParent);

				newParent.AddChild(this);
			}

			_preventRecursion = false;
		}

		public virtual IBlock GetNext() => _children.FirstOrDefault();

		public abstract ICodeBlock CreateBlock();
		#endregion

		public virtual void OnValidate()
		{
			GetDraggable();
		}

		public virtual void Awake()
		{
			GetDraggable();

			_draggable.OnBeginDrag += this.Draggable_OnBeginDrag;
			_draggable.OnSnapTo += this.Draggable_OnSnapTo;
		}

		protected virtual void GetDraggable()
		{
			if (_draggable == null)
				TryGetComponent(out _draggable);
		}

		protected virtual void Draggable_OnBeginDrag()
		{
			_top.enabled = true;
			if (_parent is BaseBuildingBlock block)
				block._bottom.enabled = true;

			this.SetParent(null);
		}

		protected virtual void Draggable_OnSnapTo(SnappingPoint from, SnappingPoint to)
		{
			if (to.Get().Component<IBlock>().InParent(out var block))
			{
				Debug.Assert(!block.Equals(this), "Disable snapping to the top point.");
				from.enabled = false;
				to.enabled = false;
				this.SetParent(block);
			}
		}

		public virtual void OnDestroy()
		{
			if (_draggable == null)
				return;

			_draggable.OnBeginDrag -= this.Draggable_OnBeginDrag;
			_draggable.OnSnapTo -= this.Draggable_OnSnapTo;
		}
	}
}
