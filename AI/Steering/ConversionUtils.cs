using Godot;

namespace SadChromaLib.AI.Steering;

public static class Vector3To2Extension
{
	/// <summary>
	/// Converts a 3D vector into 2D
	/// </summary>
	/// <param name="vector">self</param>
	/// <param name="xz">If set to true, it will use the vector's XZ coordinate instead of XY</param>
	/// <returns></returns>
	public static Vector2 To2D(this Vector3 vector, bool xz = true)
	{
		return xz
			? new Vector2(vector.X, vector.Z)
			: new Vector2(vector.X, vector.Y);
	}

	/// <summary>
	/// Creates a 2D vector from this vector's XZ coordinates.
	/// </summary>
	/// <param name="vector">self</param>
	/// <returns></returns>
	public static Vector2 XZ(this Vector3 vector)
	{
		return vector.To2D(true);
	}

	/// <summary>
	/// Creates a 2D vector from this vector's XY coordinates.
	/// </summary>
	/// <param name="vector">self</param>
	/// <returns></returns>
	public static Vector2 YZ(this Vector3 vector)
	{
		return vector.To2D(false);
	}
}

public static class Vector2To3Extension
{
	/// <summary>
	/// Creates a 3D vector from this vector's coordinates.
	/// </summary>
	/// <param name="vector">self</param>
	/// <param name="z">The value of the Z coordinate.</param>
	/// <returns></returns>
	public static Vector3 To3D(this Vector2 vector, float z = 0f)
	{
		return new(vector.X, vector.Y, z);
	}
}