using UnityEngine;

namespace Game.UI
{
	public interface IDraggableListener
	{
		void OnDragStart();
		void WhileDragging(Vector2 delta);
		void OnDragEnd();
	}
}
