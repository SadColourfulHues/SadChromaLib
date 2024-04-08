using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace SadChromaLib.Utils.Convenience;

/// <summary>
/// A wrapper struct for 'rented' arrays (Using ArrayPool)
/// </summary>
public readonly struct RentedArray<T>: IDisposable
{
    readonly T[] Items;

    public RentedArray(int size) {
        Items = ArrayPool<T>.Shared.Rent(size);
    }

    public void Dispose() {
        ArrayPool<T>.Shared.Return(Items);
    }

    public readonly T this[int key]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Items[key];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => Items[key] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator T[](RentedArray<T> arr) => arr.Items;
}