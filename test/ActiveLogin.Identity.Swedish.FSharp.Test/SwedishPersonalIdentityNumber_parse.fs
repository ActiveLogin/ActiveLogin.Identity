module ActiveLogin.Identity.Swedish.FSharp.Test.SwedishPersonalIdentityNumber_parse

open Swensen.Unquote
open Expecto
open ActiveLogin.Identity.Swedish.FSharp
open ActiveLogin.Identity.Swedish.FSharp.TestData
open PinTestHelpers
open FsCheck
open System
open System.Threading

let arbTypes =
    [ typeof<Gen.Valid12DigitGen>
      typeof<Gen.ValidPinGen>
      typeof<Gen.Max200Gen>
      typeof<Gen.WhitespaceGen>
      typeof<Gen.ValidYearGen>
      typeof<Gen.DigitsGen>
      typeof<Gen.InvalidPinStringGen> ]

let config =
    { FsCheckConfig.defaultConfig with arbitrary = arbTypes @ FsCheckConfig.defaultConfig.arbitrary }
let testProp name = testPropertyWithConfig config name
let ftestProp name = ftestPropertyWithConfig config name
let testPropWithMaxTest maxTest name = testPropertyWithConfig { config with maxTest = maxTest } name
let ftestPropWithMaxTest maxTest name = ftestPropertyWithConfig { config with maxTest = maxTest } name

let yearTurning100 = Year.map ((+) 100)

let tee f x = f x |> ignore; x

let private rng =
    // this thread-safe implementation is required to handle running lots of invocations of getRandom in parallel
    let seedGenerator = Random()
    let localGenerator = new ThreadLocal<Random>(fun _ ->
        lock seedGenerator (fun _ ->
            let seed = seedGenerator.Next()
            Random()))
    fun (min, max) -> localGenerator.Value.Next(min, max)

let printableAsciiExcludingPlus = [ 32..42 ] @ [ 44..47 ] @ [ 58..126 ] |> List.map char |> Array.ofList
let printableAscii = [ 32..47 ] @ [ 58..126 ] |> List.map char |> Array.ofList

let randomFromArray arr =
    let randomIndex() = (0, arr |> Array.length) |> rng
    arr.[randomIndex()]


let mixWith noiseSource (pin:string) =
    let chars = [ for c in pin -> c ]
    let result = ResizeArray<char>()
    noiseSource |> randomFromArray |> result.Add
    for char in chars do
        char |> result.Add
        noiseSource |> randomFromArray |> result.Add
    result |> Array.ofSeq |> String



[<Tests>]
let tests = testList "parse" [
    testProp "roundtrip for 12 digit string" <| fun (Gen.ValidPin pin) ->
        pin
        |> SwedishPersonalIdentityNumber.to12DigitString
        |> SwedishPersonalIdentityNumber.parse =! Ok pin

    testProp "roundtrip for 10 digit string with delimiter" <| fun (Gen.ValidPin pin) ->
        pin
        |> SwedishPersonalIdentityNumber.to10DigitString
        |> SwedishPersonalIdentityNumber.parse =! Ok pin

    testProp "roundtrip for 10 digit string without hyphen-delimiter" <| fun (Gen.ValidPin pin) ->
        let removeHyphen (str:string) =
            if str.[6] = '-' then str.[0..5] + str.[7..10]
            else str

        pin
        |> SwedishPersonalIdentityNumber.to10DigitString
        |> removeHyphen
        |> SwedishPersonalIdentityNumber.parse =! Ok pin

    testPropWithMaxTest 400 "roundtrip for 10 digit string 'in specific year'" <| fun (Gen.ValidPin pin) ->
        let offset = rng (0, 200)
        let year = pin.Year |> Year.map ((+) offset)

        pin
        |> SwedishPersonalIdentityNumber.to10DigitStringInSpecificYear year
        |> SwedishPersonalIdentityNumber.parseInSpecificYear year = Ok pin

    testPropWithMaxTest 400 "roundtrip for 12 digit string 'in specific year'" <| fun (Gen.ValidPin pin) ->
        let offset = rng (0, 200)
        let year = pin.Year |> Year.map ((+) offset)

        pin
        |> SwedishPersonalIdentityNumber.to12DigitString
        |> SwedishPersonalIdentityNumber.parseInSpecificYear year =! Ok pin

    testProp "roundtrip for 12 digit string mixed with random characters" <| fun (Gen.ValidPin pin) ->
        pin
        |> SwedishPersonalIdentityNumber.to12DigitString
        |> mixWith printableAscii
        |> SwedishPersonalIdentityNumber.parse =! Ok pin

    testProp "roundtrip for 10 digit string mixed with random characters except plus" <| fun (Gen.ValidPin pin) ->
        pin
        |> SwedishPersonalIdentityNumber.to10DigitString
        |> mixWith printableAsciiExcludingPlus
        |> SwedishPersonalIdentityNumber.parse =! Ok pin

    testProp "parse with empty string returns parsing error" <| fun (Gen.Whitespace str) ->
        str
        |> SwedishPersonalIdentityNumber.parse =! Error (ParsingError Empty)

    test "parse null string returns argument null error" {
        null
        |> SwedishPersonalIdentityNumber.parse =! Error ArgumentNullError
    }

    testProp "parseInSpecificYear with empty string returns parsing error" <| fun (Gen.Whitespace str, Gen.ValidYear year) ->
        let y = Year.createOrFail year
        str
        |> SwedishPersonalIdentityNumber.parseInSpecificYear y =! Error (ParsingError Empty)

    testProp "parseInSpecificYear null string returns argument null error" <| fun (Gen.ValidYear year) ->
        let y = Year.createOrFail year
        null
        |> SwedishPersonalIdentityNumber.parseInSpecificYear y =! Error ArgumentNullError

    testProp "invalid number of digits returns parsing error" <| fun (Gen.Digits digits) ->
        (not (isNull digits) && digits.Length > 0 && digits.Length <> 10 && digits.Length <> 12) ==>
            lazy (digits
                  |> SwedishPersonalIdentityNumber.parse =! Error (ParsingError Length))

    testProp "invalid pin returns parsing error" <| fun (Gen.InvalidPinString str) ->
        let result =
            str
            |> SwedishPersonalIdentityNumber.parse
        match result with
        | Error (ParsingError (Invalid _)) -> true
        | _ -> failwith "Did not return expected error"
    ]
