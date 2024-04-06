using Godot;
using System;

namespace SadChromaLib.Utils.Timing;

/// <summary> A struct that represents an activity that will be started in the future. </summary>
public struct ScheduleActionRealTime: ITimedActivity
{
	public event Action OnCompleted;

	ulong _startTime;
	float _seconds;

	/// <summary> Creates a new timed action object. (Note: do not use this constructor, instead use the TimingController's AddTaskRealtime method to schedule activities.) </summary>
	public ScheduleActionRealTime(float seconds)
		: this()
	{
		_seconds = seconds;
		_startTime = Time.GetTicksMsec();
	}

	/// <summary> Calculates the total time elapsed since it was created. (Note: this method should only be called by a TimingController object.) </summary>
	public bool Evaluate(float delta)
	{
		float currentSecs = (Time.GetTicksMsec() - _startTime) * 0.001f;

		if (currentSecs < _seconds)
			return false;

		OnCompleted?.Invoke();
		return true;
	}
}