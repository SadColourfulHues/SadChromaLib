using Godot;
using System;

namespace SadChromaLib.Utils.Convenience;

/// <summary> A collection of commonly-used maths-related methods. </summary>
public static class MathsUtils
{
    #region Min/Max

    /// <summary> Returns the smallest absolute value between a and b. </summary>
    public static float AbsMin(float a, float b)
    {
        return MathF.Min(MathF.Abs(a), MathF.Abs(b));
    }

    /// <summary> Returns the smallest absolute value between a and b. </summary>
    public static int AbsMin(int a, int b)
    {
        return Math.Min(Math.Abs(a), Math.Abs(b));
    }

    /// <summary> Returns the largest absolute value between a and b. </summary>
    public static float AbsMax(float a, float b)
    {
        return MathF.Max(MathF.Abs(a), MathF.Abs(b));
    }

    /// <summary> Returns the largest absolute value between a and b. </summary>
    public static int AbsMax(int a, int b)
    {
        return Math.Max(Math.Abs(a),  Math.Abs(b));
    }

    /// <summary> Checks if a is the largest value between a and b. (Can be configured to use AbsMax.) </summary>
    public static bool IsMaxA(float a, float b, bool absolute = false)
    {
        if (absolute) {
            return AbsMax(a, b) == MathF.Abs(a);
        }

        return MathF.Max(a, b) == a;
    }

    /// <summary> Checks if a is the largest value between a and b. (Can be configured to use AbsMax.) </summary>
    public static bool IsMaxA(int a, int b, bool absolute = false)
    {
        if (absolute) {
            return AbsMax(a, b) == Math.Abs(a);
        }

        return Math.Max(a, b) == a;
    }

    /// <summary> Checks if a is the smallest value between a and b. (Can be configured to use AbsMin.) </summary>
    public static bool IsMinA(float a, float b, bool absolute = false)
    {
        if (absolute) {
            return AbsMin(a, b) == MathF.Abs(a);
        }

        return MathF.Min(a, b) == a;
    }

    /// <summary> Checks if a is the smallest value between a and b. (Can be configured to use AbsMin.) </summary>
    public static bool IsMinA(int a, int b, bool absolute = false)
    {
        if (absolute) {
            return AbsMin(a, b) == Math.Abs(a);
        }

        return Math.Min(a, b) == a;
    }

    #endregion

    #region Sign Utils

    /// <summary> Restricts the value of x to the specified limit on both sides. (Limit is expected to be a positive floating point value.) </summary>
    public static float Restrict(float x, float limit)
    {
        if (x > 0) {
            return MathF.Min(x, limit);
        }

        return MathF.Max(x, -limit);
    }

    /// <summary> Restricts the value of x to the specified limit on both sides. (Limit is expected to be a positive floating point value.) </summary>
    public static int Restrict(int x, int limit)
    {
        if (x > 0) {
            return Math.Min(x, limit);
        }

        return Math.Max(x, -limit);
    }

    #endregion
}