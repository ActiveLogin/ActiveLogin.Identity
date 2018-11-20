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
        /// A birth number (födelsenummer) to distinguish people born on the same day.
        /// </summary>
        public int BirthNumber { get; }

        /// <summary>
        /// A checksum (kontrollsiffra) used for validation. Last digit in the PIN.
        /// </summary>
        public int Checksum { get; }

        private SwedishPersonalIdentityNumber(int year, int month, int day, int birthNumber, int checksum)
        {
            Year = year;
            Month = month;
            Day = day;

            BirthNumber = birthNumber;
            Checksum = checksum;
        }

        /// <summary>
        /// Creates an instance of <see cref="SwedishPersonalIdentityNumber"/> out of the individual parts.
        /// </summary>
        /// <param name="year">The year part.</param>
        /// <param name="month">The month part.</param>
        /// <param name="day">The day part.</param>
        /// <param name="birthNumber">The birth number part.</param>
        /// <param name="checksum">The checksum part.</param>
        /// <returns>An instance of <see cref="SwedishPersonalIdentityNumber"/> if all the paramaters are valid by themselfes and in combination.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the arguments are invalid.</exception>
        public static SwedishPersonalIdentityNumber Create(int year, int month, int day, int birthNumber, int checksum)
        {
            EnsureIsValid(year, month, day, birthNumber, checksum);
            return new SwedishPersonalIdentityNumber(year, month, day, birthNumber, checksum);
        }

        private static void EnsureIsValid(int year, int month, int day, int birthNumber, int checksum)
        {
            var validator = new SwedishPersonalIdentityNumberValidator(year, month, day, birthNumber, checksum);

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

            if (!validator.BirthNumberIsValid())
            {
                throw new ArgumentOutOfRangeException(nameof(birthNumber), birthNumber, "Invalid birth number. Must be in the range 0 to 999.");
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
            return ParseInSpecificYear(personalIdentityNumber, DateTime.UtcNow.Year);
        }

        /// <summary>
        /// Converts the string representation of the personal identity number to its <see cref="SwedishPersonalIdentityNumber"/> equivalent.
        /// </summary>
        /// <param name="personalIdentityNumber">A string representation of the personal identity number to parse.</param>
        /// <param name="parseYear">
        /// The specific year to use when checking if the person has turned / will turn 100 years old.
        /// That information changes the delimiter (- or +).
        ///
        /// For more info, see: https://www.riksdagen.se/sv/dokument-lagar/dokument/svensk-forfattningssamling/folkbokforingslag-1991481_sfs-1991-481#P18
        /// </param>
        public static SwedishPersonalIdentityNumber ParseInSpecificYear(string personalIdentityNumber, int parseYear)
        {
            try
            {
                var parsed = SwedishPersonalIdentityNumberParser.Parse(personalIdentityNumber, parseYear);
                return new SwedishPersonalIdentityNumber(parsed.Year, parsed.Month, parsed.Day, parsed.BirthNumber, parsed.Checksum);
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
        /// <param name="parseYear">
        /// The specific year to use when checking if the person has turned / will turn 100 years old.
        /// That information changes the delimiter (- or +).
        ///
        /// For more info, see: https://www.riksdagen.se/sv/dokument-lagar/dokument/svensk-forfattningssamling/folkbokforingslag-1991481_sfs-1991-481#P18
        /// </param>
        /// <param name="result">If valid, an instance of <see cref="SwedishPersonalIdentityNumber"/></param>
        public static bool TryParse(string personalIdentityNumber, int parseYear, out SwedishPersonalIdentityNumber result)
        {
            try
            {
                result = ParseInSpecificYear(personalIdentityNumber, parseYear);
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
        public string To10DigitString()
        {
            return To10DigitStringInSpecificYear(DateTime.UtcNow.Year);
        }

        /// <summary>
        /// Converts the value of the current <see cref="SwedishPersonalIdentityNumber" /> object to its equivalent 10 digit string representation. The total length, including the separator, will be 11 chars.
        /// Format is YYMMDDXSSSC, for example <example>990807-2391</example> or <example>120211+9986</example>.
        /// </summary>
        /// <param name="serializationYear">
        /// The specific year to use when checking if the person has turned / will turn 100 years old.
        /// That information changes the delimiter (- or +).
        ///
        /// For more info, see: https://www.riksdagen.se/sv/dokument-lagar/dokument/svensk-forfattningssamling/folkbokforingslag-1991481_sfs-1991-481#P18
        /// </param>
        public string To10DigitStringInSpecificYear(int serializationYear)
        {
            var years = serializationYear - Year;
            var delimiter = years >= 100 ? '+' : '-';
            var twoDigitYear = Year % 100;
            return $"{twoDigitYear:D2}{Month:D2}{Day:D2}{delimiter}{BirthNumber:D3}{Checksum}";
        }

        /// <summary>
        /// Converts the value of the current <see cref="SwedishPersonalIdentityNumber" /> object to its equivalent 12 digit string representation.
        /// Format is YYYYMMDDSSSC, for example <example>19908072391</example> or <example>191202119986</example>.
        /// </summary>
        public string To12DigitString()
        {
            return $"{Year:D4}{Month:D2}{Day:D2}{BirthNumber:D3}{Checksum}";
        }

        /// <summary>
        /// Converts the value of the current <see cref="SwedishPersonalIdentityNumber" /> object to its equivalent 12 digit string representation.
        /// Format is YYYYMMDDSSSC, for example <example>19908072391</example> or <example>191202119986</example>.
        /// </summary>
        public override string ToString()
        {
            return To12DigitString();
        }

        /// <summary>Returns a value indicating whether this instance is equal to a specified object.</summary>
        /// <param name="value">The object to compare to this instance.</param>
        /// <returns>true if <paramref name="value">value</paramref> is an instance of <see cref="SwedishPersonalIdentityNumber"></see> and equals the value of this instance; otherwise, false.</returns>
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
                   To12DigitString() == value.To12DigitString();
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return To12DigitString().GetHashCode();
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
