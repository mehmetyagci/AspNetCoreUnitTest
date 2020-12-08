using AspNetCoreUnitTest.APP;
using Moq;
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
        }

        [Theory]
        [InlineData(3, 5, 15)]
        public void Multiply_simpleValues_ReturnTotalValue(int a, int b, int expectedTotal)
        {
            mymock.Setup(x => x.multiply(a, b)).Returns(expectedTotal);

            Assert.Equal(15, _calculator.multiply(a, b));

            //var actualTotal = _calculator.multiply(a, b);

            //Assert.Equal<int>(actualTotal, expectedTotal);
        }
    }
}
