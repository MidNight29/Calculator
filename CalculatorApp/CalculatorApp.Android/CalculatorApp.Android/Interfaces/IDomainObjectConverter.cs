using System;
using System.Collections.Generic;
using System.Text;

namespace CalculatorXamarinApp.Interfaces
{
    public interface IDomainObjectConverter
    {
        object Convert(object source);
    }
}
