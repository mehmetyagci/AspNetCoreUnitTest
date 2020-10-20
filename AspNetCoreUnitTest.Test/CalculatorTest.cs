using AspNetCoreUnitTest.APP;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AspNetCoreUnitTest.Test
{
    public class CalculatorTest
    {

        [Fact]
        public void AddTest()
        {
            // Arrange -> değişlenlerin initialize edildiği yerdir
            int a = 5;
            int b = 20;
            var calculator = new Calculator();

            // Act -> 
            var total = calculator.add(a, b);

            // Assert -> doğrulama evresi
            Assert.Equal<int>(25, total);
        }
    }
}
