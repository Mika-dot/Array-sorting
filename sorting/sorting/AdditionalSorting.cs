using System;
using System.Collections.Generic;

namespace sorting
{
    public class AdditionalSorting
    {
        public static int[] Heap(int[] array)
        {
            int[] result = (int[])array.Clone();
            for (int i = result.Length / 2 - 1; i >= 0; i--)
                Heapify(result, result.Length, i);

            for (int i = result.Length - 1; i > 0; i--)
            {
                (result[0], result[i]) = (result[i], result[0]);
                Heapify(result, i, 0);
            }
            return result;
        }

        private static void Heapify(int[] a, int n, int i)
        {
            int largest = i;
            int l = i * 2 + 1;
            int r = i * 2 + 2;
            if (l < n && a[l] > a[largest]) largest = l;
            if (r < n && a[r] > a[largest]) largest = r;
            if (largest != i)
            {
                (a[i], a[largest]) = (a[largest], a[i]);
                Heapify(a, n, largest);
            }
        }

        public static int[] Cycle(int[] array)
        {
            int[] a = (int[])array.Clone();
            for (int cycleStart = 0; cycleStart < a.Length - 1; cycleStart++)
            {
                int item = a[cycleStart];
                int pos = cycleStart;
                for (int i = cycleStart + 1; i < a.Length; i++)
                    if (a[i] < item) pos++;

                if (pos == cycleStart) continue;

                while (pos < a.Length && item == a[pos]) pos++;
                if (pos == a.Length) continue;

                (item, a[pos]) = (a[pos], item);

                while (pos != cycleStart)
                {
                    pos = cycleStart;
                    for (int i = cycleStart + 1; i < a.Length; i++)
                        if (a[i] < item) pos++;

                    while (pos < a.Length && item == a[pos]) pos++;
                    if (pos == a.Length) break;

                    (item, a[pos]) = (a[pos], item);
                }
            }
            return a;
        }

        public static int[] Patience(int[] array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (array.Length < 2) return (int[])array.Clone();

            var piles = new List<List<int>>();
            var tops = new List<int>();

            foreach (int value in array)
            {
                int lo = 0;
                int hi = tops.Count;
                while (lo < hi)
                {
                    int mid = lo + ((hi - lo) >> 1);
                    if (tops[mid] >= value)
                        hi = mid;
                    else
                        lo = mid + 1;
                }

                if (lo == piles.Count)
                {
                    piles.Add(new List<int> { value });
                    tops.Add(value);
                }
                else
                {
                    piles[lo].Add(value);
                    tops[lo] = value;
                }
            }

            var heap = new List<PileHead>();
            for (int i = 0; i < piles.Count; i++)
                PushPileHead(heap, new PileHead(piles[i][piles[i].Count - 1], i));

            int[] result = new int[array.Length];
            int write = 0;

            while (heap.Count > 0)
            {
                PileHead head = PopPileHead(heap);
                result[write++] = head.Value;

                List<int> pile = piles[head.PileIndex];
                pile.RemoveAt(pile.Count - 1);
                if (pile.Count > 0)
                    PushPileHead(heap, new PileHead(pile[pile.Count - 1], head.PileIndex));
            }

            return result;
        }

        private struct PileHead
        {
            public PileHead(int value, int pileIndex)
            {
                Value = value;
                PileIndex = pileIndex;
            }

            public int Value;
            public int PileIndex;
        }

        private static bool Less(PileHead a, PileHead b)
        {
            return a.Value < b.Value || (a.Value == b.Value && a.PileIndex < b.PileIndex);
        }

        private static void PushPileHead(List<PileHead> heap, PileHead value)
        {
            int index = heap.Count;
            heap.Add(value);

            while (index > 0)
            {
                int parent = (index - 1) >> 1;
                if (!Less(heap[index], heap[parent])) break;

                PileHead temp = heap[index];
                heap[index] = heap[parent];
                heap[parent] = temp;
                index = parent;
            }
        }

        private static PileHead PopPileHead(List<PileHead> heap)
        {
            PileHead root = heap[0];
            int last = heap.Count - 1;
            heap[0] = heap[last];
            heap.RemoveAt(last);

            int index = 0;
            while (index < heap.Count)
            {
                int left = index * 2 + 1;
                if (left >= heap.Count) break;

                int right = left + 1;
                int smallest = right < heap.Count && Less(heap[right], heap[left]) ? right : left;
                if (!Less(heap[smallest], heap[index])) break;

                PileHead temp = heap[index];
                heap[index] = heap[smallest];
                heap[smallest] = temp;
                index = smallest;
            }

            return root;
        }

        [Obsolete("The previous Smooth() method was a HeapSort alias, not Smoothsort. A verified Leonardo-heap implementation is still a research task.")]
        public static int[] Smooth(int[] array)
        {
            throw new NotSupportedException(
                "Smoothsort is intentionally disabled because the previous implementation delegated to HeapSort and was mislabeled.");
        }

        public static int[] Multithreaded(int[] array)
        {
            int[] result = (int[])array.Clone();
            ParallelMergeSort(result, 0, result.Length - 1);
            return result;
        }

        private static void ParallelMergeSort(int[] a, int left, int right)
        {
            if (left >= right) return;
            int mid = left + (right - left) / 2;

            if (right - left > 1000)
            {
                System.Threading.Tasks.Parallel.Invoke(
                    () => ParallelMergeSort(a, left, mid),
                    () => ParallelMergeSort(a, mid + 1, right));
            }
            else
            {
                ParallelMergeSort(a, left, mid);
                ParallelMergeSort(a, mid + 1, right);
            }

            Merge(a, left, mid, right);
        }

        private static void Merge(int[] a, int left, int mid, int right)
        {
            int[] temp = new int[right - left + 1];
            int i = left, j = mid + 1, k = 0;

            while (i <= mid && j <= right)
                temp[k++] = a[i] <= a[j] ? a[i++] : a[j++];
            while (i <= mid) temp[k++] = a[i++];
            while (j <= right) temp[k++] = a[j++];

            Array.Copy(temp, 0, a, left, temp.Length);
        }
    }
}
