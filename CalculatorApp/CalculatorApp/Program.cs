using System;

namespace CalculatorApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Calculator calc = new Calculator();

            while (1 == 1)
            {
                var result = calc.InputCalculation(Console.ReadLine());
                Console.WriteLine(result);
                Console.WriteLine();
            }
        }
    }
}
