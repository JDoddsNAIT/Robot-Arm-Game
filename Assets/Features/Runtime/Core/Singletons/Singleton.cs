#nullable enable

namespace Features
{
	/// <summary>
	/// Simple singleton. If no instance exists at the time of <see cref="Awake"/>, this object will be set as the main.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class Singleton<T> : MonoBehaviour
		where T : Component
	{
		protected static T? _instance;

		/// <summary>
		/// Is the underlying <typeparamref name="T"/> instance not null? (Read only)
		/// </summary>
		public static bool HasInstance => _instance != null;

		/// <summary>
		/// The main <typeparamref name="T"/> instance. (Read only)
		/// </summary>
		/// <remarks>
		/// A new <see cref="GameObject"/> with a <typeparamref name="T"/> component attached will be created and returned if no main instance exists. Use the <see cref="HasInstance"/> property to perform null checks on the underlying value.
		/// </remarks>
		public static T Main {
			get {
				if (_instance == null)
					SetMain(SingletonHelpers.FindOrCreateGameObject<T>());
				return _instance!;
			}
		}

		/// <summary>
		/// Invoked when <see cref="Main"/> is set to a new instance.
		/// </summary>
		public static event OnSetMain<T> OnSetMain = delegate { };

		/// <summary>
		/// Sets <see cref="Main"/> to the given <paramref name="value"/>.
		/// </summary>
		/// <param name="value">The new main instance. Can be null.</param>
		public static void SetMain(T? value)
		{
			bool invoke = _instance != value;
			var oldValue = _instance;
			_instance = value;
			if (value is ISingleton singleton && !singleton.Initialized)
			{
				singleton.Initialize();
				Assert.IsTrue(singleton.Initialized, Messages.SINGLETON__NOT_INITIALIZED, value);
			}

			if (invoke)
				OnSetMain.Invoke(oldValue, value);
		}

		/// <summary>
		/// It is recommended to implement <see cref="ISingleton.Initialize"/> for custom initialization logic instead of overriding <see cref="Awake"/>.
		/// </summary>
		protected virtual void Awake()
		{
			if (!HasInstance)
			{
				SetMain(SingletonHelpers.GetValue<Singleton<T>, T>(this));
			}
		}
	}

	public partial class Messages
	{
		public const string SINGLETON__NOT_INITIALIZED = "{0} was not properly initialized as the singleton.";
	}
}
