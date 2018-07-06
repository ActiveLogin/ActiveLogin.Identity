using System;
using System.Text.RegularExpressions;

namespace ActiveLogin.Identity.Swedish
{
    /// <summary>
    /// Represents a Swedish Personal Identity Number (Svenskt Personnummer).
    /// https://en.wikipedia.org/wiki/Personal_identity_number_(Sweden)
    /// https://sv.wikipedia.org/wiki/Personnummer_i_Sverige
    /// </summary>
    public class SwedishPersonalIdentityNumber
    {
        public int Year { get; }
        public int Month { get; }
        public int Day { get; }
        public int SerialNumber { get; }
        public int Checksum { get; }

        public SwedishPersonalIdentityNumber(int year, int month, int day, int serialNumber, int checksum)
        {
            EnsureIsValid(year, month, day, serialNumber, checksum);

            Year = year;
            Month = month;
            Day = day;

            SerialNumber = serialNumber;
            Checksum = checksum;

            DateOfBirth = new DateTime(Year, Month, Day, 0, 0, 0, DateTimeKind.Utc);
            LegalGender = GetLegalGender(SerialNumber);
        }

        private static void EnsureIsValid(int year, int month, int day, int serialNumber, int checksum)
        {
            var validator = new SwedishPersonalIdentityNumberValidator(year, month, day, serialNumber, checksum);
            if (!validator.YearIsValid())
            {
                throw new ArgumentException("Invalid year.", nameof(year));
            }

            if (!validator.MonthIsValid())
            {
                throw new ArgumentException("Invalid month. Must be in the range 1 to 12.", nameof(month));
            }

            if (!validator.DayIsValid())
            {
                if (validator.DayIsValidCoOrdinationNumber())
                {
                    throw new ArgumentException("Invalid day of month. It might be a valid co-ordination number.", nameof(day));
                }

                throw new ArgumentException("Invalid day of month.", nameof(day));
            }

            if (!validator.SerialNumberIsValid())
            {
                throw new ArgumentException("Invalid serial number. Must be in the range 0 to 999.", nameof(serialNumber));
            }
        }

        /// <summary>
        /// Converts the string representation of the personal identity number to its <see cref="SwedishPersonalIdentityNumber"/> equivalent.
        /// </summary>
        public static SwedishPersonalIdentityNumber Parse(string personalIdentityNumber)
        {
            return Parse(personalIdentityNumber, DateTime.UtcNow);
        }

        /// <summary>
        /// Converts the string representation of the personal identity number to its <see cref="SwedishPersonalIdentityNumber"/> equivalent.
        /// </summary>
        /// <param name="personalIdentityNumber">A string representation of the personal identity number to parse.</param>
        /// <param name="pointInTime">The date to decide wheter the person is older than 100 years. That decides the delimiter (- or +).</param>
        public static SwedishPersonalIdentityNumber Parse(string personalIdentityNumber, DateTime pointInTime)
        {
            try
            {
                var parser = new SwedishPersonalIdentityNumberParser(pointInTime);
                var parsed = parser.Parse(personalIdentityNumber);
                return new SwedishPersonalIdentityNumber(parsed.fullYear, parsed.month, parsed.day, parsed.serialNumber, parsed.checksum);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("Invalid Swedish personal identity number.", nameof(personalIdentityNumber));
            }
        }

        /// <summary>
        /// Converts the string representation of the personal identity number to its <see cref="SwedishPersonalIdentityNumber"/> equivalent  and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="personalIdentityNumber">A string representation of the personal identity number to parse.</param>
        public static bool TryParse(string personalIdentityNumber, out SwedishPersonalIdentityNumber result)
        {
            try
            {
                result = Parse(personalIdentityNumber);
                return true;
            }
            catch (Exception)
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Converts the string representation of the personal identity number to its <see cref="SwedishPersonalIdentityNumber"/> equivalent  and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="personalIdentityNumber">A string representation of the personal identity number to parse.</param>
        /// <param name="pointInTime">The date to decide wheter the person is older than 100 years. That decides the delimiter (- or +).</param>
        public static bool TryParse(string personalIdentityNumber, DateTime pointInTime, out SwedishPersonalIdentityNumber result)
        {
            try
            {
                result = Parse(personalIdentityNumber, pointInTime);
                return true;
            }
            catch (Exception)
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Date of birth for the person according to the personal identity number.
        /// </summary>
        public DateTime DateOfBirth { get; private set; }

        /// <summary>
        /// Legal gender in Sweden according to the last digit of the serial number in the personal identity number.
        /// Odd number: Man
        /// Even number: Woman
        /// </summary>
        public SwedishLegalGender LegalGender { get; private set; }

        /// <summary>
        /// Converts the value of the current <see cref="SwedishPersonalIdentityNumber" /> object to its equivalent short string representation.
        /// Format is YYMMDDXSSSC, for example <example>990807-2391</example> or <example>120211+9986</example>.
        /// </summary>
        public string ToShortString()
        {
            return ToShortString(DateTime.UtcNow);
        }

        /// <summary>
        /// Converts the value of the current <see cref="SwedishPersonalIdentityNumber" /> object to its equivalent short string representation.
        /// Format is YYMMDDXSSSC, for example <example>990807-2391</example> or <example>120211+9986</example>.
        /// </summary>
        /// <param name="pointInTime">The date to decide wheter the person is older than 100 years. That decides the delimiter (- or +).</param>
        public string ToShortString(DateTime pointInTime)
        {
            var age = GetAge(pointInTime);
            var delimiter = age >= 100 ? '+' : '-';
            var twoDigitYear = Year % 100;
            return $"{twoDigitYear:D2}{Month:D2}{Day:D2}{delimiter}{SerialNumber:D3}{Checksum}";
        }

        /// <summary>
        /// Converts the value of the current <see cref="SwedishPersonalIdentityNumber" /> object to its equivalent long string representation.
        /// Format is YYYYMMDDSSSC, for example <example>19908072391</example> or <example>191202119986</example>.
        /// </summary>
        public string ToLongString()
        {
            return $"{Year:D4}{Month:D2}{Day:D2}{SerialNumber:D3}{Checksum}";
        }

        /// <summary>
        /// Converts the value of the current <see cref="SwedishPersonalIdentityNumber" /> object to its equivalent short string representation.
        /// Format is YYMMDDXSSSC, for example <example>990807-2391</example> or <example>120211+9986</example>.
        /// </summary>
        public override string ToString()
        {
            return ToShortString();
        }

        /// <summary>
        /// Get the age of the person.
        /// </summary>
        public int GetAge()
        {
            return GetAge(DateTime.UtcNow);
        }

        /// <summary>
        /// Get the age of the person.
        /// </summary>
        /// <param name="pointInTime">The date when to calulate the age.</param>
        /// <returns></returns>
        public int GetAge(DateTime pointInTime)
        {
            var dateOfBirth = DateOfBirth;
            var age = pointInTime.Year - dateOfBirth.Year;

            if (pointInTime.Month < dateOfBirth.Month ||
               (pointInTime.Month == dateOfBirth.Month && pointInTime.Day < dateOfBirth.Day))
            {
                age -= 1;
            }

            if (age < 0)
            {
                throw new Exception("The person is not yet born.");
            }

            return age;
        }

        private static SwedishLegalGender GetLegalGender(int serialNumber)
        {
            var isSerialNumberEven = serialNumber % 2 == 0;
            if (isSerialNumberEven)
            {
                return SwedishLegalGender.Woman;
            }

            return SwedishLegalGender.Man;
        }
    }
}
