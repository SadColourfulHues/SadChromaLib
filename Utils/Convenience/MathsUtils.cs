using Godot;
using System;

namespace SadChromaLib.Utils.Convenience;

/// <summary> A collection of commonly-used maths-related methods. </summary>
public static class MathsUtils
{
    #region Clamp

    /// <summary> Clamps an integer value between a specified boundary. </summary>
    public static int Clamp(int value, int minValue, int maxValue)
    {
        return Math.Min(maxValue, Math.Max(value, minValue));
    }

    /// <summary> Clamps a vector to a specified length. </summary>
    public static Vector3 Clamp(Vector3 vector, float maxLength)
    {
        float sqrLength = vector.LengthSquared();

        // No need to clamp
        if (sqrLength < (maxLength * maxLength))
            return vector;

        // Normalise then re-adjust the vector
        float length = Mathf.Sqrt(sqrLength);

        return new Vector3(
            vector.X / length * maxLength,
            vector.Y / length * maxLength,
            vector.Z / length * maxLength
        );
    }

    /// <summary> Clamps a vector to a specified length </summary>
    public static Vector2 Clamp(Vector2 vector, float maxLength)
    {
        float sqrLength = vector.LengthSquared();

        if (sqrLength < (maxLength * maxLength))
            return vector;

        float length = Mathf.Sqrt(sqrLength);

        return new Vector2(
            vector.X / length * maxLength,
            vector.Y / length * maxLength
        );
    }

    #endregion

    #region Integer Utils

    /// <summary> Restricts 'value' to a specified range. (If stopAtBounds is set, it will clamp 'value' to the extremes. Otherwise, it will wrap to the other extreme, instead.) </summary>
    public static int Restrict(int value, int min, int max, bool stopAtBounds=false)
    {
        if (value > max) {
            return stopAtBounds ? max : min;
        }
        else if (value < min) {
            return stopAtBounds ? min : max;
        }

        return value;
    }

    /// <summary> Restricts 'value' to a specified range. (If stopAtBounds is set, it will clamp 'value' to the extremes. Otherwise, it will wrap to the other extreme, instead.) </summary>
    public static uint Restrict(uint value, uint min, uint max, bool stopAtBounds=false)
    {
        if (value > max) {
            return stopAtBounds ? max : min;
        }
        else if (value < min) {
            return stopAtBounds ? min : max;
        }

        return value;
    }

    #endregion

    #region Min/Max

    /// <summary> Returns the smallest absolute value between a and b. </summary>
    public static float AbsMin(float a, float b)
    {
        return Mathf.Min(Mathf.Abs(a), Mathf.Abs(b));
    }

    /// <summary> Returns the smallest absolute value between a and b. </summary>
    public static int AbsMin(int a, int b)
    {
        return Math.Min(Math.Abs(a), Math.Abs(b));
    }

    /// <summary> Returns the largest absolute value between a and b. </summary>
    public static float AbsMax(float a, float b)
    {
        return Mathf.Max(Mathf.Abs(a), Mathf.Abs(b));
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
            return AbsMax(a, b) == Mathf.Abs(a);
        }

        return Mathf.Max(a, b) == a;
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
            return AbsMin(a, b) == Mathf.Abs(a);
        }

        return Mathf.Min(a, b) == a;
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
            return Mathf.Min(x, limit);
        }

        return Mathf.Max(x, -limit);
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