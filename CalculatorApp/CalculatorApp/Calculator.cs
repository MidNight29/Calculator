using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CalculatorApp
{
    public class Calculator
    {
        private CalculationParser _Parser;
        private List<string> _ParsedCalculation;

        public Calculator()
        {
            _Parser = new CalculationParser();
        }

        public string InputCalculation(string calculation)
        {
            _ParsedCalculation = _Parser.Parse(calculation);

            try
            {
                Calculate();
            }
            catch (ArgumentException e)
            {
                return "Erreur*" + Environment.NewLine + e.Message;
            }

            return _ParsedCalculation.FirstOrDefault();
        }

        private void Calculate()
        {
            CalculationByPriority("(");
            CalculationByPriority("sqrt(", "*", "/", "^");
            CalculationByPriority("+", "-");
        }

        private void CalculationByPriority(params string[] priority)
        {
            var priorityList = priority.ToList();

            var item = _ParsedCalculation.FirstOrDefault(c => priorityList.Contains(c));

            //Keep going as long as you have operators within the current priority list
            while (item != null)
            {
                int index = _ParsedCalculation.IndexOf(item);

                var previousValue = _Parser.ValidateNumber(_ParsedCalculation[index - 1]);
                var nextValue = _Parser.ValidateNumber(_ParsedCalculation[index + 1]);

                switch (item)
                {
                    case "+":
                        _ParsedCalculation[index] = Add(previousValue, nextValue).ToString();
                        break;
                    case "-":
                        _ParsedCalculation[index] = Substract(previousValue, nextValue).ToString();
                        break;
                    case "*":
                        _ParsedCalculation[index] = Multiply(previousValue, nextValue).ToString();
                        break;
                    case "/":
                        _ParsedCalculation[index] = Divide(previousValue, nextValue).ToString();
                        break;
                    case "^":
                        _ParsedCalculation[index] = Exponent(previousValue, nextValue).ToString();
                        break;
                    default:
                        break;
                }

                _ParsedCalculation.RemoveAt(index + 1);
                _ParsedCalculation.RemoveAt(index - 1);

                item = _ParsedCalculation.FirstOrDefault(c => priorityList.Contains(c));
            }
        }

        /// <summary>
        /// Adds the first number to the second one
        /// </summary>
        /// <param name="firstNumber"></param>
        /// <param name="secondNumber"></param>
        /// <returns></returns>
        private double Add(double firstNumber, double secondNumber)
        {
            return firstNumber + secondNumber;
        }

        

        private double NumberValidation(string firstNumber)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Substract the second number from the first one
        /// </summary>
        /// <param name="firstNumber"></param>
        /// <param name="secondNumber"></param>
        /// <returns></returns>
        private double Substract(double firstNumber, double secondNumber)
        {
            return firstNumber - secondNumber;
        }

        /// <summary>
        /// Multiplies the first number by the second one
        /// </summary>
        /// <param name="firstNumber"></param>
        /// <param name="secondNumber"></param>
        /// <returns></returns>
        private double Multiply(double firstNumber, double secondNumber)
        {
            return firstNumber * secondNumber;
        }

        /// <summary>
        /// Divides the first number by the second one
        /// </summary>
        /// <param name="firstNumber"></param>
        /// <param name="secondNumber">This should not be 0 as division by 0 is impossible</param>
        /// <returns></returns>
        private double Divide(double firstNumber, double secondNumber)
        {
            if (secondNumber == 0)
                throw new ArgumentException("Division by zero is impossible");
            return firstNumber * secondNumber;
        }

        /// <summary>
        /// The number is raised to the power of the exponent
        /// </summary>
        /// <param name="number"></param>
        /// <param name="exponent"></param>
        /// <returns></returns>
        private double Exponent(double number, double exponent)
        {
            return Math.Pow(number, exponent);
        }

        /// <summary>
        /// Square root of the specified number
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private double Square(double number)
        {
            return Math.Sqrt(number);
        }
    }
}
