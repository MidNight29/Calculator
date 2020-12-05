using CalculatorApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CalculatorAppUnitTests
{
    [TestClass]
    public class ValidationTests
    {
        private static Calculator _Calc;

        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            _Calc = new Calculator();
        }

        [TestMethod]
        public void OnePlusOne()
        {
            Assert.AreEqual("2", _Calc.CalculateResult("1+1"));
        }

        [TestMethod]
        public void OnePlusTwo()
        {
            Assert.AreEqual("3", _Calc.CalculateResult("1 + 2"));
        }

        [TestMethod]
        public void OnePlusMinusOne()
        {
            Assert.AreEqual("0", _Calc.CalculateResult("1 + -1"));
        }

        [TestMethod]
        public void MinusOneMinusMinusOne()
        {
            Assert.AreEqual("0", _Calc.CalculateResult("-1 - -1"));
        }

        [TestMethod]
        public void FiveMinusFour()
        {
            Assert.AreEqual("1", _Calc.CalculateResult("5-4"));
        }

        [TestMethod]
        public void FiveTimesTwo()
        {
            Assert.AreEqual("10", _Calc.CalculateResult("5*2"));
        }

        [TestMethod]
        public void TwoPlusFiveTimesThree()
        {
            Assert.AreEqual("21", _Calc.CalculateResult("(2+5)*3"));
        }

        [TestMethod]
        public void TenDividedByTwo()
        {
            Assert.AreEqual("5", _Calc.CalculateResult("10/2"));
        }

        [TestMethod]
        public void AdditionAndMultiplication()
        {
            Assert.AreEqual("17", _Calc.CalculateResult("2+2*5+5"));
        }

        [TestMethod]
        public void MutliplicationAndSubstraction()
        {
            Assert.AreEqual("7.4", _Calc.CalculateResult("2.8*3-1"));
        }

        [TestMethod]
        public void Exponent()
        {
            Assert.AreEqual("256", _Calc.CalculateResult("2^8"));
        }

        [TestMethod]
        public void ExponentComplex()
        {
            Assert.AreEqual("1279", _Calc.CalculateResult("2^8*5-1"));
        }

        [TestMethod]
        public void SquareRoot()
        {
            Assert.AreEqual("2", _Calc.CalculateResult("sqrt(4)"));
        }

        [TestMethod]
        public void Error()
        {
            Assert.AreEqual("Erreur*", _Calc.CalculateResult("1/0"));
        }
    }
}
