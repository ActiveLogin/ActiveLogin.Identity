using System;
using System.Text.RegularExpressions;

namespace ActiveLogin.Identity.Swedish
{
    internal class SwedishPersonalIdentityNumberParser
    {
        private readonly DateTime _pointInTime;

        public SwedishPersonalIdentityNumberParser(DateTime pointInTime)
        {
            _pointInTime = pointInTime;
        }

        public (int fullYear, int month, int day, int serialNumber, int checksum) Parse(string personalIdentityNumber)
        {
            var trimmedPersonalIdentityNumber = personalIdentityNumber.Trim();

            var longPattern = new Regex(@"^" +
                                        @"(?<year>[0-9]{4})" +
                                        @"(?<month>[0-9]{2})" +
                                        @"(?<day>[0-9]{2})" +
                                        @"(?<delimiter>[-]?)" +
                                        @"(?<serialNumber>[0-9]{3})" +
                                        @"(?<checksum>[0-9]{1})" +
                                        @"$");
            var longPatternMatch = longPattern.Match(trimmedPersonalIdentityNumber);
            if (longPatternMatch.Success)
            {
                var fullYear = int.Parse(longPatternMatch.Groups["year"].Value);
                var month = int.Parse(longPatternMatch.Groups["month"].Value);
                var day = int.Parse(longPatternMatch.Groups["day"].Value);
                var delimiter = longPatternMatch.Groups["delimiter"].Value;
                var serialNumber = int.Parse(longPatternMatch.Groups["serialNumber"].Value);
                var checksum = int.Parse(longPatternMatch.Groups["checksum"].Value);

                return (fullYear, month, day, serialNumber, checksum);
            }

            var shortPattern = new Regex(@"^" +
                                        @"(?<year>[0-9]{2})" +
                                        @"(?<month>[0-9]{2})" +
                                        @"(?<day>[0-9]{2})" +
                                        @"(?<delimiter>[-+]?)" +
                                        @"(?<serialNumber>[0-9]{3})" +
                                        @"(?<checksum>[0-9]{1})" +
                                        @"$");
            var shortPatternMatch = shortPattern.Match(trimmedPersonalIdentityNumber);
            if (shortPatternMatch.Success)
            {
                var shortYear = int.Parse(shortPatternMatch.Groups["year"].Value);
                var month = int.Parse(shortPatternMatch.Groups["month"].Value);
                var day = int.Parse(shortPatternMatch.Groups["day"].Value);
                var delimiter = shortPatternMatch.Groups["delimiter"].Value;
                var serialNumber = int.Parse(shortPatternMatch.Groups["serialNumber"].Value);
                var checksum = int.Parse(shortPatternMatch.Groups["checksum"].Value);
                var fullYear = GetFullYear(shortYear, month, day, delimiter, _pointInTime);

                return (fullYear, month, day, serialNumber, checksum);
            }

            throw new ArgumentException("Could not find a valid Swedish personal identity number.", nameof(personalIdentityNumber));
        }

        private static int GetFullYear(int shortYear, int month, int day, string delimiter, DateTime pointInTime)
        {
            var ageIsAbove100Years = delimiter == "+";
            var currentCentury = (pointInTime.Year / 100) * 100;
            var fullYear = currentCentury + shortYear;
            var dateOfBirth = new DateTime(fullYear, month, day);
            if (dateOfBirth > pointInTime)
            {
                dateOfBirth = dateOfBirth.AddYears(-100);
            }
            if (ageIsAbove100Years)
            {
                dateOfBirth = dateOfBirth.AddYears(-100);
            }
            return dateOfBirth.Year;
        }
    }
}