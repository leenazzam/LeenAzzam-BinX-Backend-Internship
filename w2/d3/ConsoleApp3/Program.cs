using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Sequential
        Console.WriteLine("Sequential:");

        await Source1();
        await Source2();
        await Source3();


        // Concurrent
        Console.WriteLine("\nConcurrent:");

        Task t1 = Source1();
        Task t2 = Source2();
        Task t3 = Source3();

        await Task.WhenAll(t1, t2, t3);


        // Cancellation
        CancellationTokenSource cts = new CancellationTokenSource();

        Task t = LongTask(cts.Token);

        await Task.Delay(2000);

        cts.Cancel();

        Console.WriteLine("Task Cancelled");
    }

    static async Task Source1()
    {
        Console.WriteLine("Source 1 Started");
        await Task.Delay(3000);
        Console.WriteLine("Source 1 Finished");
    }

    static async Task Source2()
    {
        Console.WriteLine("Source 2 Started");
        await Task.Delay(3000);
        Console.WriteLine("Source 2 Finished");
    }

    static async Task Source3()
    {
        Console.WriteLine("Source 3 Started");
        await Task.Delay(3000);
        Console.WriteLine("Source 3 Finished");
    }

    static async Task LongTask(CancellationToken token)
    {
        Console.WriteLine("Long Task Started");

        await Task.Delay(5000, token);

        Console.WriteLine("Long Task Finished");
    }
}