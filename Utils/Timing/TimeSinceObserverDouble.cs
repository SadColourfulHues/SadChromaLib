using System;
using Godot;

namespace SadChromaLib.Utils.Timing;

/// <summary> A variant of TimeSinceObserver that uses a double type internally.</summary>
public sealed partial class TimeSinceObserverDouble : RefCounted
{
    private ulong _startTime;
    private double _targetElapsedTime;

    public TimeSinceObserverDouble(double targetElapsedTime)
    {
        _targetElapsedTime = targetElapsedTime;
    }

    /// <summary> Start measuring time from this point onwards. </summary>
    public void Reset()
    {
        _startTime = Time.GetTicksMsec();
    }

    /// <summary> How much time has passed since the last 'Reset' method was called? (Returns elapsed time in seconds.) </summary>
    public double TimeSince()
    {
        return (Time.GetTicksMsec() - _startTime) * 0.001;
    }

    /// <summary> Has the elapsed time passed the target value? </summary>
    public bool HasElapsed()
    {
        return TimeSince() >= _targetElapsedTime;
    }
}