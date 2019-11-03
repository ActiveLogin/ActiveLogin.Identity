module ActiveLogin.Identity.Swedish.FSharp.Test.SwedishCoordinationNumber_parse

open Swensen.Unquote
open Expecto
open ActiveLogin.Identity.Swedish.FSharp
open FsCheck

let private isInvalidNumberOfDigits (str: string) =
    if System.String.IsNullOrWhiteSpace str then false
    else
        str
        |> String.filter isDigit
        |> (fun s -> s.Length <> 10 && s.Length <> 12)

let private validNumberTests = testList "valid coordination numbers" [
    testProp "roundtrip for 12 digit string" <| fun (Gen.CoordNum.ValidNum num) ->
        num
        |> SwedishCoordinationNumber.to12DigitString
        |> SwedishCoordinationNumber.parse =! Ok num

    testProp "roundtrip for 10 digit string with delimiter" <| fun (Gen.CoordNum.ValidNum num) ->
        num
        |> SwedishCoordinationNumber.to10DigitString
        |> Result.bind SwedishCoordinationNumber.parse =! Ok num

    testProp "roundtrip for 10 digit string without hyphen-delimiter" <| fun (Gen.CoordNum.ValidNum num) ->
        num
        |> SwedishCoordinationNumber.to10DigitString
        |> Result.map removeHyphen
        |> Result.bind SwedishCoordinationNumber.parse =! Ok num

    testProp "roundtrip for 12 digit string mixed with 'non-digits'"
        <| fun (Gen.CoordNum.ValidNum num, Gen.Char100 charArray) ->
            let charsWithoutDigits =
                charArray
                |> Array.filter (isDigit >> not)

            num
            |> SwedishCoordinationNumber.to12DigitString
            |> surroundEachChar charsWithoutDigits
            |> SwedishCoordinationNumber.parse =! Ok num

    testProp "roundtrip for 10 digit string mixed with 'non-digits' except plus"
        <| fun (Gen.CoordNum.ValidNum num, Gen.Char100 charArray) ->
            let charsWithoutPlus =
                let isDigitOrPlus (c:char) = "0123456789+".Contains c

                charArray
                |> Array.filter (isDigitOrPlus >> not)

            num
            |> SwedishCoordinationNumber.to10DigitString
            |> Result.map (surroundEachChar charsWithoutPlus)
            |> Result.bind SwedishCoordinationNumber.parse =! Ok num

    testProp "roundtrip for 10 digit string without hyphen delimiter, mixed with 'non-digits' except plus"
        <| fun (Gen.CoordNum.ValidNum num, Gen.Char100 charArray) ->
            let charsWithoutPlus =
                let isDigitOrPlus (c:char) = "0123456789+".Contains c

                charArray
                |> Array.filter (isDigitOrPlus >> not)

            num
            |> SwedishCoordinationNumber.to10DigitString
            |> Result.map removeHyphen
            |> Result.map (surroundEachChar charsWithoutPlus)
            |> Result.bind SwedishCoordinationNumber.parse =! Ok num

    testPropWithMaxTest 400 "roundtrip for 12 digit string 'in specific year'" <| fun (Gen.CoordNum.ValidNum num) ->
        let offset = rng.Next (0, 200)
        let year = num.Year |> Year.map ((+) offset)

        num
        |> SwedishCoordinationNumber.to12DigitString
        |> SwedishCoordinationNumber.parseInSpecificYear year =! Ok num

    testPropWithMaxTest 400 "roundtrip for 10 digit string 'in specific year'" <| fun (Gen.CoordNum.ValidNum num) ->
        let offset = rng.Next (0, 200)
        let year = num.Year |> Year.map ((+) offset)

        num
        |> SwedishCoordinationNumber.to10DigitStringInSpecificYear year
        |> Result.bind (SwedishCoordinationNumber.parseInSpecificYear year) = Ok num

    testPropWithMaxTest 400 "roundtrip for 10 digit string without hyphen delimeter 'in specific year'"
        <| fun (Gen.CoordNum.ValidNum num) ->
            let offset = rng.Next (0, 200)
            let year = num.Year |> Year.map ((+) offset)

            num
            |> SwedishCoordinationNumber.to10DigitStringInSpecificYear year
            |> Result.map removeHyphen
            |> Result.bind (SwedishCoordinationNumber.parseInSpecificYear year) = Ok num
    ]

let invalidNumberTests = testList "invalid coordination numbers" [
    test "null string returns argument null error" {
        null
        |> SwedishCoordinationNumber.parse =! Error ArgumentNullError }

    testProp "empty string returns parsing error" <| fun (Gen.EmptyString str) ->
        str
        |> SwedishCoordinationNumber.parse =! Error (ParsingError Empty)

    testProp "invalid number of digits returns parsing error" <| fun (Gen.Digits digits) ->
        isInvalidNumberOfDigits digits ==>
            lazy (digits
                  |> SwedishCoordinationNumber.parse =! Error (ParsingError Length))

    testProp "invalid num returns parsing error" <| fun (Gen.CoordNum.InvalidNumString str) ->
        match SwedishCoordinationNumber.parse str with
        | Error (ParsingError (Invalid _)) -> true
        | _ -> failwith "Did not return expected error"

    testProp "parseInSpecificYear with empty string returns parsing error" <| fun (Gen.EmptyString str, Gen.ValidYear year) ->
        let y = Year.createOrFail year
        str
        |> SwedishCoordinationNumber.parseInSpecificYear y =! Error (ParsingError Empty)

    testProp "parseInSpecificYear with null string returns argument null error" <| fun (Gen.ValidYear year) ->
        let y = Year.createOrFail year
        null
        |> SwedishCoordinationNumber.parseInSpecificYear y =! Error ArgumentNullError

    testPropWithMaxTest 400 "cannot convert a num to 10 digit string in a specific year when the person would be 200 years or older"
        <| fun (Gen.CoordNum.ValidNum num) ->
            let offset =
                let maxYear = System.DateTime.MaxValue.Year - num.Year.Value
                rng.Next(200, maxYear)
            let year = num.Year |> Year.map (fun year -> year + offset)

            num
            |> SwedishCoordinationNumber.to10DigitStringInSpecificYear year =! Error (InvalidSerializationYear "SerializationYear cannot be a more than 199 years after the person was born")

    testPropWithMaxTest 400 "cannot convert a num to 10 digit string in a specific year before the person was born"
        <| fun (Gen.CoordNum.ValidNum num) ->
            let offset = rng.Next(1, num.Year.Value)
            let year = num.Year |> Year.map (fun year -> year - offset)

            num
            |> SwedishCoordinationNumber.to10DigitStringInSpecificYear year =! Error (InvalidSerializationYear "SerializationYear cannot be a year before the person was born") ]

[<Tests>]
let tests = testList "SwedishCoordinationNumber.parse" [ validNumberTests ; invalidNumberTests ]
