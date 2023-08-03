using System;
using Godot;

namespace SadChromaLib.Utils.Timing;

/// <summary> An object that represents a scheduled task in a TimingController. </summary>
public interface ITimedActivity
{
	bool Evaluate(float delta);
}

/// <summary> An object that handles scheduling-related tasks. </summary>
public sealed partial class TimingController: RefCounted
{
	private const int ErrorNoSlots = -1;

	private int _maxScheduledActions;
	private readonly ITimedActivity[] _actions;

	public TimingController(int maxScheduledActions = 8)
	{
		_maxScheduledActions = maxScheduledActions;
		_actions = new ITimedActivity[maxScheduledActions];

		// Initialise array
		ClearTasks();
	}

	/// <summary> Returns the first open slot it finds in the array. </summary>
	int FindAvailableSlot()
	{
		ReadOnlySpan<ITimedActivity> actions = _actions;

		for (int i = 0; i < _maxScheduledActions; ++ i) {
			if (actions[i] != null)
				continue;

			return i;
		}

		return ErrorNoSlots;
	}

	#region Main Functions

	/// <summary> Returns the total number of active tasks in the scheduler. </summary>
	public int GetNumActiveTasks()
	{
		ReadOnlySpan<ITimedActivity> actions = _actions;
		int count = 0;

		for (int i = 0; i < _maxScheduledActions; ++ i) {
			if (actions[i] == null)
				continue;

			count ++;
		}

		return count;
	}

	/// <summary> Runs the controller's task evaluation process. </summary>
	public void Evaluate(double delta)
	{
		Evaluate((float) delta);
	}

	/// <summary> Runs the controller's task evaluation process. </summary>
	public void Evaluate(float delta)
	{
		ReadOnlySpan<ITimedActivity> actions = _actions;

		// Evaluate tasks in the controller's backlog
		for (int i = 0; i < _maxScheduledActions; ++ i) {
			if (actions[i]?.Evaluate(delta) != true)
				continue;

			_actions[i] = null;
		}
	}

	/// <summary> Schedules a new delta-bound task
	public bool AddTask(float seconds, ScheduleAction.CompletionEventHandler callback)
	{
		int slot = FindAvailableSlot();

		// Check if the task can be added to its backlog
		if (slot == ErrorNoSlots)
			return false;

		ScheduleAction action = new(seconds);
		action.Completion += callback;

		_actions[slot] = action;
		return true;
	}

	/// <summary> Schedules a new task bound to a real-time scheduled task
	public bool AddTaskRealtime(float seconds, ScheduleActionRealTime.CompletionEventHandler callback)
	{
		int slot = FindAvailableSlot();

		if (slot == ErrorNoSlots)
			return false;

		ScheduleActionRealTime action = new(seconds);
		action.Completion += callback;

		_actions[slot] = action;
		return true;
	}

	/// <summary> Removes all currently-active scheduled actions from the backlog. </summary>
	public void ClearTasks()
	{
		for (int i = 0; i < _maxScheduledActions; ++ i) {
			_actions[i] = null;
		}
	}

	#endregion
}