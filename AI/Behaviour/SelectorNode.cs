namespace SadChromaLib.AI.Behaviour;

public partial class SelectorNode: CompositeNode
{
	public SelectorNode(params BehaviourNode[] children)
		: base(children)
	{
	}

	public override Result Process(BehaviourContext context)
	{
		for (int i = 0; i < _children.Count; ++ i) {
			Result result = _children[i].Process(context);

			if (result == Result.Running || result == Result.Success)
				return result;
		}

		return Result.Failure;
	}
}