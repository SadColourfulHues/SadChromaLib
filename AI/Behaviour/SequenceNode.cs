namespace SadChromaLib.AI.Behaviour;

public partial class SequenceNode: CompositeNode
{
	public SequenceNode(params BehaviourNode[] children)
		: base(children)
	{
	}

	public override Result Process(BehaviourContext context)
	{
		for (int i = 0; i < _children.Count; ++ i) {
			Result result = _children[i].Process(context);

			if (result == Result.Running || result == Result.Failure)
				return result;
		}

		return Result.Success;
	}
}