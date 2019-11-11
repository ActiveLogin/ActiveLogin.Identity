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
    testList "SwedishPersonalIdentityNumber.create" [
        testProp "roundtrip 12DigitString -> create -> to12DigitString" <| fun (Gen.Pin.Valid12Digit str) ->
            str
            |> Gen.stringToValues
            |> SwedishPersonalIdentityNumber.create
            |> Result.map SwedishPersonalIdentityNumber.to12DigitString = Ok str

        testPropWithMaxTest 20000 "invalid year returns InvalidYear Error" <|
            fun (Gen.Pin.ValidValues (y, m, d, b, c), Gen.InvalidYear invalidYear) ->
                let result = SwedishPersonalIdentityNumber.create (invalidYear, m, d, b, c)
                result =! Error(InvalidYear invalidYear)

        testPropWithMaxTest 20000 "valid year does not return InvalidYear Error" <|
            fun (Gen.Pin.ValidValues (y, m, d, b, c), Gen.ValidYear validYear) ->
                let result = SwedishPersonalIdentityNumber.create (y, m, d, b, c)
                result <>! (Error(InvalidYear validYear))

        testProp "invalid month returns InvalidMonth Error" <|
            fun (Gen.Pin.ValidValues (y, m, d, b, c), Gen.InvalidMonth invalidMonth) ->
                let result = SwedishPersonalIdentityNumber.create (y, invalidMonth, d, b, c)
                result =! Error(InvalidMonth invalidMonth)

        testProp "valid month does not return InvalidMonth Error" <|
            fun (Gen.Pin.ValidValues (y, m, d, b, c), Gen.ValidMonth validMonth) ->
                let result = SwedishPersonalIdentityNumber.create (y, m, d, b, c)
                result <>! (Error(InvalidMonth validMonth))

        testProp "invalid day returns InvalidDay Error" <| fun (Gen.Pin.WithInvalidDay (y, m, d, b, c)) ->
            let result = SwedishPersonalIdentityNumber.create (y, m, d, b, c)
            result =! Error(InvalidDayAndCoordinationDay d)

        testProp "valid day does not return InvalidDay Error" <| fun (Gen.Pin.WithValidDay (y, m, d, b, c)) ->
            let result = SwedishPersonalIdentityNumber.create (y, m, d, b, c)
            result <>! Error(InvalidDayAndCoordinationDay d)
            result <>! Error(InvalidDay d)

        testProp "invalid birth number returns InvalidBirthNumber Error" <|
            fun (Gen.Pin.ValidValues (y, m, d, b, c), Gen.InvalidBirthNumber invalidBirthNumber) ->
                let result = SwedishPersonalIdentityNumber.create (y, m, d, invalidBirthNumber, c)
                result =! Error(InvalidBirthNumber invalidBirthNumber)

        testPropWithMaxTest 3000 "valid birth number does not return InvalidBirthNumber Error" <|
            fun (Gen.Pin.ValidValues (y, m, d, b, c), Gen.ValidBirthNumber validBirthNumber) ->
                let result = SwedishPersonalIdentityNumber.create (y, m, d, validBirthNumber, c)
                result <>! Error(InvalidBirthNumber validBirthNumber)

        testProp "invalid checksum returns InvalidChecksum Error" <|
            fun (Gen.Pin.ValidValues (y, m, d, b, c)) ->
                let invalidChecksums =
                    [ 0..9 ]
                    |> List.except [ c ]

                let withInvalidChecksums =
                    invalidChecksums
                    |> List.map (fun checksum -> (y, m, d, b, checksum))

                let invalidChecksumsAndResults =
                    withInvalidChecksums
                    |> List.map (fun (y, m, d, b, c) -> c, SwedishPersonalIdentityNumber.create (y, m, d, b, c))

                invalidChecksumsAndResults
                |> List.iter (fun (invalidChecksum, result) ->
                    match result with
                    | Error (InvalidChecksum actual) -> invalidChecksum =! actual
                    | _ -> failwith "Expected InvalidChecksum Error")

        testProp "possible coordination-number day" <| fun (Gen.Pin.ValidValues (y, m, d, b, c)) ->
            let coordinationDay = d + 60
            let result = SwedishPersonalIdentityNumber.create (y, m, coordinationDay, b, c)
            result =! Error(InvalidDay coordinationDay)

        testCase "fsharp should have no public constructor" <| fun () ->
            let typ = typeof<SwedishPersonalIdentityNumber>
            let numConstructors = typ.GetConstructors(BindingFlags.Public) |> Array.length
            numConstructors =! 0 ]
