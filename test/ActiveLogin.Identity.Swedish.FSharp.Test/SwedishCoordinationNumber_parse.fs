module ActiveLogin.Identity.Swedish.FSharp.Test.SwedishCoordinationNumber_parse

open System
open Swensen.Unquote
open Expecto
open ActiveLogin.Identity.Swedish
open Expecto
open FsCheck
open PinTestHelpers

let private isInvalidNumberOfDigits (str: string) =
    if System.String.IsNullOrWhiteSpace str then false
    else
        str
        |> String.filter isDigit
        |> (fun s -> s.Length <> 10 && s.Length <> 12)

let private validNumberTests = testList "valid coordination numbers" [
    testProp "roundtrip for 12 digit string" <| fun (Gen.CoordNum.ValidNum num) ->
        num.To12DigitString()
        |> SwedishCoordinationNumber.Parse =! num

    testProp "roundtrip for 10 digit string with delimiter" <| fun (Gen.CoordNum.ValidNum num) ->
        num.To10DigitString()
        |> SwedishCoordinationNumber.Parse =! num

    testProp "roundtrip for 10 digit string without hyphen-delimiter" <| fun (Gen.CoordNum.ValidNum num) ->
        num.To10DigitString()
        |> removeHyphen
        |> SwedishCoordinationNumber.Parse =! num

    testProp "roundtrip for 12 digit string mixed with 'non-digits'"
        <| fun (Gen.CoordNum.ValidNum num, Gen.Char100 charArray) ->
            let charsWithoutDigits =
                charArray
                |> Array.filter (isDigit >> not)

            num.To12DigitString()
            |> surroundEachChar charsWithoutDigits
            |> SwedishCoordinationNumber.Parse =! num

    testProp "roundtrip for 10 digit string mixed with 'non-digits' except plus"
        <| fun (Gen.CoordNum.ValidNum num, Gen.Char100 charArray) ->
            let charsWithoutPlus =
                let isDigitOrPlus (c:char) = "0123456789+".Contains c

                charArray
                |> Array.filter (isDigitOrPlus >> not)

            num.To10DigitString()
            |> (surroundEachChar charsWithoutPlus)
            |> SwedishCoordinationNumber.Parse =! num

    testProp "roundtrip for 10 digit string without hyphen delimiter, mixed with 'non-digits' except plus"
        <| fun (Gen.CoordNum.ValidNum num, Gen.Char100 charArray) ->
            let charsWithoutPlus =
                let isDigitOrPlus (c:char) = "0123456789+".Contains c

                charArray
                |> Array.filter (isDigitOrPlus >> not)

            num.To10DigitString()
            |> removeHyphen
            |> (surroundEachChar charsWithoutPlus)
            |> SwedishCoordinationNumber.Parse =! num

    testPropWithMaxTest 400 "roundtrip for 12 digit string 'in specific year'" <| fun (Gen.CoordNum.ValidNum num) ->
        let offset = rng.Next (0, 200)
        let year = num.Year + offset

        let str = num.To12DigitString()
        SwedishCoordinationNumber.ParseInSpecificYear(str, year) =! num

    testPropWithMaxTest 400 "roundtrip for 10 digit string 'in specific year'" <| fun (Gen.CoordNum.ValidNum num) ->
        let offset = rng.Next (0, 200)
        let year = num.Year + offset

        let str = num.To10DigitStringInSpecificYear year
        SwedishCoordinationNumber.ParseInSpecificYear(str, year) = num

    testPropWithMaxTest 400 "roundtrip for 10 digit string without hyphen delimeter 'in specific year'"
        <| fun (Gen.CoordNum.ValidNum num) ->
            let offset = rng.Next (0, 200)
            let year = num.Year + offset

            let str =
                num.To10DigitStringInSpecificYear year
                |> removeHyphen
            SwedishCoordinationNumber.ParseInSpecificYear(str, year) = num
    ]

let invalidNumberTests = testList "invalid coordination numbers" [
    test "null string returns argument null error" {
        toAction SwedishCoordinationNumber.Parse null
        |> Expect.throwsWithType<ArgumentNullException>
    }

    testProp "empty string returns parsing error" <| fun (Gen.EmptyString str) ->
        let f = toAction SwedishCoordinationNumber.Parse str
        Expect.throwsWithType<FormatException> f
        Expect.throwsWithMessage
            "String was not recognized as a valid SwedishPersonalIdentityNumber. Cannot be empty string or whitespace."
            f

    testProp "invalid number of digits returns parsing error" <| fun (Gen.Digits digits) ->
        isInvalidNumberOfDigits digits ==>
            lazy
                ( let f = toAction SwedishCoordinationNumber.Parse digits
                  Expect.throwsWithType<FormatException> f
                  Expect.throwsWithMessage "String was not recognized as a valid SwedishPersonalIdentityNumber." f )

    testProp "invalid num returns parsing error" <| fun (Gen.CoordNum.InvalidNumString str) ->
        toAction SwedishCoordinationNumber.Parse str
        |> Expect.throwsWithMessage "String was not recognized as a valid SwedishPersonalIdentityNumber."

    testProp "parseInSpecificYear with empty string returns parsing error" <| fun (Gen.EmptyString str, Gen.ValidYear year) ->
        let f  = toAction SwedishCoordinationNumber.ParseInSpecificYear (str, year)
        Expect.throwsWithType<FormatException> f
        Expect.throwsWithMessage "String was not recognized as a valid SwedishPersonalIdentityNumber. Cannot be empty string or whitespace." f

    testProp "parseInSpecificYear with null string returns argument null error" <| fun (Gen.ValidYear year) ->
        toAction SwedishCoordinationNumber.ParseInSpecificYear (null, year)
        |> Expect.throwsWithType<ArgumentNullException>

    testPropWithMaxTest 400 "cannot convert a num to 10 digit string in a specific year when the person would be 200 years or older"
        <| fun (Gen.CoordNum.ValidNum num) ->
            let offset =
                let maxYear = System.DateTime.MaxValue.Year - num.Year
                rng.Next(200, maxYear)
            let year = num.Year + offset

            toAction num.To10DigitStringInSpecificYear year
            |> Expect.throwsWithMessage "SerializationYear cannot be a more than 199 years after the person was born"

    testPropWithMaxTest 400 "cannot convert a num to 10 digit string in a specific year before the person was born"
        <| fun (Gen.CoordNum.ValidNum num) ->
            let offset = rng.Next(1, num.Year)
            let year = num.Year - offset

            toAction num.To10DigitStringInSpecificYear year
            |> Expect.throwsWithMessage "SerializationYear cannot be a year before the person was born" ]

[<Tests>]
let tests = testList "SwedishCoordinationNumber.parse" [ validNumberTests ; invalidNumberTests ]
