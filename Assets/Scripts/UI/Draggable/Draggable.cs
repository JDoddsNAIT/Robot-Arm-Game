using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Game.UI
{
	public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		[SerializeField] private RectTransform _target;

		private Vector2 _offset;

		[Space]
		[SerializeField] private UnityEvent _onBeginDrag = new();
		[SerializeField] private UnityEvent<Vector2> _onDrag = new();
		[SerializeField] private UnityEvent _onEndDrag = new();

		public event UnityAction OnBeginDrag {
			add => _onBeginDrag.AddListener(value);
			remove => _onBeginDrag.RemoveListener(value);
		}
		public event UnityAction<Vector2> OnDrag {
			add => _onDrag.AddListener(value);
			remove => _onDrag.RemoveListener(value);
		}
		public event UnityAction OnEndDrag {
			add => _onEndDrag.AddListener(value);
			remove => _onEndDrag.RemoveListener(value);
		}

		RectTransform Target => _target == null ? transform as RectTransform : _target;

		public void AddCallbacks(IDraggableListener listener)
		{
			OnBeginDrag += listener.OnDragStart;
			OnDrag += listener.WhileDragging;
			OnEndDrag += listener.OnDragEnd;
		}

		public void RemoveCallbacks(IDraggableListener listener)
		{
			OnBeginDrag -= listener.OnDragStart;
			OnDrag -= listener.WhileDragging;
			OnEndDrag -= listener.OnDragEnd;
		}


		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
		{
			_offset = (Vector2)Target.position - eventData.position;
			_onBeginDrag.Invoke();
		}

		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			Target.position = _offset + eventData.position;
			var delta = (Vector2)Target.position - (_offset + eventData.position);
			_onDrag.Invoke(delta);
		}

		void IEndDragHandler.OnEndDrag(PointerEventData eventData)
		{
			_onEndDrag.Invoke();
		}

		public void OnDestroy()
		{
			_onBeginDrag.RemoveAllListeners();
			_onDrag.RemoveAllListeners();
			_onEndDrag.RemoveAllListeners();
		}
	}
}
