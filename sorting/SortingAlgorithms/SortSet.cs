using System.Collections.Generic;

namespace SortingAlgorithms
{
    public class TreeNode //простая реализация бинарного дерева
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
    class SortSet
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


        // -----------------------------------------------------------------

        public static int[] Bubble(int[] array) //сортировка пузырьком
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

            return array;
        }
        public static int[] Shaker(int[] array) //сортировка перемешиванием
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

            return array;
        }
        public static int[] Insertion(int[] array) //сортировка вставками
        {
            for (var i = 1; i < array.Length; i++)
            {
                var key = array[i];
                var j = i;
                while ((j > 1) && (array[j - 1] > key))
                {
                    Swap(ref array[j - 1], ref array[j]);
                    j--;
                }

                array[j] = key;
            }

            return array;
        }
        public static int[] Stooge(int[] array, int startIndex, int endIndex) //сортировка по частям
        {
            if (array[startIndex] > array[endIndex])
            {
                Swap(ref array[startIndex], ref array[endIndex]);
            }

            if (endIndex - startIndex > 1)
            {
                var len = (endIndex - startIndex + 1) / 3;
                Stooge(array, startIndex, endIndex - len);
                Stooge(array, startIndex + len, endIndex);
                Stooge(array, startIndex, endIndex - len);
            }

            return array;
        }
        public static int[] Stooge(int[] array) //сортировка по частям
        {
            return Stooge(array, 0, array.Length - 1);
        }
        public static int[] Pancake(int[] array) //блинная сортировка
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

            return array;
        }
        public static int[] Shell(int[] array) //сортировки Шелла
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

            return array;
        }
        public static int[] Merge(int[] array) //сортировка слиянием
        {
            return MergeSort(array, 0, array.Length - 1);
        }
        public static int[] Selection(int[] array, int currentIndex = 0) //сортировка выбором
        {
            if (currentIndex == array.Length)
                return array;

            var index = IndexOfMin(array, currentIndex);
            if (index != currentIndex)
            {
                Swap(ref array[index], ref array[currentIndex]);
            }

            return Selection(array, currentIndex + 1);
        }
        public static int[] Quick(int[] array) //сортировка Хоара
        {
            return QuickSort(array, 0, array.Length - 1);
        }
        public static int[] Gnome(int[] unsortedArray) //Гномья сортировка
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

            return unsortedArray;
        }
        public static int[] Tree(int[] array) //метод для сортировки с помощью двоичного дерева
        {
            var treeNode = new TreeNode(array[0]);
            for (int i = 1; i < array.Length; i++)
            {
                treeNode.Insert(new TreeNode(array[i]));
            }

            return treeNode.Transform();
        }
        public static int[] Comb(int[] array) //Сортировка расческой
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

            return array;
        }
        public static int[] BasicCounting(int[] array, int k) //простой вариант сортировки подсчетом
        {
            var count = new int[k + 1];
            for (var i = 0; i < array.Length; i++)
            {
                count[array[i]]++;
            }

            var index = 0;
            for (var i = 0; i < count.Length; i++)
            {
                for (var j = 0; j < count[i]; j++)
                {
                    array[index] = i;
                    index++;
                }
            }

            return array;
        }
        public static int[] CombinedBubble(int[] array)
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

            return array;
        }
        public static int[] Heapify(int[] array) // Heapify
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
            return array;
        }
        public static int[] Cocktail(int[] array) // cocktail
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
            return array;
        }
        public static int[] OddEven(int[] array)
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

            return array;

        }


    }
}
