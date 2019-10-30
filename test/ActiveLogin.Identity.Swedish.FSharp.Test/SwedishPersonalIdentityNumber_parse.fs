module ActiveLogin.Identity.Swedish.FSharp.Test.SwedishPersonalIdentityNumber_parse

open Swensen.Unquote
open Expecto
open ActiveLogin.Identity.Swedish.FSharp
open ActiveLogin.Identity.Swedish.FSharp.TestData
open FsCheck

let private getRandomFromArray arr =
    fun () ->
        let index = rng.Next(0, Array.length arr - 1)
        arr.[index]

let private removeHyphen (str:string) =
    let isHyphen (c:char) = "-".Contains(c)
    String.filter (isHyphen >> not) str

let private surroundEachChar (chars:char[]) (pin:string) =
    let rnd = getRandomFromArray chars
    let surroundWith c = [| rnd(); c; rnd() |]

    Seq.collect surroundWith pin
    |> Array.ofSeq
    |> System.String

let private isDigit (c:char) = "0123456789".Contains(c)

let private isInvalidNumberOfDigits (str: string) =
    if System.String.IsNullOrWhiteSpace str then false
    else
        str
        |> String.filter isDigit
        |> (fun s -> s.Length <> 10 && s.Length <> 12)

let private validPinTests = testList "valid pins" [
    testProp "roundtrip for 12 digit string" <| fun (Gen.ValidPin pin) ->
        pin
        |> SwedishPersonalIdentityNumber.to12DigitString
        |> SwedishPersonalIdentityNumber.parse =! Ok pin

    testProp "roundtrip for 10 digit string with delimiter" <| fun (Gen.ValidPin pin) ->
        pin
        |> SwedishPersonalIdentityNumber.to10DigitString
        |> Result.bind SwedishPersonalIdentityNumber.parse =! Ok pin

    testProp "roundtrip for 10 digit string without hyphen-delimiter" <| fun (Gen.ValidPin pin) ->
        pin
        |> SwedishPersonalIdentityNumber.to10DigitString
        |> Result.map removeHyphen
        |> Result.bind SwedishPersonalIdentityNumber.parse =! Ok pin

    testProp "roundtrip for 12 digit string mixed with 'non-digits'"
        <| fun (Gen.ValidPin pin, Gen.Char100 charArray) ->
            let charsWithoutDigits =
                charArray
                |> Array.filter (isDigit >> not)

            pin
            |> SwedishPersonalIdentityNumber.to12DigitString
            |> surroundEachChar charsWithoutDigits
            |> SwedishPersonalIdentityNumber.parse =! Ok pin

    testProp "roundtrip for 10 digit string mixed with 'non-digits' except plus"
        <| fun (Gen.ValidPin pin, Gen.Char100 charArray) ->
            let charsWithoutPlus =
                let isDigitOrPlus (c:char) = "0123456789+".Contains c

                charArray
                |> Array.filter (isDigitOrPlus >> not)

            pin
            |> SwedishPersonalIdentityNumber.to10DigitString
            |> Result.map (surroundEachChar charsWithoutPlus)
            |> Result.bind SwedishPersonalIdentityNumber.parse =! Ok pin

    testProp "roundtrip for 10 digit string without hyphen delimiter, mixed with 'non-digits' except plus"
        <| fun (Gen.ValidPin pin, Gen.Char100 charArray) ->
            let charsWithoutPlus =
                let isDigitOrPlus (c:char) = "0123456789+".Contains c

                charArray
                |> Array.filter (isDigitOrPlus >> not)

            pin
            |> SwedishPersonalIdentityNumber.to10DigitString
            |> Result.map removeHyphen
            |> Result.map (surroundEachChar charsWithoutPlus)
            |> Result.bind SwedishPersonalIdentityNumber.parse =! Ok pin

    testPropWithMaxTest 400 "roundtrip for 12 digit string 'in specific year'" <| fun (Gen.ValidPin pin) ->
        let offset = rng.Next (0, 200)
        let year = pin.Year |> Year.map ((+) offset)

        pin
        |> SwedishPersonalIdentityNumber.to12DigitString
        |> SwedishPersonalIdentityNumber.parseInSpecificYear year =! Ok pin

    testPropWithMaxTest 400 "roundtrip for 10 digit string 'in specific year'" <| fun (Gen.ValidPin pin) ->
        let offset = rng.Next (0, 200)
        let year = pin.Year |> Year.map ((+) offset)

        pin
        |> SwedishPersonalIdentityNumber.to10DigitStringInSpecificYear year
        |> Result.bind (SwedishPersonalIdentityNumber.parseInSpecificYear year) = Ok pin

    testPropWithMaxTest 400 "roundtrip for 10 digit string without hyphen delimeter 'in specific year'"
        <| fun (Gen.ValidPin pin) ->
            let offset = rng.Next (0, 200)
            let year = pin.Year |> Year.map ((+) offset)

            pin
            |> SwedishPersonalIdentityNumber.to10DigitStringInSpecificYear year
            |> Result.map removeHyphen
            |> Result.bind (SwedishPersonalIdentityNumber.parseInSpecificYear year) = Ok pin
    ]

let invalidPinTests = testList "invalid pins" [
    test "null string returns argument null error" {
        null
        |> SwedishPersonalIdentityNumber.parse =! Error ArgumentNullError }

    testProp "empty string returns parsing error" <| fun (Gen.EmptyString str) ->
        str
        |> SwedishPersonalIdentityNumber.parse =! Error (ParsingError Empty)

    testProp "invalid number of digits returns parsing error" <| fun (Gen.Digits digits) ->
        isInvalidNumberOfDigits digits ==>
            lazy (digits
                  |> SwedishPersonalIdentityNumber.parse =! Error (ParsingError Length))

    testProp "invalid pin returns parsing error" <| fun (Gen.InvalidPinString str) ->
        match SwedishPersonalIdentityNumber.parse str with
        | Error (ParsingError (Invalid _)) -> true
        | _ -> failwith "Did not return expected error"

    testProp "parseInSpecificYear with empty string returns parsing error" <| fun (Gen.EmptyString str, Gen.ValidYear year) ->
        let y = Year.createOrFail year
        str
        |> SwedishPersonalIdentityNumber.parseInSpecificYear y =! Error (ParsingError Empty)

    testProp "parseInSpecificYear with null string returns argument null error" <| fun (Gen.ValidYear year) ->
        let y = Year.createOrFail year
        null
        |> SwedishPersonalIdentityNumber.parseInSpecificYear y =! Error ArgumentNullError

    testPropWithMaxTest 400 "cannot convert a pin to 10 digit string in a specific year when the person would be 200 years or older"
        <| fun (Gen.ValidPin pin) ->
            let offset =
                let maxYear = System.DateTime.MaxValue.Year - pin.Year.Value
                rng.Next(200, maxYear)
            let year = pin.Year |> Year.map (fun year -> year + offset)

            pin
            |> SwedishPersonalIdentityNumber.to10DigitStringInSpecificYear year =! Error (InvalidSerializationYear "SerializationYear cannot be a more than 199 years after the person was born")

    testPropWithMaxTest 400 "cannot convert a pin to 10 digit string in a specific year before the person was born"
        <| fun (Gen.ValidPin pin) ->
            let offset = rng.Next(1, pin.Year.Value)
            let year = pin.Year |> Year.map (fun year -> year - offset)

            pin
            |> SwedishPersonalIdentityNumber.to10DigitStringInSpecificYear year =! Error (InvalidSerializationYear "SerializationYear cannot be a year before the person was born") ]

[<Tests>]
let tests = testList "parse" [ validPinTests ; invalidPinTests ]
