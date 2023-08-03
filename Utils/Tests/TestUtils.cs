using Godot;
using System;

namespace SadChromaLib.Utils.Tests;

/// <summary> A convenience class that implements some commonly-used testing utilities. </summary>
public static class TestUtils
{
	/// <summary> Tests how long a task took to execute in milliseconds. </summary>
	public static void Run(string label, Action action)
	{
		ulong start = Time.GetTicksMsec();
		action.Invoke();

		GD.Print($"{label} took {Time.GetTicksMsec() - start:0000} ms to complete.");
	}
}