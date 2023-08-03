namespace SadChromaLib.AI.Behaviour;

public partial class InverterNode : DecoratorNode
{
	public InverterNode(BehaviourNode node)
		: base(node)
	{
	}

	public override Result Process(BehaviourContext context)
	{
		return Invert(_target.Process(context));
	}

	public static Result Invert(Result result)
	{
		if (result == Result.Running)
			return result;

		return result == Result.Success ?
			Result.Failure : Result.Success;
	}
}
