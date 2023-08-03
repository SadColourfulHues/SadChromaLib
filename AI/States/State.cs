using Godot;

namespace SadChromaLib.AI.States;

/// <summary> An object that represents a state. Sub-classes of it can implement various processing tasks. </summary>
public abstract partial class State : RefCounted
{
	public static StringName StateIdGeneric => "generic";

	/// <summary> A unique identifier associated with a state. If not implemented, it will return AI.State.GENERIC_ID. </summary>
	public virtual StringName GetIdentifier()
	{
		return StateIdGeneric;
	}

	/// <summary> Returns whether or not a state has the given identifier. </summary>
	public bool IsState(StringName stateId)
	{
		return GetIdentifier() == stateId;
	}

	#region Events

	/// <summary> This method is called when this state becomes 'active'. Place initialisation-related tasks here. </summary>
	public virtual void OnEnter(StateMachine smRef) {}
	/// <summary> This method is called whenever the state machine is ran. </summary>
	public virtual void OnUpdate(StateMachine smRef, float delta) {}

	/// <summary> (Optional) This method is called whenever the state machine has finished running. Place deconstruction-related tasks here. </summary>
	public virtual void OnExit(StateMachine smRef) {}

	#endregion
}