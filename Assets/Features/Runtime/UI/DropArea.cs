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

		public bool Drop(GameObject obj, Vector2 position, Vector3[] cornersArray)
		{
			_rectTransform.GetWorldCorners(cornersArray);
			var rect = new Rect(cornersArray[1], cornersArray[3] - cornersArray[1]);

			if (rect.Contains(position))
			{
				_onDraggableDropped.Invoke(obj);
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
