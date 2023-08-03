using Godot;

namespace SadChromaLib.Utils.Timing;

/// <summary> An object that represents an activity that will be started in the future. </summary>
public sealed partial class ScheduleAction : RefCounted, ITimedActivity
{
	[Signal]
	public delegate void CompletionEventHandler();

	private float _seconds;

	/// <summary> Creates a new timed action object. (Note: do not use this constructor, instead use the TimingController's AddTask method to schedule activities.) </summary>
	public ScheduleAction(float seconds)
	{
		_seconds = seconds;
	}

	/// <summary> Advances the countdown until it elapses. (Note: this method should only be called by a TimingController object.) </summary>
	public bool Evaluate(float delta)
	{
		_seconds -= delta;

		if (_seconds > 0)
			return false;

		// Start associated activity
		EmitSignal(SignalName.Completion);
		return true;
	}
}