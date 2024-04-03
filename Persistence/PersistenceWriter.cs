using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.IO;

using Godot;

using SadChromaLib.Types;

namespace SadChromaLib.Persistence;

/// <summary>
/// A helper class for serialising binary data
/// </summary>
public sealed class PersistenceWriter: IDisposable
{
    readonly BinaryWriter _writer;

    #region IDisposable

    public void Dispose() {
        _writer.Close();
    }

    #endregion

    public PersistenceWriter(string filePath)
    {
        if (File.Exists(filePath)) {
            File.Delete(filePath);
        }

        FileStream file = File.Open(filePath, FileMode.Create);
        _writer = new(file);
    }

    public PersistenceWriter(Stream stream) {
        _writer = new(stream);
    }

    #region Specialised Writers

    public void Write(AnyData data)
    {
        // AnyData format
        // Identifier '@'
        // Type (Int8)
        // Data (Variable, depending on type)

        _writer.Write('@');
        _writer.Write((byte) data.DataType);

        switch (data.DataType)
        {
            case AnyData.Type.Bool:
                _writer.Write(data.BoolValue);
                break;

            case AnyData.Type.Int:
                _writer.Write(data.IntValue);
                break;

            case AnyData.Type.Float:
                _writer.Write(data.X);
                break;

            case AnyData.Type.Vector2:
                _writer.Write(data.X);
                _writer.Write(data.Y);
                break;

            case AnyData.Type.Vector3:
                _writer.Write(data.X);
                _writer.Write(data.Y);
                _writer.Write(data.Z);
                break;

            case AnyData.Type.Colour:
                _writer.Write(data.X);
                _writer.Write(data.Y);
                _writer.Write(data.Z);
                _writer.Write(data.A);
                break;

            case AnyData.Type.String:
            case AnyData.Type.Array:
            case AnyData.Type.Dictionary:
                _writer.Write(data.Text);
                break;
        }
    }

    public void Write(ReadOnlySpan<AnyData> data)
    {
        // AnyData array format
        // Identifier 'A'
        // Count (int32)
        // AnyData (x Count)
        _writer.Write('A');
        _writer.Write(true);

        _writer.Write(data.Length);

        for (int i = 0; i < data.Length; ++i) {
            Write(data[i]);
        }
    }

    public void Write(AnyData[] data)
    {
        // AnyData array format
        // Identifier 'A'
        // Count (int32)
        // AnyData (x Count)
        _writer.Write('A');

        if (data is null) {
            _writer.Write(false);
            return;
        }

        _writer.Write(true);

        _writer.Write(data.Length);

        for (int i = 0; i < data.Length; ++i) {
            Write(data[i]);
        }
    }

    public void Write(bool[] data) => WriteArray(data, (value) => _writer.Write(value));
    public void Write(int[] data) => WriteArray(data, (value) => _writer.Write(value));
    public void Write(float[] data) => WriteArray(data, (value) => _writer.Write(value));
    public void Write(Vector2[] data) => WriteArray(data, (value) => Write(value));
    public void Write(Vector3[] data) => WriteArray(data, (value) => Write(value));
    public void Write(Color[] data) => WriteArray(data, (value) => Write(value));
    public void Write(string[] data) => WriteArray(data, (value) => _writer.Write(value));

    public void Write(HashSet<string> data)
    {
        // Hash set format
        // Identifier 'H'
        // Count (int32)
        // Data (x Count)
        _writer.Write('H');

        if (data is null) {
            _writer.Write(false);
            return;
        }

        _writer.Write(true);
        _writer.Write(data.Count);

        foreach (string item in data) {
            _writer.Write(item);
        }
    }

    public void Write(DataDict data)
    {
        // Serialised data format
        // Identifier '%'
        // Has data (boolean)
        // Count (Int32)
        // -- items -- (x Count)
        // - key (string)
        // - data (AnyData)
        _writer.Write('%');

        if (data is null) {
            _writer.Write(false);
            return;
        }

        _writer.Write(true);
        _writer.Write(data.Count);

        foreach ((string key, AnyData value) in data) {
            _writer.Write(key);
            Write(value);
        }
    }

    public void Write(ISerialisableComponent data)
        => data.Serialise(this);

    public void Write<T>(T[] data) where T: ISerialisableComponent
        => WriteArray(data, (item) => item.Serialise(this));

    #endregion

    #region Godot Primitive Writers

    public void Write(Vector2 value) {
        _writer.Write(value.X);
        _writer.Write(value.Y);
    }

    public void Write(Vector3 value) {
        _writer.Write(value.X);
        _writer.Write(value.Y);
        _writer.Write(value.Z);
    }

    public void Write(Color colour) {
        _writer.Write(colour.R);
        _writer.Write(colour.G);
        _writer.Write(colour.B);
        _writer.Write((byte) Mathf.Floor(colour.A * 255f));
    }

    #endregion

    #region Primitive Writers

    public void Write(byte value) {
        _writer.Write(value);
    }

    public void Write(int value) {
        _writer.Write(value);
    }

    public void Write(bool value) {
        _writer.Write(value);
    }

    public void Write(float value) {
        _writer.Write(value);
    }

    public void Write(string value) {
        _writer.Write(value);
    }

    public void Write(char value) {
        _writer.Write(value);
    }

    #endregion

    #region Utils

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteArray<T>(T[] data, Action<T> writeMethod)
    {
        // Serialised array format
        // Identifier 'A'
        // HasData (boolean)
        // Count (int32)
        // AnyData (x Count)
        _writer.Write('A');

        if (data is null) {
            _writer.Write(false);
            return;
        }

        _writer.Write(true);
        _writer.Write(data.Length);

        for (int i = 0; i < data.Length; ++i) {
            writeMethod(data[i]);
        }
    }

    #endregion
}