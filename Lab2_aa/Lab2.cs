using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

class SortingAnalysis
{
    static void Main()
    {
        var analysis = new SortingAnalysis();
        analysis.RunAnalysis();
    }

    public void RunAnalysis()
    {
        var sizes = new[] { 100, 1000, 5000, 10000 };
        var results = new List<TestResult>();

        foreach (var size in sizes)
        {
            foreach (ArrayType type in Enum.GetValues(typeof(ArrayType)))
            {
                var array = GenerateArray(size, type);

                results.Add(TestAlgorithm("QuickSort", (int[])array.Clone(), type, size));
                results.Add(TestAlgorithm("MergeSort", (int[])array.Clone(), type, size));
                results.Add(TestAlgorithm("HeapSort", (int[])array.Clone(), type, size));
                results.Add(TestAlgorithm("GnomeSort", (int[])array.Clone(), type, size));
            }
        }

        ExportResults(results);
    }

    #region Sorting Algorithms
    // QuickSort (O(n log n) average, O(n²) worst case)
    private void QuickSort(int[] arr, int left, int right)
    {
        if (left < right)
        {
            int pivot = Partition(arr, left, right);
            QuickSort(arr, left, pivot - 1);
            QuickSort(arr, pivot + 1, right);
        }
    }

    private int Partition(int[] arr, int left, int right)
    {
        int pivot = arr[right];
        int i = left - 1;
        for (int j = left; j < right; j++)
            if (arr[j] <= pivot) Swap(arr, ++i, j);
        Swap(arr, i + 1, right);
        return i + 1;
    }

    // MergeSort (O(n log n) stable)
    private void MergeSort(int[] arr, int left, int right)
    {
        if (left >= right) return;
        int mid = (left + right) / 2;
        MergeSort(arr, left, mid);
        MergeSort(arr, mid + 1, right);
        Merge(arr, left, mid, right);
    }

    private void Merge(int[] arr, int left, int mid, int right)
    {
        int[] temp = new int[right - left + 1];
        int i = left, j = mid + 1, k = 0;

        while (i <= mid && j <= right)
            temp[k++] = arr[i] <= arr[j] ? arr[i++] : arr[j++];
        while (i <= mid) temp[k++] = arr[i++];
        while (j <= right) temp[k++] = arr[j++];

        Array.Copy(temp, 0, arr, left, temp.Length);
    }

    // HeapSort (O(n log n) in-place)
    private void HeapSort(int[] arr)
    {
        int n = arr.Length;
        for (int i = n / 2 - 1; i >= 0; i--)
            Heapify(arr, n, i);
        for (int i = n - 1; i > 0; i--)
        {
            Swap(arr, 0, i);
            Heapify(arr, i, 0);
        }
    }

    private void Heapify(int[] arr, int n, int i)
    {
        int largest = i, left = 2 * i + 1, right = 2 * i + 2;
        if (left < n && arr[left] > arr[largest]) largest = left;
        if (right < n && arr[right] > arr[largest]) largest = right;
        if (largest != i)
        {
            Swap(arr, i, largest);
            Heapify(arr, n, largest);
        }
    }

    // Gnome Sort (O(n²) exotic algorithm)
    private void GnomeSort(int[] arr)
    {
        int pos = 0;
        while (pos < arr.Length)
        {
            if (pos == 0 || arr[pos] >= arr[pos - 1])
                pos++;
            else
                Swap(arr, pos, --pos);
        }
    }

    private void Swap(int[] arr, int i, int j) => (arr[i], arr[j]) = (arr[j], arr[i]);
    #endregion

    #region Data Generation
    private enum ArrayType { Random, Sorted, ReverseSorted, NearlySorted }

    private int[] GenerateArray(int size, ArrayType type)
    {
        var rng = new Random();
        return type switch
        {
            ArrayType.Random => Enumerable.Range(0, size)
                     .Select(_ => rng.Next(-size * 2, size * 2)).ToArray(),
            ArrayType.Sorted => Enumerable.Range(0, size).ToArray(),
            ArrayType.ReverseSorted => Enumerable.Range(0, size).Reverse().ToArray(),
            ArrayType.NearlySorted => NearlySortedArray(size, rng),
            _ => throw new ArgumentException("Invalid array type")
        };
    }

    private int[] NearlySortedArray(int size, Random rng)
    {
        var arr = Enumerable.Range(0, size).ToArray();
        for (int i = 0; i < size / 10; i++)
            Swap(arr, rng.Next(size), rng.Next(size));
        return arr;
    }
    #endregion

    #region Testing Infrastructure
    private TestResult TestAlgorithm(string name, int[] data, ArrayType type, int size)
    {
        var sw = Stopwatch.StartNew();
        switch (name)
        {
            case "QuickSort": QuickSort(data, 0, data.Length - 1); break;
            case "MergeSort": MergeSort(data, 0, data.Length - 1); break;
            case "HeapSort": HeapSort(data); break;
            case "GnomeSort": GnomeSort(data); break;
        }
        sw.Stop();
        return new TestResult(name, type.ToString(), size, sw.Elapsed.TotalMilliseconds);
    }

    private record TestResult(string Algorithm, string ArrayType, int Size, double TimeMs);

    private void ExportResults(List<TestResult> results)
    {
        Console.WriteLine("Algorithm,ArrayType,Size,TimeMs");
        foreach (var r in results)
            Console.WriteLine($"{r.Algorithm},{r.ArrayType},{r.Size},{r.TimeMs}");
    }
    #endregion
}