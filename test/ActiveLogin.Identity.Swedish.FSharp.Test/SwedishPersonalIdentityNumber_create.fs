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
open PinTestHelpers


[<Tests>]
let tests =
    testList "SwedishPersonalIdentityNumber.create" [
        testProp "roundtrip 12DigitString -> create -> to12DigitString" <| fun (Gen.Pin.Valid12Digit str) ->
            str
            |> Gen.stringToValues
            |> SwedishPersonalIdentityNumber.create
            |> SwedishPersonalIdentityNumber.to12DigitString =! str

        testPropWithMaxTest 20000 "invalid year returns InvalidYear Error" <|
            fun (Gen.Pin.ValidValues (y, m, d, b, c), Gen.InvalidYear invalidYear) ->
                toAction SwedishPersonalIdentityNumber.create (invalidYear, m, d, b, c)
                |> Expect.throwsWithMessage "Invalid year."

        testPropWithMaxTest 20000 "valid year does not return InvalidYear Error" <|
            fun (Gen.Pin.ValidValues (y, m, d, b, c), Gen.ValidYear validYear) ->
                toAction SwedishPersonalIdentityNumber.create (y, m, d, b, c)
                |> Expect.doesNotThrowWithMessage "year"

        testProp "invalid month returns InvalidMonth Error" <|
            fun (Gen.Pin.ValidValues (y, m, d, b, c), Gen.InvalidMonth invalidMonth) ->
                toAction SwedishPersonalIdentityNumber.create (y, invalidMonth, d, b, c)
                |> Expect.throwsWithMessage "Invalid month."

        testProp "valid month does not return InvalidMonth Error" <|
            fun (Gen.Pin.ValidValues (y, m, d, b, c), Gen.ValidMonth validMonth) ->
                toAction SwedishPersonalIdentityNumber.create (y, m, d, b, c)
                |> Expect.doesNotThrowWithMessage "month"

        testProp "invalid day returns InvalidDay Error" <| fun (Gen.Pin.WithInvalidDay (y, m, d, b, c)) ->
            toAction SwedishPersonalIdentityNumber.create (y, m, d, b, c)
            |> Expect.throwsWithMessage "Invalid day of month."

        testProp "valid day does not return InvalidDay Error" <| fun (Gen.Pin.WithValidDay (y, m, d, b, c)) ->
            toAction SwedishPersonalIdentityNumber.create (y, m, d, b, c)
            |> Expect.doesNotThrowWithMessage "day"

        testProp "invalid birth number returns InvalidBirthNumber Error" <|
            fun (Gen.Pin.ValidValues (y, m, d, b, c), Gen.InvalidBirthNumber invalidBirthNumber) ->
                toAction SwedishPersonalIdentityNumber.create (y, m, d, invalidBirthNumber, c)
                |> Expect.throwsWithMessage "Invalid birth number"

        testPropWithMaxTest 3000 "valid birth number does not return InvalidBirthNumber Error" <|
            fun (Gen.Pin.ValidValues (y, m, d, b, c), Gen.ValidBirthNumber validBirthNumber) ->
                toAction SwedishPersonalIdentityNumber.create (y, m, d, validBirthNumber, c)
                |> Expect.doesNotThrowWithMessage "birth"

        testProp "invalid checksum returns InvalidChecksum Error" <|
            fun (Gen.Pin.ValidValues (y, m, d, b, c)) ->
                let invalidChecksums =
                    [ 0..9 ]
                    |> List.except [ c ]

                let withInvalidChecksums =
                    invalidChecksums
                    |> List.map (fun checksum -> (y, m, d, b, checksum))

                withInvalidChecksums
                |> List.iter (fun (y, m, d, b, cs) ->
                    toAction SwedishPersonalIdentityNumber.create (y, m, d, b, cs)
                    |> Expect.throwsWithMessage "Invalid checksum." )

        testCase "fsharp should have no public constructor" <| fun () ->
            let typ = typeof<SwedishPersonalIdentityNumber>
            let numConstructors = typ.GetConstructors(BindingFlags.Public) |> Array.length
            numConstructors =! 0 ]
