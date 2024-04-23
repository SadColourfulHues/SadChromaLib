using Godot;

using System;
using System.Diagnostics;

using SadChromaLib.Utils.Convenience;

namespace SadChromaLib.Types;

/// <summary>
/// A list-like collection that stores value-type data in an internal array.
/// </summary>
/// <value></value>
public sealed class VArray<T>
    where T: struct
{
    readonly public int MaxCapacity;
    readonly T?[] _data;

    static private NullSorter Sorter = new();

    int _lastCount;
    bool _changed;

    public VArray(int capacity)
    {
        MaxCapacity = capacity;

        _data = new T?[capacity];
        _changed = false;
    }

    #region Main Functions

    /// <summary>
    /// Returns the total number of 'active' items in the array.
    /// </summary>
    /// <value></value>
    public int Count
    {
        get
        {
            if (!_changed)
                return _lastCount;

            ReadOnlySpan<T?> data = _data.AsSpan();
            int count = 0;

            for (int i = 0; i < MaxCapacity; ++i) {
                if (data[i] is null)
                    continue;

                count ++;
            }

            _lastCount = count;
            _changed = false;

            return count;
        }
    }

    /// <summary>
    /// Returns the first valid item in the array
    /// </summary>
    /// <returns></returns>
    public bool TryGetFirst(out T item)
    {
        ReadOnlySpan<T?> data = _data.AsSpan();

        for (int i = 0; i < MaxCapacity; ++i) {
            if (data[i] is null)
                continue;

            item = data[i].Value;
            return true;
        }

        item = default;
        return false;
    }

    /// <summary>
    /// Returns the first valid item from the end of the array
    /// </summary>
    /// <returns></returns>
    public bool TryGetLast(out T item)
    {
        ReadOnlySpan<T?> data = _data.AsSpan();

        for (int i = MaxCapacity; i --> 0;) {
            if (data[i] is null)
                continue;

            item = data[i].Value;
            return true;
        }

        item = default;
        return false;
    }

    public bool Add(T item)
    {
        int index = FindOpenIndex();

        if (index == -1)
        {
            #if TOOLS
            GD.PrintErr("VArray::Add: no space for new item!");
            #endif

            return false;
        }

        _data[index] = item;
        _changed = true;

        return true;
    }

    /// <summary>
    /// (Caution: no checking)
    /// Returns an item on the internal array without using visual index.
    /// </summary>
    /// <param name="index">The actual index of the item to be returned.</param>
    /// <returns></returns>
    public T? GetInternal(int index) {
        return _data[index];
    }

    public T? this[int index]
        => GetInternal(index);

    /// <summary>
    /// Returns an item based on its 'visual' index.
    /// </summary>
    /// <param name="index">The visual index of the item to be returned.</param>
    public T? Get(int index)
    {
        int trueIndex = GetVisualIndex(index);
        return trueIndex == -1 ? null : _data[trueIndex];
    }

    /// <summary>
    /// (Caution: no checking)
    /// Removes an item from the internal array without using visual index
    /// </summary>
    /// <param name="index">The actual index of the item to be removed.</param>
    public void RemoveInternal(int index)
    {
        _data[index] = null;
    }

    /// <summary>
    /// Removes an item based on its 'visual' index
    /// </summary>
    /// <param name="index">The visual index of the item to be removed.</param>
    /// <returns></returns>
    public bool Remove(int index)
    {
        int trueIndex = GetVisualIndex(index);

        if (trueIndex == -1)
            return false;

        _data[trueIndex] = null;
        _changed = true;

        return true;
    }

    /// <summary>
    /// Removes all items from this array.
    /// </summary>
    public void Clear()
    {
        for (int i = 0; i < MaxCapacity; ++i) {
            _data[i] = null;
        }

        _changed = true;
    }

    /// <summary>
    /// Returns true if the specified item is in this array.
    /// </summary>
    /// <param name="item">The item to check for.</param>
    /// <returns></returns>
    public bool Exists(T item)
    {
        ReadOnlySpan<T?> data = _data.AsSpan();

        for (int i = 0; i < MaxCapacity; ++i) {
            if (data[i]?.Equals(item) == false)
                continue;

            return true;
        }

        return false;
    }

    /// <summary>
    /// (Caution: no checking)
    /// Swaps two items in the internal array using their actual indices.
    /// </summary>
    /// <param name="a">Index of item A</param>
    /// <param name="b">Index of item B</param>
    public void SwapInternal(int a, int b)
    {
        (_data[a], _data[b]) = (_data[b], _data[a]);
        _changed = true;
    }

    /// <summary>
    /// Swaps two items based on their visual index.
    /// </summary>
    /// <param name="a">Visual index of item A.</param>
    /// <param name="b">Visual index of item B.</param>
    public void Swap(int a, int b)
    {
        Span<int> indices = stackalloc int[2] { a, b };
        GetVisualIndex(indices);

        // Invalid ind(ex)|(ices)
        if (indices[0] == -1 || indices[1] == -1)
            return;

        SwapInternal(indices[0], indices[1]);
    }

    #endregion

    #region Utils

    /// <summary>
    /// Moves non-null items to the front of the internal array.
    /// </summary>
    public void SortNulls()
    {
        Array.Sort(_data, Sorter);
    }

    /// <summary>
    /// (Caution: overwrite, no checking)
    /// <para>
    /// Directly copies the data of this array to another.
    /// This operation will throw if the target array is smaller than the source,
    /// or if the start index is too high and would go out-of-bounds.
    /// </para>
    /// For safer transfers, use the 'Transfer' method, instead.
    /// </summary>
    /// <param name="destination">The array to transfer data to.</param>
    /// <param name="start">At which index should it start overwriting data.</param>
    public void Copy(VArray<T> destination, int start = 0)
    {
        Debug.Assert(
            condition: start >= 0,
            message: "VArray::Copy: 'start' must be a positive integer!"
        );

        if (destination.MaxCapacity < (start + MaxCapacity)) {
            #if TOOLS
            GD.PrintErr("VArray: target array cannot hold data.");
            #endif

            return;
        }

        for (int i = 0; i < MaxCapacity; ++i) {
            destination._data[start + i] = _data[i];
        }

        destination._changed = true;
    }

    /// <summary>
    /// Transfers as much data to another array without overwriting its data.
    /// </summary>
    /// <param name="destination">The target array to transfer data to.</param>
    public void Transfer(VArray<T> destination, int start = 0)
    {
        Debug.Assert(
            condition: start >= 0,
            message: "VArray::Transfer: 'start' must be a positive integer!"
        );

        int trueIndex;
        int index = 0;

        int upperLimit = Math.Min(destination.MaxCapacity, start + destination.MaxCapacity);

        for (int i = 0; i < destination.MaxCapacity; ++i) {
            trueIndex = start + i;

            if (trueIndex >= upperLimit)
                break;

            if (destination._data[trueIndex] is not null)
                continue;

            destination._data[trueIndex] = _data[index];
            index ++;
        }

        destination._changed = true;
    }

    /// <summary>
    /// Puts all the active items in a C# Array
    /// </summary>
    /// <returns></returns>
    public T[] ToArray() {
        T[] array = new T[Count];

        ReadOnlySpan<T?> data = _data.AsSpan();
        int arrayIndex = 0;

        for (int i = 0; i < MaxCapacity; ++i) {
            if (data[i] is null)
                continue;

            array[arrayIndex] = data[i].Value;
            arrayIndex ++;
        }

        return array;
    }

    /// <summary>
    /// Writes all active items to a pre-allocated C# array.
    /// </summary>
    /// <returns></returns>
    public int ToArray(ref T[] array) {
        Debug.Assert(
            condition: array.Length >= MaxCapacity,
            message: "VArray:ToArrayNonAlloc: target array is not capable of storing the list's data."
        );

        ReadOnlySpan<T?> data = _data.AsSpan();
        int arrayIndex = 0;

        for (int i = 0; i < MaxCapacity; ++i) {
            if (data[i] is null)
                continue;

            array[arrayIndex] = data[i].Value;
            arrayIndex ++;
        }

        return arrayIndex;
    }

    /// <summary>
    /// Returns the internal array as a span.
    /// </summary>
    /// <returns></returns>
    public ReadOnlySpan<T?> AsSpan() {
        return _data.AsSpan();
    }

    /// <summary>
    /// Returns a rented array filled with its valid entries.
    /// Don't forget to call RentedArray.Dispose() once you're done using it!
    /// </summary>
    /// <returns></returns>
    public RentedArray<T> ToEntriesArray() {
        int count = Count;

        RentedArray<T> entries = new(count);
        int entryIdx = 0;

        for (int i = 0; i < MaxCapacity; ++i) {
            if (entryIdx >= count)
                break;

            if (_data[i] is null)
                continue;

            entries[entryIdx] = _data[i].Value;
            entryIdx ++;
        }

        return entries;
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Finds an open slot in the array.
    /// </summary>
    /// <returns></returns>
    int FindOpenIndex()
    {
        ReadOnlySpan<T?> data = _data.AsSpan();

        for (int i = 0; i < MaxCapacity; ++i) {
            if (data[i] is not null)
                continue;

            return i;
        }

        return -1;
    }

    /// <summary>
    /// Writes to a span turning visual indices into their actual forms.
    /// </summary>
    /// <param name="indices">A span of indices to be transformed.</param>
    void GetVisualIndex(Span<int> indices)
    {
        Span<(bool done, int index)> input =
            stackalloc (bool, int)[indices.Length];

        for (int i = 0; i < input.Length; ++i) {
            input[i].done = false;
        }

        ReadOnlySpan<T?> data = _data.AsSpan();
        int visualIndex = 0;

        for (int i = 0; i < MaxCapacity; ++i) {
            if (data[i] is null)
                continue;

            for (int j = 0; j < input.Length; ++j) {
                if (input[j].done ||
                    visualIndex != input[j].index)
                {
                    continue;
                }

                indices[j] = i;
                input[j].done = true;
            }

            visualIndex ++;
        }

        // Flag unfound indices
        for (int i = 0; i < input.Length; ++i) {
            if (input[i].done)
                continue;

            indices[i] = -1;
        }
    }

    /// <summary>
    /// Visual to actual index.
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    int GetVisualIndex(int index)
    {
        ReadOnlySpan<T?> data = _data.AsSpan();
        int visualIndex = 0;

        for (int i = 0; i < MaxCapacity; ++i) {
            if (data[i] is null)
                continue;

            if (index == visualIndex)
                return i;

            visualIndex ++;
        }

        return -1;
    }

    #endregion

    private class NullSorter : System.Collections.IComparer
    {
        public int Compare(object x, object y)
        {
            if (x is null && y is null)
                return 0;

            return y is null ? -1 : 1;
        }
    }
}