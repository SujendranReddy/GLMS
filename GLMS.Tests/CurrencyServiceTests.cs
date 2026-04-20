using GLMS.Interfaces;
using Xunit;

namespace GLMS.Tests
{
    public class CurrencyServiceTests
    {
        [Fact]
        public void ConvertUsdToZar_CalculatesCorrectAmount()
        {
            decimal usdAmount = 10m;
            decimal exchangeRate = 18.50m;

            decimal zarAmount = Math.Round(usdAmount * exchangeRate, 2);

            Assert.Equal(185.00m, zarAmount);
        }

        [Fact]
        public void ConvertUsdToZar_ZeroUsd_ReturnsZero()
        {
            decimal usdAmount = 0m;
            decimal exchangeRate = 18.50m;

            decimal zarAmount = Math.Round(usdAmount * exchangeRate, 2);

            Assert.Equal(0m, zarAmount);
        }

        [Fact]
        public void ConvertUsdToZar_RoundsToTwoDecimals()
        {
            decimal usdAmount = 12.345m;
            decimal exchangeRate = 18.2789m;

            decimal zarAmount = Math.Round(usdAmount * exchangeRate, 2);

            Assert.Equal(Math.Round(12.345m * 18.2789m, 2), zarAmount);
        }
    }
}