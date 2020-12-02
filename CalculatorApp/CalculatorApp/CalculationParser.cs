using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CalculatorApp
{
    public class CalculationParser
    {
        private Regex reg = new Regex(@"([+\-/*()^]|[sqrt]+|[0-9\.]+)");
        
        public List<string> Parse(string calculation)
        {
            var splittedCalculation = reg.Split(calculation)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToList();

            List<int> indexToBeRemoved = new List<int>();

            
            for (int i = 0; i < splittedCalculation.Count; i++)
            {
                //For each "-" signs
                if (splittedCalculation[i] == "-")
                {
                    //If it's the first character, it means the number is a negative
                    if (i == 0)
                    {
                        splittedCalculation[i + 1] = "-" + splittedCalculation[i + 1];
                        indexToBeRemoved.Add(i);
                    }
                    else
                    {
                        //Verify the value before
                        var valueBefore = splittedCalculation[i - 1];

                        //If it's not a number, it means the value is negative
                        if (!double.TryParse(valueBefore, out _))
                        {
                            splittedCalculation[i + 1] = "-" + splittedCalculation[i + 1];
                            indexToBeRemoved.Add(i);
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
                        if (double.TryParse(valueBefore, out _))
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

        public double ValidateNumber(string number)
        {
            if (!double.TryParse(number, out double parsedNumber))
            {
                throw new ArgumentException(number + " is not a valid number");
            }
            return parsedNumber;
        }
    }
}
