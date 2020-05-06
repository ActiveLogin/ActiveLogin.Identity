/// <remarks>
/// Tested with official test Personal Identity Numbers from Skatteverket:
/// https://skatteverket.entryscape.net/catalog/9/datasets/147
/// </remarks>
module ActiveLogin.Identity.Swedish.FSharp.Test.PersonalIdentityNumber_Constructor

open System
open Swensen.Unquote
open Expecto
open FsCheck
open ActiveLogin.Identity.Swedish
open System.Reflection
open PinTestHelpers


[<Tests>]
let tests =
    testList "PersonalIdentityNumber constructor" [
        testProp "roundtrip 12DigitString -> create -> to12DigitString" <| fun (Gen.Pin.Valid12Digit str) ->
            let pin =
                str
                |> Gen.stringToValues
                |> PersonalIdentityNumber
            pin.To12DigitString() =! str
        testPropWithMaxTest 20000 "with invalid year throws" <|
            fun (Gen.Pin.ValidValues (_, m, d, b, c), Gen.InvalidYear invalidYear) ->
                toAction PersonalIdentityNumber (invalidYear, m, d, b, c)
                |> Expect.throwsWithType<ArgumentOutOfRangeException>
                |> Expect.throwsWithMessage "Invalid year."

        testPropWithMaxTest 20000 "with valid year does not throw exception for year" <|
            fun (Gen.Pin.ValidValues (_, m, d, b, c), Gen.ValidYear validYear) ->
                toAction PersonalIdentityNumber (validYear, m, d, b, c)
                |> Expect.doesNotThrowWithMessage "Invalid year"

        testProp "with invalid month throws" <|
            fun (Gen.Pin.ValidValues (y, _, d, b, c), Gen.InvalidMonth invalidMonth) ->
                toAction PersonalIdentityNumber (y, invalidMonth, d, b, c)
                |> Expect.throwsWithType<ArgumentOutOfRangeException>
                |> Expect.throwsWithMessage "Invalid month."

        testProp "with valid month does not throw exception for month" <|
            fun (Gen.Pin.ValidValues (y, _, d, b, c), Gen.ValidMonth validMonth) ->
                toAction PersonalIdentityNumber (y, validMonth, d, b, c)
                |> Expect.doesNotThrowWithMessage "Invalid month"

        testProp "with invalid day throws" <|
            fun (Gen.Pin.WithInvalidDay (y, m, d, b, c)) ->
                toAction PersonalIdentityNumber (y, m, d, b, c)
                |> Expect.throwsWithType<ArgumentOutOfRangeException>
                |> Expect.throwsWithMessage "Invalid day of month."

        testProp "with valid day does not throw exception for day" <| fun (Gen.Pin.WithValidDay (y, m, d, b, c)) ->
            toAction PersonalIdentityNumber (y, m, d, b, c)
            |> Expect.doesNotThrowWithMessage "Invalid day"

        testProp "with invalid birth number throws" <|
            fun (Gen.Pin.ValidValues (y, m, d, _, c), Gen.InvalidBirthNumber invalidBirthNumber) ->
                toAction PersonalIdentityNumber (y, m, d, invalidBirthNumber, c)
                |> Expect.throwsWithType<ArgumentOutOfRangeException>
                |> Expect.throwsWithMessage "Invalid birth number"

        testPropWithMaxTest 3000 "with valid birth number does not throw exception for birth number" <|
            fun (Gen.Pin.ValidValues (y, m, d, _, c), Gen.ValidBirthNumber validBirthNumber) ->
                toAction PersonalIdentityNumber (y, m, d, validBirthNumber, c)
                |> Expect.doesNotThrowWithMessage "Invalid birth number"

        testProp "invalid checksum throws" <|
            fun (Gen.Pin.ValidValues (y, m, d, b, c)) ->
                let invalidChecksums =
                    [ 0..9 ]
                    |> List.except [ c ]

                let withInvalidChecksums =
                    invalidChecksums
                    |> List.map (fun checksum -> (y, m, d, b, checksum))

                withInvalidChecksums
                |> List.iter (fun (y, m, d, b, cs) ->
                    toAction PersonalIdentityNumber (y, m, d, b, cs)
                    |> Expect.throwsWithType<ArgumentException>
                    |> Expect.throwsWithMessage "Invalid checksum." )

        testCase "fsharp should have no public constructor" <| fun () ->
            let typ = typeof<PersonalIdentityNumber>
            let numConstructors = typ.GetConstructors(BindingFlags.Public) |> Array.length
            numConstructors =! 0 ]
