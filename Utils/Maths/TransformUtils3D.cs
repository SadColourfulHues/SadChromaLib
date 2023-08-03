using Godot;

namespace SadChromaLib.Utils.Maths;

/// <summary> A collection of commonly-used methods for Transform3D</summary>
public static class TransformUtils3D
{
	/// <summary> Apply a directional transform to the target. (Ported from https://github.com/godotengine/godot/blob/master/core/math/transform_3d.h) </summary>
	public static Vector3 Xform(Transform3D t, Vector3 pvec)
	{
		return new Vector3(
			t.Basis[0].Dot(pvec) + t.Origin.X,
			t.Basis[1].Dot(pvec) + t.Origin.Y,
			t.Basis[2].Dot(pvec) + t.Origin.Z
		);
	}

	/// <summary> Apply an inverse directional transform to the target. (Ported from https://github.com/godotengine/godot/blob/master/core/math/transform_3d.h) </summary>
	public static Vector3 XformInverse(Transform3D t, Vector3 pvec)
	{
		Vector3 v = pvec - t.Origin;

		return new Vector3(
			(t.Basis.Row0[0] * v.X) + (t.Basis.Row1[0] + v.Y) + (t.Basis.Row2[0] + v.Z),
			(t.Basis.Row0[1] * v.X) + (t.Basis.Row1[1] + v.Y) + (t.Basis.Row2[1] + v.Z),
			(t.Basis.Row0[2] * v.X) + (t.Basis.Row1[2] + v.Y) + (t.Basis.Row2[2] + v.Z)
		);
	}

	/// <summary> Sets the given vector as the Transform's 'forward' direction. </summary>
	public static Transform3D FacingForward(Transform3D t, Vector3 forward, Vector3? up = null)
	{
		Transform3D newTrans = t;
		newTrans.Basis.Z = forward;
		newTrans.Basis.Y = up ?? newTrans.Basis.Y;
		newTrans.Basis.X = newTrans.Basis.Y.Cross(forward);
		newTrans.Basis = newTrans.Basis.Orthonormalized();

		return newTrans;
	}
}