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
        _frameTime = 0.0f;
        _targetElapsedTime = targetElapsedTime;
    }

    #region Main Functions

    public void Reset() {
        _frameTime = 0.0f;
    }

    public void Reset(float targetElapsedTime)
    {
        _frameTime = 0.0f;
        _targetElapsedTime = targetElapsedTime;
    }

    public void Tick(float delta) {
        _frameTime += delta;
    }

    public bool HasElapsed() {
        return _frameTime >= _targetElapsedTime;
    }

    #endregion
}