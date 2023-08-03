using Godot;

using SadChromaLib.Utils.Maths;

using GDict = Godot.Collections.Dictionary;

namespace SadChromaLib.Utils.Convenience;

/// <summary> The output of a Raycast test. </summary>
public struct RaycastResult3D
{
    public bool hit;
    public Node3D body;
    public Vector3 v_position;
    public Vector3 v_normal;

    public RaycastResult3D()
    {
        hit = false;
        body = null;
        v_position = Vector3.Zero;
        v_normal = Vector3.Zero;
    }
}

/// <summary> A convenience class that simplifies calling the Godot 4's raycasting API. </summary>
public static class RaycastUtils3D
{
    const string K_BODY = "collider";
    const string K_POSITION = "position";
    const string K_NORMAL = "normal";

    /// <summary> Perform a basic ray test from one point to another. </summary>
    public static RaycastResult3D Cast(
        PhysicsDirectSpaceState3D space,
        Vector3 v_from,
        Vector3 v_to,
        uint? mask = null,
        Godot.Collections.Array<Godot.Rid> exclude = null,
        bool includeAreas = false,
        bool hitsFromInside = true,
        bool hitBackFaces = false)
    {
        RaycastResult3D result = new();

		// Initialise params
		PhysicsRayQueryParameters3D rayParams = new() {
			From = v_from,
			To = v_to,
			Exclude = exclude,
			CollideWithAreas = includeAreas,
			CollideWithBodies = true,
			HitBackFaces = hitBackFaces,
			HitFromInside = hitsFromInside
		};

		if (mask != null) {
            rayParams.CollisionMask = mask.Value;
        }

        // Perform query
        GDict rayOut = space.IntersectRay(rayParams);

        // Hit test failure
        if (rayOut.Count == 0)
            return result;

        // Hit test success
        result.body = (Node3D) rayOut[K_BODY];
        result.v_position = (Vector3) rayOut[K_POSITION];
        result.v_normal = (Vector3) rayOut[K_NORMAL];
        result.hit = true;

        return result;
    }

    /// <summary> An alternate way to call 'RaycastUtils.Cast'. To use this variant, specify the caster and desired casting direction. </summary>
    public static RaycastResult3D CastTo(
        PhysicsDirectSpaceState3D state,
        Transform3D caster,
        Vector3 v_dir,
        uint? mask = null,
        Godot.Collections.Array<Godot.Rid> exclude = null,
        bool includeAreas = false,
        bool hitsFromInside = true,
        bool hitBackFaces = false)
    {
        Vector3 v_start = caster.Origin;
        Vector3 v_end = TransformUtils3D.Xform(caster, v_dir);

        return Cast(state, v_start, v_end, mask, exclude, includeAreas, hitsFromInside, hitBackFaces);
    }
}