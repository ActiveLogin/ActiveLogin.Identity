module ActiveLogin.Identity.Swedish.FSharp.Test.PersonalIdentityNumber_Parse

open System
open Swensen.Unquote
open Expecto
open ActiveLogin.Identity.Swedish
open FsCheck


[<Tests>]
let tests =
    testList "SwedishPersonalIdentityNumber.parse" [
        testList "valid personal identity numbers" [
            testProp "roundtrip for 12 digit string" <| fun (Gen.Pin.ValidPin pin) ->
                pin.To12DigitString()
                |> PersonalIdentityNumber.Parse =! pin
            testProp "roundtrip for 10 digit string with delimiter" <| fun (Gen.Pin.ValidPin pin) ->
                pin.To10DigitString()
                |> PersonalIdentityNumber.Parse =! pin
            testProp "roundtrip for 10 digit string without hyphen-delimiter" <| fun (Gen.Pin.ValidPin pin) ->
                pin.To10DigitString()
                |> removeHyphen
                |> PersonalIdentityNumber.Parse =! pin
            testProp "roundtrip for 12 digit string mixed with 'non-digits'"
                <| fun (Gen.Pin.ValidPin pin, Gen.Char100 charArray) ->
                    let charsWithoutDigits =
                        charArray
                        |> Array.filter (isDigit >> not)
                    pin.To12DigitString()
                    |> surroundEachChar charsWithoutDigits
                    |> PersonalIdentityNumber.Parse =! pin
            testProp "roundtrip for 10 digit string mixed with 'non-digits' except plus"
                <| fun (Gen.Pin.ValidPin pin, Gen.Char100 charArray) ->
                    let charsWithoutPlus =
                        let isDigitOrPlus (c:char) = "0123456789+".Contains c
                        charArray
                        |> Array.filter (isDigitOrPlus >> not)
                    pin.To10DigitString()
                    |> (surroundEachChar charsWithoutPlus)
                    |> PersonalIdentityNumber.Parse =! pin
            testProp "roundtrip for 10 digit string without hyphen delimiter, mixed with 'non-digits' except plus"
                <| fun (Gen.Pin.ValidPin pin, Gen.Char100 charArray) ->
                    let charsWithoutPlus =
                        let isDigitOrPlus (c:char) = "0123456789+".Contains c
                        charArray
                        |> Array.filter (isDigitOrPlus >> not)
                    pin.To10DigitString()
                    |> removeHyphen
                    |> (surroundEachChar charsWithoutPlus)
                    |> PersonalIdentityNumber.Parse =! pin
            testPropWithMaxTest 400 "roundtrip for 12 digit string 'in specific year'" <| fun (Gen.Pin.ValidPin pin) ->
                let offset = rng.Next (0, 200)
                let year = pin.Year + offset
                (pin.To12DigitString(), year)
                |> PersonalIdentityNumber.ParseInSpecificYear =! pin
            testPropWithMaxTest 400 "roundtrip for 10 digit string 'in specific year'" <| fun (Gen.Pin.ValidPin pin) ->
                let offset = rng.Next (0, 200)
                let year = pin.Year + offset
                (pin.To10DigitStringInSpecificYear year, year)
                |> PersonalIdentityNumber.ParseInSpecificYear =! pin
            testPropWithMaxTest 400 "roundtrip for 10 digit string without hyphen delimeter 'in specific year'"
                <| fun (Gen.Pin.ValidPin pin) ->
                    let offset = rng.Next (0, 200)
                    let year = pin.Year + offset
                    let str =
                        pin.To10DigitStringInSpecificYear year
                        |> removeHyphen
                    PersonalIdentityNumber.ParseInSpecificYear(str, year) =! pin
            ]
        testList "invalid personal identity number" [
            test "null string throws" {
                toAction PersonalIdentityNumber.Parse null
                |> Expect.throwsWithType<ArgumentNullException>
                |> ignore
            }
            testProp "empty string returns throws" <| fun (Gen.EmptyString str) ->
                toAction PersonalIdentityNumber.Parse str
                |> Expect.throwsWithType<FormatException>
                |> Expect.throwsWithMessage
                    "String was not recognized as a valid IdentityNumber. Cannot be empty string or whitespace."
            testProp "invalid number of digits throws" <| fun (Gen.Digits digits) ->
                isInvalidNumberOfDigits digits ==>
                    lazy
                        ( toAction PersonalIdentityNumber.Parse digits
                          |> Expect.throwsWithType<FormatException>
                          |> Expect.throwsWithMessage "String was not recognized as a valid IdentityNumber." )
            testProp "pin with invalid year returns parsing error" <| fun (Gen.Pin.PinWithInvalidYear str) ->
                toAction PersonalIdentityNumber.Parse str
                |> Expect.throwsWithMessages [ "String was not recognized as a valid IdentityNumber."; "Invalid year" ]
            testProp "pin with invalid month returns parsing error" <| fun (Gen.Pin.PinWithInvalidMonth str) ->
                toAction PersonalIdentityNumber.Parse str
                |> Expect.throwsWithMessages [ "String was not recognized as a valid IdentityNumber."; " Invalid month"]
            testProp "pin with invalid day returns parsing error" <| fun (Gen.Pin.PinWithInvalidDay str) ->
                toAction PersonalIdentityNumber.Parse str
                |> Expect.throwsWithMessages ["String was not recognized as a valid IdentityNumber."; "Invalid day"]
            testProp "pin with invalid individual number returns parsing error" <| fun (Gen.Pin.PinWithInvalidIndividualNumber str) ->
                toAction PersonalIdentityNumber.Parse str
                |> Expect.throwsWithMessages [ "String was not recognized as a valid IdentityNumber."; "Invalid birth number" ]
            testProp "pin with invalid checksum returns parsing error" <| fun (Gen.Pin.PinWithInvalidChecksum str) ->
                toAction PersonalIdentityNumber.Parse str
                |> Expect.throwsWithMessages [ "String was not recognized as a valid IdentityNumber."; "Invalid checksum" ]
            testProp "parseInSpecificYear with empty string throws" <| fun (Gen.EmptyString str, Gen.ValidYear year) ->
                toAction (fun (s, y) -> PersonalIdentityNumber.ParseInSpecificYear(s,y)) (str, year)
                |> Expect.throwsWithType<FormatException>
                |> Expect.throwsWithMessage "String was not recognized as a valid IdentityNumber. Cannot be empty string or whitespace."
            testProp "parseInSpecificYear with null string throws" <| fun (Gen.ValidYear year) ->
                toAction (fun (s,y) -> PersonalIdentityNumber.ParseInSpecificYear(s,y)) (null, year)
                |> Expect.throwsWithType<ArgumentNullException>
                |> ignore
            testPropWithMaxTest 400 "cannot convert a pin to 10 digit string in a specific year when the person would be 200 years or older"
                <| fun (Gen.Pin.ValidPin pin) ->
                    let offset =
                        let maxYear = DateTime.MaxValue.Year - pin.Year
                        rng.Next(200, maxYear)
                    let year = pin.Year + offset
                    toAction pin.To10DigitStringInSpecificYear year
                    |> Expect.throwsWithMessage "SerializationYear cannot be a more than 199 years after the person was born"
            testPropWithMaxTest 400 "cannot convert a pin to 10 digit string in a specific year before the person was born"
                <| fun (Gen.Pin.ValidPin pin) ->
                    let offset = rng.Next(1, pin.Year)
                    let year = pin.Year - offset
                    toAction pin.To10DigitStringInSpecificYear year
                    |> Expect.throwsWithMessage "SerializationYear cannot be a year before the person was born"
        ]
    ]
