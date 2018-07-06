using System;

namespace ActiveLogin.Identity.Swedish
{
    internal class SwedishPersonalIdentityNumberValidator
    {
        private const int CoOrdinationNumberDaysAdded = 60;

        private int Year { get; }
        private int Month { get; }
        private int Day { get; }
        private int SerialNumber { get; }
        private int Checksum { get; }

        public SwedishPersonalIdentityNumberValidator(int year, int month, int day, int serialNumber, int checksum)
        {
            Year = year;
            Month = month;
            Day = day;

            SerialNumber = serialNumber;
            Checksum = checksum;
        }
        
        public bool DayIsValidCoOrdinationNumber()
        {
            var daysWithoutCoOrdinationNumberDaysAdded = Day - CoOrdinationNumberDaysAdded;
            return DayIsValid(daysWithoutCoOrdinationNumberDaysAdded);
        }

        public bool YearIsValid()
        {
            return Year >= DateTime.MinValue.Year && Year <= DateTime.MaxValue.Year;
        }

        public bool MonthIsValid()
        {
            return Month >= 1 && Month <= 12;
        }

        public bool DayIsValid()
        {
            return DayIsValid(Day);
        }

        private bool DayIsValid(int day)
        {
            var daysInMonth = DateTime.DaysInMonth(Year, Month);
            return day >= 1 && day <= daysInMonth;
        }

        public bool SerialNumberIsValid()
        {
            return SerialNumber >= 1 && SerialNumber <= 999;
        }
    }
}