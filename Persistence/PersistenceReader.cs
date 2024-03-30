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

    public void ReadArray(ref Span<AnyData> data)
    {
        if (!ReadTestIdentifier('A'))
            throw new InvalidDataException();

        int length = _reader.ReadInt32();

        Debug.Assert(
            condition: data.Length >= length,
            message: "PersistenceDeserialiser: target span does not have enough space to store serialised data."
        );

        for (int i = 0; i < length; ++i) {
            data[i] = ReadAny();
        }
    }

    public void ReadArray(AnyData[] array)
    {
        if (!ReadTestIdentifier('A'))
            throw new InvalidDataException();

        int length = _reader.ReadInt32();

        Debug.Assert(
            condition: array.Length >= length,
            message: "PersistenceDeserialiser: target array does not have enough space to store serialised data."
        );

        for (int i = 0; i < length; ++i) {
            array[i] = ReadAny();
        }
    }

    public AnyData[] ReadAnyArray() => ReadGenericArray(ReadAny);
    public bool[] ReadBoolArray() => ReadGenericArray(_reader.ReadBoolean);
    public int[] ReadIntArray() => ReadGenericArray(_reader.ReadInt32);
    public float[] ReadFloatArray() => ReadGenericArray(_reader.ReadSingle);
    public Vector2[] ReadVec2Array() => ReadGenericArray(ReadVec2);
    public Vector3[] ReadVec3Array() => ReadGenericArray(ReadVec3);
    public Color[] ReadColourArray() => ReadGenericArray(ReadColour);
    public string[] ReadStringArray() => ReadGenericArray(_reader.ReadString);

    public HashSet<string> ReadStringSet()
    {
        if (ReadTestIdentifier('H'))
            throw new InvalidDataException();

        int length = _reader.ReadInt32();
        HashSet<string> set = new(length);

        for (int i = 0; i < length; ++i) {
            set.Add(_reader.ReadString());
        }

        return set;
    }

    public void ReadStringSet(HashSet<string> set)
    {
        if (ReadTestIdentifier('H'))
            throw new InvalidDataException();

        int length = _reader.ReadInt32();

        for (int i = 0; i < length; ++i) {
            set.Add(_reader.ReadString());
        }
    }

    public DataDict ReadDataDict()
    {
        if (!ReadTestIdentifier('%'))
            throw new InvalidDataException();

        int length = _reader.ReadInt32();
        DataDict data = new(length);

        for (int i = 0; i < length; ++i) {
            data.Add(_reader.ReadString(), ReadAny());
        }

        return data;
    }

    public void ReadDataDict(DataDict data)
    {
        if (!ReadTestIdentifier('%'))
            throw new InvalidDataException();

        data.Clear();
        int length = _reader.ReadInt32();

        for (int i = 0; i < length; ++i) {
            data.Add(_reader.ReadString(), ReadAny());
        }
    }

    public void ReadToComponent(ISerialisableComponent component)
    {
        component.Deserialise(this);
    }

    #endregion

    #region Godot Primitive Readers

    public Vector2 ReadVec2() {
        return new(_reader.ReadSingle(), _reader.ReadSingle());
    }

    public Vector3 ReadVec3() {
        return new(_reader.ReadSingle(), _reader.ReadSingle(), _reader.ReadSingle());
    }

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

    public int ReadByte() {
        return _reader.ReadByte();
    }

    public int ReadInt() {
        return _reader.ReadInt32();
    }

    public bool ReadBool() {
        return _reader.ReadBoolean();
    }

    public float ReadFloat() {
        return _reader.ReadSingle();
    }

    public string ReadString() {
        return _reader.ReadString();
    }

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

        int count = _reader.ReadInt32();
        T[] outrray = new T[count];

        for (int i = 0; i < count; ++i) {
            outrray[i] = readMethod();
        }

        return outrray;
    }

    #endregion
}