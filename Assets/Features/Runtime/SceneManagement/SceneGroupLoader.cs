using System.Linq;
using System.Threading.Tasks;
using Eflatun.SceneReference;
using UnityEngine.SceneManagement;

namespace Features.SceneManagement
{
	public class SceneGroupLoader
	{
		public delegate void EventHandler(string name);

		public event EventHandler OnSceneLoaded;
		public event EventHandler OnSceneUnloaded;
		public event EventHandler OnSceneGroupLoaded;

		private SceneGroup _activeGroup;

		private readonly HashSet<string> _persistentScenes;

		public bool UnloadAssets { get; set; }

		public SceneGroupLoader(params SceneReference[] persistentScenes)
			: this(persistentScenes.Select(static s => s.Name))
		{ }
		public SceneGroupLoader(IEnumerable<string> persistentScenes)
			=> _persistentScenes = new HashSet<string>(persistentScenes);

		public void AddPersistentScene(string name) => _persistentScenes.Add(name);

		public async Task LoadScenesAsync(SceneGroup group, IProgress<float> progress, bool reloadScenes = false)
		{
			if (_activeGroup != null)
				await UnloadScenesAsync(_activeGroup, group.Select(static d => d.Name));

			_activeGroup = group;

			var loadedScenes = new HashSet<string>();
			for (int i = 0; i < SceneManager.sceneCount; i++)
				loadedScenes.Add(SceneManager.GetSceneAt(i).name);

			var loadGroups = group.GetLoadGroups();
			var operationGroup = new AsyncOperationGroup();

			for (int i = 0; i < loadGroups.Length; i++)
			{
				operationGroup.Operations.Clear();

				Debug.Log("Loading scenes: " + string.Join(", ", loadGroups[i]));

				foreach (var data in loadGroups[i])
				{
					if (!reloadScenes && loadedScenes.Contains(data.Name))
						continue;

					OnSceneLoaded?.Invoke(data.Name);
					var operation = SceneManager.LoadSceneAsync(data.Scene.Path, LoadSceneMode.Additive);
					operationGroup.Operations.Add(operation);
				}

				float offset = (float)i / loadGroups.Length;
				while (!operationGroup.IsDone)
				{
					await Awaitable.WaitForSecondsAsync(0.1f);
					progress?.Report(offset + operationGroup.Progress);
				}
			}

			try
			{
				var activeSceneName = _activeGroup.FindSceneNameByType(SceneType.ActiveScene);
				if (!string.IsNullOrEmpty(activeSceneName))
				{
					var activeScene = SceneManager.GetSceneByName(activeSceneName);
					if (activeScene.IsValid())
						SceneManager.SetActiveScene(activeScene);
				}
			}
			catch
			{ 
				throw;
			}

			OnSceneGroupLoaded?.Invoke(group.name);
		}

		public async Task UnloadScenesAsync(SceneGroup group, IEnumerable<string> persistentScenes = null)
		{
			var persistentScenesSet = new HashSet<string>(persistentScenes ?? Enumerable.Empty<string>());
			var unloadGroups = group.GetLoadGroups();
			var operationGroup = new AsyncOperationGroup();

			for (int i = unloadGroups.Length - 1; i >= 0; i--)
			{
				operationGroup.Operations.Clear();

				Debug.Log("Unloading scenes: " + string.Join(", ", unloadGroups[i]));
				foreach (var data in unloadGroups[i])
				{
					if (_persistentScenes.Contains(data.Name) || persistentScenesSet.Contains(data.Name))
						continue;

					OnSceneUnloaded?.Invoke(data.Name);
					var operation = SceneManager.UnloadSceneAsync(data.Scene.Path);
					operationGroup.Operations.Add(operation);
				}

				while (!operationGroup.IsDone)
				{
					await Awaitable.WaitForSecondsAsync(0.1f);
				}
			}

			if (UnloadAssets)
				await Resources.UnloadUnusedAssets();
		}
	}

	public readonly struct AsyncOperationGroup
	{
		public float Progress => Operations.Count == 0 ? 0 : Operations.Average(static o => o.progress);
		public bool IsDone => Operations.All(static o => o.isDone);
		public List<AsyncOperation> Operations { get; }

		public AsyncOperationGroup() : this(0) { }
		public AsyncOperationGroup(int capacity) => Operations = new List<AsyncOperation>(capacity);
	}
}
