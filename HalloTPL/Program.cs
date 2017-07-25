using System;
using System.Threading;
using System.Threading.Tasks;

namespace HalloTPL
{
    class Program
    {
        static void Main(string[] args)
        {
            HalloAsyncAwait();


            Console.WriteLine("Main ready....");
            Console.ReadKey();
        }

        private static void HalloParallel()
        {
            Parallel.For(0, 10000, i =>
            {
                var x = Math.Round(Math.PI * Math.PI);
                Console.WriteLine(i);
            });
            //Parallel.ForEach();
        }

        private async static void HalloAsyncAwait()
        {
            Console.WriteLine("Vor der Berechnung...");
            var result = await BerechneEtwasSchweres();
            Console.WriteLine($"Das Ergebnis: {result}");
        }

        private static Task<int> BerechneEtwasSchweres()
        {
            return Task.Run(() =>
            {
                Thread.Sleep(2000);
                Console.WriteLine("2s Schlaf beendet. :D");
                return 5;
            });
        }
    }
}