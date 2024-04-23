using System.Runtime.CompilerServices;
using System.IO;

using Godot;

namespace SadChromaLib.Utils.Convenience;

/// <summary>
/// A convenience class that implements common file name/path utilities
/// </summary>
public static class FilePathUtils
{
    #region Godot Standard Paths

    /// <summary>
    /// Expands Godot-specific URLs.
    /// <para>
    /// Do note that 'res:*' paths will not work on exported projects,
    /// but they can still be useful for editor tools.
    /// </para>
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Expand(string godotUrl) {
        return ProjectSettings.GlobalizePath(godotUrl);
    }

    #if TOOLS
    /// <summary>
    /// (Editor Tools only!) Returns the path to the project's root directory
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetProjectPath() {
        return ProjectSettings.GlobalizePath("res://");
    }
    #endif

    /// <summary>
    /// Returns the path to the user's root data directory
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetUserDir() {
        return OS.GetUserDataDir();
    }

    #endregion

    #region Persistence Paths

    /// <summary>
    /// Returns the path to the game's root save directory
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetSaveDirPath() {
		return Path.Combine(OS.GetUserDataDir(), "SaveData");
	}

    /// <summary>
    /// Returns the path to the directory of a game's saved data
    /// </summary>
    /// <param name="saveName">Name of the saved data</param>
    /// <returns></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetSaveDirPath(string saveName) {
		return Path.Combine(OS.GetUserDataDir(), "SaveData", saveName);
	}

    /// <summary>
    /// Returns the path to a specified file in a game's saved data
    /// </summary>
    /// <param name="saveName">Name of the saved data</param>
    /// <param name="fileName">Name of the persistence file</param>
    /// <returns></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetSaveFilePath(string saveName, string fileName) {
		return Path.Combine(OS.GetUserDataDir(), "SaveData", saveName, fileName);
	}

    #endregion

    #region Check Utils

    /// <summary>
	/// Returns true if a specified save dir exists
	/// </summary>
	/// <returns></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool SaveDirExists(string saveName) {
        return Directory.Exists(GetSaveDirPath(saveName));
    }

    /// <summary>
    /// Returns true if a specified persistence file exists in a save data directory
    /// </summary>
    /// <returns></returns>
    public static bool PersistenceFileExists(string saveName, string fileName) {
        return Directory.Exists(GetSaveFilePath(saveName, fileName));
    }

    #endregion
}