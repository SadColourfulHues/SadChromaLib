using Godot;

namespace SadChromaLib.AI.Behaviour;

public enum Result
{
	Running,
	Success,
	Failure
}

public abstract partial class BehaviourNode : RefCounted
{
	public abstract Result Process(BehaviourContext context);
}
