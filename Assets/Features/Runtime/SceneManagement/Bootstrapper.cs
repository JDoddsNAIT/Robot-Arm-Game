using UnityEngine.SceneManagement;

namespace Features.SceneManagement
{
	public class Bootstrapper : MonoBehaviour
	{
		public const string SCENE_NAME = "Bootstrapper";

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static async void InitializeBootstrapper()
		{
			Debug.Log("Starting bootstrapper...");
			await SceneManager.LoadSceneAsync(SCENE_NAME, LoadSceneMode.Single);
		}

		public static Bootstrapper Main { get; private set; }

		private void Awake()
		{
			Main = this;
		}
	}
}
