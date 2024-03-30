global using DataDict = System.Collections.Generic.Dictionary<string, SadChromaLib.Types.AnyData>;

using Godot;

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace SadChromaLib.Persistence;

/// <summary>
/// A persistent object that writes to its own file.
/// </summary>
public interface ISerialisable
{
	void SerialisableWrite(PersistenceWriter writer);
	void SerialisableRead(PersistenceReader reader);
	string SerialisableGetFilename();
}

/// <summary>
/// A persistent object that can be used as building blocks for standalone serialisables
/// </summary>
public interface ISerialisableComponent
{
	void Serialise(PersistenceWriter writer);
	void Deserialise(PersistenceReader reader);
}

/// <summary>
/// A shared controller object for coordinating persistence writes
/// </summary>
[GlobalClass]
public sealed partial class PersistenceController: Resource
{
	public const string SaveNameAuto = "Autosave";
	private const string SaveNameTemporary = "Transient";

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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool HasTransientData()
	{
		return Directory.Exists(GetDirPath(SaveNameTemporary));
	}

	#endregion

	#region I/O methods

	/// <summary>
	/// Returns a list of the player's saved data.
	/// </summary>
	/// <returns></returns>
	public static string[] GetSaveNames()
	{
		string baseDirPath = GetDirPath();

		if (!Directory.Exists(baseDirPath)) {
			Directory.CreateDirectory(baseDirPath);
		}

		ReadOnlySpan<string> saveNames = Directory.GetDirectories(baseDirPath);
		string[] names = new string[saveNames.Length];

		int namesCount = 0;

		for (int i = 0; i < saveNames.Length; ++ i) {
			if (saveNames[i] == SaveNameTemporary)
				continue;

			names[namesCount] = Path.GetDirectoryName(saveNames[i]);
			namesCount ++;
		}

		return names[..namesCount];
	}

	/// <summary>
	/// Deletes a save data with a given name.
	/// </summary>
	/// <param name="saveName">The name of the save data to delete.</param>
	public static void DeleteSaveData(string saveName)
	{
		string dirPath = GetDirPath(saveName);

		if (!Directory.Exists(dirPath))
			return;

		Directory.Delete(dirPath, true);
	}

	public static bool WriteToDisk(string saveName, ISerialisable serialisable)
	{
		string baseDir = GetDirPath(saveName);
		MakeSaveDirIfNeeded(saveName);

		string filePath = GetFilePath(saveName, serialisable.SerialisableGetFilename());

		using (PersistenceWriter serialiser = new(filePath)) {
			serialisable.SerialisableWrite(serialiser);
		}

		return true;
	}

	public static bool ReadFromDisk(string saveName, ISerialisable serialisable)
	{
		string baseDir = GetDirPath(saveName);
		string filePath = GetFilePath(saveName, serialisable.SerialisableGetFilename());

		if (!Directory.Exists(baseDir) ||
			!File.Exists(filePath))
		{
			return false;
		}

		using (PersistenceReader deserialiser = new(filePath)) {
			serialisable.SerialisableRead(deserialiser);
		}

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

	/// <summary>
	/// Copies one save data to another
	/// </summary>
	/// <param name="srcSaveName">The save data to copy</param>
	/// <param name="dstSaveName">The save data to paste to</param>
	/// <param name="filter">An optional function for filtering out data.</param>
	/// <returns></returns>
	public static bool CopyData(string srcSaveName, string dstSaveName, Func<string, bool> filter = null)
	{
		string srcBaseDir = GetDirPath(srcSaveName);
		string dstBaseDir = GetDirPath(dstSaveName);

		if (!Directory.Exists(dstBaseDir)) {
			Directory.CreateDirectory(dstBaseDir);
		}

		if (!Directory.Exists(srcBaseDir) ||
			!Directory.Exists(dstBaseDir))
		{
			return false;
		}

		ReadOnlySpan<string> fileList = Directory.GetFiles(srcBaseDir);

		for (int i = 0; i < fileList.Length; ++ i) {
			if (!(filter?.Invoke(fileList[i]) ?? true))
				continue;

			string fileName = Path.GetFileName(fileList[i]);

			File.Copy(
				sourceFileName: Path.Join(srcBaseDir, fileName),
				destFileName: Path.Join(dstBaseDir, fileName)
			);
		}

		return true;
	}

	/// <summary>
	/// Copies stored data from the transient save directory to an actual save slot
	/// </summary>
	/// <param name="saveName">The save data to paste to</param>
	/// <param name="filter">An optional function for filtering out data.</param>
	/// <returns></returns>
	public static bool CopyDataFromTemporary(string saveName, Func<string, bool> filter = null)
	{
		return CopyData(SaveNameTemporary, saveName, filter);
	}

	/// <summary>
	/// Copies a specified save data to the transient save directory
	/// </summary>
	/// <param name="saveName">The save data to copy</param>
	/// <param name="filter">An optional function for filtering out data.</param>
	/// <returns></returns>
	public static bool CopyDataToTemporary(string saveName, Func<string, bool> filter = null)
	{
		return CopyData(saveName, SaveNameTemporary, filter);
	}

	private static void MakeSaveDirIfNeeded(string saveName)
	{
		string dirPath = GetDirPath(saveName);

		if (Directory.Exists(dirPath))
			return;

		Directory.CreateDirectory(dirPath);
	}

	#endregion

	#region Path Utils

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string GetDirPath() {
		return Path.Combine(OS.GetUserDataDir(), "SaveData");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string GetDirPath(string saveName) {
		return Path.Combine(OS.GetUserDataDir(), "SaveData", saveName);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string GetFilePath(string saveName, string fileName) {
		return Path.Combine(OS.GetUserDataDir(), "SaveData", saveName, fileName);
	}

	#endregion
}