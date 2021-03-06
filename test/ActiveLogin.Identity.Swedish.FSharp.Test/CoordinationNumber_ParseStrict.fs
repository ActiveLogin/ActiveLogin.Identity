module ActiveLogin.Identity.Swedish.FSharp.Test.CoordinationNumber_ParseStrict

open System
open Swensen.Unquote
open Expecto
open ActiveLogin.Identity.Swedish
open FsCheck
open PinTestHelpers


let parseStrictTenDigits str = CoordinationNumber.Parse(str, StrictMode.TenDigits)
let parseStrictTenDigitsInSpecificYear (str, year) = CoordinationNumber.ParseInSpecificYear(str, year, StrictMode.TenDigits)
let parseStrictTwelveDigits str = CoordinationNumber.Parse(str, StrictMode.TwelveDigits)
let parseStrictTwelveDigitsInSpecificYear (str, year)= CoordinationNumber.ParseInSpecificYear(str, year, StrictMode.TwelveDigits)
let parseStrictTenOrTwelveDigits str = CoordinationNumber.Parse(str, StrictMode.TenOrTwelveDigits)
let parseStrictTenOrTwelveDigitsInSpecificYear (str, year) = CoordinationNumber.ParseInSpecificYear(str, year, StrictMode.TenOrTwelveDigits)


[<Tests>]
let tests =
    testList "CoordinationNumber.ParseStrict" [

        testList "valid coordination numbers - StrictMode.TenDigits" [

            testProp "roundtrip for 10 digit string with delimiter" <| fun (Gen.CoordNum.ValidNum pin) ->
                pin.To10DigitString()
                |> parseStrictTenDigits =! pin
            testProp "roundtrip for 10 digit string without hyphen-delimiter" <| fun (Gen.CoordNum.ValidNum pin) ->
                pin.To10DigitString()
                |> removeHyphen
                |> parseStrictTenDigits =! pin
            testPropWithMaxTest 400 "roundtrip for 10 digit string 'in specific year'" <| fun (Gen.CoordNum.ValidNum pin) ->
                let offset = rng.Next (0, 200)
                let year = pin.Year + offset
                (pin.To10DigitStringInSpecificYear year, year)
                |> parseStrictTenDigitsInSpecificYear =! pin
            testPropWithMaxTest 400 "roundtrip for 10 digit string without hyphen delimiter 'in specific year'"
                <| fun (Gen.CoordNum.ValidNum pin) ->
                    let offset = rng.Next (0, 200)
                    let year = pin.Year + offset
                    let str =
                        pin.To10DigitStringInSpecificYear year
                        |> removeHyphen
                    parseStrictTenDigitsInSpecificYear(str, year) =! pin
            ]

        testList "invalid coordination number - StrictMode.TenDigits" [

            testProp "12 digit string" <| fun (Gen.CoordNum.ValidNum pin) ->
                toAction parseStrictTenDigits (pin.To12DigitString())
                |> Expect.throwsWithType<FormatException>
                |> Expect.throwsWithMessage "String was not recognized as a ten digit IdentityNumber."

            testProp "string mixed with 'non-digits' except plus"
                <| fun (Gen.CoordNum.ValidNum pin, Gen.Char100 charArray) ->
                    let charsWithoutPlus =
                        let isDigitOrPlus (c:char) = "0123456789+".Contains c
                        charArray
                        |> Array.filter (isDigitOrPlus >> not)
                    let str =
                        pin.To10DigitString()
                        |> (surroundEachChar charsWithoutPlus)
                    toAction parseStrictTenDigits str
                    |> Expect.throwsWithType<FormatException>
                    |> Expect.throwsWithMessage "String was not recognized as a ten digit IdentityNumber."
            testProp "string without hyphen delimiter, mixed with 'non-digits' except plus"
                <| fun (Gen.CoordNum.ValidNum pin, Gen.Char100 charArray) ->
                    let charsWithoutPlus =
                        let isDigitOrPlus (c:char) = "0123456789+".Contains c
                        charArray
                        |> Array.filter (isDigitOrPlus >> not)
                    let str =
                        pin.To10DigitString()
                        |> removeHyphen
                        |> (surroundEachChar charsWithoutPlus)
                    toAction parseStrictTenDigits str
                    |> Expect.throwsWithType<FormatException>
                    |> Expect.throwsWithMessage "String was not recognized as a ten digit IdentityNumber."
            test "null string throws" {
                toAction parseStrictTenDigits null
                |> Expect.throwsWithType<ArgumentNullException>
                |> ignore
            }
            testProp "empty string throws" <| fun (Gen.EmptyString str) ->
                toAction parseStrictTenDigits str
                |> Expect.throwsWithType<FormatException>
                |> Expect.throwsWithMessage
                    "String was not recognized as a valid IdentityNumber. Cannot be empty string or whitespace."
            testProp "invalid number of digits throws" <| fun (Gen.Digits digits) ->
                isInvalidNumberOfDigits digits ==>
                    lazy
                        ( toAction parseStrictTenDigits digits
                          |> Expect.throwsWithType<FormatException>
                          |> Expect.throwsWithMessage "String was not recognized as a ten digit IdentityNumber." )
            testProp "pin with invalid month returns parsing error" <| fun (Gen.CoordNum.NumWithInvalidMonth str) ->
                toAction parseStrictTenDigits str.[2..]
                |> Expect.throwsWithMessages [ "String was not recognized as a valid IdentityNumber."; " Invalid month"]
            testProp "pin with invalid day returns parsing error" <| fun (Gen.CoordNum.NumWithInvalidDay str) ->
                toAction parseStrictTenDigits str.[2..]
                |> Expect.throwsWithMessages ["String was not recognized as a valid IdentityNumber."; "Invalid coordination day"]
            testProp "pin with invalid individual number returns parsing error" <| fun (Gen.CoordNum.NumWithInvalidIndividualNumber str) ->
                toAction parseStrictTenDigits str.[2..]
                |> Expect.throwsWithMessages [ "String was not recognized as a valid IdentityNumber."; "Invalid individual number" ]
            testProp "pin with invalid checksum returns parsing error" <| fun (Gen.CoordNum.NumWithInvalidChecksum str) ->
                toAction parseStrictTenDigits str.[2..]
                |> Expect.throwsWithMessages [ "String was not recognized as a valid IdentityNumber."; "Invalid checksum" ]
            testProp "parseInSpecificYear with empty string throws" <| fun (Gen.EmptyString str, Gen.ValidYear year) ->
                toAction parseStrictTenDigitsInSpecificYear (str, year)
                |> Expect.throwsWithType<FormatException>
                |> Expect.throwsWithMessage "String was not recognized as a valid IdentityNumber. Cannot be empty string or whitespace."
            testProp "parseInSpecificYear with null string throws" <| fun (Gen.ValidYear year) ->
                toAction parseStrictTenDigitsInSpecificYear (null, year)
                |> Expect.throwsWithType<ArgumentNullException>
                |> ignore
        ]

        testList "valid coordination numbers - StrictMode.TwelveDigits" [
            testProp "roundtrip for 12 digit string" <| fun (Gen.CoordNum.ValidNum pin) ->
                pin.To12DigitString()
                |> parseStrictTwelveDigits =! pin
            testPropWithMaxTest 400 "roundtrip for 12 digit string 'in specific year'" <| fun (Gen.CoordNum.ValidNum pin) ->
                let offset = rng.Next (0, 200)
                let year = pin.Year + offset
                (pin.To12DigitString(), year)
                |> parseStrictTwelveDigitsInSpecificYear =! pin
            ]


        testList "invalid coordination number - StrictMode.TwelveDigits" [

            testProp "10 digit string without hyphen-delimiter" <| fun (Gen.CoordNum.ValidNum pin) ->
                pin.To10DigitString()
                |> removeHyphen
                |> toAction parseStrictTwelveDigits
                |> Expect.throwsWithType<FormatException>
                |> Expect.throwsWithMessage "String was not recognized as a twelve digit IdentityNumber."

            testProp "10 digit string mixed with 'non-digits' except plus"
                <| fun (Gen.CoordNum.ValidNum pin, Gen.Char100 charArray) ->
                    let charsWithoutPlus =
                        let isDigitOrPlus (c:char) = "0123456789+".Contains c
                        charArray
                        |> Array.filter (isDigitOrPlus >> not)
                    pin.To10DigitString()
                    |> (surroundEachChar charsWithoutPlus)
                    |> toAction parseStrictTwelveDigits
                    |> Expect.throwsWithType<FormatException>
                    |> Expect.throwsWithMessage "String was not recognized as a twelve digit IdentityNumber."

            testProp "10 digit string with delimiter" <| fun (Gen.CoordNum.ValidNum pin) ->
                pin.To10DigitString()
                |> toAction parseStrictTwelveDigits
                |> Expect.throwsWithType<FormatException>
                |> Expect.throwsWithMessage "String was not recognized as a twelve digit IdentityNumber."

            testPropWithMaxTest 400 "10 digit string 'in specific year'" <| fun (Gen.CoordNum.ValidNum pin) ->
                let offset = rng.Next (0, 200)
                let year = pin.Year + offset
                (pin.To10DigitStringInSpecificYear year, year)
                |> toAction parseStrictTwelveDigitsInSpecificYear
                |> Expect.throwsWithType<FormatException>
                |> Expect.throwsWithMessage "String was not recognized as a twelve digit IdentityNumber."

            testProp "12 digit string mixed with 'non-digits'"
                <| fun (Gen.CoordNum.ValidNum pin, Gen.Char100 charArray) ->
                    let charsWithoutDigits =
                        charArray
                        |> Array.filter (isDigit >> not)
                    pin.To12DigitString()
                    |> surroundEachChar charsWithoutDigits
                    |> toAction parseStrictTwelveDigits
                    |> Expect.throwsWithType<FormatException>
                    |> Expect.throwsWithMessage "String was not recognized as a twelve digit IdentityNumber."

            testPropWithMaxTest 400 "10 digit string without hyphen delimiter 'in specific year'"
                <| fun (Gen.CoordNum.ValidNum pin) ->
                    let offset = rng.Next (0, 200)
                    let year = pin.Year + offset
                    let str =
                        pin.To10DigitStringInSpecificYear year
                        |> removeHyphen
                    toAction parseStrictTwelveDigitsInSpecificYear (str, year)
                    |> Expect.throwsWithType<FormatException>
                    |> Expect.throwsWithMessage "String was not recognized as a twelve digit IdentityNumber."

            test "null string throws" {
                toAction parseStrictTwelveDigits null
                |> Expect.throwsWithType<ArgumentNullException>
                |> ignore
            }
            testProp "empty string throws" <| fun (Gen.EmptyString str) ->
                toAction parseStrictTwelveDigits str
                |> Expect.throwsWithType<FormatException>
                |> Expect.throwsWithMessage
                    "String was not recognized as a valid IdentityNumber. Cannot be empty string or whitespace."
            testProp "invalid number of digits throws" <| fun (Gen.Digits digits) ->
                isInvalidNumberOfDigits digits ==>
                    lazy
                        ( toAction parseStrictTwelveDigits digits
                          |> Expect.throwsWithType<FormatException>
                          |> Expect.throwsWithMessage "String was not recognized as a twelve digit IdentityNumber." )
            testProp "pin with invalid year returns parsing error" <| fun (Gen.CoordNum.NumWithInvalidYear str) ->
                toAction parseStrictTwelveDigits str
                |> Expect.throwsWithMessages [ "String was not recognized as a valid IdentityNumber."; "Invalid year" ]
            testProp "pin with invalid month returns parsing error" <| fun (Gen.CoordNum.NumWithInvalidMonth str) ->
                toAction parseStrictTwelveDigits str
                |> Expect.throwsWithMessages [ "String was not recognized as a valid IdentityNumber."; " Invalid month"]
            testProp "pin with invalid day returns parsing error" <| fun (Gen.CoordNum.NumWithInvalidDay str) ->
                toAction parseStrictTwelveDigits str
                |> Expect.throwsWithMessages ["String was not recognized as a valid IdentityNumber."; "Invalid coordination day"]
            testProp "pin with invalid individual number returns parsing error" <| fun (Gen.CoordNum.NumWithInvalidIndividualNumber str) ->
                toAction parseStrictTwelveDigits str
                |> Expect.throwsWithMessages [ "String was not recognized as a valid IdentityNumber."; "Invalid individual number" ]
            testProp "pin with invalid checksum returns parsing error" <| fun (Gen.CoordNum.NumWithInvalidChecksum str) ->
                toAction parseStrictTwelveDigits str
                |> Expect.throwsWithMessages [ "String was not recognized as a valid IdentityNumber."; "Invalid checksum" ]
            testProp "parseInSpecificYear with empty string throws" <| fun (Gen.EmptyString str, Gen.ValidYear year) ->
                toAction parseStrictTwelveDigitsInSpecificYear (str, year)
                |> Expect.throwsWithType<FormatException>
                |> Expect.throwsWithMessage "String was not recognized as a valid IdentityNumber. Cannot be empty string or whitespace."
            testProp "parseInSpecificYear with null string throws" <| fun (Gen.ValidYear year) ->
                toAction parseStrictTwelveDigitsInSpecificYear (null, year)
                |> Expect.throwsWithType<ArgumentNullException>
                |> ignore
        ]

        testList "valid coordination numbers - StrictMode.TenOrTwelveDigits" [

            testProp "roundtrip for 10 digit string with delimiter" <| fun (Gen.CoordNum.ValidNum pin) ->
                pin.To10DigitString()
                |> parseStrictTenOrTwelveDigits =! pin
            testProp "roundtrip for 10 digit string without hyphen-delimiter" <| fun (Gen.CoordNum.ValidNum pin) ->
                pin.To10DigitString()
                |> removeHyphen
                |> parseStrictTenOrTwelveDigits =! pin
            testPropWithMaxTest 400 "roundtrip for 10 digit string 'in specific year'" <| fun (Gen.CoordNum.ValidNum pin) ->
                let offset = rng.Next (0, 200)
                let year = pin.Year + offset
                (pin.To10DigitStringInSpecificYear year, year)
                |> parseStrictTenOrTwelveDigitsInSpecificYear =! pin
            testPropWithMaxTest 400 "roundtrip for 10 digit string without hyphen delimiter 'in specific year'"
                <| fun (Gen.CoordNum.ValidNum pin) ->
                    let offset = rng.Next (0, 200)
                    let year = pin.Year + offset
                    let str =
                        pin.To10DigitStringInSpecificYear year
                        |> removeHyphen
                    parseStrictTenOrTwelveDigitsInSpecificYear(str, year) =! pin

            testProp "roundtrip for 12 digit string" <| fun (Gen.CoordNum.ValidNum pin) ->
                pin.To12DigitString()
                |> parseStrictTenOrTwelveDigits =! pin
            testPropWithMaxTest 400 "roundtrip for 12 digit string 'in specific year'" <| fun (Gen.CoordNum.ValidNum pin) ->
                let offset = rng.Next (0, 200)
                let year = pin.Year + offset
                (pin.To12DigitString(), year)
                |> parseStrictTenOrTwelveDigitsInSpecificYear =! pin
            ]

        testList "invalid coordination number - StrictMode.TenOrTwelveDigits" [

            testProp "string mixed with 'non-digits' except plus"
                <| fun (Gen.CoordNum.ValidNum pin, Gen.Char100 charArray) ->
                    let charsWithoutPlus =
                        let isDigitOrPlus (c:char) = "0123456789+".Contains c
                        charArray
                        |> Array.filter (isDigitOrPlus >> not)
                    let str =
                        pin.To10DigitString()
                        |> (surroundEachChar charsWithoutPlus)
                    toAction parseStrictTenOrTwelveDigits str
                    |> Expect.throwsWithType<FormatException>
                    |> Expect.throwsWithMessage "String was not recognized as a valid IdentityNumber."
            testProp "string without hyphen delimiter, mixed with 'non-digits' except plus"
                <| fun (Gen.CoordNum.ValidNum pin, Gen.Char100 charArray) ->
                    let charsWithoutPlus =
                        let isDigitOrPlus (c:char) = "0123456789+".Contains c
                        charArray
                        |> Array.filter (isDigitOrPlus >> not)
                    let str =
                        pin.To10DigitString()
                        |> removeHyphen
                        |> (surroundEachChar charsWithoutPlus)
                    toAction parseStrictTenOrTwelveDigits str
                    |> Expect.throwsWithType<FormatException>
                    |> Expect.throwsWithMessage "String was not recognized as a valid IdentityNumber."
            test "null string throws" {
                toAction parseStrictTenOrTwelveDigits null
                |> Expect.throwsWithType<ArgumentNullException>
                |> ignore
            }
            testProp "empty string returns throws" <| fun (Gen.EmptyString str) ->
                toAction parseStrictTenOrTwelveDigits str
                |> Expect.throwsWithType<FormatException>
                |> Expect.throwsWithMessage
                    "String was not recognized as a valid IdentityNumber. Cannot be empty string or whitespace."
            testProp "invalid number of digits throws" <| fun (Gen.Digits digits) ->
                isInvalidNumberOfDigits digits ==>
                    lazy
                        ( toAction parseStrictTenOrTwelveDigits digits
                          |> Expect.throwsWithType<FormatException>
                          |> Expect.throwsWithMessage "String was not recognized as a valid IdentityNumber." )
            testProp "pin with invalid year returns parsing error" <| fun (Gen.CoordNum.NumWithInvalidYear str) ->
                toAction parseStrictTenOrTwelveDigits str
                |> Expect.throwsWithMessages [ "String was not recognized as a valid IdentityNumber."; "Invalid year" ]
            testProp "pin with invalid month returns parsing error" <| fun (Gen.CoordNum.NumWithInvalidMonth str) ->
                toAction parseStrictTenOrTwelveDigits str
                |> Expect.throwsWithMessages [ "String was not recognized as a valid IdentityNumber."; " Invalid month"]
            testProp "pin with invalid day returns parsing error" <| fun (Gen.CoordNum.NumWithInvalidDay str) ->
                toAction parseStrictTenOrTwelveDigits str
                |> Expect.throwsWithMessages ["String was not recognized as a valid IdentityNumber."; "Invalid coordination day"]
            testProp "pin with invalid individual number returns parsing error" <| fun (Gen.CoordNum.NumWithInvalidIndividualNumber str) ->
                toAction parseStrictTenOrTwelveDigits str
                |> Expect.throwsWithMessages [ "String was not recognized as a valid IdentityNumber."; "Invalid individual number" ]
            testProp "pin with invalid checksum returns parsing error" <| fun (Gen.CoordNum.NumWithInvalidChecksum str) ->
                toAction parseStrictTenOrTwelveDigits str
                |> Expect.throwsWithMessages [ "String was not recognized as a valid IdentityNumber."; "Invalid checksum" ]
            testProp "parseInSpecificYear with empty string throws" <| fun (Gen.EmptyString str, Gen.ValidYear year) ->
                toAction parseStrictTenOrTwelveDigitsInSpecificYear (str, year)
                |> Expect.throwsWithType<FormatException>
                |> Expect.throwsWithMessage "String was not recognized as a valid IdentityNumber. Cannot be empty string or whitespace."
            testProp "parseInSpecificYear with null string throws" <| fun (Gen.ValidYear year) ->
                toAction parseStrictTenOrTwelveDigitsInSpecificYear (null, year)
                |> Expect.throwsWithType<ArgumentNullException>
                |> ignore

            testProp "10 digit string mixed with 'non-digits' except plus"
                <| fun (Gen.CoordNum.ValidNum pin, Gen.Char100 charArray) ->
                    let charsWithoutPlus =
                        let isDigitOrPlus (c:char) = "0123456789+".Contains c
                        charArray
                        |> Array.filter (isDigitOrPlus >> not)
                    pin.To10DigitString()
                    |> (surroundEachChar charsWithoutPlus)
                    |> toAction parseStrictTenOrTwelveDigits
                    |> Expect.throwsWithType<FormatException>
                    |> Expect.throwsWithMessage "String was not recognized as a valid IdentityNumber."

            testProp "12 digit string mixed with 'non-digits'"
                <| fun (Gen.CoordNum.ValidNum pin, Gen.Char100 charArray) ->
                    let charsWithoutDigits =
                        charArray
                        |> Array.filter (isDigit >> not)
                    pin.To12DigitString()
                    |> surroundEachChar charsWithoutDigits
                    |> toAction parseStrictTenOrTwelveDigits
                    |> Expect.throwsWithType<FormatException>
                    |> Expect.throwsWithMessage "String was not recognized as a valid IdentityNumber."
        ]
    ]
