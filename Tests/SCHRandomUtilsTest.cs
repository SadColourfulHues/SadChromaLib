using Godot;
using System;
using System.Collections.Generic;

using SadChromaLib.Utils.Random;
using SadChromaLib.Utils.Tests;

namespace SadChromaLib.Tests.Instances;

public sealed partial class SCHRandomUtilsTest : Node
{
    [Export]
    private int _drawCount = 1000;
    [Export]
    private RandomMethod _method = RandomMethod.Basic;
    [Export]
    private bool _normalise;

    private class TestStruct : IWeightedObject<string>
    {
        float _weight;
        public float _originalWeight;
        readonly string _value;

        public TestStruct(string value, float weight)
        {
            _value = value;
            _weight = weight;
            _originalWeight = weight;
        }

        public float GetWeight()
        {
            return _weight;
        }

        public void UpdateWeight(float newWeight)
        {
            _weight = newWeight;
        }

        public string GetValue()
        {
            return _value;
        }
    }

	public override void _EnterTree()
	{
        RandomUtils.ReseedGUID();
        WeightedBag<string> bag = null;

        // Shuffle test
        string[] shoppingList = {
            "Morning",
            "Fig Cookies",
            "Number 4",
            "Orange",
            "Virus"
        };

        TestUtils.Run("Random Shuffle", () => {
            GD.Print("Unshuffled array: ", ArrayToString(shoppingList));

            RandomUtils.Shuffle(shoppingList);
            GD.Print("Shuffled array: ", ArrayToString(shoppingList));
        });

        TestStruct[] test = {
            new TestStruct("Coal", 0.6f),
            new TestStruct("Animal Droppings", 0.5f),
            new TestStruct("Gold XP", 0.45f),
            new TestStruct("Stone Fried", 0.35f),
            new TestStruct("Wild Diamond", 0.25f),
            new TestStruct("Platinum Sun", 0.2f),
            new TestStruct("Sprinting Steel Cube", 0.15f),
            new TestStruct("Wall of Wet Eyeballs", 0.1f),
            new TestStruct("Rat Ruby", 0.01f),
            new TestStruct("Heavenly Rat Scale", 0.001f)
        };

        GD.Print("Testing picking...");
        TestStruct item = RandomUtils.Pick(test);

        GD.Print(string.Format("{0} was picked!", item.GetValue()));

        TestUtils.Run("CreateBag", () =>
            bag = new WeightedBag<string>(test, true, _normalise));

        // Repeatedly pick from the bag to see how often each element was picked
        Dictionary<string, int> counts = new(test.Length);

        TestUtils.Run(string.Format("Draw {0} items", _drawCount), () => {
            string lastPicked = "";
            int timesRepeated = 0;

            for (int i = 0; i < _drawCount; ++ i) {
                string picked = bag.Pick(_method);

                if (picked == null)
                    continue;

                if (picked == lastPicked) {
                    timesRepeated ++;
                }

                if (counts.ContainsKey(picked)) {
                    counts[picked] ++;
                }
                else {
                    counts[picked] = 1;
                }

                lastPicked = picked;
            }

            GD.Print($"Repeated draws: {timesRepeated}");
        });

        // Show results
        float drawCountF = _drawCount;

        for (int i = 0, l = test.Length; i < l; ++ i) {
            string key = test[i].GetValue();

            if (!counts.ContainsKey(key))
                continue;

            int count = counts[key];

            float prob = count / drawCountF;
            float expected = test[i]._originalWeight;

            GD.Print($"{key}: {count} / {_drawCount} ({prob * 100f:0.00}) (Expected: {expected * 100f:0.00}% Diff: {(expected - prob) * 100f:0.00}%)");
        }

        GetTree().CallDeferred("quit");
	}

    /// <summary> Combines an array string into one </summary>
    private string ArrayToString(string[] strings)
    {
        Span<char> finalStr = stackalloc char[4096];
        int charIdx = 0;

        for (int i = 0, l = strings.Length; i < l; ++ i) {
            CopyString(strings[i], ref finalStr, ref charIdx);
        }

        return finalStr[..charIdx].ToString();
    }

    private static void CopyString(ReadOnlySpan<char> source, ref Span<char> destination, ref int idx)
    {
        for (int i = 0; i < source.Length; ++ i) {
            destination[idx] = source[i];
            idx ++;
        }

        destination[idx] = ' ';
        idx ++;
    }
}
