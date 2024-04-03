global using DataDict = System.Collections.Generic.Dictionary<string, SadChromaLib.Types.AnyData>;

using Godot;

using System;
using System.IO;
using System.Runtime.CompilerServices;

using SadChromaLib.Utils.Convenience;

namespace SadChromaLib.Persistence;

/// <summary>
/// A shared controller object for coordinating persistence writes
/// </summary>
[GlobalClass]
public sealed partial class PersistenceController: Resource
{
	public const string SaveNameAuto = "Autosave";
	public const string SaveNameTemporary = "Transient";

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
	/// <para>* Use the static 'ReadFromDisk' method to perform the read operation.</para>
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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SaveToAuto() {
		Save(SaveNameAuto);
	}

	/// <summary>
	/// A shorthand for a load request through the 'SaveNameAuto' file
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void LoadFromAuto() {
		Load(SaveNameAuto);
	}

	/// <summary>
	/// Starts a read request using the last loaded save data
	/// </summary>
	/// <param name="serialisable"></param>
	public void LoadFromCurrent(ISerialisable serialisable)
	{
		if (_lastSaveName == null)
			return;

		ReadFromDisk(_lastSaveName, serialisable);
	}

	#endregion

	#region Main I/O

	/// <summary>
	/// Returns a list of the player's saved data.
	/// </summary>
	/// <returns></returns>
	public static string[] GetSaveNames()
	{
		string baseDirPath = FilePathUtils.GetSaveDirPath();

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
	/// Writes a serialisable file to the specified save data
	/// </summary>
	/// <returns></returns>
	public static bool WriteToDisk(string saveName, ISerialisable serialisable)
	{
		string baseDir = FilePathUtils.GetSaveDirPath(saveName);
		MakeSaveDirIfNeeded(saveName);

		string filePath = FilePathUtils.GetSaveFilePath(saveName, serialisable.SerialisableGetFilename());

		using (PersistenceWriter serialiser = new(filePath)) {
			serialisable.SerialisableWrite(serialiser);
		}

		return true;
	}

	/// <summary>
	/// Reads a serialisable file from the specified save data
	/// </summary>
	/// <returns></returns>
	public static bool ReadFromDisk(string saveName, ISerialisable serialisable)
	{
		string baseDir = FilePathUtils.GetSaveDirPath(saveName);
		string filePath = FilePathUtils.GetSaveFilePath(saveName, serialisable.SerialisableGetFilename());

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

	/// <summary>
	/// Copies one save data to another
	/// </summary>
	/// <param name="srcSaveName">The save data to copy</param>
	/// <param name="dstSaveName">The save data to paste to</param>
	/// <param name="filter">An optional function for filtering out data.</param>
	/// <returns></returns>
	public static bool CopyData(string srcSaveName, string dstSaveName, Func<string, bool> filter = null)
	{
		string srcBaseDir = FilePathUtils.GetSaveDirPath(srcSaveName);
		string dstBaseDir = FilePathUtils.GetSaveDirPath(dstSaveName);

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
	/// Deletes a save data with a given name.
	/// </summary>
	/// <param name="saveName">The name of the save data to delete.</param>
	public static void DeleteSaveData(string saveName)
	{
		string dirPath = FilePathUtils.GetSaveDirPath(saveName);

		if (!Directory.Exists(dirPath))
			return;

		Directory.Delete(dirPath, true);
	}

	#endregion

	#region Transient I/O

	/// <summary>
	/// Returns true if a transient save data exists.
	/// </summary>
	/// <returns></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool HasTransientData() {
		return Directory.Exists(FilePathUtils.GetSaveDirPath(SaveNameTemporary));
	}

	/// <summary>
	/// <para>Writes temporary standalone serialisers into the transient save data.</para>
	/// <para>* It will only call objects bound to the 'SaveTransientRequested' signal.</para>
	/// <para>* Use the static 'WriteToTransient' method to perform the write operation.</para>
	/// </summary>
	public void SaveTransient() {
		EmitSignal(SignalName.SaveTransientRequested);
	}

	/// <summary>
	/// <para>Reads temporary standalone serialisers from the transient save data.</para>
	/// <para>* It will only call objects bound to the 'LoadTransientRequested' signal.</para>
	/// <para>* Use the static 'ReadFromTransient' method to perform the read operation.</para>
	/// </summary>
	public void LoadTransient() {
		EmitSignal(SignalName.RestoreTransientRequested);
	}

	/// <summary>
	/// Writes the contents of a serialisable file to the transient save data dir
	/// </summary>
	public static void WriteToTransient(ISerialisable serialisable) {
		WriteToDisk(SaveNameTemporary, serialisable);
	}

	/// <summary>
	/// Reads the contents of a serialisable file from the transient save data dir
	/// </summary>
	public static void ReadFromTransient(ISerialisable serialisable) {
		ReadFromDisk(SaveNameTemporary, serialisable);
	}

	/// <summary>
	/// Copies stored data from the transient save directory to an actual save slot
	/// </summary>
	/// <param name="saveName">The save data to paste to</param>
	/// <param name="filter">An optional function for filtering out data.</param>
	/// <returns></returns>
	public static bool CopyFromTransient(string saveName, Func<string, bool> filter = null)
	{
		return CopyData(SaveNameTemporary, saveName, filter);
	}

	/// <summary>
	/// Copies a specified save data to the transient save directory
	/// </summary>
	/// <param name="saveName">The save data to copy</param>
	/// <param name="filter">An optional function for filtering out data.</param>
	/// <returns></returns>
	public static bool CopyToTransient(string saveName, Func<string, bool> filter = null)
	{
		return CopyData(saveName, SaveNameTemporary, filter);
	}

	/// <summary>
	/// Deletes the transient saved data.
	/// </summary>
	public static void DeleteTransientData() {
		DeleteSaveData(SaveNameTemporary);
	}

	#endregion

	#region Utils

	private static void MakeSaveDirIfNeeded(string saveName)
	{
		string dirPath = FilePathUtils.GetSaveDirPath(saveName);

		if (Directory.Exists(dirPath))
			return;

		Directory.CreateDirectory(dirPath);
	}

	#endregion
}