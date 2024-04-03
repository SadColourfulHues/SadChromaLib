using System;
using System.Collections.Generic;

namespace SadChromaLib.Utils.Random;

/// <summary> An interface that allows a class to be useable in a WeightedBag object. </summary>
public interface IWeightedObject<T>
{
	float GetWeight();
	void UpdateWeight(float newWeight);
	T GetValue();
}

/// <summary> An object that stores weighted elements that can be picked from with varying probabilities. </summary>
public sealed partial class WeightedBag<T>
{
	private IWeightedObject<T>[] _objects;
	private float _totalWeight;

	#region Constructors

	/// <summary> Creates a weighted bag using a given collection. </summary>
	public WeightedBag(IWeightedObject<T>[] collection)
	{
		_totalWeight = 0.0f;
		_objects = collection;

		for (int i = 0; i < _objects.Length; ++ i) {
			_totalWeight += collection[i].GetWeight();
			collection[i].UpdateWeight(_totalWeight);
		}
	}

	/// <summary> Creates a weighted bag using a given collection. </summary>
	public WeightedBag(ReadOnlySpan<IWeightedObject<T>> collection)
		: this(collection.ToArray()) {}

	/// <summary> Creates a weighted bag using a given collection. </summary>
	public WeightedBag(List<IWeightedObject<T>> collection)
		: this(collection.ToArray()) {}

	/// <summary> Creates a weighted bag using a given collection. </summary>
	public WeightedBag(Array collection)
	{
		int count = collection.Length;
		IWeightedObject<T>[] array = new IWeightedObject<T>[count];

		for (int i = 0; i < count; ++ i) {
			array[i] = (IWeightedObject<T>) collection.GetValue(i);
		}
	}

	#endregion

	#region Main Functions

	/// <summary> Picks a random element from the bag. The chance of an element getting picked is dependent on their weight. </summary>
	public T Pick(RandomMethod method = RandomMethod.Standard)
	{
		float roll = RandomUtils.Random(0.0f, _totalWeight, method);

		for (int i = 0; i < _objects.Length; ++ i) {
			if (_objects[i].GetWeight() < roll)
				continue;

			return _objects[i].GetValue();
		}

		return default;
	}

	#endregion
}
