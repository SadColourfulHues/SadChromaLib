namespace SadChromaLib.Utils.Timing;

/// <summary> A TimeSinceObserver is a utility object that can be used to measure the time since an action was started. Useful for things like hold confirmation.</summary>
public struct TimeSinceObserver
{
    private ulong _startTime;
    private float _targetElapsedTime;

    public TimeSinceObserver(float targetElapsedTime = 1.0f)
    {
        _startTime = 0;
        _targetElapsedTime = targetElapsedTime;
    }

    /// <summary> Updates the observer's elapsed time target </summary>
    public void SetTargetElapsedTime(float targetElapsedTime)
    {
        _targetElapsedTime = targetElapsedTime;
    }

    /// <summary> Start measuring time from this point onwards. </summary>
    public void Reset()
    {
        _startTime = Time.GetTicksMsec();
    }

    /// <summary> How much time has passed since the last 'Reset' method was called? (Returns elapsed time in seconds.) </summary>
    public readonly float TimeSince()
    {
        return (Time.GetTicksMsec() - _startTime) * 0.001f;
    }

    /// <summary> Has the elapsed time passed the target value? </summary>
    public readonly bool HasElapsed()
    {
        return TimeSince() >= _targetElapsedTime;
    }
}