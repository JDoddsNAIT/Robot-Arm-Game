using UnityEngine;

namespace Features.UI
{
	[RequireComponent(typeof(RectTransform))]
	public class DropArea : MonoBehaviour
	{
		private RectTransform _rectTransform;

		[SerializeField] private UnityEvent<GameObject> _onDraggableDropped;

		public event UnityAction<GameObject> OnDraggableDropped {
			add => _onDraggableDropped.AddListener(value);
			remove => _onDraggableDropped.RemoveListener(value);
		}

		public RectTransform RectTransform => _rectTransform;

		public void Awake()
		{
			_rectTransform = transform as RectTransform;
		}

		public void OnEnable()
		{
			Draggable.dropAreas.Add(this);
		}

		public void OnDisable()
		{
			Draggable.dropAreas.Remove(this);
		}

		public void DropObj(GameObject obj)
		{
			_onDraggableDropped.Invoke(obj);
		}
	}
}
