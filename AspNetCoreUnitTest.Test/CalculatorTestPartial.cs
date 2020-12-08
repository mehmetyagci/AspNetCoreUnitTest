using AspNetCoreUnitTest.APP;
using Moq;
using System;
using Xunit;

namespace AspNetCoreUnitTest.Test
{
    public class CalculatorTest
    {
        public Calculator _calculator { get; set; }
        public Mock<ICalculatorService> mymock { get; set; }
        public CalculatorTest()
        {
            mymock = new Mock<ICalculatorService>();

            //Mock Nesne
            _calculator = new Calculator(mymock.Object);

            // gerçek nesne
            //_calculator = new Calculator(new CalculatorService());
        }

        [Theory]
        [InlineData(2, 5, 7)]
        [InlineData(10, 2, 12)]
        [InlineData(-3, 15, 12)]
        public void Add_simpleValues_ReturnTotalValue(int a, int b, int expectedTotal)
        {
            mymock.Setup(x => x.add(a, b)).Returns(expectedTotal);

            var actualTotal = _calculator.add(a, b);

            Assert.Equal<int>(actualTotal, expectedTotal);
            //Assert.NotEmpty(null);
            mymock.Verify(x => x.add(a, b), Times.Once);
        }

        [Theory]
        [InlineData(3, 5, 15)]
        public void Multiply_simpleValues_ReturnTotalValue(int a, int b, int expectedTotal)
        {
            // mymock.Setup(x => x.multiply(a, b)).Returns(expectedTotal);
            // Assert.Equal(15, _calculator.multiply(a, b));

            int actualMultip = 0;
            mymock.Setup(x => x.multiply(It.IsAny<int>(), It.IsAny<int>()))
                .Callback<int, int>((x, y) => actualMultip = x * y);

            _calculator.multiply(a, b);
            Assert.Equal(expectedTotal, actualMultip);

            _calculator.multiply(5, 20);
            Assert.Equal(100, actualMultip);

            //var actualTotal = _calculator.multiply(a, b);

            //Assert.Equal<int>(actualTotal, expectedTotal);
        }

        [Theory]
        [InlineData(0, 5)]
        public void Multiply_ZeroValue_ReturnException(int a, int b)
        {
            mymock.Setup(x => x.multiply(a, b)).Throws(new System.Exception("a=0 olamaz"));

            Exception exception = Assert.Throws<Exception>(() => _calculator.multiply(a, b));

            Assert.Equal("a=0 olamaz", exception.Message);
        }
    }
}
