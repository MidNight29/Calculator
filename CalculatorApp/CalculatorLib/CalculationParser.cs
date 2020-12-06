using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CalculatorLib
{
    public class CalculationParser
    {
        private Regex reg = new Regex(@"([+\-/*()^]|[sqrt]+|[0-9\.]+)");
        
        public List<string> Parse(string calculation)
        {
            var splittedCalculation = reg.Split(calculation)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToList();

            splittedCalculation = ParseNegativesAndParathesis(splittedCalculation);

            return splittedCalculation;
        }

        /// <summary>
        /// This function makes sure that negative numbers have their "-" sign with them
        /// and that the parenthesis have their "*" in front of them if needed
        /// </summary>
        /// <param name="splittedCalculation"></param>
        /// <returns></returns>
        public List<string> ParseNegativesAndParathesis(List<string> splittedCalculation)
        {
            List<int> indexToBeRemoved = new List<int>();

            for (int i = 0; i < splittedCalculation.Count; i++)
            {
                //For each "-" signs
                if (splittedCalculation[i] == "-")
                {
                    var valueAfter = splittedCalculation[i + 1];

                    //If it's the first character and the next character is a number, it means the number is a negative
                    if (i == 0)
                    {
                        if (decimal.TryParse(valueAfter, out _))
                        {
                            splittedCalculation[i] = "-1";
                            splittedCalculation.Insert(i + 1, "*");
                        }
                    }
                    else
                    {
                        //Verify the value before
                        var valueBefore = splittedCalculation[i - 1];

                        //If it's not a number, it means the value is negative
                        if (!decimal.TryParse(valueBefore, out _) && decimal.TryParse(valueAfter, out _))
                        {
                            splittedCalculation[i] = "-1";
                            splittedCalculation.Insert(i + 1, "*");
                        }
                    }
                }

                //For each "(" signs
                if (splittedCalculation[i] == "(")
                {
                    //Make sure this is not the first character
                    if (i != 0)
                    {
                        //Verify the value before
                        var valueBefore = splittedCalculation[i - 1];

                        //If it's a number, add a "*" sign, this way it will multiply the result from the parenthesis to it
                        if (decimal.TryParse(valueBefore, out _))
                        {
                            splittedCalculation.Insert(i, "*");
                        }
                    }

                }
            }

            indexToBeRemoved = indexToBeRemoved.OrderByDescending(i => i).ToList();
            indexToBeRemoved.ForEach(i => splittedCalculation.RemoveAt(i));

            return splittedCalculation;
        }

        public decimal ValidateNumber(string number)
        {
            if (!decimal.TryParse(number, out decimal parsedNumber))
            {
                throw new ArgumentException(number + " is not a valid number");
            }
            return parsedNumber;
        }
    }
}
