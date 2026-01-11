#nullable enable

using System.Linq;

namespace Features.Persistence
{
	public sealed class GameDataManager : PersistentSingleton<GameDataManager>
	{
		[SerializeField] private string _saveName = string.Empty;
		[SerializeField] private GameData _gameData = new();
		private readonly IDataService<GameData> _dataService = new FileDataService<GameData>(serializer: new JsonSerializer());

		private static readonly Comparer<bool> _boolComparer = Comparer<bool>.Create(static (x, y) => (x, y) switch {
			(true, true) or (false, false) => 0,
			(false, true) => +1,
			(true, false) => -1,
		});

		public bool GameLoaded => !string.IsNullOrWhiteSpace(_saveName);
		public GameData GameData => _gameData;

		/// <summary>
		/// Creates a new save game. Unsaved game data will be lost.
		/// </summary>
		/// <param name="name"></param>
		/// <exception cref="ArgumentNullException"/>
		public void NewGame(string name)
		{
			ThrowHelper.IfNullOrWhiteSpace(name, nameof(name));
			_saveName = name;
			_gameData = new();
		}

		/// <summary>
		/// Saves the current game data to disk. Save file will be overwritten.
		/// </summary>
		/// <returns></returns>
		public bool SaveGame() => GameLoaded && _dataService.Save(_saveName, _gameData, overwrite: true);

		/// <summary>
		/// Loads the game with the given name. Unsaved game data will be lost.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"/>
		public bool LoadGame(string name)
		{
			ThrowHelper.IfNullOrWhiteSpace(name, nameof(name));
			if (_dataService.Load(name, out var data))
			{
				_saveName = name;
				_gameData = data;
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Deletes the save file with the given name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"/>
		public bool DeleteSave(string name)
		{
			ThrowHelper.IfNullOrWhiteSpace(name, nameof(name));
			return _dataService.Delete(name);
		}

		/// <summary>
		/// Binds <paramref name="data"/> to any suitable owner.
		/// </summary>
		/// <typeparam name="TOwner"></typeparam>
		/// <typeparam name="TData"></typeparam>
		/// <param name="data"></param>
		/// <param name="overwrite"></param>
		public static void Bind<TOwner, TData>(TData data, bool overwrite = false)
			where TOwner : MonoBehaviour, IDataOwner<TData>
			where TData : IData, new()
		{
			ThrowHelper.IfNull(data, nameof(data));

			var owner = FindOwner(data, FindObjectsByType<TOwner>(FindObjectsInactive.Include, FindObjectsSortMode.None));
			if (owner == null)
				throw DataBindException.OwnerNotFound();

			Bind(data, owner, overwrite);
		}

		/// <summary>
		/// Explicitly binds <paramref name="data"/> to <paramref name="owner"/>.
		/// </summary>
		/// <typeparam name="TOwner"></typeparam>
		/// <typeparam name="TData"></typeparam>
		/// <param name="data">The data to bind to <paramref name="owner"/>. Can be null.</param>
		/// <param name="owner">The new owner of <paramref name="data"/>. Cannot be null.</param>
		/// <param name="overwrite">The <paramref name="owner"/>'s current binding will be replaced with <paramref name="data"/>.</param>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="DataBindException"/>
		public static void Bind<TOwner, TData>(TData? data, TOwner owner, bool overwrite = false)
			where TOwner : MonoBehaviour, IDataOwner<TData>
			where TData : IData, new()
		{
			ThrowHelper.IfNull(owner, nameof(owner));

			if (overwrite || data is null)
			{
				owner.Unbind();
			}
			else if (owner.IsBound())
			{
				throw DataBindException.CannotOverwrite(owner);
			}

			if (data is not null)
			{
				owner.Bind(data);

				if (!owner.IsBound())
					throw DataBindException.IdMismatch(owner);
			}
		}

		/// <summary>
		/// Binds each data object to a unique owner.
		/// </summary>
		/// <typeparam name="TOwner"></typeparam>
		/// <typeparam name="TData"></typeparam>
		/// <param name="data"></param>
		/// <param name="overwrite">If a chosen owner is already bound, its data may be overwritten.</param>
		public static void BindAll<TOwner, TData>(IEnumerable<TData> data, bool overwrite = false)
			where TOwner : MonoBehaviour, IDataOwner<TData>
			where TData : IData, new()
		{
			var possibleOwners = FindObjectsByType<TOwner>(FindObjectsInactive.Include, FindObjectsSortMode.None);

			foreach (var row in data)
			{
				try
				{
					TOwner? owner = FindOwner(row, possibleOwners);
					if (owner == null)
						throw DataBindException.OwnerNotFound();
					Bind(row, owner, overwrite);
				}
				catch (DataBindException ex)
				{
					Debug.LogWarning(ex.Message);
					continue;
				}
			}
		}

		private static TOwner? FindOwner<TOwner, TData>(TData data, TOwner[] possibleOwners)
			where TOwner : MonoBehaviour, IDataOwner<TData>
			where TData : IData, new()
		{
			return possibleOwners
				.OrderBy(o => o.Id == data.Owner, _boolComparer)
				.ThenBy(static o => !o.IsBound(), _boolComparer)
				.FirstOrDefault();
		}
	}

	[Serializable]
	public class DataBindException : Exception
	{
		public const string
			  NO_SUITABLE_OWNER = "Data binding failed: No suitable owner was found."
			, OWNER_ID_MISMATCH = "Data binding failed: The ID of {0} does not match that of the data's owner."
			, OWNER_ALREADY_BOUND = "Data binding failed: {0} is already bound to another data object."
			, GENERIC_FAILURE = "Failed to bind data to {0}."
			;

		public DataBindException() { }
		public DataBindException(string message) : base(message) { }
		public DataBindException(string message, Exception inner) : base(message, inner) { }
		protected DataBindException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

		public static DataBindException OwnerNotFound() => new(NO_SUITABLE_OWNER);
		public static DataBindException IdMismatch(object owner) => new(string.Format(OWNER_ID_MISMATCH, owner));
		public static DataBindException CannotOverwrite(object owner) => new(string.Format(OWNER_ALREADY_BOUND, owner));
		public static DataBindException GenericFailure(object owner) => new(string.Format(GENERIC_FAILURE, owner));
	}
}
