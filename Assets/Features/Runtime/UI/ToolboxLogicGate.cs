namespace Features.UI
{
	public class ToolboxLogicGate : MonoBehaviour
	{
		public Transform prefabParent;
		public GameObject prefab;

		public void OnDrop()
		{
			transform.localPosition = Vector3.zero;
		}
	}
}
