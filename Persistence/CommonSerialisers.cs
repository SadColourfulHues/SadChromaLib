using Godot;

namespace SadChromaLib.Persistence;

using SerialisedData = Godot.Collections.Dictionary<StringName, Variant>;

public static class Vector2Serialiser
{
	private static StringName KeyX => "x";
	private static StringName KeyY => "y";

	public static SerialisedData Serialise(this Vector2 vec)
	{
		return new() {
			[KeyX] = vec.X,
			[KeyY] = vec.Y
		};
	}

	public static Vector2 Deserialise(SerialisedData data)
	{
		return new Vector2(
			x: (float) data[KeyX],
			y: (float) data[KeyY]
		);
	}
}

public static class Vector3Serialiser
{
	private static StringName KeyX => "x";
	private static StringName KeyY => "y";
	private static StringName KeyZ => "z";

	public static SerialisedData Serialise(this Vector3 vec)
	{
		return new() {
			[KeyX] = vec.X,
			[KeyY] = vec.Y,
			[KeyZ] = vec.Z
		};
	}

	public static Vector3 Deserialise(SerialisedData data)
	{
		return new Vector3(
			x: (float) data[KeyX],
			y: (float) data[KeyY],
			z: (float) data[KeyZ]
		);
	}
}

public static class ColourSerialiser
{
	private static StringName KeyR => "r";
	private static StringName KeyG => "g";
	private static StringName KeyB => "b";
	private static StringName KeyA => "a";

	public static SerialisedData Serialise(this Color colour)
	{
		return new() {
			[KeyR] = colour.R,
			[KeyG] = colour.G,
			[KeyB] = colour.B,
			[KeyA] = colour.A
		};
	}

	public static Color Deserialise(SerialisedData data)
	{
		return new Color(
			r: (float) data[KeyR],
			g: (float) data[KeyG],
			b: (float) data[KeyB],
			a: (float) data[KeyA]
		);
	}
}