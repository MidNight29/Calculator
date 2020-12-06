using System;
using System.Collections.Generic;
using System.Text;

namespace CalculatorLib
{
    public class CalculationException: Exception
    {
        public CalculationException(string message):base(message)
        {

        }
    }
}
