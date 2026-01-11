#nullable enable

using System.IO;
using System.Linq;

namespace Features.Persistence
{
	/// <summary>
	/// An implementation of <see cref="IDataService{T}"/> that saves and loads <typeparamref name="TData"/> from a file.
	/// </summary>
	public class FileDataService<TData> : IDataService<TData>
		where TData : class
	{
		private readonly ISerializer _serializer;
		public string FileExtension { get; }
		/// <summary>
		/// The root directory. Defaults to <see cref="Application.persistentDataPath"/>.
		/// </summary>
		public string DataPath { get; init; } = Application.persistentDataPath;

		/// <summary>
		/// Constructs a new <see cref="FileDataService{TData}"/> with the give <paramref name="serializer"/> and <paramref name="fileExtension"/>.
		/// </summary>
		/// <param name="serializer"></param>
		/// <param name="fileExtension"></param>
		public FileDataService(ISerializer serializer, string fileExtension)
		{
			_serializer = serializer;
			FileExtension = fileExtension;
		}

		/// <summary>
		/// Constructs a new <see cref="FileDataService{TData}"/> with a given <see cref="IFileSerializer"/>.
		/// </summary>
		/// <param name="serializer"></param>
		public FileDataService(IFileSerializer serializer) : this(serializer, serializer.FileExtension) { }

		protected virtual string GetFilePath(string name)
			=> Path.Combine(DataPath, string.Concat(name, '.', FileExtension));

		/// <summary>
		/// Saves the given <typeparamref name="TData"/> to a file with it's name.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="overwrite">Should existing files be overwritten?</param>
		/// <returns><see langword="true"/> if <paramref name="data"/> was saved successfully.</returns>
		/// <exception cref="IOException"></exception>
		public bool Save(string name, TData data, bool overwrite = true)
		{
			var path = GetFilePath(name);
			string? serializedData;

			try
			{
				if (!overwrite && File.Exists(path))
					throw new IOException(string.Format(Messages.FILE_DATA_SERVICE__CANNOT_OVERWRITE_FILE, name, FileExtension));

				if (_serializer.TrySerialize(data, out serializedData))
				{
					var dirName = Path.GetDirectoryName(path);
					if (!Directory.Exists(dirName))
						Directory.CreateDirectory(dirName);
					File.WriteAllText(path, serializedData);
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				serializedData = null;
			}
			return serializedData is not null;
		}

		/// <inheritdoc cref="IDataService{TData}.Load(string, out TData)"/>
		/// <exception cref="FileNotFoundException"/>
		public bool Load(string name, [NotNullWhen(true)] out TData? data)
		{
			var path = GetFilePath(name);

			try
			{
				ThrowHelper.IfFileNotExists(path, Messages.FILE_DATA_SERVICE__PERSISTENT_DATA_NOT_FOUND, name);
				string dataString = File.ReadAllText(path);
				_serializer.TryDeserialize(dataString, out data);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				data = null;
			}
			return data is not null;
		}

		public bool Delete(string name)
		{
			var path = GetFilePath(name);
			if (File.Exists(path))
			{
				File.Delete(path);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool Exists(string name) => File.Exists(GetFilePath(name));

		public IEnumerable<string> GetAll()
			=> Directory.EnumerateFiles(DataPath)
			.Where(s => s.EndsWith(FileExtension))
			.Select(Path.GetFileNameWithoutExtension);
	}

	public partial class Messages
	{
		public const string FILE_DATA_SERVICE__CANNOT_OVERWRITE_FILE = "The file '{0}.{1}' already exists and cannot be overwritten.";
		public const string FILE_DATA_SERVICE__PERSISTENT_DATA_NOT_FOUND = "No persistent data was found with the name {0}.";
	}
}
