using CalculatorLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalculatorAppUnitTests
{
    [TestClass]
    public class ComplexTests
    {
        private static Calculator _Calc;

        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            _Calc = new Calculator();
        }

        [TestMethod]
        public void MinusThreePlusTwo()
        {
            Assert.AreEqual("-5", _Calc.CalculateResult("-(3+2)"));
        }

        [TestMethod]
        public void MultiplicationOfDecimals()
        {
            Assert.AreEqual("-18.4", _Calc.CalculateResult("8*-2.3"));
        }

        [TestMethod]
        public void SquaredMultiplication()
        {
            Assert.AreEqual("2", _Calc.CalculateResult("sqrt(2*2)"));
        }

        [TestMethod]
        public void SquaredNegative()
        {
            Assert.AreEqual("Erreur*", _Calc.CalculateResult("sqrt(-4)"));
        }

        [TestMethod]
        public void NegativeExponentThree()
        {
            Assert.AreEqual("-8", _Calc.CalculateResult("-2^3"));
        }

        [TestMethod]
        public void NegativeExponentFour()
        {
            Assert.AreEqual("-16", _Calc.CalculateResult("-2^4"));
        }

        [TestMethod]
        public void Complex()
        {
            Assert.AreEqual("-1017", _Calc.CalculateResult("10+23-42(4/2+3)^2"));
        }

        [TestMethod]
        public void ExponentWithParenthesis()
        {
            Assert.AreEqual("16", _Calc.CalculateResult("(-2)^4"));
            Assert.AreEqual("-16", _Calc.CalculateResult("-2^4"));
            Assert.AreEqual("-16", _Calc.CalculateResult("(-2^4)"));
            Assert.AreEqual("16", _Calc.CalculateResult("((-2)^4)"));
        }

        [TestMethod]
        public void DoubleParenthesis()
        {
            Assert.AreEqual("12", _Calc.CalculateResult("((2+2)(2+1))"));
        }
    }
}
