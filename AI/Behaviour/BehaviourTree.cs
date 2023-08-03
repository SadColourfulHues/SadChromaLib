using Godot;

namespace SadChromaLib.AI.Behaviour;

public sealed partial class ProcessTree : RefCounted
{
	private BehaviourNode _root;
	private BehaviourContext _context;

	public ProcessTree(BehaviourNode root)
	{
		_root = root;
		_context = new();
	}

	public void SetRoot(BehaviourNode root)
	{
		_root = root;
	}

	public void Process()
	{
		_root.Process(_context);
	}

	public BehaviourContext GetContext()
	{
		return _context;
	}
}