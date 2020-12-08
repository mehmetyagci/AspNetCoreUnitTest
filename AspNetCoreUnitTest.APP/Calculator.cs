using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreUnitTest.APP
{
    public class Calculator
    {

        private ICalculatorService _calculatorService { get; set; }

        public Calculator(ICalculatorService calculatorService)
        {
            _calculatorService = calculatorService;
        }

        public int add(int a, int b)
        {
            return _calculatorService.add(a, b);
        }

        public int multiply(int a, int b)
        {
            return _calculatorService.multiply(a, b);
        }

        // Doğrudan yazma yerine Service ve Interface ile bağımlılığı azaltılıyor.
        //public int Add(int a, int b)
        //{
        //    // https://www.calculator.com/add/2/3 ortalama her bir istek 5 sn. sürüyor
        //    if(a == 0 || b == 0)
        //    {
        //        return 0;
        //    }

        //    return a + b;
        //}
    }
}
