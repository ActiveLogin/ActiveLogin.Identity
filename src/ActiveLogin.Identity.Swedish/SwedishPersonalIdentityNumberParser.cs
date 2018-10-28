using System;
using System.Text.RegularExpressions;

namespace ActiveLogin.Identity.Swedish
{
    internal static class SwedishPersonalIdentityNumberParser
    {
        public static SwedishPersonalIdentityNumberParts Parse(string personalIdentityNumber, DateTime date)
        {
            var trimmedPersonalIdentityNumber = personalIdentityNumber.Trim();

            if (TryParseShortPattern(trimmedPersonalIdentityNumber, date, out var parsedShort))
            {
                return parsedShort;
            }

            if (TryParseLongPattern(trimmedPersonalIdentityNumber, out var parsedLong))
            {
                return parsedLong;
            }

            throw new ArgumentException("Could not find a valid Swedish personal identity number.", nameof(personalIdentityNumber));
        }

        /// <summary>
        /// YYYYMMDDSSSC, YYYYMMDD-SSSC or YYYYMMDD+SSSC
        /// </summary>
        private static bool TryParseShortPattern(string personalIdentityNumber, DateTime date, out SwedishPersonalIdentityNumberParts parts)
        {
            var pattern = new Regex(@"^" +
                            @"(?<year>[0-9]{2})" +
                            @"(?<month>[0-9]{2})" +
                            @"(?<day>[0-9]{2})" +
                            @"(?<delimiter>[-+]?)" +
                            @"(?<serialNumber>[0-9]{3})" +
                            @"(?<checksum>[0-9]{1})" +
                            @"$");

            var match = pattern.Match(personalIdentityNumber);
            if (!match.Success)
            {
                parts = null;
                return false;
            }

            var partsFromMatch = GetPartsFromMatch(match);
            var delimiter = GetStringValue(match, "delimiter");
            var fullYear = GetFullYear(partsFromMatch.Year, partsFromMatch.Month, partsFromMatch.Day, delimiter, date);
        
            parts = new SwedishPersonalIdentityNumberParts(fullYear, partsFromMatch.Month, partsFromMatch.Day, partsFromMatch.SerialNumber, partsFromMatch.Checksum);
            return true;
        }

        /// <summary>
        /// YYYYMMDDSSSC or YYYYMMDD-SSSC
        /// </summary>
        private static bool TryParseLongPattern(string personalIdentityNumber, out SwedishPersonalIdentityNumberParts parts)
        {
            var pattern = new Regex(@"^" +
                                    @"(?<year>[0-9]{4})" +
                                    @"(?<month>[0-9]{2})" +
                                    @"(?<day>[0-9]{2})" +
                                    @"(?<delimiter>[-]?)" +
                                    @"(?<serialNumber>[0-9]{3})" +
                                    @"(?<checksum>[0-9]{1})" +
                                    @"$");

            var match = pattern.Match(personalIdentityNumber);
            if (!match.Success)
            {
                parts = null;
                return false;
            }

            parts = GetPartsFromMatch(match);
            return true;
        }

        private static SwedishPersonalIdentityNumberParts GetPartsFromMatch(Match match)
        {
            var year = GetIntValue(match, "year");
            var month = GetIntValue(match, "month");
            var day = GetIntValue(match, "day");
            var serialNumber = GetIntValue(match, "serialNumber");
            var checksum = GetIntValue(match, "checksum");

            return new SwedishPersonalIdentityNumberParts(year, month, day, serialNumber, checksum);
        }

        private static int GetIntValue(Match longPatternMatch, string groupName)
        {
            return int.Parse(longPatternMatch.Groups[groupName].Value);
        }

        private static string GetStringValue(Match longPatternMatch, string groupName)
        {
            return longPatternMatch.Groups[groupName].Value;
        }

        private static int GetFullYear(int shortYear, int month, int day, string delimiter, DateTime date)
        {
            var currentCentury = (date.Year / 100) * 100;
            var fullYear = currentCentury + shortYear;

            var dateOfBirth = new DateTime(fullYear, month, day, 0, 0, 0, DateTimeKind.Utc);
            var firstDayOfYearInDateOfBirth = new DateTime(dateOfBirth.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            if (firstDayOfYearInDateOfBirth > date)
            {
                dateOfBirth = dateOfBirth.AddYears(-100);
            }

            var ageIsAbove100Years = delimiter == "+";
            if (ageIsAbove100Years)
            {
                dateOfBirth = dateOfBirth.AddYears(-100);
            }

            return dateOfBirth.Year;
        }

        public class SwedishPersonalIdentityNumberParts
        {
            public SwedishPersonalIdentityNumberParts(int year, int month, int day, int serialNumber, int checksum)
            {
                Year = year;
                Month = month;
                Day = day;
                SerialNumber = serialNumber;
                Checksum = checksum;
            }

            public int Year { get; }
            public int Month { get; }
            public int Day { get; }
            public int SerialNumber { get; }
            public int Checksum { get; }
        }
    }
}