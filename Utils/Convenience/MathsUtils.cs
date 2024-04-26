using System;
using System.Runtime.CompilerServices;

using Godot;

namespace SadChromaLib.Utils.Convenience;

/// <summary> A collection of commonly-used maths-related methods. </summary>
public static class MathsUtils
{
    #region 3D Utils

    /// <summary>
    /// Returns true if origin's forward is facing target
    /// </summary>
    /// <param name="forward">The forward direction of the origin</param>
    /// <param name="origin">The origin's position</param>
    /// <param name="target">The target's position</param>
    /// <param name="threshold">How much room for error is allowed when performing the check</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFacing(Vector3 forward, Vector3 origin, Vector3 target, float threshold = 0.25f) {
        return forward.Dot((target - origin).Normalized()) >= 1.0f - threshold;
    }

    /// <summary>
    /// Returns true if the target position is on the origin's left side
    /// </summary>
    /// <param name="forward">The forward direction of the origin</param>
    /// <param name="origin">The origin's position</param>
    /// <param name="target">The target's position</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsOnLeftSide(Vector3 forward, Vector3 origin, Vector3 target) {
        return Vector3.Up.Dot(forward.Cross((target - origin).Normalized())) < 0.0f;
    }

    /// <summary>
    /// Returns true if the target position is on the origin's left side
    /// </summary>
    /// <param name="forward">The forward direction of the origin</param>
    /// <param name="up">Up vector override</param>
    /// <param name="origin">The origin's position</param>
    /// <param name="target">The target's position</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsOnLeftSide(Vector3 forward, Vector3 up, Vector3 origin, Vector3 target) {
        return up.Dot(forward.Cross((target - origin).Normalized())) < 0.0f;
    }

    #endregion

    #region Min/Max

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Clamp(float v, float min, float max) {
        return MathF.Max(min, MathF.Min(max, v));
    }

    /// <summary> Returns the smallest absolute value between a and b. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float AbsMin(float a, float b) {
        return MathF.Min(MathF.Abs(a), MathF.Abs(b));
    }

    /// <summary> Returns the smallest absolute value between a and b. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int AbsMin(int a, int b) {
        return Math.Min(Math.Abs(a), Math.Abs(b));
    }

    /// <summary> Returns the largest absolute value between a and b. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float AbsMax(float a, float b) {
        return MathF.Max(MathF.Abs(a), MathF.Abs(b));
    }

    /// <summary> Returns the largest absolute value between a and b. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int AbsMax(int a, int b) {
        return Math.Max(Math.Abs(a),  Math.Abs(b));
    }

    /// <summary> Checks if a is the largest value between a and b. (Can be configured to use AbsMax.) </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMaxA(float a, float b) {
        return MathF.Max(a, b) == a;
    }

    /// <summary> Checks if a is the largest value between a and b. (Can be configured to use AbsMax.) </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMaxA(int a, int b) {
        return Math.Max(a, b) == a;
    }

    /// <summary> [absolute value] Checks if a is the largest value between a and b. (Can be configured to use AbsMax.) </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAbsMaxA(float a, float b) {
        return MathF.Max(a, b) == a;
    }

    /// <summary> [absolute value] Checks if a is the largest value between a and b. (Can be configured to use AbsMax.) </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAbsMaxA(int a, int b) {
        return Math.Max(a, b) == a;
    }

    /// <summary> Checks if a is the smallest value between a and b.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMinA(float a, float b) {
        return MathF.Min(a, b) == a;
    }

    /// <summary> Checks if a is the smallest value between a and b. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMinA(int a, int b) {
        return Math.Min(a, b) == a;
    }

    /// <summary> [absolute value] Checks if a is the smallest value between a and b.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAbsMinA(float a, float b) {
        return AbsMin(a, b) == a;
    }

    /// <summary> [absolute value] Checks if a is the smallest value between a and b. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAbsMinA(int a, int b) {
        return AbsMin(a, b) == a;
    }

    #endregion
}