using UnityEngine;

namespace Features.UI
{
	public class BuildArea : MonoBehaviour
	{
		[SerializeField] private Transform _spawnArea;

		public void OnDropLogicGate(GameObject gameObject)
		{
			if (gameObject.TryGetComponent(out ToolboxLogicGate component))
			{
				var clone = Instantiate(component.prefab, _spawnArea);
				clone.transform.position = gameObject.transform.position;
			}
		}
	}
}
