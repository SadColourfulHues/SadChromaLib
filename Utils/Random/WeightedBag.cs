using Godot;
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
public sealed partial class WeightedBag<T> : RefCounted
{
	private IWeightedObject<T>[] _objects;
	private float _totalWeight;
	private bool _isNormalised;

	#region Constructors

	public WeightedBag(List<IWeightedObject<T>> collection, bool normalise = false)
	{
		CreateBag(collection.ToArray(), normalise);
	}

	public WeightedBag(IWeightedObject<T>[] collection, bool useAsIs = true, bool normalise = false)
	{
		if (useAsIs) {
			CreateBag(collection, normalise);
			return;
		}

		CreateBag((IWeightedObject<T>[]) collection.Clone(), normalise);
	}

	public WeightedBag(Array collection, bool normalise = false)
	{
		int count = collection.Length;
		IWeightedObject<T>[] array = new IWeightedObject<T>[count];

		for (int i = 0; i < count; ++ i) {
			array[i] = (IWeightedObject<T>) collection.GetValue(i);
		}

		CreateBag(array, normalise);
	}

	/// <summary> Creates a weighted bag using a given collection. </summary>
	void CreateBag(IWeightedObject<T>[] collection, bool normalise)
	{
		_totalWeight = 0.0f;
		_isNormalised = normalise;

		// Apply normalisation (if enabled)
		if (normalise) {
			Normalise(collection);
		}

		_objects = collection;

		for (int i = 0; i < _objects.Length; ++ i) {
			_totalWeight += collection[i].GetWeight();
			collection[i].UpdateWeight(_totalWeight);
		}
	}

	#endregion

	#region Main Functions

	/// <summary> Picks a random element from the bag. The chance of an element getting picked is dependent on their weight. </summary>
	public T Pick(RandomMethod method = RandomMethod.Standard)
	{
		float roll = _isNormalised
			? RandomUtils.Random(0.0f, 1.0f, method)
			: RandomUtils.Random(0.0f, _totalWeight, method);

		for (int i = 0; i < _objects.Length; ++ i) {
			if (_objects[i].GetWeight() < roll)
				continue;

			return _objects[i].GetValue();
		}

		return default;
	}

	#endregion

	#region Helpers

	/// <summary> Normalises the weights of a collection. </summary>
	public static void Normalise(IWeightedObject<T>[] collection)
	{
		int count = collection.Length;
		float collectionTotalWeight = 0.0f;

		// Get the total weight
		for (int i = 0; i < count; ++ i) {
			collectionTotalWeight += collection[i].GetWeight();
		}

		// Normalise data
		for (int j = 0; j < count; ++ j) {
			float weight = collection[j].GetWeight();
			collection[j].UpdateWeight(weight / collectionTotalWeight);
		}
	}

	public void DebugPrintWeights()
	{
		for (int i = 0; i < _objects.Length; ++ i) {
			GD.Print(_objects[i].GetWeight());
		}
	}

	#endregion
}
