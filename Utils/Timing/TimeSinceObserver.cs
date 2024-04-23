namespace SadChromaLib.Utils.Timing;

/// <summary> A utility object that can be used for measuring the time since an action was started. Useful for things like debouncers and hold confirmation timers.</summary>
public struct TimeSinceObserver
{
    private long _startTicks;
    private double _targetElapsedTime;

    public TimeSinceObserver(double targetElapsedTime = 1.0)
    {
        _startTicks = 0;
        _targetElapsedTime = targetElapsedTime;
    }

    #region Main Functions

    /// <summary> Updates the observer's elapsed time target </summary>
    public void SetTargetElapsedTime(double targetElapsedTime) {
        _targetElapsedTime = targetElapsedTime;
    }

    /// <summary> Start measuring time from this point onwards. </summary>
    public void Reset() {
        _startTicks = TimingUtils.GetTicks();
    }

    /// <summary> Start measuring time from this point onwards, while also updating the target elapsed time. </summary>
    public void Reset(double targetElapsedTime)
    {
        _startTicks = TimingUtils.GetTicks();
        _targetElapsedTime = targetElapsedTime;
    }

    /// <summary> How much time has passed since the last 'Reset' method was called? (Returns elapsed time in seconds.) </summary>
    public readonly double TimeSinceSecs()
        => TimingUtils.SecsSince(_startTicks * TimingUtils.SecsFac);

    /// <summary> How much time has passed since the last 'Reset' method was called? (Returns elapsed time in milliseconds.) </summary>
    public readonly double TimeSinceMsecs()
        => TimingUtils.MsecsSince(_startTicks * TimingUtils.MsecsFac);

    /// <summary> How much time has passed since the last 'Reset' method was called? (Returns elapsed time in nanoseconds.) </summary>
    public readonly long TimeSinceNsecs()
        => TimingUtils.NsecsSince(_startTicks * 100);

    #endregion

    #region Elapsed Checks

    /// <summary> Has the elapsed time passed the target value? (in seconds) </summary>
    public readonly bool HasElapsed() {
        return TimingUtils.SecsSince(_startTicks * TimingUtils.SecsFac) >= _targetElapsedTime;
    }

    /// <summary> Has the elapsed time passed the target value? (in milliseconds) </summary>
    public readonly bool HasElapsedMsecs() {
        return TimingUtils.MsecsSince(_startTicks * TimingUtils.MsecsFac) >= _targetElapsedTime;
    }

    /// <summary> Has the elapsed time passed the target value? (in milliseconds) </summary>
    public readonly bool HasElapsedNsecs() {
        return TimingUtils.NsecsSince(_startTicks * 100) >= _targetElapsedTime;
    }

    #endregion
}