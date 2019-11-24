module ActiveLogin.Identity.Swedish.FSharp.Test.SwedishPersonalIdentityNumber_parse

open System
open Swensen.Unquote
open Expecto
open ActiveLogin.Identity.Swedish.FSharp
open ActiveLogin.Identity.Swedish.FSharp.TestData
open FsCheck


let private isInvalidNumberOfDigits (str: string) =
    if System.String.IsNullOrWhiteSpace str then false
    else
        str
        |> String.filter isDigit
        |> (fun s -> s.Length <> 10 && s.Length <> 12)

let private validPinTests = testList "valid pins" [
    testProp "roundtrip for 12 digit string" <| fun (Gen.Pin.ValidPin pin) ->
        pin
        |> SwedishPersonalIdentityNumber.to12DigitString
        |> SwedishPersonalIdentityNumber.parse =! pin

    testProp "roundtrip for 10 digit string with delimiter" <| fun (Gen.Pin.ValidPin pin) ->
        pin
        |> SwedishPersonalIdentityNumber.to10DigitString
        |> SwedishPersonalIdentityNumber.parse =! pin

    testProp "roundtrip for 10 digit string without hyphen-delimiter" <| fun (Gen.Pin.ValidPin pin) ->
        pin
        |> SwedishPersonalIdentityNumber.to10DigitString
        |> removeHyphen
        |> SwedishPersonalIdentityNumber.parse =! pin

    testProp "roundtrip for 12 digit string mixed with 'non-digits'"
        <| fun (Gen.Pin.ValidPin pin, Gen.Char100 charArray) ->
            let charsWithoutDigits =
                charArray
                |> Array.filter (isDigit >> not)

            pin
            |> SwedishPersonalIdentityNumber.to12DigitString
            |> surroundEachChar charsWithoutDigits
            |> SwedishPersonalIdentityNumber.parse =! pin

    testProp "roundtrip for 10 digit string mixed with 'non-digits' except plus"
        <| fun (Gen.Pin.ValidPin pin, Gen.Char100 charArray) ->
            let charsWithoutPlus =
                let isDigitOrPlus (c:char) = "0123456789+".Contains c

                charArray
                |> Array.filter (isDigitOrPlus >> not)

            pin
            |> SwedishPersonalIdentityNumber.to10DigitString
            |> (surroundEachChar charsWithoutPlus)
            |> SwedishPersonalIdentityNumber.parse =! pin

    testProp "roundtrip for 10 digit string without hyphen delimiter, mixed with 'non-digits' except plus"
        <| fun (Gen.Pin.ValidPin pin, Gen.Char100 charArray) ->
            let charsWithoutPlus =
                let isDigitOrPlus (c:char) = "0123456789+".Contains c

                charArray
                |> Array.filter (isDigitOrPlus >> not)

            pin
            |> SwedishPersonalIdentityNumber.to10DigitString
            |> removeHyphen
            |> (surroundEachChar charsWithoutPlus)
            |> SwedishPersonalIdentityNumber.parse =! pin

    testPropWithMaxTest 400 "roundtrip for 12 digit string 'in specific year'" <| fun (Gen.Pin.ValidPin pin) ->
        let offset = rng.Next (0, 200)
        let year = pin.Year + offset

        pin
        |> SwedishPersonalIdentityNumber.to12DigitString
        |> SwedishPersonalIdentityNumber.parseInSpecificYear year =! pin

    testPropWithMaxTest 400 "roundtrip for 10 digit string 'in specific year'" <| fun (Gen.Pin.ValidPin pin) ->
        let offset = rng.Next (0, 200)
        let year = pin.Year + offset

        pin
        |> SwedishPersonalIdentityNumber.to10DigitStringInSpecificYear year
        |> (SwedishPersonalIdentityNumber.parseInSpecificYear year) = pin

    testPropWithMaxTest 400 "roundtrip for 10 digit string without hyphen delimeter 'in specific year'"
        <| fun (Gen.Pin.ValidPin pin) ->
            let offset = rng.Next (0, 200)
            let year = pin.Year + offset

            pin
            |> SwedishPersonalIdentityNumber.to10DigitStringInSpecificYear year
            |> removeHyphen
            |> (SwedishPersonalIdentityNumber.parseInSpecificYear year) = pin
    ]

let invalidPinTests = testList "invalid pins" [
    test "null string returns argument null error" {
        toAction SwedishPersonalIdentityNumber.parse null
        |> Expect.throwsWithType<ArgumentNullException>
    }

    testProp "empty string returns parsing error" <| fun (Gen.EmptyString str) ->
        let f = toAction SwedishPersonalIdentityNumber.parse str
        Expect.throwsWithType<FormatException> f
        Expect.throwsWithMessage
            "String was not recognized as a valid SwedishPersonalIdentityNumber. Cannot be empty string or whitespace."
            f

    testProp "invalid number of digits returns parsing error" <| fun (Gen.Digits digits) ->
        isInvalidNumberOfDigits digits ==>
            lazy
                ( let f = toAction SwedishPersonalIdentityNumber.parse digits
                  Expect.throwsWithType<FormatException> f
                  Expect.throwsWithMessage "String was not recognized as a valid SwedishPersonalIdentityNumber." f )

    testProp "invalid pin returns parsing error" <| fun (Gen.Pin.InvalidPinString str) ->
        toAction SwedishPersonalIdentityNumber.parse str
        |> Expect.throwsWithMessage "String was not recognized as a valid SwedishPersonalIdentityNumber."

    testProp "parseInSpecificYear with empty string returns parsing error" <| fun (Gen.EmptyString str, Gen.ValidYear year) ->
        let f  = toAction2 SwedishPersonalIdentityNumber.parseInSpecificYear year str
        Expect.throwsWithType<FormatException> f
        Expect.throwsWithMessage "String was not recognized as a valid SwedishPersonalIdentityNumber. Cannot be empty string or whitespace." f

    testProp "parseInSpecificYear with null string returns argument null error" <| fun (Gen.ValidYear year) ->
        toAction2 SwedishPersonalIdentityNumber.parseInSpecificYear year null
        |> Expect.throwsWithType<ArgumentNullException>

    testPropWithMaxTest 400 "cannot convert a pin to 10 digit string in a specific year when the person would be 200 years or older"
        <| fun (Gen.Pin.ValidPin pin) ->
            let offset =
                let maxYear = System.DateTime.MaxValue.Year - pin.Year
                rng.Next(200, maxYear)
            let year = pin.Year + offset

            toAction2 SwedishPersonalIdentityNumber.to10DigitStringInSpecificYear year pin
            |> Expect.throwsWithMessage "SerializationYear cannot be a more than 199 years after the person was born"

    testPropWithMaxTest 400 "cannot convert a pin to 10 digit string in a specific year before the person was born"
        <| fun (Gen.Pin.ValidPin pin) ->
            let offset = rng.Next(1, pin.Year)
            let year = pin.Year - offset

            toAction2 SwedishPersonalIdentityNumber.to10DigitStringInSpecificYear year pin
            |> Expect.throwsWithMessage "SerializationYear cannot be a year before the person was born" ]

[<Tests>]
let tests = testList "SwedishPersonalIdentityNumber.parse" [ validPinTests ; invalidPinTests ]
