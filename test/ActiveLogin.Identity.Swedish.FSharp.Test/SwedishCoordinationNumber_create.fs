/// <remarks>
/// Tested with offical test Personal Identity Numbers from Skatteverket:
/// https://skatteverket.entryscape.net/catalog/9/datasets/147
/// </remarks>
module ActiveLogin.Identity.Swedish.FSharp.Test.SwedishCoordinationNumber_create

open Swensen.Unquote
open Expecto
open FsCheck
open ActiveLogin.Identity.Swedish.FSharp
open ActiveLogin.Identity.Swedish.FSharp.TestData
open System.Reflection

[<Tests>]
let tests =
    testList "SwedishCoordinationNumber.create" [
        testProp "roundtrip 12DigitString -> create -> to12DigitString" <| fun (Gen.CoordNum.Valid12Digit str) ->
            str
            |> Gen.stringToValues
            |> SwedishCoordinationNumber.create
            |> Result.map SwedishCoordinationNumber.to12DigitString =! Ok str

        testPropWithMaxTest 20000 "invalid year returns InvalidYear Error" <|
            fun (Gen.CoordNum.ValidValues (y, m, d, b, c), Gen.InvalidYear invalidYear) ->
                let result = SwedishCoordinationNumber.create (invalidYear, m, d, b, c)
                result =! Error(InvalidYear invalidYear)

        testPropWithMaxTest 20000 "valid year does not return InvalidYear Error" <|
            fun (Gen.CoordNum.ValidValues (y, m, d, b, c), Gen.ValidYear validYear) ->
                let result = SwedishCoordinationNumber.create (validYear, m, d, b, c)
                result <>! (Error(InvalidYear validYear))

        testProp "invalid month returns InvalidMonth Error" <|
            fun (Gen.CoordNum.ValidValues (y, m, d, b, c), Gen.InvalidMonth invalidMonth) ->
                let result = SwedishCoordinationNumber.create (y, invalidMonth, d, b, c)
                result =! Error(InvalidMonth invalidMonth)

        testProp "valid month does not return InvalidMonth Error" <|
            fun (Gen.CoordNum.ValidValues (y, m, d, b, c), Gen.ValidMonth validMonth) ->
                let result = SwedishCoordinationNumber.create (y, validMonth, d, b, c)
                result <>! (Error(InvalidMonth validMonth))

        testProp "invalid day returns InvalidDay Error" <| fun (Gen.CoordNum.WithInvalidDay (y, m, d, b, c)) ->
            let result = SwedishCoordinationNumber.create (y, m, d, b, c)
            result =! Error(InvalidDayAndCoordinationDay d)

        testProp "valid day does not return InvalidDay Error" <| fun (Gen.CoordNum.WithValidDay (y, m, d, b, c)) ->
            let result = SwedishCoordinationNumber.create (y, m, d, b, c)
            result <>! Error(InvalidDayAndCoordinationDay d)
            result <>! Error(InvalidDay d)

        testProp "invalid birth number returns InvalidBirthNumber Error" <|
            fun (Gen.CoordNum.ValidValues (y, m, d, b, c), Gen.InvalidBirthNumber invalidBirthNumber) ->
                let result = SwedishCoordinationNumber.create (y, m, d, invalidBirthNumber, c)
                result =! Error(InvalidBirthNumber invalidBirthNumber)

        testPropWithMaxTest 3000 "valid birth number does not return InvalidBirthNumber Error" <|
            fun (Gen.CoordNum.ValidValues (y, m, d, b, c), Gen.ValidBirthNumber validBirthNumber) ->
                let result = SwedishCoordinationNumber.create (y, m, d, validBirthNumber, c )
                result <>! Error(InvalidBirthNumber validBirthNumber)

        testProp "invalid checksum returns InvalidChecksum Error" <|
            fun (Gen.CoordNum.ValidValues (y, m, d, b, c)) ->
                let invalidChecksums =
                    [ 0..9 ]
                    |> List.except [ c ]

                let withInvalidChecksums =
                    invalidChecksums
                    |> List.map (fun checksum -> (y, m, d, b, checksum))

                let invalidChecksumsAndResults =
                    withInvalidChecksums
                    |> List.map (fun (y, m, d, b, c) -> c, SwedishCoordinationNumber.create (y, m, d, b, c))

                invalidChecksumsAndResults
                |> List.iter (fun (invalidChecksum, result) ->
                    match result with
                    | Error (InvalidChecksum actual) -> invalidChecksum =! actual
                    | _ -> failwith "Expected InvalidChecksum Error")

        testCase "fsharp should have no public constructor" <| fun () ->
            let typ = typeof<SwedishCoordinationNumber>
            let numConstructors = typ.GetConstructors(BindingFlags.Public) |> Array.length
            numConstructors =! 0 ]
