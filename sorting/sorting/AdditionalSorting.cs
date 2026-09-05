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
            List<List<int>> piles = new List<List<int>>();
            foreach (int value in array)
            {
                int index = piles.FindIndex(p => p[p.Count - 1] >= value);
                if (index < 0)
                    piles.Add(new List<int> { value });
                else
                    piles[index].Add(value);
            }

            List<int> result = new List<int>();
            while (piles.Count > 0)
            {
                int best = 0;
                for (int i = 1; i < piles.Count; i++)
                    if (piles[i][piles[i].Count - 1] < piles[best][piles[best].Count - 1])
                        best = i;

                result.Add(piles[best][piles[best].Count - 1]);
                piles[best].RemoveAt(piles[best].Count - 1);
                if (piles[best].Count == 0) piles.RemoveAt(best);
            }
            return result.ToArray();
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
