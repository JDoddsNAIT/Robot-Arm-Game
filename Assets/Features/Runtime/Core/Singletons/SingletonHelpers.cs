#nullable enable

namespace Features
{
	public delegate void OnSetMain<in T>(T? oldInstance, T? newInstance);

	/// <summary>
	/// Indicates this type should be initialized when set as the main instance of this type.
	/// </summary>
	public interface ISingleton
	{
		/// <summary>
		/// Has this object been initialized? (Read only)
		/// </summary>
		bool Initialized { get; }

		/// <summary>
		/// Initialize this object as the main instance.
		/// </summary>
		void Initialize();
	}

	/// <summary>
	/// Static helper class for singletons.
	/// </summary>
	internal static class SingletonHelpers
	{
		public static T FindOrCreateGameObject<T>() where T : Component
		{
			T instance = Object.FindAnyObjectByType<T>();
			if (instance == null)
			{
				instance = new GameObject(name: $"{typeof(T).Name} (Auto-Generated)").AddComponent<T>();
				Debug.LogFormat(Messages.SINGLETON__CREATED_SUPPLEMENTRY_OBJECT, typeof(T));
			}
			return instance;
		}

		public static T FindOrCreateScriptableObject<T>() where T : ScriptableObject
		{
			T instance = Object.FindAnyObjectByType<T>();
			if (instance == null)
			{
				instance = ScriptableObject.CreateInstance<T>();
				instance.name = $"{typeof(T).Name} (Auto-Generated)";
				Debug.LogFormat(Messages.SINGLETON__CREATED_SUPPLEMENTRY_OBJECT, typeof(T));
			}
			return instance;
		}

		public static T GetValue<TSingleton, T>(TSingleton singleton)
			where TSingleton : Singleton<T>
			where T : Component
		{
			if (singleton is T component || (singleton != null && singleton.TryGetComponent(out component)))
			{
				return component;
			}
			else
			{
				throw new InvalidOperationException(string.Format(Messages.SINGLETON__COMPONENT_CONVERSION_FAILED, typeof(TSingleton), typeof(T)));
			}
		}

		public static TSingleton GetSingleton<TSingleton, T>(T component)
			where TSingleton : Singleton<T>
			where T : Component
		{
			if (component is TSingleton singleton || (component != null && component.TryGetComponent(out singleton)))
			{
				return singleton;
			}
			else
			{
				throw new InvalidOperationException(string.Format(Messages.SINGLETON__COMPONENT_CONVERSION_FAILED, typeof(TSingleton), typeof(T)));
			}
		}
	}

	public partial class Messages
	{
		public const string
			  SINGLETON__CREATED_SUPPLEMENTRY_OBJECT = "Created a supplementary {0} instance as the instance property was accessed but no instances existed."
			, SINGLETON__COMPONENT_CONVERSION_FAILED = "Failed to convert between {0} and {1}. {1} should be a " + nameof(Component) + " that either inherits from {0} or is attached to the same object as the singleton."
			, SINGLETON__ASSET_CONVERSION_FAILED = "Failed to convert between {0} and {1}. {1} must be a" + nameof(ScriptableObject) + " that inherits from {0}."
			;
	}
}
