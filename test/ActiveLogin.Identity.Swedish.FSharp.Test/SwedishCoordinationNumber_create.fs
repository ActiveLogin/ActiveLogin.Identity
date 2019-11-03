/// <remarks>
/// Tested with offical test Personal Identity Numbers from Skatteverket:
/// https://skatteverket.entryscape.net/catalog/9/datasets/147
/// </remarks>
module ActiveLogin.Identity.Swedish.FSharp.Test.SwedishCoordinationNumber_create

open Swensen.Unquote
open Expecto
open FsCheck
open ActiveLogin.Identity.Swedish.FSharp
open System.Reflection
open ActiveLogin.Identity.Swedish.FSharp.TestData


[<Tests>]
let tests =
    testList "SwedishCoordinationNumber.create" [
        testProp "roundtrip 12DigitString -> create -> to12DigitString" <| fun (Gen.CoordNum.Valid12Digit str) ->
            str
            |> Gen.stringToValues
            |> SwedishCoordinationNumber.create
            |> Result.map SwedishCoordinationNumber.to12DigitString =! Ok str

        testPropWithMaxTest 20000 "invalid year returns InvalidYear Error" <|
            fun (Gen.CoordNum.ValidValues validValues, Gen.InvalidYear invalidYear) ->
                let input = { validValues with Year = invalidYear }
                let result = SwedishCoordinationNumber.create input
                result =! Error(InvalidYear invalidYear)

        testPropWithMaxTest 20000 "valid year does not return InvalidYear Error" <|
            fun (Gen.CoordNum.ValidValues values, Gen.ValidYear validYear) ->
                let input = { values with Year = validYear }
                let result = SwedishCoordinationNumber.create input
                result <>! (Error(InvalidYear validYear))

        testProp "invalid month returns InvalidMonth Error" <|
            fun (Gen.CoordNum.ValidValues validValues, Gen.InvalidMonth invalidMonth) ->
                let input = { validValues with Month = invalidMonth }
                let result = SwedishCoordinationNumber.create input
                result =! Error(InvalidMonth invalidMonth)

        testProp "valid month does not return InvalidMonth Error" <|
            fun (Gen.CoordNum.ValidValues values, Gen.ValidMonth validMonth) ->
                let input = { values with Month = validMonth }
                let result = SwedishCoordinationNumber.create input
                result <>! (Error(InvalidMonth validMonth))

        testProp "invalid day returns InvalidDay Error" <| fun (Gen.CoordNum.WithInvalidDay input) ->
            let result = SwedishCoordinationNumber.create input
            result =! Error(InvalidDayAndCoordinationDay input.Day )

        testProp "valid day does not return InvalidDay Error" <| fun (Gen.CoordNum.WithValidDay input) ->
            let result = SwedishCoordinationNumber.create input
            result <>! Error(InvalidDayAndCoordinationDay input.Day)
            result <>! Error(InvalidDay input.Day)

        testProp "invalid birth number returns InvalidBirthNumber Error" <|
            fun (Gen.CoordNum.ValidValues validValues, Gen.InvalidBirthNumber invalidBirthNumber) ->
                let input = { validValues with BirthNumber = invalidBirthNumber }
                let result = SwedishCoordinationNumber.create input
                result =! Error(InvalidBirthNumber invalidBirthNumber)

        testPropWithMaxTest 3000 "valid birth number does not return InvalidBirthNumber Error" <|
            fun (Gen.CoordNum.ValidValues validValues, Gen.ValidBirthNumber validBirthNumber) ->
                let input = { validValues with BirthNumber = validBirthNumber}
                let result = SwedishCoordinationNumber.create input
                result <>! Error(InvalidBirthNumber validBirthNumber)

        testProp "invalid checksum returns InvalidChecksum Error" <|
            fun (Gen.CoordNum.ValidValues values) ->
                let invalidChecksums =
                    [ 0..9 ]
                    |> List.except [ values.Checksum ]

                let withInvalidChecksums =
                    invalidChecksums
                    |> List.map (fun checksum -> { values with Checksum = checksum })

                let invalidChecksumsAndResults =
                    withInvalidChecksums
                    |> List.map (fun values -> values.Checksum, SwedishCoordinationNumber.create values)

                invalidChecksumsAndResults
                |> List.iter (fun (invalidChecksum, result) ->
                    match result with
                    | Error (InvalidChecksum actual) -> invalidChecksum =! actual
                    | _ -> failwith "Expected InvalidChecksum Error")

        testCase "fsharp should have no public constructor" <| fun () ->
            let typ = typeof<SwedishCoordinationNumber>
            let numConstructors = typ.GetConstructors(BindingFlags.Public) |> Array.length
            numConstructors =! 0 ]
