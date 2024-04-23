using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.IO;

using Godot;

using SadChromaLib.Types;

namespace SadChromaLib.Persistence;

/// <summary>
/// A helper class for deserialising binary data
/// </summary>
public sealed class PersistenceReader: IDisposable
{
    readonly BinaryReader _reader;

    #region IDisposable

    public void Dispose() {
        _reader.Close();
    }

    #endregion

    public PersistenceReader(string filePath)
    {
        Debug.Assert(
            condition: File.Exists(filePath),
            message: $"PersistenceDeserialiser: \"{filePath}\" does not exist!"
        );

        FileStream file = File.Open(filePath, FileMode.Open);
        _reader = new(file);
    }

    public PersistenceReader(Stream stream) {
        _reader = new(stream);
    }

    #region Specialised Readers

    public bool ReadTestIdentifier(char identifier) {
        return _reader.ReadChar() == identifier;
    }

    /// <summary> Reads the next bytes as an AnyData</summary>
    /// <returns></returns>
    public AnyData ReadAny()
    {
        if (!ReadTestIdentifier('@'))
            throw new InvalidDataException();

        AnyData.Type dataType = (AnyData.Type) _reader.ReadByte();

        switch (dataType)
        {
            case AnyData.Type.Bool:
                return _reader.ReadBoolean();

            case AnyData.Type.Int:
                return _reader.ReadInt32();

            case AnyData.Type.Float:
                return _reader.ReadSingle();

            case AnyData.Type.Vector2:
                return ReadVec2();

            case AnyData.Type.Vector3:
                return ReadVec3();

            case AnyData.Type.Colour:
                return ReadColour();

            case AnyData.Type.String:
            case AnyData.Type.Array:
            case AnyData.Type.Dictionary:
                return _reader.ReadString();

            default:
                throw new InvalidDataException();
        }
    }

    /// <summary> Reads the next bytes as an array of AnyData</summary>
    public AnyData[] ReadAnyArray() => ReadGenericArray(ReadAny);
    /// <summary> Reads the next bytes as an array of booleans</summary>
    public bool[] ReadBoolArray() => ReadGenericArray(_reader.ReadBoolean);
    /// <summary> Reads the next bytes as an array of integers</summary>
    public int[] ReadIntArray() => ReadGenericArray(_reader.ReadInt32);
    /// <summary> Reads the next bytes as an array of float</summary>
    public float[] ReadFloatArray() => ReadGenericArray(_reader.ReadSingle);
    /// <summary> Reads the next bytes as an array of Vector2</summary>
    public Vector2[] ReadVec2Array() => ReadGenericArray(ReadVec2);
    /// <summary> Reads the next bytes as an array of Vector3</summary>
    public Vector3[] ReadVec3Array() => ReadGenericArray(ReadVec3);
    /// <summary> Reads the next bytes as an array of colours</summary>
    public Color[] ReadColourArray() => ReadGenericArray(ReadColour);
    /// <summary> Reads the next bytes as an array of strings</summary>
    public string[] ReadStringArray() => ReadGenericArray(_reader.ReadString);

    /// <summary> Reads the next bytes as an array of components of a given type. </summary>
    /// <returns></returns>
    public T[] ReadComponents<T>()
        where T: struct, ISerialisableComponent
    {
        if (!ReadTestIdentifier('A'))
            throw new InvalidDataException();

        if (!ReadBool())
            return null;

        int count = _reader.ReadInt32();
        T[] components = new T[count];

        for (int i = 0; i < count; ++i) {
            components[i] = default;
            components[i].Deserialise(this);
        }

        return components;
    }

    /// <summary> Reads the next bytes as a set of strings. (Commonly used for ID 'availability' sets) </summary>
    public HashSet<string> ReadStringSet()
    {
        if (ReadTestIdentifier('H'))
            throw new InvalidDataException();

        if (!ReadBool())
            return null;

        int length = _reader.ReadInt32();
        HashSet<string> set = new(length);

        for (int i = 0; i < length; ++i) {
            set.Add(_reader.ReadString());
        }

        return set;
    }

    /// <summary> Reads the next bytes as a SCHLib data dictionary. </summary>
    public DataDict ReadDataDict()
    {
        if (!ReadTestIdentifier('%'))
            throw new InvalidDataException();

        if (!ReadBool())
            return null;

        int length = _reader.ReadInt32();
        DataDict data = new(length);

        for (int i = 0; i < length; ++i) {
            data.Add(_reader.ReadString(), ReadAny());
        }

        return data;
    }

    /// <summary> [reuse] Reads the next bytes as an array of AnyData</summary>
    public void ReadAnyArray(Span<AnyData> data) => ReadGenericArrayNoAlloc(ReadAny, data);
    /// <summary> [reuse] Reads the next bytes as an array of booleans</summary>
    public void ReadBoolArray(Span<bool> data) => ReadGenericArrayNoAlloc(_reader.ReadBoolean, data);
    /// <summary> [reuse] Reads the next bytes as an array of integers</summary>
    public void ReadIntArray(Span<int> data) => ReadGenericArrayNoAlloc(_reader.ReadInt32, data);
    /// <summary> [reuse] Reads the next bytes as an array of float</summary>
    public void ReadFloatArray(Span<float> data) => ReadGenericArrayNoAlloc(_reader.ReadSingle, data);
    /// <summary> [v] Reads the next bytes as an array of Vector2</summary>
    public void ReadVec2Array(Span<Vector2> data) => ReadGenericArrayNoAlloc(ReadVec2, data);
    /// <summary> [reuse] Reads the next bytes as an array of Vector3</summary>
    public void ReadVec3Array(Span<Vector3> data) => ReadGenericArrayNoAlloc(ReadVec3, data);
    /// <summary> [reuse] Reads the next bytes as an array of colours</summary>
    public void ReadColourArray(Span<Color> data) => ReadGenericArrayNoAlloc(ReadColour, data);
    /// <summary> [reuse, no null checking] Deserialises a component. </summary>
    public void ReadToComponent(ISerialisableComponent component) => component.Deserialise(this);

    /// <summary> [reuse, no null checking] Deserialises an array of components.</summary>
    public void ReadToComponents<T>(ReadOnlySpan<T> components)
        where T: ISerialisableComponent
    {
        if (!ReadTestIdentifier('A'))
            throw new InvalidDataException();

        if (!ReadBool())
            return;

        int count = _reader.ReadInt32();

        for (int i = 0; i < count; ++i) {
            components[i].Deserialise(this);
        }
    }

    /// <summary> [reuse, no null checking] Deserialises an array of components.</summary>
    public void ReadToComponents<T>(T[] components)
        where T: ISerialisableComponent
    {
        if (!ReadTestIdentifier('A'))
            throw new InvalidDataException();

        if (!ReadBool())
            return;

        int count = _reader.ReadInt32();

        for (int i = 0; i < count; ++i) {
            components[i].Deserialise(this);
        }
    }

    /// <summary> [reuse, no null checking] Reads the next bytes as a set of strings.</summary>
    public void ReadStringSet(HashSet<string> set)
    {
        if (ReadTestIdentifier('H'))
            throw new InvalidDataException();

        if (!ReadBool())
            return;

        int length = _reader.ReadInt32();
        set.Clear();

        for (int i = 0; i < length; ++i) {
            set.Add(_reader.ReadString());
        }
    }

    /// <summary> [reuse, no null checking] Reads the next bytes as a SCHLib data dictionary. </summary>
    public void ReadDataDict(DataDict data)
    {
        if (!ReadTestIdentifier('%'))
            throw new InvalidDataException();

        if (!ReadBool())
            return;

        data.Clear();
        int length = _reader.ReadInt32();

        for (int i = 0; i < length; ++i) {
            data.Add(_reader.ReadString(), ReadAny());
        }
    }

    #endregion

    #region Godot Primitive Readers

    /// <summary> Reads the next bytes as a Vector2 </summary>
    /// <returns></returns>
    public Vector2 ReadVec2() {
        return new(_reader.ReadSingle(), _reader.ReadSingle());
    }

    /// <summary> Reads the next bytes as a Vector3 </summary>
    /// <returns></returns>
    public Vector3 ReadVec3() {
        return new(_reader.ReadSingle(), _reader.ReadSingle(), _reader.ReadSingle());
    }

    /// <summary> Reads the next bytes as a colour </summary>
    /// <returns></returns>
    public Color ReadColour()
    {
        return new(
            _reader.ReadSingle(),
            _reader.ReadSingle(),
            _reader.ReadSingle(), (float)
            _reader.ReadByte() / 255f);
    }

    #endregion

    #region Primitive Readers

    /// <summary> Reads the next byte </summary>
    /// <returns></returns>
    public int ReadByte() {
        return _reader.ReadByte();
    }

    /// <summary> Reads the next bytes as an integer </summary>
    /// <returns></returns>
    public int ReadInt() {
        return _reader.ReadInt32();
    }

    /// <summary> Reads the next byte as a boolean </summary>
    /// <returns></returns>
    public bool ReadBool() {
        return _reader.ReadBoolean();
    }

    /// <summary> Reads the next bytes as a float </summary>
    /// <returns></returns>
    public float ReadFloat() {
        return _reader.ReadSingle();
    }

    /// <summary> Reads the next bytes as a string </summary>
    /// <returns></returns>
    public string ReadString() {
        return _reader.ReadString();
    }

    /// <summary> Reads the next byte as a char </summary>
    /// <returns></returns>
    public char ReadChar() {
        return _reader.ReadChar();
    }

    #endregion

    #region Utils

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private T[] ReadGenericArray<T>(Func<T> readMethod)
    {
        if (!ReadTestIdentifier('A'))
            throw new InvalidDataException();

        if (!ReadBool())
            return null;

        int count = _reader.ReadInt32();
        T[] outrray = new T[count];

        for (int i = 0; i < count; ++i) {
            outrray[i] = readMethod();
        }

        return outrray;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReadGenericArrayNoAlloc<T>(Func<T> readMethod, Span<T> output)
        where T: struct
    {
        if (!ReadTestIdentifier('A'))
            throw new InvalidDataException();

        if (!ReadBool())
            return;

        int count = _reader.ReadInt32();

        Debug.Assert(
            condition: count < output.Length,
            message: "ReadGenericArrayNoAlloc: target span does not have enough space to hold the serialised data."
        );

        for (int i = 0; i < count; ++i) {
            output[i] = readMethod();
        }
    }

    #endregion
}