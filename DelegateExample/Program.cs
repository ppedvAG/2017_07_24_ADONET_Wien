using System;

namespace DelegateExample
{
    public delegate string MyDelegate(int zahl, double wert);

    class Program
    {
        static void Main(string[] args)
        {
            MyDelegate del = new MyDelegate(MeineMethode);

            string result = del.Invoke(5, 90.8);

            Console.WriteLine(result);
            Console.ReadKey();
        }

        private static string MeineMethode(int i, double d)
        {
            return (i + d).ToString();
        }
    }
}