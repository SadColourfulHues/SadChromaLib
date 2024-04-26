using Godot;

using System;
using System.Runtime.CompilerServices;

namespace SadChromaLib.Utils.Convenience;

/// <summary>
/// A collection of commonly-used functionality for tweening
/// </summary>
public static class TweenUtils
{
    /// <summary>
    /// Prepares a reusable tweener for use
    /// </summary>
    /// <param name="owner">The owner of the tween</param>
    /// <param name="tweenRef">Pointer to the reusable tween variable.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Create(Node owner, ref Tween tweenRef)
    {
        if (GodotObject.IsInstanceValid(tweenRef) && tweenRef.IsRunning()) {
            tweenRef.Kill();
        }

        tweenRef = owner.CreateTween();
    }

    /// <summary>
    /// Binds a nullable callback to a tween
    /// </summary>
    public static void BindCallback(Tween tween, Action callback)
    {
        if (callback is null)
            return;

        tween.TweenCallback(Callable.From(callback));
    }
}