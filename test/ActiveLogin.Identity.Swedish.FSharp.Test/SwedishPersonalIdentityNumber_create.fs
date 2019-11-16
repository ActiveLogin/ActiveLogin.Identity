/// <remarks>

/// Tested with offical test Personal Identity Numbers from Skatteverket:
/// https://skatteverket.entryscape.net/catalog/9/datasets/147
/// </remarks>
module ActiveLogin.Identity.Swedish.FSharp.Test.SwedishPersonalIdentityNumber_create

open Swensen.Unquote
open Expecto
open FsCheck
open ActiveLogin.Identity.Swedish.FSharp
open System.Reflection
open ActiveLogin.Identity.Swedish.FSharp.TestData


[<Tests>]
let tests =
    testList "create" [
        ftestProp "roundtrip 12DigitString -> create -> to12DigitString" <| fun (Gen.Valid12Digit str) ->
            str
            |> Gen.stringToValues
            |> SwedishPersonalIdentityNumber.create
            |> Result.map SwedishPersonalIdentityNumber.to12DigitString = Ok str

        testPropWithMaxTest 20000 "invalid year returns InvalidYear Error" <|
            fun (Gen.ValidValues validValues, Gen.InvalidYear invalidYear) ->
                let input = { validValues with Year = invalidYear }
                let result = SwedishPersonalIdentityNumber.create input
                result =! Error(InvalidYear invalidYear)

        testPropWithMaxTest 20000 "valid year does not return InvalidYear Error" <|
            fun (Gen.ValidValues values, Gen.ValidYear validYear) ->
                let input = { values with Year = validYear }
                let result = SwedishPersonalIdentityNumber.create input
                result <>! (Error(InvalidYear validYear))

        testProp "invalid month returns InvalidMonth Error" <|
            fun (Gen.ValidValues validValues, Gen.InvalidMonth invalidMonth) ->
                let input = { validValues with Month = invalidMonth }
                let result = SwedishPersonalIdentityNumber.create input
                result =! Error(InvalidMonth invalidMonth)

        testProp "valid month does not return InvalidMonth Error" <|
            fun (Gen.ValidValues values, Gen.ValidMonth validMonth) ->
                let input = { values with Month = validMonth }
                let result = SwedishPersonalIdentityNumber.create input
                result <>! (Error(InvalidMonth validMonth))

        testProp "invalid day returns InvalidDay Error" <| fun (Gen.WithInvalidDay input) ->
            let result = SwedishPersonalIdentityNumber.create input
            result =! Error(InvalidDayAndCoordinationDay input.Day )

        testProp "valid day does not return InvalidDay Error" <| fun (Gen.WithValidDay input) ->
            let result = SwedishPersonalIdentityNumber.create input
            result <>! Error(InvalidDayAndCoordinationDay input.Day)
            result <>! Error(InvalidDay input.Day)

        testProp "invalid birth number returns InvalidBirthNumber Error" <|
            fun (Gen.ValidValues validValues, Gen.InvalidBirthNumber invalidBirthnumber) ->
                let input = { validValues with BirthNumber = invalidBirthnumber }
                let result = SwedishPersonalIdentityNumber.create input
                result =! Error(InvalidBirthNumber invalidBirthnumber)

        testPropWithMaxTest 3000 "valid birth number does not return InvalidBirthNumber Error" <|
            fun (Gen.ValidValues validValues, Gen.ValidBirthNumber validBirthNumber) ->
                let input = { validValues with BirthNumber = validBirthNumber}
                let result = SwedishPersonalIdentityNumber.create input
                result <>! Error(InvalidBirthNumber validBirthNumber)

        testProp "invalid checksum returns InvalidChecksum Error" <|
            fun (Gen.ValidValues values) ->
                let invalidChecksums =
                    [ 0..9 ]
                    |> List.except [ values.Checksum ]

                let withInvalidChecksums =
                    invalidChecksums
                    |> List.map (fun checksum -> { values with Checksum = checksum })

                let invalidChecksumsAndResults =
                    withInvalidChecksums
                    |> List.map (fun values -> values.Checksum, SwedishPersonalIdentityNumber.create values)

                invalidChecksumsAndResults
                |> List.iter (fun (invalidChecksum, result) ->
                    match result with
                    | Error (InvalidChecksum actual) -> invalidChecksum =! actual
                    | _ -> failwith "Expected InvalidChecksum Error")

        testProp "possible coordination-number day" <| fun (Gen.ValidValues values) ->
            let coordinationDay = values.Day + 60
            let result = { values with Day = coordinationDay } |> SwedishPersonalIdentityNumber.create
            result =! Error(InvalidDay coordinationDay)

        testCase "fsharp should have no public constructor" <| fun () ->
            let typ = typeof<SwedishPersonalIdentityNumber>
            let numConstructors = typ.GetConstructors(BindingFlags.Public) |> Array.length
            numConstructors =! 0 ]
