using Features.Persistence;
using UnityEngine;

namespace Features.UI
{
	public class BuildArea : MonoBehaviour
	{
		[SerializeField] private SerializableGuid _levelId = Guid.NewGuid();
		[SerializeField] private Transform _spawnArea;

		public SerializableGuid Id => _levelId;
		
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
