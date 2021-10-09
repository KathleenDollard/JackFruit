using System;
using System.Linq;

namespace Playground
{

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            B(A);
            Index();
        }

        static void A() { }

        static void B(Action d) { }

        public static void Index()
        {
            var list = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            var fullList = Enumerable.Range(1, 100).ToArray();
            Console.WriteLine(fullList.Length);

            var smallSlice = fullList[1..3];
            Console.WriteLine($"fullList[1..3] C#: {OutputArray(smallSlice)}");
            var smallSlice2 = fullList[2..3];
            Console.WriteLine($"fullList[2..3] C#: {OutputArray(smallSlice2)}");
            var unboundedBeginning = fullList[..2];
            Console.WriteLine($"fullList[..2] C#: {OutputArray(unboundedBeginning)}");
            var unboundedEnd = fullList[98..];
            Console.WriteLine($"fullList[98..] C#: {OutputArray(unboundedEnd)}");
            var fromEnd = fullList[^2..];
            Console.WriteLine($"fullList[^2..] C#: {OutputArray(fromEnd)}");

            var x = list[^2..];

            Console.WriteLine($"list[^2..] {OutputArray(x)}");
            Console.WriteLine();

            static string OutputArray(int[] arr)
                => String.Concat(arr.Select(y => y.ToString() + ", "));

        }
    }
}
