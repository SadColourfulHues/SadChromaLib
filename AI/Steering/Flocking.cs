using Godot;

namespace SadChromaLib.AI.Steering;

public static class FlockingBehaviours
{
	/// <summary>
	/// Calculates a body's updated velocity so that it 'aligns' with its neighbours
	/// </summary>
	/// <param name="velocity">The body's current velocity</param>
	/// <param name="neighbourVelocities">An array of its neighbour's velocities.</param>
	/// <param name="acceleration">The body's acceleration rate.</param>
	/// <param name="maxVelocity">The body's maximum allowed velocity.</param>
	public static void Alignment(
		ref Vector2 velocity,
		Vector2[] neighbourVelocities,
		float acceleration,
		float maxVelocity)
	{
		if (neighbourVelocities.Length < 1)
			return;

		Vector2 totalVelocity = Vector2.Zero;

		for (int i = 0; i < neighbourVelocities.Length; ++ i) {
			totalVelocity += neighbourVelocities[i];
		}

		velocity = SteeringBehaviours.Steer(
			current: velocity,
			target: totalVelocity.Normalized() * maxVelocity,
			acceleration: acceleration,
			maxVelocity: maxVelocity
		);
	}

	/// <summary>
	/// Calculates a body's updated velocity so that it converges towards the neighbourhood's centre
	/// </summary>
	/// <param name="velocity">The body's current velocity</param>
	/// <param name="position">The body's current position.</param>
	/// <param name="neighbourPositions">An array of its neighbour's positions.</param>
	/// <param name="acceleration">The body's acceleration rate.</param>
	/// <param name="maxVelocity">The body's maximum allowed velocity.</param>
	public static void Cohesion(
		ref Vector2 velocity,
		Vector2 position,
		Vector2[] neighbourPositions,
		float acceleration,
		float maxVelocity)
	{
		if (neighbourPositions.Length < 1)
			return;

		Vector2 averagePosition = Vector2.Zero;
		float count = 0.0f;

		for (int i = 0; i < neighbourPositions.Length; ++ i) {
			averagePosition += neighbourPositions[i];
			count ++;
		}

		averagePosition /= count;

		SteeringBehaviours.Steer(
			current: velocity,
			target: (averagePosition - position).Normalized() * maxVelocity,
			acceleration: acceleration,
			maxVelocity: maxVelocity
		);
	}

	/// <summary>
	/// Calculates a body's updated velocity so that it avoids its neighbours once it reaches a certain threshold.
	/// </summary>
	/// <param name="velocity">The body's current velocity</param>
	/// <param name="position">The body's current position.</param>
	/// <param name="neighbourPositions">An array of its neighbour's positions.</param>
	/// <param name="seperationDistance">How close should the body be before it starts repelling against its neighbours.</param>
	/// <param name="acceleration">The body's acceleration rate.</param>
	/// <param name="maxVelocity">The body's maximum allowed velocity.</param>
	public static void Separation(
		ref Vector2 velocity,
		Vector2 position,
		Vector2[] neighbourPositions,
		float separationDistance,
		float acceleration,
		float maxVelocity)
	{
		if (neighbourPositions.Length < 1)
			return;

		Vector2 separationDir = Vector2.Zero;

		for (int i = 0; i < neighbourPositions.Length; ++ i) {
			float distanceSquared = position.DistanceSquaredTo(neighbourPositions[i]);

			if (distanceSquared > separationDistance)
				continue;

			separationDir += (position - neighbourPositions[i]).Normalized() / Mathf.Sqrt(distanceSquared);
		}

		SteeringBehaviours.Steer(
			current: velocity,
			target: separationDir.Normalized() * maxVelocity,
			acceleration: acceleration,
			maxVelocity: maxVelocity
		);
	}
}