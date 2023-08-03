namespace SadChromaLib.AI.Behaviour;

public abstract partial class DecoratorNode : BehaviourNode
{
	protected BehaviourNode _target;

	protected DecoratorNode(BehaviourNode target)
	{
		_target  = target;
	}
}
