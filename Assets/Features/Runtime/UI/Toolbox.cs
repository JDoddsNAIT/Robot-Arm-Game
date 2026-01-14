using UnityEngine;

namespace Features.UI
{
	public class Toolbox : MonoBehaviour
	{
		[SerializeField] private Transform _content;
		[SerializeField] private GameObject _contentPrefab;
		[SerializeField] private GameObject[] _spawnables;

		private void Start()
		{
			foreach (var obj in _spawnables)
			{
				var clone = Instantiate(_contentPrefab, _content);
				clone.name = obj.name;
				var toolboxLogicGate = clone.GetComponent<ToolboxLogicGate>();
				toolboxLogicGate.prefab = obj;
			}
		}

		public void DestroyObject(GameObject obj) => Destroy(obj);
	}
}
