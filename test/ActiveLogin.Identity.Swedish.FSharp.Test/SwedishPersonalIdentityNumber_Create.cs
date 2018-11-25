using System;
using Xunit;

namespace ActiveLogin.Identity.Swedish.Test
{
    /// <remarks>
    /// Tested with offical test Personal Identity Numbers from Skatteverket:
    /// https://skatteverket.entryscape.net/catalog/9/datasets/147
    /// </remarks>
    public class SwedishPersonalIdentityNumber_Create
    {
        [Theory]
        [InlineData(-1, 01, 01, 239, 2)]
        [InlineData(int.MaxValue, 01, 01, 239, 2)]
        public void Throws_When_Invalid_Year(int year, int month, int day, int birthNumber, int checksum)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => SwedishPersonalIdentityNumber.Create(year, month, day, birthNumber, checksum));
            Assert.Contains("Invalid year.", ex.Message);
        }

        [Theory]
        [InlineData(2018, 0, 01, 239, 2)]
        [InlineData(2018, 13, 01, 239, 2)]
        public void Throws_When_Invalid_Month(int year, int month, int day, int birthNumber, int checksum)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => SwedishPersonalIdentityNumber.Create(year, month, day, birthNumber, checksum));
            Assert.Contains("Invalid month.", ex.Message);
        }

        [Theory]
        [InlineData(2018, 01, 0, 239, 2)]
        [InlineData(2018, 01, 32, 239, 2)]
        [InlineData(2018, 02, 30, 239, 2)]
        public void Throws_When_Invalid_Day(int year, int month, int day, int birthNumber, int checksum)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => SwedishPersonalIdentityNumber.Create(year, month, day, birthNumber, checksum));
            Assert.Contains("Invalid day of month.", ex.Message);
        }

        [Theory]
        [InlineData(2018, 01, 61, 239, 2)]
        public void Throws_When_Possible_CoOrdinationNumber(int year, int month, int day, int birthNumber, int checksum)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => SwedishPersonalIdentityNumber.Create(year, month, day, birthNumber, checksum));
            Assert.Contains("Invalid day of month.", ex.Message);
        }

        [Theory]
        [InlineData(2018, 01, 01, 0, 2)]
        [InlineData(2018, 01, 01, 1000, 2)]
        public void Throws_When_Invalid_BirthNumber(int year, int month, int day, int birthNumber, int checksum)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => SwedishPersonalIdentityNumber.Create(year, month, day, birthNumber, checksum));
            Assert.Contains("Invalid birth number.", ex.Message);
        }

        [Theory]
        [InlineData(2018, 01, 01, 239, 3)]
        [InlineData(2018, 01, 01, 239, 4)]
        public void Throws_When_Invalid_Checksum(int year, int month, int day, int birthNumber, int checksum)
        {
            var ex = Assert.Throws<ArgumentException>(() => SwedishPersonalIdentityNumber.Create(year, month, day, birthNumber, checksum));
            Assert.Contains("Invalid checksum.", ex.Message);
        }

        [Theory]
        [InlineData(1899, 09, 13, 980, 1)]
        [InlineData(1999, 08, 07, 239, 1)]
        [InlineData(2000, 01, 02, 239, 1)]
        [InlineData(2018, 01, 01, 239, 2)]
        public void Accepts_Valid_Personal_Identity_Number(int year, int month, int day, int birthNumber, int checksum)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Create(year, month, day, birthNumber, checksum);
            Assert.Equal(year, personalIdentityNumber.Year);
            Assert.Equal(month, personalIdentityNumber.Month);
            Assert.Equal(day, personalIdentityNumber.Day);
            Assert.Equal(birthNumber, personalIdentityNumber.BirthNumber);
            Assert.Equal(checksum, personalIdentityNumber.Checksum);
        }
    }
}
