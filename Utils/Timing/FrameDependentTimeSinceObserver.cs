using System;

namespace SadChromaLib.Utils.Timing;

/// <summary>
/// <para>
/// A utility object that can be used to check how much frame time has elapsed
/// since an action was started. Useful for things like cooldowns.
/// </para>
///
/// <para>
/// [Required]
/// Call its 'Tick' method in a process method to update its frame time.
/// </para>
/// </summary>
public struct FrameDependentTimeSinceObserver
{
    float _frameTime;
    float _targetElapsedTime;

    public FrameDependentTimeSinceObserver(float targetElapsedTime = 0.0f)
    {
        _frameTime = targetElapsedTime;
        _targetElapsedTime = targetElapsedTime;
    }

    #region Main Functions

    /// <summary> Resets the timer. </summary>
    public void Reset() {
        _frameTime = 0.0f;
    }

    /// <summary> Resets the timer and updates the target elapsed time. </summary>
    public void Reset(float targetElapsedTime)
    {
        _frameTime = 0.0f;
        _targetElapsedTime = targetElapsedTime;
    }

    /// <summary> Increments the time with a given frame delta time </summary>
    public void Tick(float delta) {
        _frameTime += delta;
    }

    /// <summary> Returns the elapsed frame time since the last reset. </summary>
    public readonly float TimeSince() {
        return _frameTime;
    }

    /// <summary> Has enough frame time has passed since the last reset? </summary>
    public readonly bool HasElapsed()
        => _frameTime >= _targetElapsedTime;

    #endregion
}