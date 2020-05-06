module ConsoleSample.FSharp

open ActiveLogin.Identity.Swedish
open ActiveLogin.Identity.Swedish.Extensions

let samplePins = [ "990913+9801"; "120211+9986"; "990807-2391"; "180101-2392"; "180101.2392" ]
let sampleCoordinationNumbers = [ "900778-2395" ]
let sampleInvalidNumbers = [ "ABC" ]

module SwedishPersonalIdentityNumber =
    let tryParse input =
        match PersonalIdentityNumber.TryParse input with
        | true, num -> Some num
        | false, _ -> None

    let print (num: PersonalIdentityNumber) =
        printfn "   .ToString(): %O" num
        printfn "   .To10DigitString(): %s" (num.To10DigitString())
        printfn "   .To12DigitString(): %s" (num.To12DigitString())
        printfn "   .Year: %O" num.Year
        printfn "   .Month: %O" num.Month
        printfn "   .Day: %O" num.Day
        printfn "   .BirthNumber: %O" num.BirthNumber
        printfn "   .Checksum: %O" num.Checksum
        printfn "   .GetDateOfBirthHint(): %s" (num.GetDateOfBirthHint().ToShortDateString())
        printfn "   .GetAgeHint(): %O" (num.GetAgeHint())
        printfn "   .GetGenderHint(): %O" (num.GetGenderHint())
        printfn "   .IsTestNumber(): %O" (num.IsTestNumber)

let sampleStrings = samplePins @ sampleCoordinationNumbers @ sampleInvalidNumbers

let printSection withTopSpace header =
    if withTopSpace then printfn ""
    printfn "%s" header
    printfn "###################################################################"
    printfn ""

let printKeyValueInfo = printfn "%s: %s"

let printSpace() = printfn ""; printfn ""

let printHeader withTopSpace header args =
    if withTopSpace then printfn ""
    printfn header args
    printfn "---------------------------------------"

let parseAndPrint parse print rawString =
    printHeader true "Input: %s" rawString
    rawString
    |> parse
    |> function
    | Some num -> print num
    | None ->
        eprintfn "Unable to parse the input as an IndividualIdentityNumber"
        printSpace()


        // private static void WriteIndividualIdentityNumberInfo(string rawIndividualIdentityNumber)
        // {
        //     WriteHeader($"Input: {rawIndividualIdentityNumber}");
        //     if (IndividualIdentityNumber.TryParse(rawIndividualIdentityNumber, out var identityNumber))
        //     {
        //         WriteIndividualIdentityNumberInfo(identityNumber);
        //     }
        //     else
        //     {
        //         Console.Error.WriteLine("Unable to parse the input as a IndividualIdentityNumber.");
        //         WriteSpace();
        //     }
        // }
        // static void Main(string[] args)
        // {
        //     WriteSection("Sample for SwedishPersonalIdentityNumber, SwedishCoordinationNumber and IndividualIdentityNumber.", false);


        //     // SwedishPersonalIdentityNumber

        //     WriteSection("Parse Swedish personal identity numbers");
        //     Sample_ParseSwedishPersonalIdentityNumbers();

        //     WriteSection("Show Swedish personal identity number test data");
        //     Sample_ShowSwedishPersonalIdentityNumberTestData();


        //     // SwedishCoordinationNumber

        //     WriteSection("Parse Swedish coordination numbers");
        //     Sample_ParseSwedishCoordinationNumbers();

        //     WriteSection("Show Swedish coordination number test data");
        //     Sample_ShowSwedishCoordinationNumberTestData();


        //     // IndividualIdentityNumber

        //     WriteSection("Parse individual identity numbers");
        //     Sample_ParseIndividualIdentityNumbers();

        //     WriteSection("Parse user input");
        //     Sample_ParseUserInput();



        //     Console.ReadLine();
        // }

        // #region Samples_SwedishPersonalIdentityNumbers

        // private static void Sample_ParseSwedishPersonalIdentityNumbers()
        // {
        //     foreach (var input in RawInputs)
        //     {
        //         WriteHeader($"Input: {input}");
        //         if (SwedishPersonalIdentityNumber.TryParse(input, out var identityNumber))
        //         {
        //             WriteSwedishPersonalIdentityNumberInfo(identityNumber);
        //         }
        //         else
        //         {
        //             Console.Error.WriteLine("Unable to parse the input as a SwedishPersonalIdentityNumber.");
        //             WriteSpace();
        //         }
        //     }
        // }

        // private static void Sample_ShowSwedishPersonalIdentityNumberTestData()
        // {
        //     WriteHeader("A Personal Identity Number that can be used for testing, represented as 10 digit string:");
        //     WriteLine(SwedishPersonalIdentityNumberTestData.GetRandom().To10DigitString());

        //     WriteHeader("A Personal Identity Number that can be used for testing, represented as 12 digit string:");
        //     WriteLine(SwedishPersonalIdentityNumberTestData.GetRandom().To12DigitString());

        //     WriteHeader("A Personal Identity Number that can be used for testing:");
        //     WriteSwedishPersonalIdentityNumberInfo(SwedishPersonalIdentityNumberTestData.GetRandom());
        // }

        // #endregion

        // #region Samples_SwedishCoordinationNumbers

        // private static void Sample_ParseSwedishCoordinationNumbers()
        // {
        //     foreach (var input in RawInputs)
        //     {
        //         WriteHeader($"Input: {input}");
        //         if (SwedishCoordinationNumber.TryParse(input, out var identityNumber))
        //         {
        //             WriteSwedishCoordinationNumberInfo(identityNumber);
        //         }
        //         else
        //         {
        //             Console.Error.WriteLine("Unable to parse the input as a SwedishCoordinationNumber.");
        //             WriteSpace();
        //         }
        //     }
        // }

        // private static void Sample_ShowSwedishCoordinationNumberTestData()
        // {
        //     WriteHeader("A Coordination Number that can be used for testing, represented as 10 digit string:");
        //     WriteLine(SwedishCoordinationNumberTestData.GetRandom().To10DigitString());

        //     WriteHeader("A Coordination Number that can be used for testing, represented as 12 digit string:");
        //     WriteLine(SwedishCoordinationNumberTestData.GetRandom().To12DigitString());

        //     WriteHeader("A Coordination Number that can be used for testing:");
        //     WriteSwedishCoordinationNumberInfo(SwedishCoordinationNumberTestData.GetRandom());
        // }

        // #endregion

        // #region Samples_IndiviudalIdentityNumbers

        // private static void Sample_ParseIndividualIdentityNumbers()
        // {
        //     foreach (var input in RawInputs)
        //     {
        //         WriteIndividualIdentityNumberInfo(input);
        //     }
        // }

        // private static void Sample_ParseUserInput()
        // {
        //     WriteHeader("What is your (Swedish) Personal Identity Number or Coordination Number?");
        //     var userRawPersonalIdentityNumber = Console.ReadLine();
        //     WriteIndividualIdentityNumberInfo(userRawPersonalIdentityNumber);
        // }

        // #endregion

        // #region WriteUtils

        // private static void WriteSwedishPersonalIdentityNumberInfo(SwedishPersonalIdentityNumber identityNumber)
        // {
        //     WriteIndividualIdentityNumberInfo(IndividualIdentityNumber.FromSwedishPersonalIdentityNumber(identityNumber));
        // }

        // private static void WriteSwedishCoordinationNumberInfo(SwedishCoordinationNumber identityNumber)
        // {
        //     WriteIndividualIdentityNumberInfo(IndividualIdentityNumber.FromSwedishCoordinationNumber(identityNumber));
        // }


        // private static void WriteIndividualIdentityNumberInfo(IndividualIdentityNumber identityNumber)
        // {
        //     if (identityNumber.IsSwedishPersonalIdentityNumber)
        //     {
        //         WriteKeyValueInfo("Type", "SwedishPersonalIdentityNumber");
        //     }
        //     else if (identityNumber.IsSwedishCoordinationNumber)
        //     {
        //         WriteKeyValueInfo("Type", "SwedishCoordinationNumber");
        //     }

        //     WriteKeyValueInfo("   .ToString()", identityNumber.ToString());
        //     WriteKeyValueInfo("   .To10DigitString()", identityNumber.To10DigitString());
        //     WriteKeyValueInfo("   .To12DigitString()", identityNumber.To12DigitString());

        //     WriteKeyValueInfo("   .Year", identityNumber.Year.ToString());
        //     WriteKeyValueInfo("   .Month", identityNumber.Month.ToString());
        //     WriteKeyValueInfo("   .Day", identityNumber.Day.ToString());
        //     WriteKeyValueInfo("   .BirthNumber", identityNumber.BirthNumber.ToString());
        //     WriteKeyValueInfo("   .Checksum", identityNumber.Checksum.ToString());

        //     WriteKeyValueInfo("   .GetDateOfBirthHint()", identityNumber.GetDateOfBirthHint().ToShortDateString());
        //     WriteKeyValueInfo("   .GetAgeHint()", identityNumber.GetAgeHint().ToString());

        //     WriteKeyValueInfo("   .GetGenderHint()", identityNumber.GetGenderHint().ToString());

        //     if (identityNumber.IsSwedishPersonalIdentityNumber)
        //     {
        //         // IsTestNumber is an extension method from the package ActiveLogin.Identity.Swedish.TestData
        //         WriteKeyValueInfo("   .IsTestNumber()", identityNumber.SwedishPersonalIdentityNumber.IsTestNumber().ToString());
        //     }
        //     if (identityNumber.IsSwedishCoordinationNumber)
        //     {
        //         // IsTestNumber is an extension method from the package ActiveLogin.Identity.Swedish.TestData
        //         WriteKeyValueInfo("   .IsTestNumber()", identityNumber.SwedishCoordinationNumber.IsTestNumber().ToString());
        //     }

        //     WriteSpace();
        // }




        // #endregion


[<EntryPoint>]
let main argv =
    printSection false "Sample for SwedishPersonalIdentityNumber."
    printSection true "Parse Swedish personal identity numbers"
    sampleStrings
    |> List.iter (parseAndPrint SwedishPersonalIdentityNumber.tryParse SwedishPersonalIdentityNumber.print)

    0 // return an integer exit code
