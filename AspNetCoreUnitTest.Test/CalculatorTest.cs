using AspNetCoreUnitTest.APP;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AspNetCoreUnitTest.Test
{
    public class CalculatorTest
    {
        public Calculator _calculator { get; set; }
        public CalculatorTest()
        {
            _calculator = new Calculator();
        }

        [Fact]
        public void AddTest()
        {
            // Arrange -> değişlenlerin initialize edildiği yerdir
            int a = 5;
            int b = 20;

            // Act -> 
            var total = _calculator.Add(a, b);

            // Assert -> doğrulama evresi
            Assert.Equal<int>(25, total);
        }

        [Theory]
        [InlineData(2, 5, 7)]
        [InlineData(10, 2, 12)]
        [InlineData(-3, 15, 12)]
        public void Add_simpleValues_ReturnTotalValue(int a, int b, int expectedTotal)
        {

            var actualTotal = _calculator.Add(a, b);

            Assert.Equal<int>(actualTotal, expectedTotal);
        }

        [Theory]
        [InlineData(0, 5, 0)]
        [InlineData(-3, 0, 0)]
        public void Add_zeroValues_ReturnZeroValue(int a, int b, int expectedTotal)
        {
            var actualTotal = _calculator.Add(a, b);

            Assert.Equal<int>(actualTotal, expectedTotal);
        }


        [Fact]
        public void AddTestContains()
        {
            Assert.Contains("Mehmet", "Mehmet Yağcı");
        }

        [Fact]
        public void AddTestContainsList()
        {
            var names = new List<string>() { "Mehmet", "Şule", "Yasin", "Ümit" };
            Assert.Contains(names, x => x == "Şule");
        }

        [Fact]
        public void AddTestContainsList2()
        {
            var names = new List<string>() { "Mehmet", "Şule", "Yasin", "Ümit" };
            Assert.Contains(names, x => x == "Hakan");
        }

        [Fact]
        public void AddTestDoesNotContain()
        {
            Assert.DoesNotContain("Yasin", "Mehmet Yağcı");
        }

        [Fact]
        public void AssertTrue()
        {
            Assert.True(5 > 2);
        }

        [Fact]
        public void AssertTrue2()
        {
            Assert.True("".GetType() == typeof(string));
        }

        [Fact]
        public void AssertFalse()
        {
            Assert.False(2 > 5);
        }

        [Fact]
        public void AssertMatches()
        {
            var regEx = "^dog";

            Assert.Matches(regEx, "dog fight");
        }

        [Fact]
        public void AssertDoesNotMatch()
        {
            var regEx = "^dog";

            Assert.DoesNotMatch(regEx, "tiger fight");
        }

        [Fact]
        public void AssertStartsWith()
        {
            Assert.StartsWith("meh", "mehmet");
        }

        [Fact]
        public void AssertEndsWith()
        {
            Assert.EndsWith("met", "mehmet");
        }

        [Fact]
        public void AssertEmpty()
        {
            Assert.Empty(new List<string>());
        }

        [Fact]
        public void AssertNotEmpty()
        {
            Assert.NotEmpty(new List<string>() { "Mehmet" });
        }

        [Fact]
        public void AssertInRange()
        {
            Assert.InRange(10, 2, 20);
        }

        [Fact]
        public void AssertNotInRange()
        {
            Assert.NotInRange(30, 2, 20);
        }

        [Fact]
        public void AssertSingle()
        {
            Assert.Single(new List<string>() { "Mehmet" });
            Assert.Single<string>(new List<string>() { "Mehmet" });
        }

        [Fact]
        public void AssertSingleFalse()
        {
            Assert.Single(new List<string>() { "Mehmet", "Şule" });
        }

        [Fact]
        public void AssertIsType()
        {
            Assert.IsType<string>("Mehmet");
        }

        [Fact]
        public void AssertIsNotType()
        {
            Assert.IsNotType<string>(5);
        }

        [Fact]
        public void AssertIsAssignableFrom()
        {
            Assert.IsAssignableFrom<IEnumerable<string>>(new List<string>());

            Assert.IsAssignableFrom<Object>(2);

        }

        [Fact]
        public void AssertNull()
        {
            string deger = null;
            Assert.Null(deger);
        }

        [Fact]
        public void AssertNotNull()
        {
            string deger = "mehmet";
            Assert.NotNull(deger);
        }

        [Fact]
        public void AssertEqual()
        {
            Assert.Equal(2, 2);
        }

        [Fact]
        public void AssertNotEqual()
        {
            Assert.NotEqual(2, 5);
        }
    }
}
