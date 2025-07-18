using System;
using System.Collections.Generic;
using Game.CodeBlocks;
using UnityEngine;

namespace Game.UI
{
	[DisallowMultipleComponent]
	public abstract class BaseInstruction : MonoBehaviour, IInstruction
	{
		[SerializeField] protected Transform _defaultParent;

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

			if (_parent.Equals(newParent))
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
	}
}
