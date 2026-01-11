#nullable enable

namespace Features
{
	/// <summary>
	/// If no instance exists at the time of <see cref="Awake"/>, this object will be set as the main. Otherwise this object will be destroyed.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class PersistentSingleton<T> : Singleton<T>
		where T : Component
	{
		protected override void Awake()
		{
			if (!HasInstance)
			{
				transform.SetParent(null);
				if (Application.isPlaying)
					DontDestroyOnLoad(this);
				SetMain(SingletonHelpers.GetValue<PersistentSingleton<T>, T>(this));
			}
			else if (_instance != this)
			{
				if (Application.isPlaying)
					Destroy(this);
			}
		}
	}
}
