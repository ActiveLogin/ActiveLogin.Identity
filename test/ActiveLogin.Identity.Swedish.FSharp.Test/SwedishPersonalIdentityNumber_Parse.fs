module ActiveLogin.Identity.Swedish.FSharp.Test.SwedishPersonalIdentityNumber_Parse

open System
open Swensen.Unquote
open Expecto
open ActiveLogin.Identity.Swedish
open ActiveLogin.Identity.Swedish.TestData
open FsCheck


let private isInvalidNumberOfDigits (str: string) =
    if System.String.IsNullOrWhiteSpace str then false
    else
        str
        |> String.filter isDigit
        |> (fun s -> s.Length <> 10 && s.Length <> 12)

let private validPinTests = testList "valid pins" [
    testProp "roundtrip for 12 digit string" <| fun (Gen.Pin.ValidPin pin) ->
        pin.To12DigitString()
        |> SwedishPersonalIdentityNumber.Parse =! pin

    testProp "roundtrip for 10 digit string with delimiter" <| fun (Gen.Pin.ValidPin pin) ->
        pin.To10DigitString()
        |> SwedishPersonalIdentityNumber.Parse =! pin

    testProp "roundtrip for 10 digit string without hyphen-delimiter" <| fun (Gen.Pin.ValidPin pin) ->
        pin.To10DigitString()
        |> removeHyphen
        |> SwedishPersonalIdentityNumber.Parse =! pin

    testProp "roundtrip for 12 digit string mixed with 'non-digits'"
        <| fun (Gen.Pin.ValidPin pin, Gen.Char100 charArray) ->
            let charsWithoutDigits =
                charArray
                |> Array.filter (isDigit >> not)

            pin.To12DigitString()
            |> surroundEachChar charsWithoutDigits
            |> SwedishPersonalIdentityNumber.Parse =! pin

    testProp "roundtrip for 10 digit string mixed with 'non-digits' except plus"
        <| fun (Gen.Pin.ValidPin pin, Gen.Char100 charArray) ->
            let charsWithoutPlus =
                let isDigitOrPlus (c:char) = "0123456789+".Contains c

                charArray
                |> Array.filter (isDigitOrPlus >> not)

            pin.To10DigitString()
            |> (surroundEachChar charsWithoutPlus)
            |> SwedishPersonalIdentityNumber.Parse =! pin

    testProp "roundtrip for 10 digit string without hyphen delimiter, mixed with 'non-digits' except plus"
        <| fun (Gen.Pin.ValidPin pin, Gen.Char100 charArray) ->
            let charsWithoutPlus =
                let isDigitOrPlus (c:char) = "0123456789+".Contains c

                charArray
                |> Array.filter (isDigitOrPlus >> not)

            pin.To10DigitString()
            |> removeHyphen
            |> (surroundEachChar charsWithoutPlus)
            |> SwedishPersonalIdentityNumber.Parse =! pin

    testPropWithMaxTest 400 "roundtrip for 12 digit string 'in specific year'" <| fun (Gen.Pin.ValidPin pin) ->
        let offset = rng.Next (0, 200)
        let year = pin.Year + offset

        (pin.To12DigitString(), year)
        |> SwedishPersonalIdentityNumber.ParseInSpecificYear =! pin

    testPropWithMaxTest 400 "roundtrip for 10 digit string 'in specific year'" <| fun (Gen.Pin.ValidPin pin) ->
        let offset = rng.Next (0, 200)
        let year = pin.Year + offset

        (pin.To10DigitStringInSpecificYear year, year)
        |> SwedishPersonalIdentityNumber.ParseInSpecificYear = pin

    testPropWithMaxTest 400 "roundtrip for 10 digit string without hyphen delimeter 'in specific year'"
        <| fun (Gen.Pin.ValidPin pin) ->
            let offset = rng.Next (0, 200)
            let year = pin.Year + offset

            let str =
                pin.To10DigitStringInSpecificYear year
                |> removeHyphen
            SwedishPersonalIdentityNumber.ParseInSpecificYear(str, year) =! pin
    ]

let invalidPinTests = testList "invalid pins" [
    test "null string throws" {
        toAction SwedishPersonalIdentityNumber.Parse null
        |> Expect.throwsWithType<ArgumentNullException>
        |> ignore
    }

    testProp "empty string returns throws" <| fun (Gen.EmptyString str) ->
        toAction SwedishPersonalIdentityNumber.Parse str
        |> Expect.throwsWithType<FormatException>
        |> Expect.throwsWithMessage
            "String was not recognized as a valid SwedishPersonalIdentityNumber. Cannot be empty string or whitespace."

    testProp "invalid number of digits throws" <| fun (Gen.Digits digits) ->
        isInvalidNumberOfDigits digits ==>
            lazy
                ( toAction SwedishPersonalIdentityNumber.Parse digits
                  |> Expect.throwsWithType<FormatException>
                  |> Expect.throwsWithMessage "String was not recognized as a valid SwedishPersonalIdentityNumber." )

    testProp "invalid pin returns throws" <| fun (Gen.Pin.InvalidPinString str) ->
        toAction SwedishPersonalIdentityNumber.Parse str
        |> Expect.throwsWithMessage "String was not recognized as a valid SwedishPersonalIdentityNumber."

    testProp "parseInSpecificYear with empty string throws" <| fun (Gen.EmptyString str, Gen.ValidYear year) ->
        toAction SwedishPersonalIdentityNumber.ParseInSpecificYear (str, year)
        |> Expect.throwsWithType<FormatException>
        |> Expect.throwsWithMessage "String was not recognized as a valid SwedishPersonalIdentityNumber. Cannot be empty string or whitespace."

    testProp "parseInSpecificYear with null string throws" <| fun (Gen.ValidYear year) ->
        toAction SwedishPersonalIdentityNumber.ParseInSpecificYear (null, year)
        |> Expect.throwsWithType<ArgumentNullException>
        |> ignore

    testPropWithMaxTest 400 "cannot convert a pin to 10 digit string in a specific year when the person would be 200 years or older"
        <| fun (Gen.Pin.ValidPin pin) ->
            let offset =
                let maxYear = System.DateTime.MaxValue.Year - pin.Year
                rng.Next(200, maxYear)
            let year = pin.Year + offset

            toAction pin.To10DigitStringInSpecificYear year
            |> Expect.throwsWithMessage "SerializationYear cannot be a more than 199 years after the person was born"

    testPropWithMaxTest 400 "cannot convert a pin to 10 digit string in a specific year before the person was born"
        <| fun (Gen.Pin.ValidPin pin) ->
            let offset = rng.Next(1, pin.Year)
            let year = pin.Year - offset

            toAction pin.To10DigitStringInSpecificYear year
            |> Expect.throwsWithMessage "SerializationYear cannot be a year before the person was born" ]

[<Tests>]
let tests = testList "SwedishPersonalIdentityNumber.parse" [ validPinTests ; invalidPinTests ]
