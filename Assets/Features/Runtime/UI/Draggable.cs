using UnityEngine.EventSystems;

namespace Features.UI
{
	[RequireComponent(typeof(RectTransform))]
	public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		public static List<DropArea> dropAreas = new();

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
			if (dropAreas.Count > 0)
			{
				var corners = new Vector3[4];
				Target.GetWorldCorners(corners);
				var rect = GetRectFromFourCornersArray(corners);

				var areaCorners = new Vector3[4];
				foreach (var area in dropAreas)
				{
					area.RectTransform.GetWorldCorners(areaCorners);
					var areaRect = GetRectFromFourCornersArray(areaCorners);
					if (areaRect.Overlaps(rect))
					{
						area.DropObj(Target.gameObject);
						break;
					}
				}
			}
			_onEndDrag.Invoke(eventData.position);
		}

		private static Rect GetRectFromFourCornersArray(Vector3[] corners)
		{
			var min = corners[0];
			for (int i = 1; i < corners.Length; i++)
				min = Vector3.Min(min, corners[i]);
			var max = corners[0];
			for (int i = 1; i < corners.Length; i++)
				max = Vector3.Max(max, corners[i]);
			return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
		}
	}
}
