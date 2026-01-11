#nullable enable

namespace Features
{
	/// <summary>
	/// If no instance exists or this object has an equal or greater priority than main at the time of <see cref="Awake"/>, this object will be set as the main
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class RegulatorSingleton<T> : Singleton<T>
		where T : Component
	{
		private static int _instancePriority = int.MinValue;
		protected int _singletonPriority = 0;

		static RegulatorSingleton() => OnSetMain += RegulatorSingleton_OnSetMain;

		private static void RegulatorSingleton_OnSetMain(T? oldInstance, T? newInstance)
		{
			if (newInstance != null)
			{
				var singleton = SingletonHelpers.GetSingleton<RegulatorSingleton<T>, T>(newInstance);
				_instancePriority = singleton._singletonPriority;
			}
			else
			{
				_instancePriority = int.MinValue;
			}
		}

		protected override void Awake()
		{
			transform.SetParent(null);
			DontDestroyOnLoad(this);
			if (Application.isPlaying && HasInstance)
				Destroy(Main);
			SetMain(SingletonHelpers.GetValue<RegulatorSingleton<T>, T>(this));
		}
	}
}
