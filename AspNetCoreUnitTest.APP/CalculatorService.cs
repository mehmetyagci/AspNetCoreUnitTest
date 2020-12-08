using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreUnitTest.APP
{
    public class CalculatorService : ICalculatorService
    {
        public int add(int a, int b)
        {
            // https://www.calculator.com/add/2/3 ortalama her bir istek 5 sn. sürüyor
            if (a == 0 || b == 0)
            {
                return 0;
            }

            return a + b;
        }

        public int multiply(int a, int b)
        {
            if (a == 0)
            {
                throw new Exception("a=0 olamaz");
            }
            return a * b;
        }

    }
}
