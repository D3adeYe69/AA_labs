using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ScottPlot;

class FibonacciAlgorithms
{
    const int maxN = 16000; // Upper limit for Fibonacci number calculation

    static void Main()
    {
        // Generate only 40 evenly spaced values between 1 and maxN
        List<int> ns = Enumerable.Range(0, 40).Select(i => 1 + i * (maxN - 1) / 39).ToList();
        List<double> timesMemoization = new List<double>();
        List<double> timesModulo = new List<double>();
        List<double> timesContinuedFraction = new List<double>();

        // Measure execution times for different Fibonacci algorithms
        foreach (int n in ns)
        {
            timesMemoization.Add(MeasureTime(FibonacciMemoization, n));
            timesModulo.Add(MeasureTime(FibonacciModulo, n));
            timesContinuedFraction.Add(MeasureTime(FibonacciContinuedFraction, n));
        }

        // Generate separate graphs for each Fibonacci method
        GenerateGraph(ns.ToArray(), timesMemoization, "Memoization Fibonacci Time", "Memoization_Fibonacci_Time.png");
        GenerateGraph(ns.ToArray(), timesModulo, "Modulo Fibonacci Time", "Modulo_Fibonacci_Time.png");
        GenerateGraph(ns.ToArray(), timesContinuedFraction, "Continued Fraction Fibonacci Time", "ContinuedFraction_Fibonacci_Time.png");
    }

    // Measure execution time
    static double MeasureTime(Func<int, long> func, int n)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        func(n);
        stopwatch.Stop();
        return stopwatch.Elapsed.TotalMilliseconds; // Return time in milliseconds
    }

    // 1. Memoization Fibonacci (O(n)) - Uses recursion and stores results to avoid recomputation
    static Dictionary<int, long> memo = new Dictionary<int, long>(); // Cache to store computed Fibonacci numbers
    static long FibonacciMemoization(int n)
    {
        if (n <= 1) return n;
        if (memo.ContainsKey(n)) return memo[n]; // If the value is already computed, return it

        // Compute and store in the cache
        memo[n] = FibonacciMemoization(n - 1) + FibonacciMemoization(n - 2);
        return memo[n];
    }

    // 2. Modulo Fibonacci (O(log n)) - Uses matrix exponentiation with modular arithmetic
    static long FibonacciModulo(int n)
    {
        const long MOD = 1000000007; // Prevents overflow for large numbers
        if (n == 0) return 0;
        long[,] F = { { 1, 1 }, { 1, 0 } };
        PowerMod(F, n - 1, MOD);
        return F[0, 0] % MOD;
    }

    static void PowerMod(long[,] F, int n, long mod)
    {
        if (n <= 1) return;
        long[,] M = { { 1, 1 }, { 1, 0 } };
        PowerMod(F, n / 2, mod);
        MultiplyMod(F, F, mod);
        if (n % 2 != 0) MultiplyMod(F, M, mod);
    }

    static void MultiplyMod(long[,] F, long[,] M, long mod)
    {
        long x = (F[0, 0] * M[0, 0] + F[0, 1] * M[1, 0]) % mod;
        long y = (F[0, 0] * M[0, 1] + F[0, 1] * M[1, 1]) % mod;
        long z = (F[1, 0] * M[0, 0] + F[1, 1] * M[1, 0]) % mod;
        long w = (F[1, 0] * M[0, 1] + F[1, 1] * M[1, 1]) % mod;

        F[0, 0] = x;
        F[0, 1] = y;
        F[1, 0] = z;
        F[1, 1] = w;
    }

    // 3. Continued Fraction Fibonacci (O(n)) - Uses continued fractions to approximate Fibonacci numbers
    static long FibonacciContinuedFraction(int n)
    {
        if (n <= 1) return n;
        double result = 0;
        for (int i = n; i >= 0; i--)
        {
            result = 1 / (2 + result);
        }
        return (long)(result + 2); // Return as a long integer after approximation
    }

    // Generate the graph with given times
    static void GenerateGraph(int[] ns, List<double> times, string title, string filename)
    {
        var plt = new ScottPlot.Plot();
        plt.Add.Scatter(ns.Select(x => (int)x).ToArray(), times.ToArray());
        plt.Title(title);
        plt.XLabel("n (Fibonacci Term)");
        plt.YLabel("Execution Time (ms)");

        // Set the axis limits to make the graph more visible
        plt.Axes.AutoScale();

        // Save the plot
        string path = Path.Combine(Directory.GetCurrentDirectory(), filename);
        plt.SavePng(path, 800, 600);
        Console.WriteLine($"Plot saved as {filename}");
    }
}
