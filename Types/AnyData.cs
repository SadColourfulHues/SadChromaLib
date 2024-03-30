using Godot;

using System;
using System.IO;

using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using SadChromaLib.Persistence;

namespace SadChromaLib.Types;

/// <summary>
/// A union for holding common data types.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct AnyData
{
    public enum Type: byte
    {
        Unknown = 0,
        Bool = 1,
        Int = 2,
        Float = 3,
        Vector2 = 4,
        Vector3 = 5,
        Colour = 6,
        String = 7,
        Array = 8,
        Dictionary = 9,
        Any = 10
    }

    [FieldOffset(0)]
    public bool BoolValue;

    [FieldOffset(0)]
    public int IntValue;

    [FieldOffset(0)]
    public float X;

    [FieldOffset(4)]
    public float Y;

    [FieldOffset(8)]
    public float Z;

    [FieldOffset(12)]
    public byte A;

    [FieldOffset(13)]
    public Type DataType;

    [FieldOffset(16)]
    public string Text;

    public AnyData(bool value)
        : this()
    {
        BoolValue = value;
        DataType = Type.Bool;
    }

    public AnyData(int value)
        : this()
    {
        IntValue = value;
        DataType = Type.Int;
    }

    public AnyData(float value)
        : this()
    {
        X = value;
        DataType = Type.Float;
    }

    public AnyData(string text, Type? @override = null)
        : this()
    {
        Text = text;
        DataType = @override ?? Type.String;
    }

    public AnyData(Vector2 v)
        : this()
    {
        X = v.X;
        Y = v.Y;

        DataType = Type.Vector2;
    }

    public AnyData(Vector3 v)
        : this()
    {
        X = v.X;
        Y = v.Y;
        Z = v.Z;

        DataType = Type.Vector3;
    }

    public AnyData(Color v)
        : this()
    {
        X = v.R;
        Y = v.G;
        Z = v.B;
        A = (byte) Mathf.Floor(255f * v.A);

        DataType = Type.Colour;
    }

    public AnyData(bool[] data)
        : this()
    {
        SerialiseArray(
            data: data,
            internalType: Type.Bool,
            (writer, v) => writer.Write(v)
        );
    }

    public AnyData(int[] data)
        : this()
    {
        SerialiseArray(
            data: data,
            internalType: Type.Int,
            (writer, v) => writer.Write(v)
        );
    }

    public AnyData(float[] data)
        : this()
    {
        SerialiseArray(
            data: data,
            internalType: Type.Float,
            (writer, v) => writer.Write(v)
        );
    }

    public AnyData(Vector2[] data)
        : this()
    {
        SerialiseArray(
            data: data,
            internalType: Type.Vector2,
            (writer, v) => writer.Write(v)
        );
    }

    public AnyData(Vector3[] data)
        : this()
    {
        SerialiseArray(
            data: data,
            internalType: Type.Vector3,
            (writer, v) => writer.Write(v)
        );
    }

    public AnyData(Color[] data)
        : this()
    {
        SerialiseArray(
            data: data,
            internalType: Type.Colour,
            (writer, v) => writer.Write(v)
        );
    }

    public AnyData(string[] data)
        : this()
    {
        SerialiseArray(
            data: data,
            internalType: Type.String,
            (writer, v) => writer.Write(v)
        );
    }

    public AnyData(AnyData[] data)
        : this()
    {
        SerialiseArray(
            data: data,
            internalType: Type.Any,
            (writer, v) => writer.Write(v)
        );
    }

    public AnyData(DataDict data)
        : this()
    {
        MemoryStream buffer = new();

        using (PersistenceWriter writer = new(buffer)) {
            writer.Write(data.Count);

            foreach ((string key, AnyData value) in data) {
                writer.Write(key);
                writer.Write(value);
            }

            using (StreamReader reader = new(buffer)) {
                Text = reader.ReadToEnd();
            }
        }

        DataType = Type.Dictionary;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public readonly Vector2 AsV2() { return new(X, Y); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public readonly Vector3 AsV3() { return new(X, Y, Z); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public readonly Color AsColour() { return new(X, Y, Z, (float)A/255f); }

    public bool[] ToBoolArray()
    {
        return DeserialiseArray(
            expectedInternalType: Type.Bool,
            (reader) => reader.ReadBool()
        );
    }

    public int[] ToIntArray()
    {
        return DeserialiseArray(
            expectedInternalType: Type.Int,
            (reader) => reader.ReadInt()
        );
    }

    public float[] ToFloatArray()
    {
        return DeserialiseArray(
            expectedInternalType: Type.Float,
            (reader) => reader.ReadFloat()
        );
    }

    public Vector2[] ToVec2Array()
    {
        return DeserialiseArray(
            expectedInternalType: Type.Vector2,
            (reader) => reader.ReadVec2()
        );
    }

    public Vector3[] ToVec3Array()
    {
        return DeserialiseArray(
            expectedInternalType: Type.Vector3,
            (reader) => reader.ReadVec3()
        );
    }

    public Color[] ToColourArray()
    {
        return DeserialiseArray(
            expectedInternalType: Type.Colour,
            (reader) => reader.ReadColour()
        );
    }

    public string[] ToStringArray()
    {
        return DeserialiseArray(
            expectedInternalType: Type.String,
            (reader) => reader.ReadString()
        );
    }

    public AnyData[] ToAnyArray()
    {
        return DeserialiseArray(
            expectedInternalType: Type.Any,
            (reader) => reader.ReadAny()
        );
    }

    public DataDict ToDataDict()
    {
        if (DataType != Type.Array)
            return null;

        MemoryStream buffer = new(System.Text.Encoding.ASCII.GetBytes(Text));
        DataDict data = new();

        using (PersistenceReader reader = new(buffer)) {
            int count = reader.ReadInt();

            for (int i = 0; i < count; ++i) {
                data.Add(
                    key: reader.ReadString(),
                    value: reader.ReadAny()
                );
            }
        }

        return data;
    }

    #region Implicit Conversions

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator AnyData(bool value) { return new(value); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator AnyData(int value) { return new(value); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator AnyData(float value) { return new(value); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator AnyData(Vector2 value) { return new(value); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator AnyData(Vector3 value) { return new(value); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator AnyData(Color value) { return new(value); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator AnyData(string value) { return new(value); }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator AnyData(bool[] value) { return new(value); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator AnyData(int[] value) { return new(value); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator AnyData(float[] value) { return new(value); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator AnyData(Vector2[] value) { return new(value); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator AnyData(Vector3[] value) { return new(value); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator AnyData(Color[] value) { return new(value); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator AnyData(string[] value) { return new(value); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator AnyData(AnyData[] value) { return new(value); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator AnyData(DataDict value) { return new(value); }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator bool(AnyData d) { return d.BoolValue; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator int(AnyData d) { return d.IntValue; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator float(AnyData d) { return d.X; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator Vector2(AnyData d) { return d.AsV2(); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator Vector3(AnyData d) { return d.AsV3(); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator Color(AnyData d) { return d.AsColour(); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator string(AnyData d) { return d.Text; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator bool[](AnyData d) { return d.ToBoolArray(); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator int[](AnyData d) { return d.ToIntArray(); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator float[](AnyData d) { return d.ToFloatArray(); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator Vector2[](AnyData d) { return d.ToVec2Array(); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator Vector3[](AnyData d) { return d.ToVec3Array(); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator Color[](AnyData d) { return d.ToColourArray(); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator AnyData[](AnyData d) { return d.ToAnyArray(); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator DataDict(AnyData d) { return d.ToDataDict(); }

    #endregion

    #region Utilities

    private void SerialiseArray<T>(T[] data, Type internalType, Action<PersistenceWriter, T> writeMethod)
    {
        MemoryStream buffer = new();

        using (PersistenceWriter writer = new(buffer)) {
            writer.Write((byte) internalType);
            writer.Write(data.Length);

            for (int i = 0; i < data.Length; ++i) {
                writeMethod(writer, data[i]);
            }

            using (StreamReader reader = new(buffer)) {
                Text = reader.ReadToEnd();
            }
        }

        DataType = Type.Array;
    }

    private T[] DeserialiseArray<T>(Type expectedInternalType, Func<PersistenceReader, T> readMethod)
    {
        if (DataType != Type.Array)
            return null;

        MemoryStream buffer = new(System.Text.Encoding.ASCII.GetBytes(Text));
        T[] array = null;

        using (PersistenceReader reader = new(buffer)) {
            Type internalType = (Type) reader.ReadByte();

            if (internalType != expectedInternalType)
                return array;

            int count = reader.ReadInt();
            array = new T[count];

            for (int i = 0; i < count; ++i) {
                array[i] = readMethod(reader);
            }
        }

        return array;
    }

    #endregion
}