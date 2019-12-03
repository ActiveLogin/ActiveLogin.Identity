/// <remarks>
/// Tested with offical test Personal Identity Numbers from Skatteverket:
/// https://skatteverket.entryscape.net/catalog/9/datasets/147
/// </remarks>
module ActiveLogin.Identity.Swedish.FSharp.Test.SwedishCoordinationNumber_Constructor

open Swensen.Unquote
open Expecto
open FsCheck
open ActiveLogin.Identity.Swedish
open System.Reflection


[<Tests>]
let tests =
    testList "SwedishCoordinationNumber constructor" [
        testProp "roundtrip 12DigitString -> create -> to12DigitString" <| fun (Gen.CoordNum.Valid12Digit str) ->
            let pin =
                str
                |> Gen.stringToValues
                |> SwedishCoordinationNumber
            pin.To12DigitString() =! str

        testPropWithMaxTest 20000 "invalid year returns InvalidYear Error" <|
            fun (Gen.CoordNum.ValidValues (y, m, d, b, c), Gen.InvalidYear invalidYear) ->
                toAction SwedishCoordinationNumber (invalidYear, m, d, b, c)
                |> Expect.throwsWithMessage "Invalid year."

        testPropWithMaxTest 20000 "valid year does not return InvalidYear Error" <|
            fun (Gen.CoordNum.ValidValues (y, m, d, b, c), Gen.ValidYear validYear) ->
                toAction SwedishCoordinationNumber (validYear, m, d, b, c)
                |> Expect.doesNotThrowWithMessage "year"

        testProp "invalid month returns InvalidMonth Error" <|
            fun (Gen.CoordNum.ValidValues (y, m, d, b, c), Gen.InvalidMonth invalidMonth) ->
                toAction SwedishCoordinationNumber (y, invalidMonth, d, b, c)
                |> Expect.throwsWithMessage "Invalid month."

        testProp "valid month does not return InvalidMonth Error" <|
            fun (Gen.CoordNum.ValidValues (y, m, d, b, c), Gen.ValidMonth validMonth) ->
                toAction SwedishCoordinationNumber (y, validMonth, d, b, c)
                |> Expect.doesNotThrowWithMessage "month"

        testProp "invalid day returns InvalidDay Error" <| fun (Gen.CoordNum.WithInvalidDay (y, m, d, b, c)) ->
            toAction SwedishCoordinationNumber (y, m, d, b, c)
            |> Expect.throwsWithMessage "Invalid coordination day."

        testProp "valid day does not return InvalidDay Error" <| fun (Gen.CoordNum.WithValidDay (y, m, d, b, c)) ->
            toAction SwedishCoordinationNumber (y, m, d, b, c)
            |> Expect.doesNotThrowWithMessage "day"

        testProp "invalid birth number returns InvalidBirthNumber Error" <|
            fun (Gen.CoordNum.ValidValues (y, m, d, b, c), Gen.InvalidBirthNumber invalidBirthNumber) ->
                toAction SwedishCoordinationNumber (y, m, d, invalidBirthNumber, c)
                |> Expect.throwsWithMessage "Invalid birth number."

        testPropWithMaxTest 3000 "valid birth number does not return InvalidBirthNumber Error" <|
            fun (Gen.CoordNum.ValidValues (y, m, d, b, c), Gen.ValidBirthNumber validBirthNumber) ->
                toAction SwedishCoordinationNumber (y, m, d, validBirthNumber, c )
                |> Expect.doesNotThrowWithMessage "birth"

        testProp "invalid checksum returns InvalidChecksum Error" <|
            fun (Gen.CoordNum.ValidValues (y, m, d, b, c)) ->
                let invalidChecksums =
                    [ 0..9 ]
                    |> List.except [ c ]

                let withInvalidChecksums =
                    invalidChecksums
                    |> List.map (fun checksum -> (y, m, d, b, checksum))

                withInvalidChecksums
                |> List.iter (fun (y, m, d, b, cs) ->
                    toAction SwedishCoordinationNumber (y, m, d, b, cs)
                    |> Expect.throwsWithMessage "Invalid checksum." )

        testCase "fsharp should have no public constructor" <| fun () ->
            let typ = typeof<SwedishCoordinationNumber>
            let numConstructors = typ.GetConstructors(BindingFlags.Public) |> Array.length
            numConstructors =! 0 ]
