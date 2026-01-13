using System.Linq;
using System.Threading.Tasks;
using Eflatun.SceneReference;
using UnityEngine.UI;

namespace Features.SceneManagement
{
	public class SceneGroupManager : MonoBehaviour
	{
		/// <summary>
		/// The most recently created instance of <see cref="SceneGroupManager"/>.
		/// </summary>
		public static SceneGroupManager Main { get; private set; }

		[SerializeField] private Image _loadingProgressBar;
		[SerializeField] private float _fillSpeed = 0.5f;
		[SerializeField] private Canvas _loadingCanvas;
		[SerializeField] private Camera _loadingCamera;
		[Space]
		[Tooltip("List of scenes that will always remain loaded.")]
		[SerializeField] private SceneReference[] _persistentScenes = Array.Empty<SceneReference>();
		[Tooltip("The scene group to load at startup.")]
		[SerializeField] private SceneGroup _initialGroup;
		[Tooltip("List of scene groups that can be indexed through this object.")]
		[SerializeField] private SceneGroup[] _sceneGroups = new SceneGroup[1];

		private readonly SceneGroupLoader _loader = new();
		private float _targetProgress;
		private bool _isLoading;

		public event SceneGroupLoader.EventHandler OnSceneLoaded {
			add => _loader.OnSceneLoaded += value;
			remove => _loader.OnSceneLoaded -= value;
		}
		public event SceneGroupLoader.EventHandler OnSceneUnloaded {
			add => _loader.OnSceneUnloaded += value;
			remove => _loader.OnSceneUnloaded -= value;
		}
		public event SceneGroupLoader.EventHandler OnSceneGroupLoaded {
			add => _loader.OnSceneGroupLoaded += value;
			remove => _loader.OnSceneGroupLoaded -= value;
		}

		private float FillAmount {
			get => _loadingProgressBar != null ? _loadingProgressBar.fillAmount : 0;
			set { if (_loadingProgressBar != null) _loadingProgressBar.fillAmount = value; }
		}

		public void Awake()
		{
			Main = this;
			foreach (var scene in _persistentScenes)
			{
				_loader.AddPersistentScene(scene.Name);
			}
		}

		private async void Start()
		{
			if (_initialGroup != null)
				await LoadSceneGroup(_initialGroup);
		}

		private void Update()
		{
			if (!_isLoading) return;

			float currentFillAmount = FillAmount;
			float progressDifference = Mathf.Abs(currentFillAmount - _targetProgress);
			float speed = _fillSpeed * progressDifference;
			FillAmount = Mathf.Lerp(currentFillAmount, _targetProgress, speed * Time.unscaledDeltaTime);
		}

		public async Task LoadSceneGroup(string name)
		{
			SceneGroup group = _sceneGroups.FirstOrDefault(g => g.name == name);
			if (group == null) throw new ArgumentOutOfRangeException(nameof(name));
			await LoadSceneGroup(group);
		}

		public async Task LoadSceneGroup(int index)
		{
			if (index < 0 || index >= _sceneGroups.Length) throw new ArgumentOutOfRangeException(nameof(index));
			await LoadSceneGroup(_sceneGroups[index]);
		}

		public async Task LoadSceneGroup(SceneGroup group)
		{
			ThrowHelper.IfNull(group, nameof(group));

			FillAmount = 0;
			_targetProgress = 0f;

			var progress = new LoadProgress();
			progress.OnProgress += target => _targetProgress = Mathf.Max(target, _targetProgress);
			EnableLoadingScreen();
			await _loader.LoadScenesAsync(group, progress);
			EnableLoadingScreen(false);
		}

		private void EnableLoadingScreen(bool enabled = true)
		{
			_isLoading = enabled;
			if (_loadingCanvas != null)
				_loadingCanvas.enabled = enabled;
			if (_loadingCamera != null)
				_loadingCamera.enabled = enabled;
		}
	}

	public class LoadProgress : IProgress<float>
	{
		public event Action<float> OnProgress;
		const float ratio = 1f;

		public void Report(float value) => OnProgress?.Invoke(value / ratio);
	}
}
