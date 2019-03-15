module ActiveLogin.Identity.Swedish.FSharp.Test.SwedishPersonalIdentityNumber_parse

open Swensen.Unquote
open Expecto
open Expecto.Flip
open ActiveLogin.Identity.Swedish.FSharp
open ActiveLogin.Identity.Swedish.FSharp.TestData
open PinTestHelpers
open Arbitraries
open FsCheck

let config = FsCheckConfig.defaultConfig
let addToConfig arbTypes = { config with arbitrary = arbTypes @ config.arbitrary }

let testProp arbTypes name =
    testPropertyWithConfig (addToConfig arbTypes) name
let ftestProp arbTypes name = 
    ftestPropertyWithConfig (addToConfig arbTypes) name
let ptestProp arbTypes name = 
    ptestPropertyWithConfig (addToConfig arbTypes) name

let yearTurning100 (pin:SwedishPersonalIdentityNumberValues) = 
    pin.Year 
    |> (+) 100 
    |> Year.create 
    |> function 
    | Ok y -> y 
    | Error e -> e |> failwithf "Test setup error %A"

type RandomLessThan100() =
    static member RandomLessThan100() : Arbitrary<int> = 
        Gen.choose(1, 99) |> Arb.fromGen

[<Tests>]
let tests = testList "parse" [ 
    testProp [ typeof<Valid12Digit> ] "Can parse any valid 12-digit string" <| fun (input, expected) ->
                let pin = input |> SwedishPersonalIdentityNumber.parse
                pin |> Expect.equalPin expected

    // this does not include 10 digit strings without delimiter
    testProp [ typeof<Valid10Digit> ] "Can parse any valid 10-digit string" <| fun (input, expected) ->
        let pin = input |> SwedishPersonalIdentityNumber.parse
        pin |> Expect.equalPin expected

    testProp [ typeof<Valid10DigitWithPlusDelimiter> ] 
        "Can parse valid 10-digit string with plus delimiter for person the year they turn 100" <| 
            fun (input, expected: SwedishPersonalIdentityNumberValues) ->
                let pin = input |> SwedishPersonalIdentityNumber.parseInSpecificYear (yearTurning100 expected)
                pin |> Expect.equalPin expected

    testProp [ typeof<Valid10DigitStringWithAnyDelimiterExceptPlus> ] 
        "Cannot correctly parse 10-digit string when person is turning 100 and delimiter is anything else than plus" <| 
            fun (input, expected: SwedishPersonalIdentityNumberValues) ->
                let yearTurning100 = yearTurning100 expected
                let pin = input |> SwedishPersonalIdentityNumber.parseInSpecificYear yearTurning100
                pin |> Expect.isOk "should be ok"
                pin |> Result.iter (fun p -> p.Year |> Year.value <>! expected.Year)

    testProp [ typeof<Valid10DigitWithPlusDelimiter>; typeof<RandomLessThan100> ] 
        "Cannot correctly parse 10-digit string with plus delimiter when parseYear is before person turned 100" <| 
            fun ((input, expected: SwedishPersonalIdentityNumberValues), lessThan100) ->
                let yearWhenNotTurned100 = 
                    expected.Year - lessThan100 
                    |> Year.create 
                    |> function 
                    | Ok y -> y 
                    | Error _ -> failwith "test setup error"
                let pin = input |> SwedishPersonalIdentityNumber.parseInSpecificYear yearWhenNotTurned100
                pin |> Expect.isOk "should be ok"
                pin |> Result.iter (fun p -> p.Year |> Year.value <>! expected.Year)

    testProp [ typeof<Valid12DigitStringWithAnyDelimiter> ] "Can parse 12-digit string with any delimiter" <| 
        fun (input, expected) ->
            let pin = input |> SwedishPersonalIdentityNumber.parse
            pin |> Expect.equalPin expected

    testProp [ typeof<Valid10DigitStringWithAnyDelimiterExceptPlus> ] 
        "Can parse 10-digit string for person < 100 years of age with any delimiter as long as it is not plus" <|
            fun (input, expected) ->
                let pin = input |> SwedishPersonalIdentityNumber.parse
                pin |> Expect.equalPin expected

    testProp [ typeof<Valid12DigitStringMixedWithCharacters> ] 
        "Can parse valid 12 digit string even if it has leading-, trailing- and characters mixed into it" <| 
            fun (input, expected) ->
                let pin = input |> SwedishPersonalIdentityNumber.parse
                pin |> Expect.equalPin expected

    testProp [ typeof<Valid10DigitStringMixedWithCharacters> ] 
        "Can parse valid 10 digit string even if it has leading-, trailing- and characters mixed into it" <| 
            fun (input, expected) ->
                let pin = input |> SwedishPersonalIdentityNumber.parse
                pin |> Expect.equalPin expected 
    // testProp [ typeof<EmptyOrWhitespaceString> ] "Parse with empty or whitespace string returns error" <| fun input ->
    //     let result = SwedishPersonalIdentityNumber.parse input
    //     result =! (Empty |> ParsingError |> Error)
    ]

        // [Fact]
        // public void Same_Number_Will_Use_Different_Delimiter_When_Parsed_On_Or_After_Person_Turns_100()
        // {
        //     var withHyphen = "120211-9986";
        //     var withPlus = "120211+9986";

        //     var pinBeforeTurning100 = SwedishPersonalIdentityNumber.ParseInSpecificYear(withHyphen, 2011);
        //     var pinOnYearTurning100 = SwedishPersonalIdentityNumber.ParseInSpecificYear(withPlus, 2012);
        //     var pinAfterTurning100 = SwedishPersonalIdentityNumber.ParseInSpecificYear(withPlus, 2013);

        //     var expected = new SwedishPersonalIdentityNumber(1912, 02, 11, 998, 6);
        //     Assert.Equal(expected, pinBeforeTurning100);
        //     Assert.Equal(expected, pinOnYearTurning100);
        //     Assert.Equal(expected, pinAfterTurning100);
        // }





        // [Fact]
        // public void Parse_Throws_FormatException_When_Empty_String()
        // {
        //     var ex = Assert.Throws<FormatException>(() => SwedishPersonalIdentityNumber.Parse(""));

        //     Assert.Contains(InvalidSwedishPersonalIdentityNumberErrorMessage, ex.Message);
        //     Assert.Contains("Cannot be empty string or whitespace", ex.Message);
        // }

        // [Fact]
        // public void Parse_Throws_ArgumentException_When_Whitespace_String()
        // {
        //     var ex = Assert.Throws<FormatException>(() => SwedishPersonalIdentityNumber.Parse(" "));

        //     Assert.Contains(InvalidSwedishPersonalIdentityNumberErrorMessage, ex.Message);
        //     Assert.Contains("Cannot be empty string or whitespace", ex.Message);
        // }

        // [Fact]
        // public void Parse_Throws_ArgumentNullException_When_Null()
        // {
        //     var ex = Assert.Throws<ArgumentNullException>(() => SwedishPersonalIdentityNumber.Parse(null));

        //     Assert.Contains("personalIdentityNumber", ex.Message);
        // }

        // [Fact]
        // public void ParseInSpecificYear_Throws_FormatException_When_Empty_String()
        // {
        //     var ex = Assert.Throws<FormatException>(() => SwedishPersonalIdentityNumber.ParseInSpecificYear("", 2018));

        //     Assert.Contains(InvalidSwedishPersonalIdentityNumberErrorMessage, ex.Message);
        //     Assert.Contains("Cannot be empty string or whitespace", ex.Message);
        // }

        // [Fact]
        // public void ParseInSpecificYear_Throws_FormatException_When_Whitespace_String()
        // {
        //     var ex = Assert.Throws<FormatException>(() => SwedishPersonalIdentityNumber.ParseInSpecificYear(" ", 2018));

        //     Assert.Contains(InvalidSwedishPersonalIdentityNumberErrorMessage, ex.Message);
        //     Assert.Contains("Cannot be empty string or whitespace", ex.Message);
        // }

        // [Fact]
        // public void ParseInSpecificYear_Throws_ArgumentNullException_When_Null()
        // {
        //     var ex = Assert.Throws<ArgumentNullException>(() => SwedishPersonalIdentityNumber.ParseInSpecificYear(null, 2018));

        //     Assert.Contains("personalIdentityNumber", ex.Message);
        // }

        // [Theory]
        // [InlineData("99-09-13+980-1", "189909139801")]
        // [InlineData("18-99-09-13-980-1", "189909139801")]
        // [InlineData("99.09.13-+980.1", "189909139801")]
        // [InlineData("18.99.09.13.980.1", "189909139801")]
        // [InlineData("1899-09-13-980-1", "189909139801")]
        // [InlineData("18 99 09 13 980 1", "189909139801")]
        // [InlineData("18A99B09C13D980E1", "189909139801")]
        // [InlineData("+18990913+9801+", "189909139801")]
        // [InlineData("ABC189909139801ABC", "189909139801")]
        // [InlineData("\"18\"\"99\"09\"13\"980\"1", "189909139801")]
        // [InlineData("**18*99***09*13*980**1*", "189909139801")]
        // [InlineData("\\18//99/;09\n13\t980\r1\n\r", "189909139801")]
        // [InlineData("ü18ü99ù09ę13é980á1ö", "189909139801")]
        // [InlineData("18----------------------------------------------------------------99-09-13-980-1", "189909139801")]
        // [InlineData("18--DROP TABLE USERS; 99-09-13-980-1", "189909139801")]
        // [InlineData("9909+13+9801", "189909139801")]
        // [InlineData("189909+13+9801", "189909139801")]
        // [InlineData("18+99+09+13+9801", "189909139801")]
        // [InlineData("18+99+09+13+98+01", "189909139801")]
        // [InlineData("1+8+9+9+0+9+1+3+9+8+0+1", "189909139801")]
        // [InlineData("19990807+2391", "199908072391")]
        // [InlineData("990807+2391", "189908072391")]
        // [InlineData("990913�9801", "199909139801")]
        // [InlineData("19990913�9801", "199909139801")]
        // [InlineData("990913_9801", "199909139801")]
        // [InlineData("19990913_9801", "199909139801")]
        // [InlineData("990913.9801", "199909139801")]
        // [InlineData("19990913.9801", "199909139801")]
        // public void Parses_When_Contains_Chars(string personalIdentityNumberString, string expectedPersonalIdentityNumberString)
        // {
        //     var personalIdentityNumber = SwedishPersonalIdentityNumber.ParseInSpecificYear(personalIdentityNumberString, 2018);
        //     Assert.Equal(expectedPersonalIdentityNumberString, personalIdentityNumber.To12DigitString());
        // }

        // [Theory]
        // [InlineData("123")]
        // [InlineData("12345678901")]
        // [InlineData("1234567890123")]
        // public void Throws_FormatException_When_Invalid_Number_Of_Digits(string personalIdentityNumberString)
        // {
        //     var ex = Assert.Throws<FormatException>(() => SwedishPersonalIdentityNumber.ParseInSpecificYear(personalIdentityNumberString, 2018));
        //     Assert.Contains(InvalidSwedishPersonalIdentityNumberErrorMessage, ex.Message);
        // }

        // [Theory]
        // // [InlineData("199913139801")]
        // // [InlineData("199909139802")]
        // [InlineData("199909329801")]
        // public void Throws_FormatException_When_Invalid_Pin(string personalIdentityNumberString)
        // {
        //     var ex = Assert.Throws<FormatException>(() => SwedishPersonalIdentityNumber.ParseInSpecificYear(personalIdentityNumberString, 2018));
        //     Assert.Contains(InvalidSwedishPersonalIdentityNumberErrorMessage, ex.Message);
        // }
