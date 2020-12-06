using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CalculatorLib
{
    public class Calculator
    {
        private CalculationParser _Parser;
        private List<string> _ParsedCalculation;

        public Calculator()
        {
            _Parser = new CalculationParser();
        }


        public string CalculateResult(List<string> parsedCalculation)
        {
            //Save the parsed calculation
            _ParsedCalculation = parsedCalculation;

            try
            {
                //Do the actual calculation
                ExecuteCalculation();
            }
            catch (CalculationException e)
            {
                //In case of an "expected" error, return "Erreur*"
                return "Erreur*";
            }

            //Return the final result
            return _ParsedCalculation.FirstOrDefault();
        }

        public string CalculateResult(string calculation)
        {
            //Parse the calculation
            var parsedCalculation = _Parser.Parse(calculation);

            return CalculateResult(parsedCalculation);
        }

        private void ExecuteCalculation()
        {
            //Run the calculation for each priority in the right order
            CalculationByPriority("(");
            CalculationByPriority("^", "sqrt");
            CalculationByPriority("*", "/");
            CalculationByPriority("+", "-");
        }

        /// <summary>
        /// This function is doing the calculation based on specified operators, left to right
        /// </summary>
        /// <param name="priority">List of operators for this priority</param>
        private void CalculationByPriority(params string[] priority)
        {
            var priorityList = priority.ToList();

            var item = _ParsedCalculation.FirstOrDefault(c => priorityList.Contains(c));

            //Keep going as long as you have operators within the current priority list
            while (item != null)
            {
                int index = _ParsedCalculation.IndexOf(item);

                decimal nextValue = 0;
                decimal previousValue = 0;

                if (item != "(")
                {
                    nextValue = _Parser.ValidateNumber(_ParsedCalculation[index + 1]);
                    if (item != "sqrt")
                    {
                        previousValue = _Parser.ValidateNumber(_ParsedCalculation[index - 1]);
                    }
                }

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
                    case "sqrt":
                        _ParsedCalculation[index] = Square(nextValue).ToString();
                        break;
                    case "(":
                        HandleParenthesis(index);
                        break;
                    default:
                        break;
                }

                if (item != "(")
                {
                    _ParsedCalculation.RemoveAt(index + 1);

                    if (item != "sqrt")
                    {
                        _ParsedCalculation.RemoveAt(index - 1);
                    }
                }

                //Get the next operator for this priority
                item = _ParsedCalculation.FirstOrDefault(c => priorityList.Contains(c));
            }
        }

        private void HandleParenthesis(int index)
        {
            int openingParenthesisFound = 0;
            bool foundClosingIndex = false;
            int closingIndex = index + 1;

            //Get the next closing parenthesis, if there's an opening parenthesis, make sure you get it's closing too
            while (!foundClosingIndex)
            {
                if (_ParsedCalculation[closingIndex] == "(")
                {
                    openingParenthesisFound++;
                }
                else if (_ParsedCalculation[closingIndex] == ")")
                {
                    if (openingParenthesisFound > 0)
                    {
                        openingParenthesisFound--;
                    }
                    else
                    {
                        foundClosingIndex = true;
                    }
                }

                if (!foundClosingIndex)
                {
                    closingIndex++;
                }
            }

            //Then get everything in between the parenthesis, create a new calculator to calculate the result and put it back in the calculation
            _ParsedCalculation[index] = new Calculator().CalculateResult(_ParsedCalculation.GetRange(index + 1, closingIndex - index - 1));

            //Remove what has been calculated
            _ParsedCalculation.RemoveRange(index + 1, closingIndex - index);

            //Make sure the negative signs and the parenthesis multiplications are updated
            _ParsedCalculation = _Parser.ParseNegativesAndParathesis(_ParsedCalculation);
        }

        /// <summary>
        /// Adds the first number to the second one
        /// </summary>
        /// <param name="firstNumber"></param>
        /// <param name="secondNumber"></param>
        /// <returns></returns>
        private decimal Add(decimal firstNumber, decimal secondNumber)
        {
            return firstNumber + secondNumber;
        }

        /// <summary>
        /// Substract the second number from the first one
        /// </summary>
        /// <param name="firstNumber"></param>
        /// <param name="secondNumber"></param>
        /// <returns></returns>
        private decimal Substract(decimal firstNumber, decimal secondNumber)
        {
            return firstNumber - secondNumber;
        }

        /// <summary>
        /// Multiplies the first number by the second one
        /// </summary>
        /// <param name="firstNumber"></param>
        /// <param name="secondNumber"></param>
        /// <returns></returns>
        private decimal Multiply(decimal firstNumber, decimal secondNumber)
        {
            //return Convert.Todecimal(Convert.ToDecimal(firstNumber) * Convert.ToDecimal(secondNumber));
            return firstNumber * secondNumber;
        }

        /// <summary>
        /// Divides the first number by the second one
        /// </summary>
        /// <param name="firstNumber"></param>
        /// <param name="secondNumber">This should not be 0 as division by 0 is impossible</param>
        /// <returns></returns>
        private decimal Divide(decimal firstNumber, decimal secondNumber)
        {
            if (secondNumber == 0)
                throw new CalculationException("Division by zero is impossible");
            return firstNumber / secondNumber;
        }

        /// <summary>
        /// The number is raised to the power of the exponent
        /// </summary>
        /// <param name="number"></param>
        /// <param name="exponent"></param>
        /// <returns></returns>
        private decimal Exponent(decimal number, decimal exponent)
        {
            return (decimal)Math.Pow((double)number, (double)exponent);
        }

        /// <summary>
        /// Square root of the specified number
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private decimal Square(decimal number)
        {
            if (number < 0)
            {
                throw new CalculationException("Cannot get the square root of a negative number");
            }
            return (decimal)Math.Sqrt((double)number);
        }
    }
}
