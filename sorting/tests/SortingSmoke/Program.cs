using System;
using System.Collections.Generic;
using System.Linq;
using sorting;

internal static class Program
{
    private static int Main()
    {
        var algorithms = new Dictionary<string, Func<int[], int[]>>
        {
            ["Intro"] = ModernSorting.Intro,
            ["PDQ-inspired"] = ModernSorting.PdqInspired,
            ["PowerSort"] = ModernSorting.Power,
            ["PSRS"] = values => ModernSorting.Psrs(values, Math.Min(4, Environment.ProcessorCount)),
            ["LinearModelBuckets"] = values => ModernSorting.LinearModelBuckets(values),
            ["SignedLsdRadix"] = ModernSorting.SignedLsdRadix,
        };

        var cases = new List<int[]>
        {
            Array.Empty<int>(),
            new[] { 1 },
            new[] { 2, 1 },
            new[] { 5, -1, 5, 0, -1, 9 },
            Enumerable.Range(0, 128).ToArray(),
            Enumerable.Range(0, 128).Reverse().ToArray(),
            Enumerable.Repeat(7, 256).ToArray(),
        };

        var rng = new Random(20260905);
        foreach (int size in new[] { 3, 17, 31, 257, 4096, 10000 })
        {
            int[] data = new int[size];
            for (int i = 0; i < size; i++)
                data[i] = rng.Next(-size, size + 1);
            cases.Add(data);
        }

        foreach (KeyValuePair<string, Func<int[], int[]>> algorithm in algorithms)
        {
            foreach (int[] input in cases)
            {
                int[] original = (int[])input.Clone();
                int[] expected = (int[])input.Clone();
                Array.Sort(expected);

                int[] actual = algorithm.Value(input);

                if (!actual.SequenceEqual(expected))
                {
                    Console.Error.WriteLine($"FAIL: {algorithm.Key}, n={input.Length}");
                    return 1;
                }

                if (!input.SequenceEqual(original))
                {
                    Console.Error.WriteLine($"FAIL: {algorithm.Key} mutated its input, n={input.Length}");
                    return 2;
                }
            }

            Console.WriteLine($"PASS: {algorithm.Key}");
        }

        Console.WriteLine("All modern sorting smoke tests passed.");
        return 0;
    }
}
