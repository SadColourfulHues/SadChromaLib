using Godot;

using SadChromaLib.Utils.Convenience;

namespace SadChromaLib.Utils.Maths;

/// <summary> A collection of useful methods specialising in Node3D relations. </summary>
public static class SpatialUtils3D
{
    const float BEST_DIR_PENALTY_COST = 10.0f;
    const float BEST_DIR_ADJACENT_PENALTY_COST = BEST_DIR_PENALTY_COST / 2.0f;

    static readonly Vector3[] DIRECTIONS = {
        Vector3.Left,
        (Vector3.Left + Vector3.Forward).Normalized(),
        Vector3.Forward,
        (Vector3.Right + Vector3.Forward).Normalized(),
        Vector3.Right,
        (Vector3.Right + Vector3.Back).Normalized(),
        Vector3.Back,
        (Vector3.Left + Vector3.Back).Normalized(),
    };

    /// <summary> Returns a look vector where a is pointing towards b. </summary>
    public static Vector3 LookVec(Vector3 a, Vector3 b)
    {
        return (b - a).Normalized();
    }

    /// <summary> Returns a look vector where a is pointing towards b. </summary>
    public static Vector3 LookVec(Node3D a, Node3D b)
    {
        return LookVec(a.GlobalPosition, b.GlobalPosition);
    }

    /// <summary> Calculates the 'look dot' between the given unit dir and look vec. </summary>
    public static float LookDot(Vector3 axis, Vector3 look)
    {
        return axis.Dot(look);
    }

    /// <summary> Calcuates the 'look dot' between the given unit dir and object. </summary>
    public static float LookDot(Node3D a, Node3D b, LookDotAxis axis)
    {
        Vector3 lookAxis = Vector3.Zero;

        switch (axis)
        {
            case LookDotAxis.Forward:
                lookAxis = a.Transform.Basis.Z;
                break;

            case LookDotAxis.Right:
                lookAxis = a.Transform.Basis.X;
                break;

            case LookDotAxis.Up:
                lookAxis = a.Transform.Basis.Y;
                break;
        }

        Vector3 look = LookVec(a, b);
        return LookDot(lookAxis, look);
    }

    /// <summary> Returns the 'side' b is located in. </summary>
    /// Adapted from https://forum.unity.com/threads/left-right-test-function.31420/
    public static Side SideTest(
        Node3D a,
        Node3D b,
        Vector3 up,
        LookDotAxis frontDotAxis = LookDotAxis.Forward,
        float frontDotThresh = 0.75f)
    {
        Vector3 lookVec = LookVec(a, b);
        Vector3 look = a.Transform.Basis.Z.Cross(lookVec);
        float sideDot = look.Dot(up);

        if (sideDot > 0.0f)
        {
            return Side.Right;
        }
        else if (sideDot < 0.0f)
        {
            return Side.Left;
        }

        float frontDot = LookDot(a, b, frontDotAxis);

        if (frontDot >= frontDotThresh) {
            return Side.Front;
        }
        else if (frontDot <= -frontDotThresh) {
            return Side.Back;
        }

        return Side.Unknown;
    }

    /// <summary> Returns true if b is in front of a. </summary>
    public static bool IsInFront(Node3D a, Node3D b, float checkMargin = 0.1f)
    {
        float lookDot = LookDot(a, b, LookDotAxis.Forward);
        return lookDot > (1.0f - checkMargin);
    }

    /// <summary> Returns true if b is behind a </summary>
    public static bool IsBehind(Node3D a, Node3D b, float checkMargin = 0.1f)
    {
        float lookDot = LookDot(a, b, LookDotAxis.Forward);
        return lookDot < (-1.0f + checkMargin);
    }

    /// <summary> Returns true if b is at the left side of a </summary>
    public static bool IsAtLeft(Node3D a, Node3D b)
    {
        return SideTest(a, b, Vector3.Up) == Side.Left;
    }

    /// <summary> Returns true if b is at the left side of a </summary>
    public static bool IsAtRight(Node3D a, Node3D b)
    {
        return SideTest(a, b, Vector3.Up) == Side.Right;
    }

    /// <summary> Returns an estimated 'best' direction that points towards the target position. </summary>
    public static Vector3 BestDir(
        Node3D body,
        Vector3 position,
        float testLength = 1.0f,
        uint? testMask = null,
        Godot.Collections.Array<Rid> testExclude = null,
        Vector3? positionOverride = null)
    {
        Vector3 bodyPos = positionOverride ?? body.GlobalPosition;
        Vector3 look = (position - bodyPos).Normalized();

        PhysicsDirectSpaceState3D state = body.GetWorld3D().DirectSpaceState;
        float[] dirCosts = new float[8];

        // Initial pass: calculate similarity to the target look vec
        for (int i = 0; i < 8; ++ i)
        {
            dirCosts[i] = look.Dot(DIRECTIONS[i]);
        }

        // Second pass: apply spatial context
        for (int i = 0; i < 8; ++ i)
        {
            RaycastResult3D result = RaycastUtils3D.Cast(
                state,
                bodyPos,
                bodyPos + (DIRECTIONS[i] * testLength),
                testMask,
                testExclude
            );

            // If that particular direction is blocked, apply a penalty cost
            if (result.hit)
            {
                dirCosts[i] -= BEST_DIR_PENALTY_COST;

                // Propagate penalty costs to adjacent directions
                dirCosts[(i+7) % 8] -= BEST_DIR_ADJACENT_PENALTY_COST;
                dirCosts[(i+1) % 8] -= BEST_DIR_ADJACENT_PENALTY_COST;
            }
        }

        // Find the smallest dir cost
        float bestCost = float.MinValue;
        int bestIndex = -1;

        for (int i = 0; i < 8; ++ i) {
            if (dirCosts[i] > bestCost) {
                bestIndex = i;
                bestCost = dirCosts[i];
            }
        }

        // If somehow no direction was found, simply return the default look vec
        if (bestIndex == -1) {
            return look;
        }

        return DIRECTIONS[bestIndex];
    }

    /// <summary> The axis to perform the look dot against. </summary>
    public enum LookDotAxis
    {
        Right,
        Up,
        Forward
    }

    /// <summary> A non-specific position relative to an object. </summary>
    public enum Side
    {
        Left,
        Right,
        Front,
        Back,
        Unknown
    }
}