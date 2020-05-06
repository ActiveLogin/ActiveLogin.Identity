using System;
using ActiveLogin.Identity.Swedish;
using ActiveLogin.Identity.Swedish.Extensions;
using ActiveLogin.Identity.Swedish.TestData;

namespace ConsoleSample
{
    public class Program
    {
        private static readonly string[] RawInputs =
        {
            "990913+9801", // Swedish personal identity number
            "120211+9986", // Swedish personal identity number
            "990807-2391", // Swedish personal identity number
            "180101-2392", // Swedish personal identity number
            "180101.2392", // Swedish personal identity number

            "900778-2395", // Swedish coordination number

            "ABC", // Invalid
        };

        static void Main(string[] args)
        {
            WriteSection("Sample for SwedishPersonalIdentityNumber, SwedishCoordinationNumber and IndividualIdentityNumber.", false);


            // SwedishPersonalIdentityNumber

            WriteSection("Parse Swedish personal identity numbers");
            Sample_ParseSwedishPersonalIdentityNumbers();

            WriteSection("Show Swedish personal identity number test data");
            Sample_ShowSwedishPersonalIdentityNumberTestData();


            // SwedishCoordinationNumber

            WriteSection("Parse Swedish coordination numbers");
            Sample_ParseSwedishCoordinationNumbers();

            WriteSection("Show Swedish coordination number test data");
            Sample_ShowSwedishCoordinationNumberTestData();


            // IndividualIdentityNumber

            WriteSection("Parse individual identity numbers");
            Sample_ParseIndividualIdentityNumbers();

            WriteSection("Parse user input");
            Sample_ParseUserInput();



            Console.ReadLine();
        }

        #region Samples_SwedishPersonalIdentityNumbers

        private static void Sample_ParseSwedishPersonalIdentityNumbers()
        {
            foreach (var input in RawInputs)
            {
                WriteHeader($"Input: {input}");
                if (PersonalIdentityNumber.TryParse(input, out var identityNumber))
                {
                    WriteSwedishPersonalIdentityNumberInfo(identityNumber);
                }
                else
                {
                    Console.Error.WriteLine("Unable to parse the input as a SwedishPersonalIdentityNumber.");
                    WriteSpace();
                }
            }
        }

        private static void Sample_ShowSwedishPersonalIdentityNumberTestData()
        {
            WriteHeader("A Personal Identity Number that can be used for testing, represented as 10 digit string:");
            WriteLine(PersonalIdentityNumberTestData.GetRandom().To10DigitString());

            WriteHeader("A Personal Identity Number that can be used for testing, represented as 12 digit string:");
            WriteLine(PersonalIdentityNumberTestData.GetRandom().To12DigitString());

            WriteHeader("A Personal Identity Number that can be used for testing:");
            WriteSwedishPersonalIdentityNumberInfo(PersonalIdentityNumberTestData.GetRandom());
        }

        #endregion

        #region Samples_SwedishCoordinationNumbers

        private static void Sample_ParseSwedishCoordinationNumbers()
        {
            foreach (var input in RawInputs)
            {
                WriteHeader($"Input: {input}");
                if (CoordinationNumber.TryParse(input, out var identityNumber))
                {
                    WriteSwedishCoordinationNumberInfo(identityNumber);
                }
                else
                {
                    Console.Error.WriteLine("Unable to parse the input as a SwedishCoordinationNumber.");
                    WriteSpace();
                }
            }
        }

        private static void Sample_ShowSwedishCoordinationNumberTestData()
        {
            WriteHeader("A Coordination Number that can be used for testing, represented as 10 digit string:");
            WriteLine(CoordinationNumberTestData.GetRandom().To10DigitString());

            WriteHeader("A Coordination Number that can be used for testing, represented as 12 digit string:");
            WriteLine(CoordinationNumberTestData.GetRandom().To12DigitString());

            WriteHeader("A Coordination Number that can be used for testing:");
            WriteSwedishCoordinationNumberInfo(CoordinationNumberTestData.GetRandom());
        }

        #endregion

        #region Samples_IndiviudalIdentityNumbers

        private static void Sample_ParseIndividualIdentityNumbers()
        {
            foreach (var input in RawInputs)
            {
                WriteIndividualIdentityNumberInfo(input);
            }
        }

        private static void Sample_ParseUserInput()
        {
            WriteHeader("What is your (Swedish) Personal Identity Number or Coordination Number?");
            var userRawPersonalIdentityNumber = Console.ReadLine();
            WriteIndividualIdentityNumberInfo(userRawPersonalIdentityNumber);
        }

        #endregion

        #region WriteUtils

        private static void WriteSwedishPersonalIdentityNumberInfo(PersonalIdentityNumber identityNumber)
        {
            WriteIndividualIdentityNumberInfo(IndividualIdentityNumber.FromPersonalIdentityNumber(identityNumber));
        }

        private static void WriteSwedishCoordinationNumberInfo(CoordinationNumber identityNumber)
        {
            WriteIndividualIdentityNumberInfo(IndividualIdentityNumber.FromCoordinationNumber(identityNumber));
        }

        private static void WriteIndividualIdentityNumberInfo(string rawIndividualIdentityNumber)
        {
            WriteHeader($"Input: {rawIndividualIdentityNumber}");
            if (IndividualIdentityNumber.TryParse(rawIndividualIdentityNumber, out var identityNumber))
            {
                WriteIndividualIdentityNumberInfo(identityNumber);
            }
            else
            {
                Console.Error.WriteLine("Unable to parse the input as a IndividualIdentityNumber.");
                WriteSpace();
            }
        }

        private static void WriteIndividualIdentityNumberInfo(IndividualIdentityNumber identityNumber)
        {
            if (identityNumber.IsPersonalIdentityNumber)
            {
                WriteKeyValueInfo("Type", "SwedishPersonalIdentityNumber");
            }
            else if (identityNumber.IsCoordinationNumber)
            {
                WriteKeyValueInfo("Type", "SwedishCoordinationNumber");
            }

            WriteKeyValueInfo("   .ToString()", identityNumber.ToString());
            WriteKeyValueInfo("   .To10DigitString()", identityNumber.To10DigitString());
            WriteKeyValueInfo("   .To12DigitString()", identityNumber.To12DigitString());
            WriteKeyValueInfo("   .GetGenderHint()", identityNumber.GetGenderHint().ToString());

            if (identityNumber.IsPersonalIdentityNumber)
            {
                // IsTestNumber is an extension method from the package ActiveLogin.Identity.Swedish.TestData
                WriteKeyValueInfo("   .IsTestNumber()", identityNumber.PersonalIdentityNumber.IsTestNumber().ToString());
            }
            if (identityNumber.IsCoordinationNumber)
            {
                // IsTestNumber is an extension method from the package ActiveLogin.Identity.Swedish.TestData
                WriteKeyValueInfo("   .IsTestNumber()", identityNumber.CoordinationNumber.IsTestNumber().ToString());
            }

            WriteSpace();
        }

        private static void WriteLine(string info)
        {
            Console.WriteLine(info);
        }

        private static void WriteHeader(string header, bool withTopSpace = true)
        {
            if (withTopSpace)
            {
                Console.WriteLine();
            }

            Console.WriteLine(header);
            Console.WriteLine("---------------------------------------");
        }

        private static void WriteSection(string header, bool withTopSpace = true)
        {
            if (withTopSpace)
            {
                Console.WriteLine();
            }

            Console.WriteLine(header);
            Console.WriteLine("###################################################################");
            Console.WriteLine();
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

        #endregion
    }
}
