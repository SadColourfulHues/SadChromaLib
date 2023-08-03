using Godot;
using Godot.Collections;

namespace SadChromaLib.AI.Behaviour;

public sealed partial class BehaviourContext: RefCounted
{
	private Dictionary<StringName, Variant> _state;

	public BehaviourContext()
	{
		_state = new();
	}

	public bool Write(StringName key, Variant value, bool preventOverwrite = false)
	{
		if (preventOverwrite && _state.ContainsKey(key))
			return false;

		_state[key] = value;
		return true;
	}

	public T Read<[MustBeVariant] T>(StringName key, bool removeOnRead = false, T defaultValue = default)
	{
		if (!_state.ContainsKey(key))
			return defaultValue;

		T value = _state[key].As<T>();

		if (removeOnRead)
			_state.Remove(key);

		return value;
	}

	public void Remove(StringName key)
	{
		_state.Remove(key);
	}

	public bool HasKey(StringName key)
	{
		return _state.ContainsKey(key);
	}

	public void Reset()
	{
		_state.Clear();
	}
}