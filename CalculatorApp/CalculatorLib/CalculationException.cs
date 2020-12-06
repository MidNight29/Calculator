using System;
using System.Collections.Generic;
using System.Text;

namespace CalculatorLib
{
    public class CalculationException: ArgumentException
    {
        public CalculationException(string message):base(message)
        {

        }
    }
}
