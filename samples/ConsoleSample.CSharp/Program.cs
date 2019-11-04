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
            "199008672397",
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

            Console.WriteLine("Here is a valid 10 digit string that can be used for testing:");
            Console.WriteLine("----------------------");
            Console.WriteLine(SwedishPersonalIdentityNumberTestData.GetRandom().To10DigitString());

            WriteSpace();

            Console.WriteLine("Here is a valid 12 digit string that can be used for testing:");
            Console.WriteLine("----------------------");
            Console.WriteLine(SwedishPersonalIdentityNumberTestData.GetRandom().To12DigitString());

            WriteSpace();

            Console.WriteLine("Here is a personal identity number that can be used for testing:");
            Console.WriteLine("----------------------");
            var randomPin = SwedishPersonalIdentityNumberTestData.GetRandom();
            WritePersonalIdentityNumberInfo(IdentityNumber.FromSwedishPersonalIdentityNumber(randomPin));

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
            if (IdentityNumber.TryParse(rawPersonalIdentityNumber, out var identityNumber))
            {
                WritePersonalIdentityNumberInfo(identityNumber);
            }
            else
            {
                Console.Error.WriteLine("Unable to parse the input as a SwedishPersonalIdentityNumber.");
            }
        }

        private static void WritePersonalIdentityNumberInfo(IdentityNumber identityNumber)
        {
            if (identityNumber.IsSwedishPersonalIdentityNumber)
            {
                Console.WriteLine("SwedishPersonalIdentityNumber");
            }
            else if (identityNumber.IsSwedishCoordinationNumber)
            {
                Console.WriteLine("SwedishCoordinationNumber");
            }
            WriteKeyValueInfo("   .ToString()", identityNumber.ToString());
            WriteKeyValueInfo("   .To10DigitString()", identityNumber.To10DigitString());
            WriteKeyValueInfo("   .To12DigitString()", identityNumber.To12DigitString());

            WriteKeyValueInfo("   .Year", identityNumber.Year.ToString());
            WriteKeyValueInfo("   .Month", identityNumber.Month.ToString());
            WriteKeyValueInfo("   .Day", identityNumber.Day.ToString());
            WriteKeyValueInfo("   .BirthNumber", identityNumber.BirthNumber.ToString());
            WriteKeyValueInfo("   .Checksum", identityNumber.Checksum.ToString());

            WriteKeyValueInfo("   .GetDateOfBirthHint()", identityNumber.GetDateOfBirthHint().ToShortDateString());
            WriteKeyValueInfo("   .GetAgeHint()", identityNumber.GetAgeHint().ToString());

            WriteKeyValueInfo("   .GetGenderHint()", identityNumber.GetGenderHint().ToString());

            if (identityNumber.IsSwedishPersonalIdentityNumber)
            {
                // IsTestNumber is an extension method from the package ActiveLogin.Identity.Swedish.TestData
                WriteKeyValueInfo("   .IsTestNumber()", identityNumber.SwedishPersonalIdentityNumber.IsTestNumber().ToString());
            }
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
