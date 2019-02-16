using System;
using ActiveLogin.Identity.Swedish;
using ActiveLogin.Identity.Swedish.Extensions;
using ActiveLogin.Identity.Swedish.TestData;

namespace ConsoleSample
{
    class Program
    {
        private static readonly string[] RawPersonalIdentityNumberSamples =
        {
            "990913+9801",
            "120211+9986",
            "990807-2391",
            "180101-2392",

            "180101.2392",
            "ABC",
        };

        static void Main(string[] args)
        {
            Console.WriteLine("Sample showing possible uses of SwedishPersonalIdentityNumber.");
            WriteSpace();

            foreach (var sample in RawPersonalIdentityNumberSamples)
            {
                WritePersonalIdentityNumberInfo(sample);
                WriteSpace();
            }

            Console.WriteLine("Here are some valid 10 digit strings that can be used for testing:");
            Console.WriteLine("----------------------");
            foreach (var pin in SwedishPersonalIdentityTestNumbers.Random10DigitStrings(3))
            {
                Console.WriteLine(pin);
            }

            WriteSpace();

            Console.WriteLine("Here are some valid 12 digit strings that can be used for testing:");
            Console.WriteLine("----------------------");
            foreach (var pin in SwedishPersonalIdentityTestNumbers.Random12DigitStrings(3))
            {
                Console.WriteLine(pin);
            }

            WriteSpace();

            Console.WriteLine("Here is a personal identity number that can be used for testing:");
            Console.WriteLine("----------------------");
            var randomPin = SwedishPersonalIdentityTestNumbers.RandomPin();
            WritePersonalIdentityNumberInfo(randomPin);
            Console.WriteLine($"Is it a test number? {SwedishPersonalIdentityTestNumbers.IsTestNumber(randomPin)}");

            WriteSpace();

            Console.WriteLine("What is your (Swedish) Personal Identity Number?");
            var userRawPersonalIdentityNumber = Console.ReadLine();
            WritePersonalIdentityNumberInfo(userRawPersonalIdentityNumber);
            WriteSpace();

            Console.ReadLine();
        }

        private static void WritePersonalIdentityNumberInfo(string rawPersonalIdentityNumber)
        {
            WriteHeader($"Input: {rawPersonalIdentityNumber}");
            if (SwedishPersonalIdentityNumber.TryParse(rawPersonalIdentityNumber, out var personalIdentityNumber))
            {
                WritePersonalIdentityNumberInfo(personalIdentityNumber);
            }
            else
            {
                Console.Error.WriteLine("Unable to parse the input as a SwedishPersonalIdentityNumber.");
            }
        }

        private static void WritePersonalIdentityNumberInfo(SwedishPersonalIdentityNumber personalIdentityNumber)
        {
            Console.WriteLine("SwedishPersonalIdentityNumber");
            WriteKeyValueInfo("   .ToString()", personalIdentityNumber.ToString());
            WriteKeyValueInfo("   .To10DigitString()", personalIdentityNumber.To10DigitString());
            WriteKeyValueInfo("   .To12DigitString()", personalIdentityNumber.To12DigitString());

            WriteKeyValueInfo("   .Year", personalIdentityNumber.Year.ToString());
            WriteKeyValueInfo("   .Month", personalIdentityNumber.Month.ToString());
            WriteKeyValueInfo("   .Day", personalIdentityNumber.Day.ToString());
            WriteKeyValueInfo("   .BirthNumber", personalIdentityNumber.BirthNumber.ToString());
            WriteKeyValueInfo("   .Checksum", personalIdentityNumber.Checksum.ToString());

            WriteKeyValueInfo("   .GetDateOfBirthHint()", personalIdentityNumber.GetDateOfBirthHint().ToShortDateString());
            WriteKeyValueInfo("   .GetAgeHint()", personalIdentityNumber.GetAgeHint().ToString());

            WriteKeyValueInfo("   .GetGenderHint()", personalIdentityNumber.GetGenderHint().ToString());
        }

        private static void WriteHeader(string header)
        {
            Console.WriteLine(header);
            Console.WriteLine("----------------------");
        }

        private static void WriteKeyValueInfo(string key, string value)
        {
            Console.WriteLine($"{key}: {value}");
        }

        private static void WriteSpace()
        {
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
