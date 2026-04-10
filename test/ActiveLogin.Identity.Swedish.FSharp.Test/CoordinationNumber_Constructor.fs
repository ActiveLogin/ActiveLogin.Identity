/// <remarks>
/// Tested with official test Personal Identity Numbers from Skatteverket:
/// https://skatteverket.entryscape.net/catalog/9/datasets/147
/// </remarks>
module ActiveLogin.Identity.Swedish.FSharp.Test.CoordinationNumber_Constructor

open System
open Swensen.Unquote
open Expecto
open FsCheck
open ActiveLogin.Identity.Swedish
open System.Reflection


[<Tests>]
let tests =
    testList "CoordinationNumber constructor" [
        testProp "roundtrip 12DigitString -> create -> to12DigitString" <| fun (Gen.CoordNum.Valid12Digit str) ->
            let pin =
                str
                |> Gen.stringToValues
                |> CoordinationNumber
            pin.To12DigitString() =! str

        testPropWithMaxTest 20000 "with invalid year throws" <| fun (Gen.CoordNum.ValidValues (_, m, d, b, c), Gen.InvalidYear invalidYear) ->
            let action = fun () -> CoordinationNumber (invalidYear, m, d, b, c) |> ignore
            Expect.throwsWithType<ArgumentOutOfRangeException> action
            Expect.throwsWithMessage "Invalid year" action

        testPropWithMaxTest 20000 "with valid year does not throw exception for year" <|
            fun (Gen.CoordNum.ValidValues (_, m, d, b, c), Gen.ValidYear validYear) ->
                toAction CoordinationNumber (validYear, m, d, b, c)
                |> Expect.doesNotThrowWithMessage "Invalid year"

        testProp "with invalid month throws exception" <| fun (Gen.CoordNum.WithInvalidMonth withInvalidMonth) ->
            let action = fun () -> CoordinationNumber withInvalidMonth |> ignore
            Expect.throwsWithType<ArgumentOutOfRangeException> action
            Expect.throwsWithMessage "Invalid month for coordination number" action

        testProp "valid month does not throw exception for month" <|
            fun (Gen.CoordNum.ValidValues (y, _, d, b, c), Gen.ValidMonth validMonth) ->
                toAction CoordinationNumber (y, validMonth, d, b, c)
                |> Expect.doesNotThrowWithMessage "Invalid month for coordination number"

        testProp "with invalid day throws" <| fun (Gen.CoordNum.WithInvalidDay withInvalidDay) ->
            let action = fun () -> CoordinationNumber withInvalidDay |> ignore
            Expect.throwsWithType<ArgumentOutOfRangeException> action
            Expect.throwsWithMessage "Invalid coordination day" action

        testProp "with valid day does not throw exception for day" <| fun (Gen.CoordNum.WithValidDay withValidDay) ->
            toAction CoordinationNumber withValidDay
            |> Expect.doesNotThrowWithMessage "Invalid coordination day"

        testProp "with invalid individual number throws" <| fun (Gen.CoordNum.ValidValues (y, m, d, _, c), Gen.InvalidBirthNumber invalidBirthNumber) ->
            let action = fun () -> CoordinationNumber (y, m, d, invalidBirthNumber, c) |> ignore
            Expect.throwsWithType<ArgumentOutOfRangeException> action
            Expect.throwsWithMessage "Invalid individual number" action

        testPropWithMaxTest 3000 "with valid individual number does not throw exception for birth number" <|
            fun (Gen.CoordNum.ValidValues (y, m, d, _, c), Gen.ValidBirthNumber validBirthNumber) ->
                toAction CoordinationNumber (y, m, d, validBirthNumber, c )
                |> Expect.doesNotThrowWithMessage "Invalid individual number"

        testProp "with invalid checksum throws" <| fun (Gen.CoordNum.ValidValues (y, m, d, b, c)) ->
            let invalidChecksums =
                [0..9] |> List.except [c]
        
            let withInvalidChecksums =
                invalidChecksums |> List.map (fun cs -> (y, m, d, b, cs))
        
            withInvalidChecksums
            |> List.iter (fun (y, m, d, b, cs) ->
                let action = fun () -> CoordinationNumber (y, m, d, b, cs) |> ignore
                Expect.throwsWithType<ArgumentException> action
                Expect.throwsWithMessage "Invalid checksum" action
            )
    ]
