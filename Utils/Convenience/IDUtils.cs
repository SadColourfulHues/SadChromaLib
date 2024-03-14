using Godot;

namespace SadChromaLib.Utils.Convenience;

public static class IDUtils
{
    /// <summary>
    /// Checks if two ID strings are matching. (This assumes that both strings are interned.)
    /// </summary>
    /// <returns></returns>
    public static bool Test(string a, string b)
    {
        return object.ReferenceEquals(a, b);
    }

    /// <summary>
    /// Checks if two ID strings are matching.
    /// </summary>
    /// <returns></returns>
    public static bool Test(StringName a, StringName b)
    {
        return a == b;
    }
}