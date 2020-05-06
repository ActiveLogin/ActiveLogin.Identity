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

        testPropWithMaxTest 20000 "with invalid year throws" <|
            fun (Gen.CoordNum.ValidValues (_, m, d, b, c), Gen.InvalidYear invalidYear) ->
                toAction CoordinationNumber (invalidYear, m, d, b, c)
                |> Expect.throwsWithType<ArgumentOutOfRangeException>
                |> Expect.throwsWithMessage "Invalid year."

        testPropWithMaxTest 20000 "with valid year does not throw exception for year" <|
            fun (Gen.CoordNum.ValidValues (_, m, d, b, c), Gen.ValidYear validYear) ->
                toAction CoordinationNumber (validYear, m, d, b, c)
                |> Expect.doesNotThrowWithMessage "year"

        testProp "with invalid month throws exception" <|
            fun (Gen.CoordNum.WithInvalidMonth withInvalidMonth) ->
                toAction CoordinationNumber withInvalidMonth
                |> Expect.throwsWithType<ArgumentOutOfRangeException>
                |> Expect.throwsWithMessage "Invalid month for coordination number"

        testProp "valid month does not throw exception for month" <|
            fun (Gen.CoordNum.ValidValues (y, _, d, b, c), Gen.ValidMonth validMonth) ->
                toAction CoordinationNumber (y, validMonth, d, b, c)
                |> Expect.doesNotThrowWithMessage "month"

        testProp "with invalid day throws" <|
            fun (Gen.CoordNum.WithInvalidDay withInvalidDay) ->
                toAction CoordinationNumber withInvalidDay
                |> Expect.throwsWithType<ArgumentOutOfRangeException>
                |> Expect.throwsWithMessage "Invalid coordination day."

        testProp "with valid day does not throw exception for day" <| fun (Gen.CoordNum.WithValidDay withValidDay) ->
            toAction CoordinationNumber withValidDay
            |> Expect.doesNotThrowWithMessage "Invalid coordination day"

        testProp "with invalid individual number throws" <|
            fun (Gen.CoordNum.ValidValues (y, m, d, _, c), Gen.InvalidBirthNumber invalidBirthNumber) ->
                toAction CoordinationNumber (y, m, d, invalidBirthNumber, c)
                |> Expect.throwsWithType<ArgumentOutOfRangeException>
                |> Expect.throwsWithMessage "Invalid individual number."

        testPropWithMaxTest 3000 "with valid birth number does not throw exception for birth number" <|
            fun (Gen.CoordNum.ValidValues (y, m, d, _, c), Gen.ValidBirthNumber validBirthNumber) ->
                toAction CoordinationNumber (y, m, d, validBirthNumber, c )
                |> Expect.doesNotThrowWithMessage "birth"

        testProp "with invalid checksum throws" <|
            fun (Gen.CoordNum.ValidValues (y, m, d, b, c)) ->
                let invalidChecksums =
                    [ 0..9 ]
                    |> List.except [ c ]

                let withInvalidChecksums =
                    invalidChecksums
                    |> List.map (fun checksum -> (y, m, d, b, checksum))

                withInvalidChecksums
                |> List.iter (fun (y, m, d, b, cs) ->
                    toAction CoordinationNumber (y, m, d, b, cs)
                    |> Expect.throwsWithType<ArgumentException>
                    |> Expect.throwsWithMessage "Invalid checksum." )

        testCase "fsharp should have no public constructor" <| fun () ->
            let typ = typeof<CoordinationNumber>
            let numConstructors = typ.GetConstructors(BindingFlags.Public) |> Array.length
            numConstructors =! 0 ]
