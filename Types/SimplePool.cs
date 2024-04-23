using Godot;
using System;
using System.Diagnostics;

namespace SadChromaLib.Types;

public interface IPoolHandler<T> where T: class
{
	/// <summary>
	/// This method is called whenever the pool needs to create a new object.
	/// </summary>
	/// <returns></returns>
	T PoolCreate();

	/// <summary>
	/// This method is called to check whether or not an object is available.
	/// </summary>
	/// <param name="object"></param>
	bool PoolIsAvailable(T @object);
}

/// <summary>
/// An object that implements basic pooling support for common types.
/// </summary>
/// <typeparam name="T"></typeparam>
public partial class SimplePool<T>
	where T: class
{
	protected readonly IPoolHandler<T> _handler;

	protected readonly T[] _values;
	protected uint _maxCapacity;

	public SimplePool(uint maxCapacity, IPoolHandler<T> eventHandler)
	{
		_handler = eventHandler;

		Debug.Assert(
			condition: _handler != null,
			message: "Pool Handler is invalid!"
		);

		_values = new T[maxCapacity];
		_maxCapacity = maxCapacity;

		for (uint i = 0; i < maxCapacity; ++ i) {
			_values[i] = null;
		}
	}

	/// <summary>
	/// Obtains an object reference from the pool.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	/// This is raised when the pool can no longer create new objects
	/// </exception>
	/// <returns></returns>
	public T Get()
	{
		ReadOnlySpan<T> values = _values;
		int? firstAvailable = null;

		// Try to recycle inactive objects first
		for (int i = 0; i < _maxCapacity; ++ i) {
			if (!_handler.PoolIsAvailable(values[i])) {
				// Mark the first 'available' slot in the array for later.
				if (values[i] is null) {
					firstAvailable ??= i;
				}

				continue;
			}

			return values[i];
		}

		// If no slots were marked,
		// we can assume that the pool can no longer hold any more objects
		if (firstAvailable == null) {
			#if TOOLS
			GD.PrintErr("SimplePool: pool can no longer contain any more data!");
			#endif

			return null;
		}

		T @object = _handler.PoolCreate();
		_values[firstAvailable.Value] = @object;

		return @object;
	}

	/// <summary>
	/// Pre-allocates a set amount of items
	/// </summary>
	/// <param name="count">The amount of items to reserve. (Caps at max capacity.)</param>
	public void Reserve(uint count)
	{
		uint amountReserved = 0;
		count = Math.Min(_maxCapacity, count);

		for (uint i = 0; i < _maxCapacity; ++i) {
			if (amountReserved >= count)
				break;

			if (_values[i] is not null)
				continue;

			_values[i] = _handler.PoolCreate();
			amountReserved ++;
		}
	}
}
