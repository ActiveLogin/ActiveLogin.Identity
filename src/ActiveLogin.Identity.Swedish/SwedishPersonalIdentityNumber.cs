using System;
using System.Collections.Generic;

namespace ActiveLogin.Identity.Swedish
{
    /// <summary>
    /// Represents a Swedish Personal Identity Number (Svenskt Personnummer).
    /// https://en.wikipedia.org/wiki/Personal_identity_number_(Sweden)
    /// https://sv.wikipedia.org/wiki/Personnummer_i_Sverige
    /// </summary>
    public class SwedishPersonalIdentityNumber : IEquatable<SwedishPersonalIdentityNumber>
    {
        /// <summary>
        /// The year for date of birth.
        /// </summary>
        public int Year { get; }

        /// <summary>
        /// The month for date of birth.
        /// </summary>
        public int Month { get; }

        /// <summary>
        /// The day for date of birth.
        /// </summary>
        public int Day { get; }

        /// <summary>
        /// A serial number to distinguish people born on the same day. 
        /// </summary>
        public int SerialNumber { get; }

        /// <summary>
        /// A checksum (last digit in personal identity number) used for validation.
        /// </summary>
        public int Checksum { get; }

        /// <summary>
        /// Date of birth for the person according to the personal identity number.
        /// </summary>
        public DateTime DateOfBirth { get; }

        private SwedishPersonalIdentityNumber(int year, int month, int day, int serialNumber, int checksum)
        {
            Year = year;
            Month = month;
            Day = day;

            SerialNumber = serialNumber;
            Checksum = checksum;

            DateOfBirth = GetDateOfBirth(Year, Month, Day);
        }

        /// <summary>
        /// Creates an instance of <see cref="SwedishPersonalIdentityNumber"/> out of the individual parts.
        /// </summary>
        /// <param name="year">The year part.</param>
        /// <param name="month">The month part.</param>
        /// <param name="day">The day part.</param>
        /// <param name="serialNumber">The serial number part.</param>
        /// <param name="checksum">The checksum part.</param>
        /// <returns>An instance of <see cref="SwedishPersonalIdentityNumber"/> if all the paramaters are valid by themselfes and in combination.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the arguments are invalid.</exception>
        public static SwedishPersonalIdentityNumber Create(int year, int month, int day, int serialNumber, int checksum)
        {
            EnsureIsValid(year, month, day, serialNumber, checksum);
            return new SwedishPersonalIdentityNumber(year, month, day, serialNumber, checksum);
        }

        private static void EnsureIsValid(int year, int month, int day, int serialNumber, int checksum)
        {
            var validator = new SwedishPersonalIdentityNumberValidator(year, month, day, serialNumber, checksum);

            if (!validator.YearIsValid())
            {
                throw new ArgumentOutOfRangeException(nameof(year), year, "Invalid year.");
            }

            if (!validator.MonthIsValid())
            {
                throw new ArgumentOutOfRangeException(nameof(month), month, "Invalid month. Must be in the range 1 to 12.");
            }

            if (!validator.DayIsValid())
            {
                if (validator.DayIsValidCoOrdinationNumber())
                {
                    throw new ArgumentOutOfRangeException(nameof(day), day, "Invalid day of month. It might be a valid co-ordination number.");
                }

                throw new ArgumentOutOfRangeException(nameof(day), day, "Invalid day of month.");
            }

            if (!validator.SerialNumberIsValid())
            {
                throw new ArgumentOutOfRangeException(nameof(serialNumber), serialNumber, "Invalid serial number. Must be in the range 0 to 999.");
            }

            if (!validator.ChecksumIsValid())
            {
                throw new ArgumentException("Invalid checksum.", nameof(checksum));
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
        /// <param name="date">The date to decide wheter the person is older than 100 years. That decides the delimiter (- or +).</param>
        public static SwedishPersonalIdentityNumber Parse(string personalIdentityNumber, DateTime date)
        {
            try
            {
                var parsed = SwedishPersonalIdentityNumberParser.Parse(personalIdentityNumber, date);
                return new SwedishPersonalIdentityNumber(parsed.Year, parsed.Month, parsed.Day, parsed.SerialNumber, parsed.Checksum);
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
        /// <param name="result">If valid, an instance of <see cref="SwedishPersonalIdentityNumber"/></param>
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
        /// <param name="date">The date to decide wheter the person is older than 100 years. That decides the delimiter (- or +).</param>
        /// <param name="result">If valid, an instance of <see cref="SwedishPersonalIdentityNumber"/></param>
        public static bool TryParse(string personalIdentityNumber, DateTime date, out SwedishPersonalIdentityNumber result)
        {
            try
            {
                result = Parse(personalIdentityNumber, date);
                return true;
            }
            catch (Exception)
            {
                result = null;
                return false;
            }
        }

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
        /// <param name="date">The date to decide wheter the person is older than 100 years. That decides the delimiter (- or +).</param>
        public string ToShortString(DateTime date)
        {
            var age = GetAge(date);
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
        /// <param name="date">The date when to calulate the age.</param>
        /// <returns></returns>
        public int GetAge(DateTime date)
        {
            var dateOfBirth = DateOfBirth;
            var age = date.Year - dateOfBirth.Year;

            if (date.Month < dateOfBirth.Month ||
               (date.Month == dateOfBirth.Month && date.Day < dateOfBirth.Day))
            {
                age -= 1;
            }

            if (age < 0)
            {
                throw new Exception("The person is not yet born.");
            }

            return age;
        }

        private static DateTime GetDateOfBirth(int year, int month, int day)
        {
            return new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
        }

        /// <summary>Returns a value indicating whether this instance is equal to a specified object.</summary>
        /// <param name="value">The object to compare to this instance.</param>
        /// <returns>true if <paramref name="value">value</paramref> is an instance of <see cref="T:System.DateTime"></see> and equals the value of this instance; otherwise, false.</returns>
        public override bool Equals(object value)
        {
            return Equals(value as SwedishPersonalIdentityNumber);
        }

        /// <summary>Returns a value indicating whether the value of this instance is equal to the value of the specified <see cref="SwedishPersonalIdentityNumber"></see> instance.</summary>
        /// <param name="value">The object to compare to this instance.</param>
        /// <returns>true if the <paramref name="value">value</paramref> parameter equals the value of this instance; otherwise, false.</returns>
        public bool Equals(SwedishPersonalIdentityNumber value)
        {
            return value != null &&
                   ToLongString() == value.ToLongString();
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return ToLongString().GetHashCode();
        }

        public static bool operator ==(SwedishPersonalIdentityNumber number1, SwedishPersonalIdentityNumber number2)
        {
            return EqualityComparer<SwedishPersonalIdentityNumber>.Default.Equals(number1, number2);
        }

        public static bool operator !=(SwedishPersonalIdentityNumber number1, SwedishPersonalIdentityNumber number2)
        {
            return !(number1 == number2);
        }
    }
}
