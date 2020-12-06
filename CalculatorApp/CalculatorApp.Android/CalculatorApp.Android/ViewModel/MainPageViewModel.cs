using MvvmCross.Commands;
using MvvmCross.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using CalculatorLib;

namespace CalculatorXamarinApp.ViewModel
{
    public class MainPageViewModel : ViewModelBase
    {
        public string Calculation;
        public string Result;

        private IMvxAsyncCommand calculateCommand;

        public MainPageViewModel(IMvxNavigationService navigationService) : base(navigationService)
        {
        }

        public IMvxAsyncCommand CalculateCommand => CreateCommand(ref calculateCommand, async () =>
        {
            Calculator calc = new Calculator();
            Result = calc.CalculateResult(Calculation);
        });
    }
}
