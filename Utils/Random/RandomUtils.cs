using Godot;
using System;
using System.Diagnostics;

namespace SadChromaLib.Utils.Random;

/// <summary> A helper class that helps with random-number generation tasks. </summary>
public static class RandomUtils
{
	private readonly static PCG32 _rng = new();

	#region Seeding Methods

	/// <summary> Randomises the random number generator's seed. </summary>
	public static void Reseed()
	{
		GD.Randomize();
		_rng.Seed((Time.GetTicksMsec() % (ulong.MaxValue / 2)) + GD.Randi());
	}

	/// <summary> Randomises the random number generator's seed using a GUID. </summary>
	public static void ReseedGUID()
	{
		GD.Randomize();
		_rng.Seed((ulong) Guid.NewGuid().GetHashCode());
	}

	/// <summary> Sets the random number generator's seed to a specific value. </summary>
	public static void SetSeed(ulong seed)
	{
		ulong lseed = (ulong) seed;
		GD.Seed(lseed);
		_rng.Seed(seed);
	}

	#endregion

	#region Generation Methods

	/// <summary> Returns a random float value using Godot's 'rand_range' method. </summary>
	public static float StandardFloat(float minValue = 0.0f, float maxValue = 1.0f)
	{
		return (float) GD.RandRange(minValue, maxValue);
	}

	/// <summary> Returns a random value between 0.0 - 1.0. </summary>
	public static float BasicFloat()
	{
		return _rng.NextFloat();
	}

	/// <summary> Returns a random integer value. Control its highest output value using the modulo '%' operator. </summary>
	public static int BasicInt()
	{
		return _rng.Next();
	}

	/// <summary> Returns a random float value between the specified range. </summary>
	public static float RangeFloat(float minValue = 0.0f, float maxValue = 1.0f)
	{
		return minValue + (BasicFloat() * (maxValue - minValue));
	}

	/// <summary> Returns a random integer value between the specified range. </summary>
	public static long RangeLong(int minValue = 0, int maxValue = 100, bool addOne = false)
	{
		return minValue + (BasicInt() % (maxValue - minValue + (addOne ? 1 : 0)));
	}

	/// <summary> Returns a random float value by picking a random element from an array filled with randomised values. </summary>
	public static float ShuffleRand(float minValue = 0.0f, float maxValue = 1.0f, int size = 8, RandomMethod method = RandomMethod.WeightedAlt)
	{
		float[] values = new float[size];

		// Generate an array of random floats
		for (int i = 0; i < size; ++ i) {
			values[i] = Random(minValue, maxValue, method);
		}

		// Shuffle using Fisher-Yates
		for (int j = size; j --> 1;) {
			int i = (int) (BasicInt() % j);

			if (i == j)
				continue;

			(values[j], values[i]) = (values[i], values[j]);
		}

		// Return a random element
		return Pick(values);
	}

	/// <summary> Returns a weighted random float value. (Adapted from https://stackoverflow.com/questions/29915888/weighted-random-number-without-predefined-values.) </summary>
	public static float WeightedRand(float minValue = 0.0f, float maxValue = 1.0f)
	{
		return MathF.Floor(
			(MathF.Abs(BasicFloat() - BasicFloat()) * (1.0f + maxValue - minValue)) + minValue
		);
	}

	/// <summary> A variation of the WeightedRand method that uses a randomsied factor. </summary>
	public static float WeightedRandAlt(float minValue = 0.0f, float maxValue = 1.0f, float fac = 0.5f)
	{
		if (BasicFloat() <= fac) {
			return RangeFloat(minValue, maxValue);
		}

		return minValue + ((maxValue - minValue) * 0.5f * (BasicFloat() + BasicFloat()));
	}

	#endregion

	#region Main Functions

	/// <summary> Returns a random float value using the specified method. (The 'factor' parameter is only used by the 'Weighted' generator method.) </summary>
	public static float Random(float minValue = 0.0f, float maxValue = 1.0f, RandomMethod method = RandomMethod.Basic, float factor = 0.5f)
	{
		return method switch {
			RandomMethod.Basic => RangeFloat(minValue, maxValue),
			RandomMethod.Shuffle => ShuffleRand(minValue, maxValue),
			RandomMethod.Weighted => WeightedRand(minValue, maxValue),
			RandomMethod.WeightedAlt => WeightedRandAlt(minValue, maxValue, factor),
			_ => StandardFloat(minValue, maxValue),
		};
	}

	/// <summary> Returns a random integer value between the specified range. </summary>
	public static int Random(int minValue = 0, int maxValue = 100, bool addOne = false)
	{
		return (int) RangeLong(minValue, maxValue, addOne);
	}

	/// <summary> Generates a randomised range from the given edge points. </summary>
	public static int[] Range(int start, int end, bool shuffle = true)
	{
		int[] indices = new int[end-start];
		int idx = 0;

		for (int i = start; i < end; ++ i) {
			indices[idx] = i;
			idx ++;
		}

		if (shuffle) {
			Shuffle(indices);
		}

		return indices;
	}

	/// <summary> Generates a randomised range from the given edge points. </summary>
	public static void Range(ref Span<int> indices, int start, int end, bool shuffle = true)
	{
		Debug.Assert(
			indices.Length == (end - start),
			"SadChromaLib.Range (Span): range->span size mismatch!"
		);

		int idx = 0;

		for (int i = start; i < end; ++ i) {
			indices[idx] = i;
			idx ++;
		}

		if (shuffle) {
			Shuffle(ref indices);
		}
	}

	/// <summary> Returns true if the generator returns a number smaller than the value of the 'probability' parameter. </summary>
	public static bool Chance(float probability, RandomMethod method = RandomMethod.Shuffle)
	{
		return Random(0.0f, 1.0f, method) < probability;
	}

	/// <summary> Generates a 2D gaussian distribution based on the given parameters. </summary>
	public static Vector2 Gaussian(float mean = 0.0f, float stddev = 1.0f, RandomMethod method = RandomMethod.Basic)
	{
		float theta = Mathf.Tau * Random(0.0f, 1.0f, method);
		float rho = Mathf.Sqrt(-2.0f * Mathf.Log(1.0f - Random(0.0f, 1.0f, method)));
		float scale = stddev * rho;

		return new Vector2(
			mean + (scale * Mathf.Cos(theta)),
			mean + (scale * Mathf.Sin(theta))
		);
	}

	/// <summary> Returns a random point in a circle. </summary>
	public static Vector2 Circle(float radius = 1.0f, float mean = 0.0f, float stddev = 1.0f, RandomMethod method = RandomMethod.Basic)
	{
		return Gaussian(mean, stddev, method).Normalized() * radius;
	}

	/// <summary> Returns a random direction in 2D space. </summary>
	public static Vector2 RandomDir()
	{
		Vector2[] dirs = {
			Vector2.Right,
			Vector2.Up,
			(0.5f * Vector2.Right) + (0.5f * Vector2.Up),
			(0.5f * Vector2.Right) + (0.5f * Vector2.Down)
		};

		return Pick(dirs) * Random(-1.0f, 1.0f);
	}

	/// <summary> Picks a random element from an array. </summary>
	public static T Pick<T>(T[] array)
	{
		return array[BasicInt() % array.Length];
	}

	/// <summary> Picks a random element from a span. </summary>
	public static T Pick<T>(Span<T> span)
	{
		return span[(int) BasicInt() % span.Length];
	}

	/// <summary> Picks a random element from a Godot array. </summary>
	public static Variant Pick(Godot.Collections.Array array)
	{
		return array[(int) (BasicInt() % array.Count)];
	}

	/// <summary> Picks a random element from a Godot array. </summary>
	public static T Pick<[MustBeVariant] T>(Godot.Collections.Array<T> array)
	{
		return array[(int) (BasicInt() % array.Count)];
	}

	/// <summary> Picks a random element from a list. </summary>
	public static T Pick<T>(System.Collections.Generic.List<T> list)
	{
		return list[(int) (BasicInt() % list.Count)];
	}

	/// <summary> Shuffles a selected array using the Fisher-Yates algorithm. </summary>
	public static void Shuffle<T>(T[] array)
	{
		for (int j = array.Length; j --> 1;) {
			int i = (int) (BasicInt() % j);

			if (i == j)
				continue;

			(array[j], array[i]) = (array[i], array[j]);
		}
	}

	public static void Shuffle<T>(ref Span<T> span)
	{
		for (int j = span.Length; j --> 1;) {
			int i = (int) (BasicInt() % j);

			if (i == j)
				continue;

			(span[j], span[i]) = (span[i], span[j]);
		}
	}

	#endregion
}

/// <summary> The method to use to generate a random number. </summary>
public enum RandomMethod
{
	Standard,
	Basic,
	Shuffle,
	Weighted,
	WeightedAlt
}
