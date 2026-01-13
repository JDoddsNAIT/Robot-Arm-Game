using UnityEngine.EventSystems;

namespace Features.UI
{
	[RequireComponent(typeof(RectTransform))]
	public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		private Vector2 _offset;

		[SerializeField] private RectTransform _target;
		[Space]
		[SerializeField] private UnityEvent<Vector2> _onStartDrag;
		[SerializeField] private UnityEvent<Vector2> _onEndDrag;

		private RectTransform Target => _target == null ? this.transform as RectTransform : _target;

		public event UnityAction<Vector2> OnStartDrag {
			add => _onStartDrag.AddListener(value);
			remove => _onStartDrag.RemoveListener(value);
		}

		public event UnityAction<Vector2> OnEndDrag {
			add => _onEndDrag.AddListener(value);
			remove => _onEndDrag.RemoveListener(value);
		}

		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
		{
			_offset = (Vector2)Target.position - eventData.position;
			_onStartDrag.Invoke(eventData.position);
		}

		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			Target.position = _offset + eventData.position;
		}

		void IEndDragHandler.OnEndDrag(PointerEventData eventData)
		{
			_onEndDrag.Invoke(eventData.position);
		}
	}
}
