using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreUnitTest.APP
{
    public interface ICalculatorService
    {
        int add(int a, int b);

        int multiply(int a, int b);

    }
}
