using System;
using System.Runtime.CompilerServices;

namespace SadChromaLib.Utils.Timing;

/// <summary>
/// A convenience class implementing functions related to time-watching
/// </summary>
public static class TimingUtils
{
    public const double MsecsFac = 1.0 / (double) TimeSpan.TicksPerMillisecond;
    public const double SecsFac = 1.0 / (double) TimeSpan.TicksPerSecond;

    #region Getters

    /// <summary>
    /// Returns the current number of ticks (at the current time) in nanoseconds.
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long GetTicksNsec()
        => 100 * DateTime.Now.Ticks;

    /// <summary>
    /// Returns the current number of ticks (at the current time).
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long GetTicks()
        => DateTime.Now.Ticks;

    /// <summary>
    /// Returns the current number of ticks (at the current time) in milliseconds.
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double GetTicksMsec()
        => DateTime.Now.Ticks * MsecsFac;

    /// <summary>
    /// Returns the current number of ticks (at the current time) in seconds.
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double GetTicksSecs()
        => DateTime.Now.Ticks * SecsFac;

    #endregion

    #region Time Span Utils

    /// <summary>
    /// Returns the number of ticks since the specified starting point.
    /// </summary>
    /// <param name="start">Starting time in ticks.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long TicksSince(long start)
        => DateTime.Now.Ticks - start;

    /// <summary>
    /// Returns the number of nanoseconds since the specified starting point.
    /// </summary>
    /// <param name="start">Starting time in nanoseconds.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long NsecsSince(long start)
        => 100 * (DateTime.Now.Ticks - start);

    /// <summary>
    /// Returns the number of milliseconds since the specified starting point.
    /// </summary>
    /// <param name="start">Starting time in milliseconds.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double MsecsSince(double start)
        => (DateTime.Now.Ticks * MsecsFac) - start;

    /// <summary>
    /// Returns the number of seconds since the specified starting point.
    /// </summary>
    /// <param name="start">Starting time in seconds.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double SecsSince(double start)
        => (DateTime.Now.Ticks * SecsFac) - start;

    #endregion
}