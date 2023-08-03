using Godot;

using SadChromaLib.Utils.Random;

namespace SadChromaLib.AI.Steering;

public static class SteeringBehaviours
{
	/// <summary>
	/// A simple steering algorithm
	/// </summary>
	/// <param name="current">The body's current velocity</param>
	/// <param name="target">The body's target velocity</param>
	/// <param name="acceleration">The body's acceleration rate.</param>
	/// <param name="maxVelocity">The body's maximum allowed velocity.</param>
	/// <returns></returns>
	public static Vector2 Steer(
		Vector2 current,
		Vector2 target,
		float acceleration,
		float maxVelocity)
	{
		Vector2 steer = (target - current).Normalized() * acceleration;
		return (target + steer).LimitLength(maxVelocity);
	}

	/// <summary>
	/// A basic steering algorithm that makes the body move towards (or seek towards) the target position.
	/// </summary>
	/// <param name="velocity">The body's current velocity.</param>
	/// <param name="position">The body's current position.</param>
	/// <param name="desiredPosition">The target position.</param>
	/// <param name="acceleration">The body's acceleration rate.</param>
	/// <param name="maxVelocity">The body's maximum allowed velocity.</param>
	/// <returns></returns>
	public static Vector2 Seek(
		Vector2 velocity,
		Vector2 position,
		Vector2 desiredPosition,
		float acceleration,
		float maxVelocity)
	{
		return Steer(
			current: velocity,
			target: (desiredPosition - position).Normalized() * maxVelocity,
			acceleration: acceleration,
			maxVelocity: maxVelocity
		);
	}

	/// <summary>
	/// A basic steering algorithm that makes the body move away (or flee) from the target position.
	/// </summary>
	/// <param name="velocity">The body's current velocity.</param>
	/// <param name="position">The body's current position.</param>
	/// <param name="desiredPosition">The target position.</param>
	/// <param name="acceleration">The body's acceleration rate.</param>
	/// <param name="maxVelocity">The body's maximum allowed velocity.</param>
	/// <returns></returns>
	public static Vector2 Flee(
		Vector2 velocity,
		Vector2 position,
		Vector2 desiredPosition,
		float acceleration,
		float maxVelocity)
	{
		return Steer(
			current: velocity,
			target: (position - desiredPosition).Normalized(),
			acceleration: acceleration,
			maxVelocity: maxVelocity
		);
	}

	/// <summary>
	/// A more advanced version of seek that tries to predict a moving target's future position.
	/// </summary>
	/// <param name="velocity">The body's current velocity.</param>
	/// <param name="position">The body's current position.</param>
	/// <param name="targetPosition">The target body's position.</param>
	/// <param name="targetVelocity">The target body's velocity.</param>
	/// <param name="acceleration">The body's acceleration rate.</param>
	/// <param name="maxVelocity">The body's maximum allowed velocity.</param>
	/// <param name="t">How far ahead into the future should the algorithm try to predict?</param>
	/// <returns></returns>
	public static Vector2 Pursuit(
		Vector2 velocity,
		Vector2 position,
		Vector2 targetPosition,
		Vector2 targetVelocity,
		float acceleration,
		float maxVelocity,
		float t = 0.2f)
	{
		return Seek(
			velocity: velocity,
			position: position,
			desiredPosition: targetPosition + (targetVelocity * t),
			acceleration: acceleration,
			maxVelocity: maxVelocity
		);
	}

	/// <summary>
	/// A more advanced version of flee that tries to predict against a moving target's position
	/// </summary>
	/// <param name="velocity">The body's current velocity.</param>
	/// <param name="position">The body's current position.</param>
	/// <param name="targetPosition">The target body's position.</param>
	/// <param name="targetVelocity">The target body's velocity.</param>
	/// <param name="acceleration">The body's acceleration rate.</param>
	/// <param name="maxVelocity">The body's maximum allowed velocity.</param>
	/// <param name="t">How far ahead into the future should the algorithm try to predict?</param>
	/// <returns></returns>
	public static Vector2 Evade(
		Vector2 velocity,
		Vector2 position,
		Vector2 targetPosition,
		Vector2 targetVelocity,
		float acceleration,
		float maxVelocity)
	{
		return Flee(
			velocity: velocity,
			position: position,
			desiredPosition: targetPosition + targetVelocity,
			acceleration: acceleration,
			maxVelocity: maxVelocity
		);
	}

	/// <summary>
	/// A steering algorithm that makes the body slow down as it approaches its target position.
	/// </summary>
	/// <param name="velocity">The body's current velocity.</param>
	/// <param name="position">The body's current position.</param>
	/// <param name="desiredPosition">The target position.</param>
	/// <param name="arrivalDistance">How big the slowdown effect area should be.</param>
	/// <param name="acceleration">The body's acceleration rate.</param>
	/// <param name="maxVelocity">The body's maximum allowed velocity.</param>
	/// <returns></returns>
	public static Vector2 Arrival(
		Vector2 velocity,
		Vector2 position,
		Vector2 desiredPosition,
		float arrivalDistance,
		float acceleration,
		float maxVelocity)
	{
		Vector2 dir = desiredPosition - position;
		float distance = dir.LengthSquared();

		dir = dir.Normalized();

		if (distance < (arrivalDistance * arrivalDistance)) {
			dir *= distance/arrivalDistance * maxVelocity;
		}
		else {
			dir *= maxVelocity;
		}

		return Steer(
			current: velocity,
			target: dir,
			acceleration: acceleration,
			maxVelocity: maxVelocity
		);
	}

	/// <summary>
	/// A steering behaviour that tries to emulate an 'aimless' wander type of movement
	/// </summary>
	/// <param name="velocity">The body's current velocity.</param>
	/// <param name="position">The body's current position.</param>
	/// <param name="maxDistance">The maximum wander distance.</param>
	/// <param name="maxAngle">The maximum turn angle.</param>
	/// <param name="acceleration">The body's acceleration rate.</param>
	/// <param name="maxVelocity">The body's maximum allowed velocity.</param>
	/// <returns></returns>
	public static Vector2 Wander(
		Vector2 velocity,
		Vector2 position,
		float maxDistance,
		float maxAngle,
		float acceleration,
		float maxVelocity)
	{
		Vector2 nextVelocity = velocity.Normalized() * maxDistance;
		float angle = (RandomUtils.BasicFloat() * maxAngle) - (maxAngle * 0.5f);

		Vector2 displacement = new(0f, 1f);
		displacement *= maxDistance;
		displacement = displacement.Rotated(angle);

		return Seek(
			velocity: velocity,
			position: position,
			desiredPosition: nextVelocity + displacement,
			acceleration: acceleration,
			maxVelocity: maxVelocity
		);
	}
}