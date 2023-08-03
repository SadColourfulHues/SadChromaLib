using Godot;
using System;

using SadChromaLib.Utils.Tests;
using SadChromaLib.Utils.Timing;

namespace SadChromaLib.Tests.Instances;

public sealed partial class SCHTimingTest : Control
{
    private TimingController _scheduler;
    private double _ticks = 1.0;

    private Label _taskCountLabel;

	public override void _Ready()
	{
        _taskCountLabel = GetNode<Label>("%TaskCounter");

        TestUtils.Run("Create timing controller", () =>
            _scheduler = new TimingController(5));

        TestUtils.Run("Add tasks", () => {
            for (int i = 0; i < 4; ++ i) {
                _scheduler.AddTask(2.0f * i, SayHi);
            }

            _scheduler.AddTaskRealtime(10.0f, Quit);

            // The scheduler should no longer be able to hold more tasks
            if (!_scheduler.AddTask(2.0f, SayHi)) {
                GD.Print("Test: add excess task failed (success!)");
            }
            else {
                GD.Print("Test: add excess task succeeded (failed!)");
            }
        });

        UpdateTaskCount();
	}

	public override void _Process(double delta)
	{
		_scheduler.Evaluate(delta);
	}

	private void UpdateTaskCount()
    {
        _taskCountLabel.Text = $"Tasks remaining: {_scheduler.GetNumActiveTasks() - 1}";
    }

    private void SayHi()
    {
        GD.Print("Hi!");
        UpdateTaskCount();
    }

    private void Quit()
    {
        UpdateTaskCount();
        GetTree().Quit();
    }
}
