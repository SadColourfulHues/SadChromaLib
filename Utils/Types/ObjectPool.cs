using Godot;

using System;

namespace SadChromaLib.Utils.Types;

public interface IObjectPoolHandler<T> where T: GodotObject, new()
{
	void ObjectCreated(T @object);
	bool ObjectIsAvailable(T @object);
}

/// <summary>
/// A type of SimplePool for Godot objects
/// </summary>
/// <typeparam name="T"></typeparam>
public partial class ObjectPool<T> : SimplePool<T> where T : GodotObject, new()
{
	public ObjectPool(uint maxCapacity, IObjectPoolHandler<T> objectHandler)
		: base(maxCapacity, new DefaultObjectHandler(objectHandler))
	{
	}

	/// <summary>
	/// Frees all active objects in the pool
	/// </summary>
	public void Purge()
	{
		ReadOnlySpan<T> values = _values;

		for (int i = 0; i < _maxCapacity; ++ i) {
			if (!IsInstanceValid(values[i]))
				continue;

			values[i].CallDeferred(GodotObject.MethodName.Free);
			_values[i] = null;
		}
	}

	private class DefaultObjectHandler: IPoolHandler<T>
	{
		private readonly IObjectPoolHandler<T> _handler;

		public DefaultObjectHandler(IObjectPoolHandler<T> handler)
		{
			_handler = handler;
		}

		public T PoolCreate()
		{
			T @object = new();
			_handler.ObjectCreated(@object);

			return @object;
		}

		public bool PoolIsAvailable(T @object)
		{
			return IsInstanceValid(@object) && _handler.ObjectIsAvailable(@object);
		}

		public void PoolDestroy(T _)
		{
		}
	}
}