using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sorting
{
    /// <summary>
    /// Modern / research-oriented sorting algorithms.
    ///
    /// The methods clone the input and return a sorted copy, so benchmark
    /// harnesses can give every algorithm identical immutable input.
    /// </summary>
    public static class ModernSorting
    {
        private const int InsertionThreshold = 24;

        public static int[] Intro(int[] array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            int[] result = (int[])array.Clone();
            if (result.Length < 2) return result;

            int depthLimit = 2 * FloorLog2(result.Length);
            IntroSort(result, 0, result.Length - 1, depthLimit);
            return result;
        }

        /// <summary>
        /// A practical pattern-aware introspective quicksort.
        ///
        /// This is intentionally labelled "PDQ-inspired" rather than a literal
        /// port of Orson Peters' pdqsort: it keeps the same engineering ideas
        /// (median pivoting, insertion-sort small partitions, heapsort fallback,
        /// bad-partition budgeting and early exit for ordered partitions) without
        /// claiming source-level equivalence with the reference implementation.
        /// </summary>
        public static int[] PdqInspired(int[] array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            int[] result = (int[])array.Clone();
            if (result.Length < 2) return result;

            int badPartitionBudget = FloorLog2(result.Length);
            PdqInspiredSort(result, 0, result.Length - 1, badPartitionBudget);
            return result;
        }

        /// <summary>
        /// Stable natural-run mergesort using the Powersort node-power merge policy.
        /// </summary>
        public static int[] Power(int[] array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            int[] result = (int[])array.Clone();
            int n = result.Length;
            if (n < 2) return result;

            var stack = new List<Run>();
            Run current = FindNaturalRun(result, 0);

            while (current.End < n)
            {
                Run next = FindNaturalRun(result, current.End);
                int power = NodePower(current.Start, current.Length, next.Length, n);

                while (stack.Count > 0 && stack[stack.Count - 1].Power > power)
                {
                    Run previous = stack[stack.Count - 1];
                    stack.RemoveAt(stack.Count - 1);
                    current = MergeRuns(result, previous, current);
                }

                current.Power = power;
                stack.Add(current);
                current = next;
            }

            while (stack.Count > 0)
            {
                Run previous = stack[stack.Count - 1];
                stack.RemoveAt(stack.Count - 1);
                current = MergeRuns(result, previous, current);
            }

            return result;
        }

        /// <summary>
        /// Parallel Sorting by Regular Sampling (PSRS).
        ///
        /// Local chunks are introsorted in parallel, regular samples choose
        /// global pivots, then every processor range is partitioned into buckets
        /// and each bucket is k-way merged in parallel.
        /// </summary>
        public static int[] Psrs(int[] array, int? workers = null)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));

            int n = array.Length;
            if (n < 2) return (int[])array.Clone();

            int p = workers ?? Environment.ProcessorCount;
            p = Math.Max(1, Math.Min(p, n));

            if (p == 1 || n < 4096)
                return Intro(array);

            int[] data = (int[])array.Clone();
            Range[] ranges = BuildRanges(n, p);

            Parallel.For(0, p, i =>
            {
                Range range = ranges[i];
                if (range.Length > 1)
                {
                    int depthLimit = 2 * FloorLog2(range.Length);
                    IntroSort(data, range.Start, range.EndExclusive - 1, depthLimit);
                }
            });

            int[] samples = new int[p * (p - 1)];
            int sampleIndex = 0;
            for (int i = 0; i < p; i++)
            {
                Range range = ranges[i];
                for (int s = 1; s < p; s++)
                {
                    int offset = (int)((long)s * range.Length / p);
                    if (offset >= range.Length) offset = range.Length - 1;
                    samples[sampleIndex++] = data[range.Start + offset];
                }
            }

            Array.Sort(samples, 0, sampleIndex);

            int[] pivots = new int[p - 1];
            for (int i = 1; i < p; i++)
            {
                int idx = Math.Min(sampleIndex - 1, i * p - 1);
                pivots[i - 1] = samples[idx];
            }

            int[,] boundaries = new int[p, p + 1];
            for (int chunk = 0; chunk < p; chunk++)
            {
                Range range = ranges[chunk];
                boundaries[chunk, 0] = range.Start;
                int searchStart = range.Start;

                for (int bucket = 0; bucket < p - 1; bucket++)
                {
                    searchStart = UpperBound(data, searchStart, range.EndExclusive, pivots[bucket]);
                    boundaries[chunk, bucket + 1] = searchStart;
                }

                boundaries[chunk, p] = range.EndExclusive;
            }

            int[] bucketSizes = new int[p];
            for (int bucket = 0; bucket < p; bucket++)
            {
                int size = 0;
                for (int chunk = 0; chunk < p; chunk++)
                    size += boundaries[chunk, bucket + 1] - boundaries[chunk, bucket];
                bucketSizes[bucket] = size;
            }

            int[] bucketStarts = new int[p + 1];
            for (int i = 0; i < p; i++)
                bucketStarts[i + 1] = bucketStarts[i] + bucketSizes[i];

            int[] output = new int[n];
            Parallel.For(0, p, bucket =>
            {
                MergeBucket(data, boundaries, p, bucket, output, bucketStarts[bucket]);
            });

            return output;
        }

        /// <summary>
        /// Experimental learned-sort-inspired baseline for Int32 data.
        ///
        /// A one-parameter linear CDF approximation predicts a coarse bucket.
        /// Buckets are then introsorted independently. It is intentionally simple:
        /// the goal is to expose where model-based distribution helps or hurts.
        /// </summary>
        public static int[] LinearModelBuckets(int[] array, int bucketCount = 0)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            int n = array.Length;
            if (n < 2) return (int[])array.Clone();

            int min = array[0];
            int max = array[0];
            for (int i = 1; i < n; i++)
            {
                if (array[i] < min) min = array[i];
                if (array[i] > max) max = array[i];
            }

            if (min == max) return (int[])array.Clone();

            if (bucketCount <= 0)
                bucketCount = Math.Max(8, Math.Min(2048, (int)Math.Sqrt(n)));

            var buckets = new List<int>[bucketCount];
            long span = (long)max - min + 1L;

            for (int i = 0; i < n; i++)
            {
                int bucket = (int)(((long)array[i] - min) * bucketCount / span);
                if (bucket >= bucketCount) bucket = bucketCount - 1;

                List<int> list = buckets[bucket];
                if (list == null)
                {
                    list = new List<int>();
                    buckets[bucket] = list;
                }
                list.Add(array[i]);
            }

            int[] result = new int[n];
            int write = 0;

            for (int b = 0; b < bucketCount; b++)
            {
                List<int> list = buckets[b];
                if (list == null || list.Count == 0) continue;

                int[] local = list.ToArray();
                if (local.Length > 1)
                    IntroSort(local, 0, local.Length - 1, 2 * FloorLog2(local.Length));

                Array.Copy(local, 0, result, write, local.Length);
                write += local.Length;
            }

            return result;
        }

        /// <summary>
        /// Four-pass stable LSD radix sort for signed Int32 values.
        /// </summary>
        public static int[] SignedLsdRadix(int[] array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            int n = array.Length;
            if (n < 2) return (int[])array.Clone();

            int[] src = (int[])array.Clone();
            int[] dst = new int[n];

            for (int shift = 0; shift < 32; shift += 8)
            {
                int[] counts = new int[256];

                for (int i = 0; i < n; i++)
                {
                    uint key = unchecked((uint)(src[i] ^ int.MinValue));
                    counts[(key >> shift) & 0xFF]++;
                }

                int sum = 0;
                for (int i = 0; i < counts.Length; i++)
                {
                    int count = counts[i];
                    counts[i] = sum;
                    sum += count;
                }

                for (int i = 0; i < n; i++)
                {
                    uint key = unchecked((uint)(src[i] ^ int.MinValue));
                    int digit = (int)((key >> shift) & 0xFF);
                    dst[counts[digit]++] = src[i];
                }

                int[] swap = src;
                src = dst;
                dst = swap;
            }

            return src;
        }

        private static void IntroSort(int[] a, int lo, int hi, int depthLimit)
        {
            while (hi - lo > InsertionThreshold)
            {
                if (depthLimit == 0)
                {
                    HeapSortRange(a, lo, hi);
                    return;
                }

                depthLimit--;
                int pivot = PartitionMedianOfThree(a, lo, hi);

                if (pivot - lo < hi - pivot)
                {
                    IntroSort(a, lo, pivot - 1, depthLimit);
                    lo = pivot + 1;
                }
                else
                {
                    IntroSort(a, pivot + 1, hi, depthLimit);
                    hi = pivot - 1;
                }
            }

            InsertionSortRange(a, lo, hi);
        }

        private static void PdqInspiredSort(int[] a, int lo, int hi, int badPartitionBudget)
        {
            while (hi - lo > InsertionThreshold)
            {
                if (IsNonDecreasing(a, lo, hi))
                    return;

                if (badPartitionBudget == 0)
                {
                    HeapSortRange(a, lo, hi);
                    return;
                }

                int pivot = PartitionMedianOfThree(a, lo, hi);
                int leftSize = pivot - lo;
                int rightSize = hi - pivot;
                int total = hi - lo + 1;

                if (Math.Min(leftSize, rightSize) < total / 8)
                    badPartitionBudget--;

                if (leftSize < rightSize)
                {
                    PdqInspiredSort(a, lo, pivot - 1, badPartitionBudget);
                    lo = pivot + 1;
                }
                else
                {
                    PdqInspiredSort(a, pivot + 1, hi, badPartitionBudget);
                    hi = pivot - 1;
                }
            }

            InsertionSortRange(a, lo, hi);
        }

        private static bool IsNonDecreasing(int[] a, int lo, int hi)
        {
            for (int i = lo + 1; i <= hi; i++)
                if (a[i - 1] > a[i]) return false;
            return true;
        }

        private static int PartitionMedianOfThree(int[] a, int lo, int hi)
        {
            int mid = lo + ((hi - lo) >> 1);

            if (a[mid] < a[lo]) Swap(a, lo, mid);
            if (a[hi] < a[lo]) Swap(a, lo, hi);
            if (a[hi] < a[mid]) Swap(a, mid, hi);

            int pivotValue = a[mid];
            Swap(a, mid, hi - 1);

            int i = lo;
            int j = hi - 1;

            while (true)
            {
                while (a[++i] < pivotValue) { }
                while (a[--j] > pivotValue) { }

                if (i >= j) break;
                Swap(a, i, j);
            }

            Swap(a, i, hi - 1);
            return i;
        }

        private static void InsertionSortRange(int[] a, int lo, int hi)
        {
            if (lo >= hi) return;

            for (int i = lo + 1; i <= hi; i++)
            {
                int value = a[i];
                int j = i - 1;
                while (j >= lo && a[j] > value)
                {
                    a[j + 1] = a[j];
                    j--;
                }
                a[j + 1] = value;
            }
        }

        private static void HeapSortRange(int[] a, int lo, int hi)
        {
            int count = hi - lo + 1;
            for (int i = count / 2 - 1; i >= 0; i--)
                SiftDown(a, i, count, lo);

            for (int end = count - 1; end > 0; end--)
            {
                Swap(a, lo, lo + end);
                SiftDown(a, 0, end, lo);
            }
        }

        private static void SiftDown(int[] a, int root, int count, int offset)
        {
            while (true)
            {
                int child = root * 2 + 1;
                if (child >= count) return;

                if (child + 1 < count && a[offset + child] < a[offset + child + 1])
                    child++;

                if (a[offset + root] >= a[offset + child])
                    return;

                Swap(a, offset + root, offset + child);
                root = child;
            }
        }

        private static int FloorLog2(int value)
        {
            int result = 0;
            while ((value >>= 1) != 0) result++;
            return result;
        }

        private static Run FindNaturalRun(int[] a, int start)
        {
            int n = a.Length;
            if (start >= n) return new Run(start, 0);
            if (start == n - 1) return new Run(start, 1);

            int end = start + 2;

            if (a[start + 1] < a[start])
            {
                while (end < n && a[end] < a[end - 1]) end++;
                Array.Reverse(a, start, end - start);
            }
            else
            {
                while (end < n && a[end] >= a[end - 1]) end++;
            }

            return new Run(start, end - start);
        }

        private static int NodePower(int start1, int len1, int len2, int totalLength)
        {
            long a = 2L * start1 + len1;
            long b = a + len1 + len2;
            long n = totalLength;
            int power = 0;

            while (true)
            {
                power++;

                if (a >= n)
                {
                    a -= n;
                    b -= n;
                }
                else if (b >= n)
                {
                    return power;
                }

                a <<= 1;
                b <<= 1;
            }
        }

        private static Run MergeRuns(int[] a, Run left, Run right)
        {
            if (left.End != right.Start)
                throw new InvalidOperationException("Powersort attempted to merge non-adjacent runs.");

            int[] temp = new int[left.Length + right.Length];
            int i = left.Start;
            int j = right.Start;
            int leftEnd = left.End;
            int rightEnd = right.End;
            int k = 0;

            while (i < leftEnd && j < rightEnd)
                temp[k++] = a[i] <= a[j] ? a[i++] : a[j++];

            while (i < leftEnd) temp[k++] = a[i++];
            while (j < rightEnd) temp[k++] = a[j++];

            Array.Copy(temp, 0, a, left.Start, temp.Length);
            return new Run(left.Start, temp.Length);
        }

        private static Range[] BuildRanges(int n, int p)
        {
            var ranges = new Range[p];
            int baseSize = n / p;
            int remainder = n % p;
            int start = 0;

            for (int i = 0; i < p; i++)
            {
                int length = baseSize + (i < remainder ? 1 : 0);
                ranges[i] = new Range(start, start + length);
                start += length;
            }

            return ranges;
        }

        private static int UpperBound(int[] a, int lo, int hiExclusive, int value)
        {
            while (lo < hiExclusive)
            {
                int mid = lo + ((hiExclusive - lo) >> 1);
                if (a[mid] <= value)
                    lo = mid + 1;
                else
                    hiExclusive = mid;
            }
            return lo;
        }

        private static void MergeBucket(
            int[] data,
            int[,] boundaries,
            int chunkCount,
            int bucket,
            int[] output,
            int outputStart)
        {
            var heap = new MinHeap(chunkCount);

            for (int chunk = 0; chunk < chunkCount; chunk++)
            {
                int start = boundaries[chunk, bucket];
                int end = boundaries[chunk, bucket + 1];
                if (start < end)
                    heap.Push(new HeapItem(data[start], chunk, start, end));
            }

            int write = outputStart;
            while (heap.Count > 0)
            {
                HeapItem item = heap.Pop();
                output[write++] = item.Value;

                int next = item.Index + 1;
                if (next < item.EndExclusive)
                    heap.Push(new HeapItem(data[next], item.Chunk, next, item.EndExclusive));
            }
        }

        private static void Swap(int[] a, int i, int j)
        {
            if (i == j) return;
            int temp = a[i];
            a[i] = a[j];
            a[j] = temp;
        }

        private sealed class Run
        {
            public Run(int start, int length)
            {
                Start = start;
                Length = length;
            }

            public int Start { get; private set; }
            public int Length { get; private set; }
            public int End { get { return Start + Length; } }
            public int Power { get; set; }
        }

        private struct Range
        {
            public Range(int start, int endExclusive)
            {
                Start = start;
                EndExclusive = endExclusive;
            }

            public int Start { get; private set; }
            public int EndExclusive { get; private set; }
            public int Length { get { return EndExclusive - Start; } }
        }

        private struct HeapItem
        {
            public HeapItem(int value, int chunk, int index, int endExclusive)
            {
                Value = value;
                Chunk = chunk;
                Index = index;
                EndExclusive = endExclusive;
            }

            public int Value;
            public int Chunk;
            public int Index;
            public int EndExclusive;
        }

        private sealed class MinHeap
        {
            private readonly HeapItem[] _items;
            private int _count;

            public MinHeap(int capacity)
            {
                _items = new HeapItem[Math.Max(1, capacity)];
            }

            public int Count { get { return _count; } }

            public void Push(HeapItem item)
            {
                int index = _count++;
                _items[index] = item;

                while (index > 0)
                {
                    int parent = (index - 1) >> 1;
                    if (!Less(_items[index], _items[parent])) break;

                    HeapItem temp = _items[index];
                    _items[index] = _items[parent];
                    _items[parent] = temp;
                    index = parent;
                }
            }

            public HeapItem Pop()
            {
                HeapItem root = _items[0];
                _count--;

                if (_count > 0)
                {
                    _items[0] = _items[_count];
                    int index = 0;

                    while (true)
                    {
                        int left = index * 2 + 1;
                        if (left >= _count) break;

                        int right = left + 1;
                        int smallest = right < _count && Less(_items[right], _items[left]) ? right : left;

                        if (!Less(_items[smallest], _items[index])) break;

                        HeapItem temp = _items[index];
                        _items[index] = _items[smallest];
                        _items[smallest] = temp;
                        index = smallest;
                    }
                }

                return root;
            }

            private static bool Less(HeapItem a, HeapItem b)
            {
                if (a.Value != b.Value) return a.Value < b.Value;
                if (a.Chunk != b.Chunk) return a.Chunk < b.Chunk;
                return a.Index < b.Index;
            }
        }
    }
}
