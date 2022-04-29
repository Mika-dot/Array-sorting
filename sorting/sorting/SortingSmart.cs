using System;
using System.Collections.Generic;
using System.Linq;

namespace sorting
{
    class TreeNode //простая реализация бинарного дерева
    {
        public TreeNode(int data)
        {
            Data = data;
        }
        public int Data { get; set; } //данные
        public TreeNode Left { get; set; } //левая ветка дерева
        public TreeNode Right { get; set; } //правая ветка дерева
        public void Insert(TreeNode node) //рекурсивное добавление узла в дерево
        {
            if (node.Data < Data)
            {
                if (Left == null)
                {
                    Left = node;
                }
                else
                {
                    Left.Insert(node);
                }
            }
            else
            {
                if (Right == null)
                {
                    Right = node;
                }
                else
                {
                    Right.Insert(node);
                }
            }
        }
        public int[] Transform(List<int> elements = null) //преобразование дерева в отсортированный массив
        {
            if (elements == null)
            {
                elements = new List<int>();
            }

            if (Left != null)
            {
                Left.Transform(elements);
            }

            elements.Add(Data);

            if (Right != null)
            {
                Right.Transform(elements);
            }

            return elements.ToArray();
        }
    }
    public class SortingSmart
    {
        static void Merge(int[] array, int lowIndex, int middleIndex, int highIndex) //метод для слияния массивов
        {
            var left = lowIndex;
            var right = middleIndex + 1;
            var tempArray = new int[highIndex - lowIndex + 1];
            var index = 0;

            while ((left <= middleIndex) && (right <= highIndex))
            {
                if (array[left] < array[right])
                {
                    tempArray[index] = array[left];
                    left++;
                }
                else
                {
                    tempArray[index] = array[right];
                    right++;
                }

                index++;
            }

            for (var i = left; i <= middleIndex; i++)
            {
                tempArray[index] = array[i];
                index++;
            }

            for (var i = right; i <= highIndex; i++)
            {
                tempArray[index] = array[i];
                index++;
            }

            for (var i = 0; i < tempArray.Length; i++)
            {
                array[lowIndex + i] = tempArray[i];
            }
        }
        static int[] MergeSort(int[] array, int lowIndex, int highIndex) //сортировка слиянием
        {
            if (lowIndex < highIndex)
            {
                var middleIndex = (lowIndex + highIndex) / 2;
                MergeSort(array, lowIndex, middleIndex);
                MergeSort(array, middleIndex + 1, highIndex);
                Merge(array, lowIndex, middleIndex, highIndex);
            }

            return array;
        }
        static int IndexOfMax(int[] array, int n)//метод для получения индекса максимального элемента подмассива
        {
            int result = 0;
            for (var i = 1; i <= n; ++i)
            {
                if (array[i] > array[result])
                {
                    result = i;
                }
            }

            return result;
        }
        static int IndexOfMin(int[] array, int n) //метод поиска позиции минимального элемента подмассива, начиная с позиции n
        {
            int result = n;
            for (var i = n; i < array.Length; ++i)
            {
                if (array[i] < array[result])
                {
                    result = i;
                }
            }

            return result;
        }
        static void Flip(int[] array, int end)//метод для переворота массива
        {
            for (var start = 0; start < end; start++, end--)
            {
                var temp = array[start];
                array[start] = array[end];
                array[end] = temp;
            }
        }
        static void Swap(ref int e1, ref int e2) //метод обмена элементов
        {
            var temp = e1;
            e1 = e2;
            e2 = temp;
        }
        static int Partition(int[] array, int minIndex, int maxIndex) //метод возвращающий индекс опорного элемента
        {
            var pivot = minIndex - 1;
            for (var i = minIndex; i < maxIndex; i++)
            {
                if (array[i] < array[maxIndex])
                {
                    pivot++;
                    Swap(ref array[pivot], ref array[i]);
                }
            }

            pivot++;
            Swap(ref array[pivot], ref array[maxIndex]);
            return pivot;
        }
        static int[] QuickSort(int[] array, int minIndex, int maxIndex) //быстрая сортировка
        {
            if (minIndex >= maxIndex)
            {
                return array;
            }

            var pivotIndex = Partition(array, minIndex, maxIndex);
            QuickSort(array, minIndex, pivotIndex - 1);
            QuickSort(array, pivotIndex + 1, maxIndex);

            return array;
        }
        static int GetNextStep(int s) //метод для генерации следующего шага
        {
            s = s * 1000 / 1247;
            return s > 1 ? s : 1;
        }
        static void Heapify(int[] arr, int heapSize, int heapRoot)
        {
            int max = heapRoot;

            int leftChildIndex = 2 * heapRoot + 1;
            int rightChildIndex = 2 * heapRoot + 2;


            if (leftChildIndex < heapSize && arr[leftChildIndex] > arr[max])
                max = leftChildIndex;


            if (rightChildIndex < heapSize && arr[rightChildIndex] > arr[max])
                max = rightChildIndex;


            if (max != heapRoot)
            {
                int swap = arr[heapRoot];
                arr[heapRoot] = arr[max];
                arr[max] = swap;

                Heapify(arr, heapSize, max);
            }
        }
        private static void SwapBogo(ref int[] arr, int u, int v)
        {
            int temp = arr[u];
            arr[u] = arr[v];
            arr[v] = temp;
        }
        private static bool sorted(int[] arr)
        {
            bool ret = true;
            for (int i = 0; i < arr.Length - 1; i++)
            {
                if (arr[i] > arr[i + 1])
                {
                    ret = false;
                    break;
                }
            }
            return ret;
        }
        private static void insertionSort(int[] arr, int left, int right)
        {
            for (int i = left + 1; i <= right; i++)
            {
                int temp = arr[i];
                int j = i - 1;

                while (j >= left && arr[j] > temp)
                {
                    arr[j + 1] = arr[j];
                    j--;
                }
                arr[j + 1] = temp;
            }
        }
        private static void merge(int[] arr, int l, int m, int r)
        {
            // original array is broken in two parts  
            // left and right array  
            int len1 = m - l + 1, len2 = r - m;
            int[] left = new int[len1];
            int[] right = new int[len2];

            for (int x = 0; x < len1; x++)
                left[x] = arr[l + x];

            for (int x = 0; x < len2; x++)
                right[x] = arr[m + 1 + x];

            int i = 0;
            int j = 0;
            int k = l;

            // after comparing, we merge those two array  
            // in larger sub array  
            while (i < len1 && j < len2)
            {
                if (left[i] <= right[j])
                {
                    arr[k] = left[i];
                    i++;
                }
                else
                {
                    arr[k] = right[j];
                    j++;
                }
                k++;
            }

            // copy remaining elements of left, if any  
            while (i < len1)
            {
                arr[k] = left[i];
                k++;
                i++;
            }

            // copy remaining element of right, if any  
            while (j < len2)
            {
                arr[k] = right[j];
                k++;
                j++;
            }
        }
        private static T[] InitializeArray<T>(int length) where T : new()
        {
            T[] array = new T[length];
            for (int i = 0; i < length; ++i)
            {
                array[i] = new T();
            }

            return array;
        }
        private static int[] MSDRadix(int[] array, int l, int r, int d, int[] temp)
        {
            if (l >= r)
            {
                return new int[0];
            }

            const int k = 256;

            var count = new int[k + 2];
            for (var i = l; i <= r; i++)
            {
                var j = Key(array[i]);
                count[j + 2]++;
            }

            for (var i = 1; i < count.Length; i++)
            {
                count[i] += count[i - 1];
            }

            for (var i = l; i <= r; i++)
            {
                var j = Key(array[i]);
                temp[count[j + 1]++] = array[i];
            }

            for (var i = l; i <= r; i++)
            {
                array[i] = temp[i - l];
            }

            for (var i = 0; i < k; i++)
            {
                MSDRadix(array, l + count[i], l + count[i + 1] - 1, d + 1, temp);
            }

            int Key(int s) => d >= s ? -1 : s;

            return array;
        }


        // -----------------------------------------------------------------

        public static void Bubble(ref int[] array) //сортировка пузырьком
        {
            var len = array.Length;
            for (var i = 1; i < len; i++)
            {
                for (var j = 0; j < len - i; j++)
                {
                    if (array[j] > array[j + 1])
                    {
                        Swap(ref array[j], ref array[j + 1]);
                    }
                }
            }

        }
        public static void Shaker(ref int[] array) //сортировка перемешиванием
        {
            for (var i = 0; i < array.Length / 2; i++)
            {
                var swapFlag = false;
                //проход слева направо
                for (var j = i; j < array.Length - i - 1; j++)
                {
                    if (array[j] > array[j + 1])
                    {
                        Swap(ref array[j], ref array[j + 1]);
                        swapFlag = true;
                    }
                }

                //проход справа налево
                for (var j = array.Length - 2 - i; j > i; j--)
                {
                    if (array[j - 1] > array[j])
                    {
                        Swap(ref array[j - 1], ref array[j]);
                        swapFlag = true;
                    }
                }

                //если обменов не было выходим
                if (!swapFlag)
                {
                    break;
                }
            }

        }
        public static void Insertion(ref int[] array) //сортировка вставками
        {
            int n = array.Length;
            for (int i = 1; i < n; ++i)
            {
                int key = array[i];
                int j = i - 1;

                // Move elements of arr[0..i-1],
                // that are greater than key,
                // to one position ahead of
                // their current position
                while (j >= 0 && array[j] > key)
                {
                    array[j + 1] = array[j];
                    j = j - 1;
                }
                array[j + 1] = key;
            }
        }
        public static void Stooge(ref int[] array, int startIndex, int endIndex) //сортировка по частям
        {
            if (array[startIndex] > array[endIndex])
            {
                Swap(ref array[startIndex], ref array[endIndex]);
            }

            if (endIndex - startIndex > 1)
            {
                var len = (endIndex - startIndex + 1) / 3;
                Stooge(ref array, startIndex, endIndex - len);
                Stooge(ref array, startIndex + len, endIndex);
                Stooge(ref array, startIndex, endIndex - len);
            }

        }
        public static void Stooge(ref int[] array) //сортировка по частям
        {
            Stooge(ref array, 0, array.Length - 1);
        }
        public static void Pancake(ref int[] array) //блинная сортировка
        {
            for (var subArrayLength = array.Length - 1; subArrayLength >= 0; subArrayLength--)
            {
                //получаем позицию максимального элемента подмассива
                var indexOfMax = IndexOfMax(array, subArrayLength);
                if (indexOfMax != subArrayLength)
                {
                    //переворот массива до индекса максимального элемента
                    //максимальный элемент подмассива расположится вначале
                    Flip(array, indexOfMax);
                    //переворот всего подмассива
                    Flip(array, subArrayLength);
                }
            }

        }
        public static void Shell(ref int[] array) //сортировки Шелла
        {
            //расстояние между элементами, которые сравниваются
            var d = array.Length / 2;
            while (d >= 1)
            {
                for (var i = d; i < array.Length; i++)
                {
                    var j = i;
                    while ((j >= d) && (array[j - d] > array[j]))
                    {
                        Swap(ref array[j], ref array[j - d]);
                        j = j - d;
                    }
                }

                d = d / 2;
            }

        }
        public static void Merge(ref int[] array) //сортировка слиянием
        {
            MergeSort(array, 0, array.Length - 1);
        }
        public static void Selection(ref int[] array, int currentIndex = 0) //сортировка выбором
        {
            if (currentIndex == array.Length)
                return;

            var index = IndexOfMin(array, currentIndex);
            if (index != currentIndex)
            {
                Swap(ref array[index], ref array[currentIndex]);
            }

             Selection(ref array, currentIndex + 1);
        }
        public static void Quick(ref int[] array) //сортировка Хоара
        {
            QuickSort(array, 0, array.Length - 1);
        }
        public static void Gnome(ref int[] unsortedArray) //Гномья сортировка
        {
            var index = 1;
            var nextIndex = index + 1;

            while (index < unsortedArray.Length)
            {
                if (unsortedArray[index - 1] < unsortedArray[index])
                {
                    index = nextIndex;
                    nextIndex++;
                }
                else
                {
                    Swap(ref unsortedArray[index - 1], ref unsortedArray[index]);
                    index--;
                    if (index == 0)
                    {
                        index = nextIndex;
                        nextIndex++;
                    }
                }
            }
        }
        public static void Tree(ref int[] array) //метод для сортировки с помощью двоичного дерева
        {
            var treeNode = new TreeNode(array[0]);
            for (int i = 1; i < array.Length; i++)
            {
                treeNode.Insert(new TreeNode(array[i]));
            }

            array = treeNode.Transform();
        }
        public static void Comb(ref int[] array) //Сортировка расческой
        {
            var arrayLength = array.Length;
            var currentStep = arrayLength - 1;

            while (currentStep > 1)
            {
                for (int i = 0; i + currentStep < array.Length; i++)
                {
                    if (array[i] > array[i + currentStep])
                    {
                        Swap(ref array[i], ref array[i + currentStep]);
                    }
                }

                currentStep = GetNextStep(currentStep);
            }

            //сортировка пузырьком
            for (var i = 1; i < arrayLength; i++)
            {
                var swapFlag = false;
                for (var j = 0; j < arrayLength - i; j++)
                {
                    if (array[j] > array[j + 1])
                    {
                        Swap(ref array[j], ref array[j + 1]);
                        swapFlag = true;
                    }
                }

                if (!swapFlag)
                {
                    break;
                }
            }
        }
        public static void BasicCounting(ref int[] array) //простой вариант сортировки подсчетом
        {
            int n = array.Length;
            int max = 0;
            for (int i = 0; i < n; i++)
            {
                if (max < array[i])
                {
                    max = array[i];
                }
            }

            int[] freq = new int[max + 1];
            for (int i = 0; i < max + 1; i++)
            {
                freq[i] = 0;
            }
            for (int i = 0; i < n; i++)
            {
                freq[array[i]]++;
            }

            for (int i = 0, j = 0; i <= max; i++)
            {
                while (freq[i] > 0)
                {
                    array[j] = i;
                    j++;
                    freq[i]--;
                }
            }
        }
        public static void CombinedBubble(ref int[] array)
        {
            int length = array.Length;

            int temp = array[0];

            for (int i = 0; i < length; i++)
            {
                for (int j = i + 1; j < length; j++)
                {
                    if (array[i] > array[j])
                    {
                        temp = array[i];

                        array[i] = array[j];

                        array[j] = temp;
                    }
                }
            }
        }
        public static void Heapify(ref int[] array) // Heapify
        {
            for (int i = array.Length / 2 - 1; i >= 0; i--)
                Heapify(array, array.Length, i);

            for (int i = array.Length - 1; i >= 0; i--)
            {
                int temp = array[0];
                array[0] = array[i];
                array[i] = temp;
                Heapify(array, i, 0);
            }
        }
        public static void Cocktail(ref int[] array) // cocktail
        {
            int start = 0;
            int end = array.Length - 1;
            int temp;
            bool swapped = true;

            while (swapped)
            {
                swapped = false;

                for (int i = 0; i < end; i++)
                {
                    if (array[i] > array[i + 1])
                    {
                        temp = array[i];
                        array[i] = array[i + 1];
                        array[i + 1] = temp;
                        swapped = true;
                    }
                }

                if (!swapped)
                {
                    break;
                }

                swapped = false;
                end -= 1;

                for (int i = end; i >= start; i--)
                {
                    if (array[i] > array[i + 1])
                    {
                        temp = array[i];
                        array[i] = array[i + 1];
                        array[i + 1] = temp;
                        swapped = true;
                    }
                }

                start += 1;
            }
        }
        public static void OddEven(ref int[] array)
        {
            //Initialization
            int Flag = 0;
            int temp = 0;

            //Initialize flag =0 or f!=1
            while (Flag == 0)
            {

                /*Initialize Flag is 1
                When both if condiotion is false so the flag remain 1 
                and the while loop is terminate*/

                Flag = 1;

                //use Even Loop for comparing even idexes of an array 

                for (int i = 0; i < array.Length - 1; i += 2)
                {
                    /* Use if conditon for comparing adjacents elements
                   
                     if they are in wrong order than swap*/

                    if (array[i] > array[i + 1])
                    {

                        temp = array[i];
                        array[i] = array[i + 1];
                        array[i + 1] = temp;
                        //This Flag variable is always remain 0 when if condition is true
                        Flag = 0;
                    }
                }


                //use Odd Loop for comparing odd idexes of an array 
                for (int i = 1; i < array.Length - 1; i += 2)
                {

                    /* Use if conditon for comparing adjacents elements
                        if they are in wrong order than swap*/
                    if (array[i] > array[i + 1])
                    {
                        //This Flag variable is always remain 0 when if condition is true
                        temp = array[i];
                        array[i] = array[i + 1];
                        array[i + 1] = temp;
                        Flag = 0;
                    }
                }

            }
            //return sorted array

        } // OddEven
        public static void BinaryInsertion(ref int[] array)
        {
            int count = 0;
            for (int i = 0; i < array.Length; i++)
            {
                int tmp = array[i]; int left = 0; int right = i - 1;
                while (left <= right)
                {
                    int m = (left + right) / 2; //определение индекса среднего элемента
                    if (tmp < array[m])
                        right = m - 1; // сдвиг правой
                    else left = m + 1; //или левой границы
                    count++;
                }

                for (int j = i - 1; j >= left; j--)
                {
                    array[j + 1] = array[j]; // сдвиг элементов
                                         // count++;
                }

                array[left] = tmp; // вставка элемента на нужное место
            }
        } // BinaryInsertion
        public static void Bogo(ref int[] array)
        {
            while (!sorted(array))
            {
                Random rdm = new Random();
                for (int i = 0; i < array.Length; i++)
                {
                    {

                        for (int u = 0; u < array.Length; u++)
                        {
                            SwapBogo(ref array, u, rdm.Next(0, array.Length - 1));
                        }
                    }
                }

            }

        } // Bogo
        public static void Cycle(ref int[] array)
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                int item = array[i];
                int pos = i;
                for (int j = i + 1; j < array.Length; j++)
                {
                    if (array[j] < item)
                    {
                        pos++;
                    }
                }
                if (pos == i)
                {
                    continue;
                }
                while (item == array[pos])
                {
                    pos++;
                }
                int var = item;
                item = array[pos];
                array[pos] = var;
                while (pos != i)
                {
                    pos = i;
                    for (int j = i + 1; j < array.Length; j++)
                    {
                        if (array[j] < item)
                        {
                            pos++;
                        }
                    }
                    while (item == array[pos])
                    {
                        pos++;
                    }
                    int temp = item;
                    item = array[pos];
                    array[pos] = temp;
                }
            }

        } // Cycle
        public static void Exchange(ref int[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                for (int j = i; j < array.Length; j++)
                {
                    if (array[j] < array[i])
                    {
                        int container = array[j];
                        array[j] = array[i];
                        array[i] = container;
                    }
                }
            }

        } // Exchange
        public static void Heap(ref int[] array)
        {
            int n = array.Length;


            for (int i = n / 2 - 1; i >= 0; i--)
                Heapify(array, n, i);

            for (int i = n - 1; i > 0; i--)
            {

                int temp = array[0];
                array[0] = array[i];
                array[i] = temp;

                Heapify(array, i, 0);
            }

        } // Heap
        public static void Tim(ref int[] array)
        {
            int RUN = 32;

            for (int i = 0; i < array.Length; i += RUN)
                insertionSort(array, i, Math.Min((i + 31), (array.Length - 1)));

            for (int size = RUN; size < array.Length; size = 2 * size)
            {
                for (int left = 0; left < array.Length; left += 2 * size)
                {
                    int mid = left + size - 1;
                    int right = Math.Min((left + 2 * size - 1), (array.Length - 1));

                    merge(array, left, mid, right);
                }
            }

        } // Tim
        public static void Counting(ref int[] array)
        {
            int min = 0;
            int max = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] < min)
                    min = array[i];
                if (array[i] > max)
                    max = array[i];
            }

            int[] count = new int[max - min + 1];
            int z = 0;

            for (int i = 0; i < count.Length; i++)
                count[i] = 0;

            for (int i = 0; i < array.Length; i++)
                count[array[i] - min]++;

            for (int i = min; i <= max; i++)
            {
                while (count[i - min]-- > 0)
                {
                    array[z] = i;
                    ++z;
                }
            }

        } // Counting
        public static void Bucket(ref int[] array)
        {
            int minValue = array[0];
            int maxValue = array[0];

            for (int i = 1; i < array.Length; i++)
            {
                if (array[i] > maxValue)
                    maxValue = array[i];
                if (array[i] < minValue)
                    minValue = array[i];
            }

            List<int>[] bucket = new List<int>[maxValue - minValue + 1];

            for (int i = 0; i < bucket.Length; i++)
            {
                bucket[i] = new List<int>();
            }

            for (int i = 0; i < array.Length; i++)
            {
                bucket[array[i] - minValue].Add(array[i]);
            }

            int k = 0;
            for (int i = 0; i < bucket.Length; i++)
            {
                if (bucket[i].Count > 0)
                {
                    for (int j = 0; j < bucket[i].Count; j++)
                    {
                        array[k] = bucket[i][j];
                        k++;
                    }
                }
            }

        } // Bucket
        public static void Radix(ref int[] array)
        {
            var numbers = new Queue<int>(array);
            var buckets = InitializeArray<Queue<int>>(10);
            int m = 10;
            int n = 1;
            for (int i = numbers.Max().ToString().Length; i > 0; i--)
            {
                while (numbers.Count > 0)
                {
                    buckets[numbers.Peek() % m / n].Enqueue(numbers.Dequeue());
                }
                foreach (Queue<int> bucket in buckets)
                {
                    while (bucket.Count > 0)
                    {
                        numbers.Enqueue(bucket.Dequeue());
                    }
                }
                m *= 10;
                n *= 10;
            }
            array = numbers.ToArray();
        } // Radix
        public static void MSDRadix(ref int[] array) => MSDRadix(array, 0, array.Length - 1, 0, new int[array.Length]); // MSDRadix

    }

}
