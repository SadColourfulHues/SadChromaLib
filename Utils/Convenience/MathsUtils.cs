using System;
using System.Runtime.CompilerServices;

namespace SadChromaLib.Utils.Convenience;

/// <summary> A collection of commonly-used maths-related methods. </summary>
public static class MathsUtils
{
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