using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace SadChromaLib.Utils.Convenience;

/// <summary>
/// A wrapper struct for 'rented' arrays (Using ArrayPool)
/// </summary>
public readonly struct RentedArray<T>: IDisposable
{
    readonly T[] _items;

    public RentedArray(int size) {
        _items = ArrayPool<T>.Shared.Rent(size);
    }

    public void Dispose() {
        ArrayPool<T>.Shared.Return(_items);
    }

    #region Array OPs

    /// <summary>
    /// [no bounds checking] Returns an array segment of the rented array using a specified range.
    /// </summary>
    /// <returns></returns>
    public readonly ArraySegment<T> Slice(int start, int end) {
        return new(_items, start, end);
    }

    public readonly T this[int key]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _items[key];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _items[key] = value;
    }

    public readonly ArraySegment<T> this[Range range]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Slice(range.Start.Value, range.End.Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator T[](RentedArray<T> arr) => arr._items;

    #endregion
}