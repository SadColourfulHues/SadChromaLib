using Godot;

namespace SadChromaLib.AI.Behaviour;

public sealed partial class DelayNode : DecoratorNode
{
	private float _delay;
	private ulong _lastProcessTime;

	public DelayNode(BehaviourNode target, float delay)
		: base(target)
	{
		_delay = delay;
	}

	public override Result Process(BehaviourContext context)
	{
		ulong currentTime = Time.GetTicksMsec();
		float timeSinceLastProcess = TimeSince(currentTime, _lastProcessTime);

		if (timeSinceLastProcess < _delay)
			return Result.Failure;

		_lastProcessTime = currentTime;
		return _target.Process(context);
	}

	private static float TimeSince(ulong now, ulong time)
	{
		return (now - time) * 0.01f;
	}
}
