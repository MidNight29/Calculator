using CalculatorLib;
using System;

namespace CalculatorConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Calculator calc = new Calculator();

            while (1 == 1)
            {
                var result = calc.CalculateResult(Console.ReadLine());
                Console.WriteLine(result);
                Console.WriteLine();
            }
        }
    }
}
