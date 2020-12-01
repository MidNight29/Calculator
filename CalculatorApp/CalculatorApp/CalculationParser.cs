using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CalculatorApp
{
    public class CalculationParser
    {
        private Regex reg = new Regex(@"([+-/*//()^]|[sqrt]+|[0-9]+)");
        
        public List<string> Parse(string calculation)
        {
            var splittedCalculation = reg.Split(calculation)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToList();

            List<int> indexToBeRemoved = new List<int>();

            //For each "-" signs
            for (int i = 0; i < splittedCalculation.Count; i++)
            {
                if (splittedCalculation[i] == "-")
                {
                    //Verify the value before
                    var valueBefore = splittedCalculation[i - 1];

                    //If it's not a number, it means the value is negative
                    if (!Double.TryParse(valueBefore, out _))
                    {
                        splittedCalculation[i + 1] = "-" + splittedCalculation[i + 1];
                        indexToBeRemoved.Add(i);
                    }
                }
            }

            indexToBeRemoved.ForEach(i => splittedCalculation.RemoveAt(i));            

            return splittedCalculation;
        }

        public double ValidateNumber(string number)
        {
            if (!Double.TryParse(number, out double parsedNumber))
            {
                throw new ArgumentException(number + " is not a valid number");
            }
            return parsedNumber;
        }
    }
}
