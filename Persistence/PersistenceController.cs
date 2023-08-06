using Godot;

using System;

namespace SadChromaLib.Persistence;

using SerialisedData = Godot.Collections.Dictionary<StringName, Variant>;

/// <summary>
/// A persistent object that writes to its own file.
/// </summary>
public interface ISerialisable
{
	void SerialisableWrite(FileAccess fileRef);
	void SerialisableRead(string serialisedData);
	string SerialisableGetFilename();
}

/// <summary>
/// A persistent object that can be used as building blocks for standalone serialisables
/// </summary>
public interface ISerialisableComponent
{
	SerialisedData Serialise();
	void Deserialise(SerialisedData data);
}

/// <summary>
/// A shared controller object for coordinating persistence writes
/// </summary>
[GlobalClass]
public sealed partial class PersistenceController: Resource
{
	public const string SaveNameAuto = "Autosave";
	private const string BaseDir = "user://SaveData";
	private const string SaveNameTemporary = "_tmp";

	[Signal]
	public delegate void WillLoadEventHandler();

	[Signal]
	public delegate void SaveRequestedEventHandler(PersistenceController persistence, string saveName);

	[Signal]
	public delegate void LoadRequestedEventHandler(PersistenceController persistence, string saveName);

	[Signal]
	public delegate void SaveTransientRequestedEventHandler();

	[Signal]
	public delegate void RestoreTransientRequestedEventHandler();

	private string _lastSaveName;

	#region Main Functions

	/// <summary>
	/// <para>Tells standalone serialisables to write their data to disk.</para>
	/// <para>* It will only call objects bound to the 'SaveRequested' signal.</para>
	/// <para>* Use the static 'WriteToDisk' method to perform the write operation.</para>
	/// </summary>
	/// <param name="saveName">The name of the target save directory.</param>
	public void Save(string saveName)
	{
		MakeSaveDirIfNeeded(saveName);
		EmitSignal(SignalName.SaveRequested, this, saveName);

		_lastSaveName = saveName;
	}

	/// <summary>
	/// <para>Tells standalone serialisables to read their data from disk.</para>
	/// <para>* It will only call objects bound to the 'LoadRequested' signal.</para>
	/// <para>* Use the static 'ReadFromDisk' method to perform the write operation.</para>
	/// </summary>
	/// <param name="saveName">The name of the target save directory.</param>
	public void Load(string saveName)
	{
		EmitSignal(SignalName.WillLoad);
		EmitSignal(SignalName.LoadRequested, this, saveName);
	}

	/// <summary>
	/// A shorthand for a save request through the 'SaveNameAuto' file
	/// </summary>
	public void SaveAutoSave()
	{
		Save(SaveNameAuto);
	}

	/// <summary>
	/// A shorthand for a load request through the 'SaveNameAuto' file
	/// </summary>
	public void LoadAutoSave()
	{
		Load(SaveNameAuto);
	}

	#endregion

	#region Transient Data

	/// <summary>
	/// Starts a read request using the last loaded save data
	/// </summary>
	/// <param name="serialisable"></param>
	public void RestoreFromCurrent(ISerialisable serialisable)
	{
		if (_lastSaveName == null)
			return;

		ReadFromDisk(_lastSaveName, serialisable);
	}

	/// <summary>
	/// <para>Writes temporary standalone serialisers into the transient save data.</para>
	/// <para>* It will only call objects bound to the 'SaveTransientRequested' signal.</para>
	/// <para>* Use the static 'WriteToTemporary' method to perform the write operation.</para>
	/// </summary>
	public void SaveTransient()
	{
		EmitSignal(SignalName.SaveTransientRequested);
	}

	/// <summary>
	/// <para>Reads temporary standalone serialisers from the transient save data.</para>
	/// <para>* It will only call objects bound to the 'LoadTransientRequested' signal.</para>
	/// <para>* Use the static 'ReadFromTemporary' method to perform the write operation.</para>
	/// </summary>
	public void RestoreTransient()
	{
		EmitSignal(SignalName.RestoreTransientRequested);
	}

	/// <summary>
	/// Deletes the transient saved data.
	/// </summary>
	public static void DeleteTransientData()
	{
		DeleteSaveData(SaveNameTemporary);
	}

	/// <summary>
	/// Returns true if a transient save data exists.
	/// </summary>
	/// <returns></returns>
	public static bool HasTransientData()
	{
		string baseDir = GetBaseDir(SaveNameTemporary);
		return DirAccess.DirExistsAbsolute(baseDir);
	}

	#endregion

	#region I/O methods

	/// <summary>
	/// Returns a list of the player's saved data.
	/// </summary>
	/// <returns></returns>
	public static string[] GetSaveNames()
	{
		if (!DirAccess.DirExistsAbsolute(BaseDir)) {
			DirAccess.MakeDirRecursiveAbsolute(BaseDir);
		}

		ReadOnlySpan<string> saveNames = DirAccess.GetDirectoriesAt(BaseDir);
		Span<string> names = new string[saveNames.Length];

		int namesCount = 0;

		for (int i = 0; i < saveNames.Length; ++ i) {
			if (saveNames[i] == SaveNameTemporary)
				continue;

			names[namesCount] = saveNames[i];
			namesCount ++;
		}

		return names[..namesCount].ToArray();
	}

	/// <summary>
	/// Deletes a save data with a given name.
	/// </summary>
	/// <param name="saveName">The name of the save data to delete.</param>
	public static void DeleteSaveData(string saveName)
	{
		string baseDir = GetBaseDir(saveName);

		if (!DirAccess.DirExistsAbsolute(baseDir))
			return;

		DirAccess directory = DirAccess.Open(baseDir);
		string[] dataFiles = directory.GetFiles();

		for (int i = 0; i < dataFiles.Length; ++ i) {
			directory.Remove(dataFiles[i]);
		}

		DirAccess.RemoveAbsolute(baseDir);
	}

	public static bool WriteToDisk(string saveName, ISerialisable serialisable)
	{
		string baseDir = GetBaseDir(saveName);
		MakeSaveDirIfNeeded(saveName);

		string filePath = $"{baseDir}/{serialisable.SerialisableGetFilename()}";
		FileAccess file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write);

		if (file == null)
			return false;

		serialisable.SerialisableWrite(file);

		if (file.IsOpen()) {
			file.Close();
		}

		return true;
	}

	public static bool ReadFromDisk(string saveName, ISerialisable serialisable)
	{
		string baseDir = GetBaseDir(saveName);
		string filePath = $"{baseDir}/{serialisable.SerialisableGetFilename()}";

		if (!DirAccess.DirExistsAbsolute(baseDir) ||
			!FileAccess.FileExists(filePath))
		{
			return false;
		}

		FileAccess file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);

		if (file == null)
			return false;

		serialisable.SerialisableRead(file.GetAsText());
		file.Close();

		return true;
	}

	public static void WriteToTemporary(ISerialisable serialisable)
	{
		WriteToDisk(SaveNameTemporary, serialisable);
	}

	public static void ReadFromTemporary(ISerialisable serialisable)
	{
		ReadFromDisk(SaveNameTemporary, serialisable);
	}

	public static bool CopyData(string srcSaveName, string dstSaveName, Func<string, bool> filter)
	{
		string srcBaseDir = GetBaseDir(srcSaveName);
		string dstBaseDir = GetBaseDir(dstSaveName);

		if (!DirAccess.DirExistsAbsolute(dstBaseDir)) {
			DirAccess.MakeDirRecursiveAbsolute(dstBaseDir);
		}

		if (!DirAccess.DirExistsAbsolute(srcBaseDir) ||
			!DirAccess.DirExistsAbsolute(dstBaseDir))
		{
			return false;
		}

		ReadOnlySpan<string> fileList = DirAccess.GetFilesAt(srcBaseDir);

		for (int i = 0; i < fileList.Length; ++ i) {
			if (!(filter?.Invoke(fileList[i]) ?? true))
				continue;

			string srcFilePath = $"{srcBaseDir}/{fileList[i]}";
			string dstFilePath = $"{dstBaseDir}/{fileList[i]}";

			DirAccess.CopyAbsolute(srcFilePath, dstFilePath);
		}

		return true;
	}

	public static bool CopyDataFromTemporary(string saveName, Func<string, bool> filter)
	{
		return CopyData(SaveNameTemporary, saveName, filter);
	}

	public static bool CopyDataToTemporary(string saveName, Func<string, bool> filter)
	{
		return CopyData(saveName, SaveNameTemporary, filter);
	}

	private static void MakeSaveDirIfNeeded(string saveName)
	{
		string baseDir = GetBaseDir(saveName);

		if (DirAccess.DirExistsAbsolute(baseDir))
			return;

		DirAccess.MakeDirRecursiveAbsolute(baseDir);
	}

	#endregion

	private static string GetBaseDir(string saveName)
	{
		return $"{BaseDir}/{saveName}";
	}
}