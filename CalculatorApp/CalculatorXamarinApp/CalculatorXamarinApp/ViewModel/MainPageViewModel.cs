using CalculatorLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace CalculatorXamarinApp.ViewModel
{
    public class MainPageViewModel:INotifyPropertyChanged
    {

        private string calculation = "";
        private string result = "";

        public string Calculation
        {
            set { SetProperty(ref calculation, value); }
            get { return calculation; }
        }

        public string Result
        {
            set { SetProperty(ref result, value); }
            get { return result; }
        }
        public ICommand GetResultCommand { private set; get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainPageViewModel()
        {
            GetResultCommand = new Command(
                execute: () =>
                {
                    Calculator calc = new Calculator();
                    Result = calc.CalculateResult(Calculation);
                }
            );
        }
        

        bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Object.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
