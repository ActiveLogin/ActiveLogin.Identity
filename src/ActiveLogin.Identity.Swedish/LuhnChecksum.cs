using System.Collections.Generic;
using System.Linq;

namespace ActiveLogin.Identity.Swedish
{
    /// <summary>
    /// Implementation of algorithm descirbed in https://en.wikipedia.org/wiki/Luhn_algorithm
    /// </summary>
    internal class LuhnChecksum
    {
        public static int GetChecksum(string digits)
        {
            var ints = digits.Select(x => int.Parse(x.ToString()));
            return GetChecksum(ints);
        }

        public static int GetChecksum(IEnumerable<int> digits)
        {
            var luhnSum = digits.Reverse()
                .Select((digit, index) => index % 2 == 0 ? digit * 2 : digit)
                .Reverse()
                .Select(digit => digit > 9 ? digit - 9 : digit)
                .Sum();

            return (luhnSum * 9) % 10;
        }
    }
}
