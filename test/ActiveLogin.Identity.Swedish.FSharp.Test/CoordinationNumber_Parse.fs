module ActiveLogin.Identity.Swedish.FSharp.Test.CoordinationNumber_Parse

open System
open Swensen.Unquote
open Expecto
open ActiveLogin.Identity.Swedish
open FsCheck
open PinTestHelpers

let parse str = CoordinationNumber.Parse(str, StrictMode.Off)
let parseInSpecificYear(str, year) = CoordinationNumber.ParseInSpecificYear(str, year, StrictMode.Off)

[<Tests>]
let tests =
    testList "CoordinationNumber.parse" [
        testList "valid coordination numbers" [
            testProp "roundtrip for 12 digit string" <| fun (Gen.CoordNum.ValidNum num) ->
                num.To12DigitString()
                |> parse =! num
            testProp "roundtrip for 10 digit string with delimiter" <| fun (Gen.CoordNum.ValidNum num) ->
                num.To10DigitString()
                |> parse =! num
            testProp "roundtrip for 10 digit string without hyphen-delimiter" <| fun (Gen.CoordNum.ValidNum num) ->
                num.To10DigitString()
                |> removeHyphen
                |> parse =! num
            testProp "roundtrip for 12 digit string mixed with 'non-digits'"
                <| fun (Gen.CoordNum.ValidNum num, Gen.Char100 charArray) ->
                    let charsWithoutDigits =
                        charArray
                        |> Array.filter (isDigit >> not)
                    num.To12DigitString()
                    |> surroundEachChar charsWithoutDigits
                    |> parse =! num
            testProp "roundtrip for 10 digit string mixed with 'non-digits' except plus"
                <| fun (Gen.CoordNum.ValidNum num, Gen.Char100 charArray) ->
                    let charsWithoutPlus =
                        let isDigitOrPlus (c:char) = "0123456789+".Contains c
                        charArray
                        |> Array.filter (isDigitOrPlus >> not)
                    num.To10DigitString()
                    |> (surroundEachChar charsWithoutPlus)
                    |> parse =! num
            testProp "roundtrip for 10 digit string without hyphen delimiter, mixed with 'non-digits' except plus"
                <| fun (Gen.CoordNum.ValidNum num, Gen.Char100 charArray) ->
                    let charsWithoutPlus =
                        let isDigitOrPlus (c:char) = "0123456789+".Contains c
                        charArray
                        |> Array.filter (isDigitOrPlus >> not)
                    num.To10DigitString()
                    |> removeHyphen
                    |> (surroundEachChar charsWithoutPlus)
                    |> parse =! num
            testPropWithMaxTest 400 "roundtrip for 12 digit string 'in specific year'" <| fun (Gen.CoordNum.ValidNum num) ->
                let offset = rng.Next (0, 200)
                let year = num.Year + offset
                let str = num.To12DigitString()
                parseInSpecificYear(str, year) =! num
            testPropWithMaxTest 400 "roundtrip for 10 digit string 'in specific year'" <| fun (Gen.CoordNum.ValidNum num) ->
                let offset = rng.Next (0, 200)
                let year = num.Year + offset
                let str = num.To10DigitStringInSpecificYear year
                parseInSpecificYear(str, year) = num
            testPropWithMaxTest 400 "roundtrip for 10 digit string without hyphen delimeter 'in specific year'"
                <| fun (Gen.CoordNum.ValidNum num) ->
                    let offset = rng.Next (0, 200)
                    let year = num.Year + offset
                    let str =
                        num.To10DigitStringInSpecificYear year
                        |> removeHyphen
                    parseInSpecificYear(str, year) = num
            ]
        testList "invalid coordination numbers" [
            test "null string returns argument null error" {
                toAction parse null
                |> Expect.throwsWithType<ArgumentNullException> |> ignore
            }
            testProp "empty string returns parsing error" <| fun (Gen.EmptyString str) ->
                toAction parse str
                |> Expect.throwsWithType<FormatException>
                |> Expect.throwsWithMessage
                    "String was not recognized as a valid IdentityNumber. Cannot be empty string or whitespace."
            testProp "invalid number of digits throws" <| fun (Gen.Digits digits) ->
                isInvalidNumberOfDigits digits ==>
                    lazy
                        ( toAction parse digits
                          |> Expect.throwsWithType<FormatException>
                          |> Expect.throwsWithMessage "String was not recognized as a valid IdentityNumber." )
            testProp "num with invalid year returns parsing error" <| fun (Gen.CoordNum.NumWithInvalidYear str) ->
                toAction parse str
                |> Expect.throwsWithMessages [ "String was not recognized as a valid IdentityNumber."; "Invalid year" ]
            testProp "num with invalid month returns parsing error" <| fun (Gen.CoordNum.NumWithInvalidMonth str) ->
                toAction parse str
                |> Expect.throwsWithMessages [ "String was not recognized as a valid IdentityNumber."; " Invalid month for coordination number"]
            testProp "num with invalid day returns parsing error" <| fun (Gen.CoordNum.NumWithInvalidDay str) ->
                toAction parse str
                |> Expect.throwsWithMessages ["String was not recognized as a valid IdentityNumber."; "Invalid coordination day"]
            testProp "num with invalid individual number returns parsing error" <| fun (Gen.CoordNum.NumWithInvalidIndividualNumber str) ->
                toAction parse str
                |> Expect.throwsWithMessages [ "String was not recognized as a valid IdentityNumber."; "Invalid individual number" ]
            testProp "num with invalid checksum returns parsing error" <| fun (Gen.CoordNum.NumWithInvalidChecksum str) ->
                toAction parse str
                |> Expect.throwsWithMessages [ "String was not recognized as a valid IdentityNumber."; "Invalid checksum" ]
            testProp "parseInSpecificYear with empty string returns parsing error" <| fun (Gen.EmptyString str, Gen.ValidYear year) ->
                toAction parseInSpecificYear (str, year)
                |> Expect.throwsWithType<FormatException>
                |> Expect.throwsWithMessage "String was not recognized as a valid IdentityNumber. Cannot be empty string or whitespace."
            testProp "parseInSpecificYear with null string throws" <| fun (Gen.ValidYear year) ->
                toAction parseInSpecificYear (null, year)
                |> Expect.throwsWithType<ArgumentNullException>
                |> ignore
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
                    |> Expect.throwsWithMessage "SerializationYear cannot be a year before the person was born"
        ]
    ]
