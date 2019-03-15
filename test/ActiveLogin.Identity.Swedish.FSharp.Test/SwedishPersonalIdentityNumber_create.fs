/// <remarks>
/// Tested with offical test Personal Identity Numbers from Skatteverket:
/// https://skatteverket.entryscape.net/catalog/9/datasets/147
/// </remarks>
module ActiveLogin.Identity.Swedish.FSharp.Test.SwedishPersonalIdentityNumber_create

open Swensen.Unquote
open Expecto
open ActiveLogin.Identity.Swedish.FSharp
open System.Reflection
open Arbitraries
open ActiveLogin.Identity.Swedish.FSharp.TestData
open PinTestHelpers

let config = FsCheckConfig.defaultConfig
let testProp arbTypes name = testPropertyWithConfig (addToConfig config arbTypes) name
let ftestProp arbTypes name = ftestPropertyWithConfig (addToConfig config arbTypes) name
let ptestProp arbTypes name = ptestPropertyWithConfig (addToConfig config arbTypes) name

[<Tests>]
let tests = testList "create" [ 
    testProp [ typeof<ValidValuesGen> ] "valid pin" <| fun values ->
        let result = values |> SwedishPersonalIdentityNumber.create
        result |> Expect.equalPin values
    testProp [ typeof<InvalidYearGen> ] "Invalid year" <| fun values ->
        let result = values |> SwedishPersonalIdentityNumber.create
        result =! Error(InvalidYear values.Year)
    testProp [ typeof<InvalidMonthGen> ] "Invalid month" <| fun values ->
        let result = values |> SwedishPersonalIdentityNumber.create
        result =! Error(InvalidMonth values.Month)
    testProp [ typeof<InvalidDayGen> ] "Invalid day" <| fun values ->
        let result = values |> SwedishPersonalIdentityNumber.create
        result =! Error(InvalidDayAndCoordinationDay values.Day)
    testProp [ typeof<ValidValuesGen> ] "Possible coordination-number day" <| fun values ->
        let coordinationDay = values.Day + 60
        let result = { values with Day = coordinationDay } |> SwedishPersonalIdentityNumber.create
        result =! Error(InvalidDay coordinationDay)
    testProp [ typeof<InvalidBirthNumberGen> ] "Invalid birthnumber" <| fun values ->
        let result = values |> SwedishPersonalIdentityNumber.create
        result =! Error(InvalidBirthNumber values.BirthNumber)
    testProp [ typeof<ValidValuesGen> ]
        "With all other values valid only 1 out of 10 checksums is valid" <| fun values ->
        let isValid x =
            match x with
            | Ok _ -> true
            | Error _ -> false

        let results =
            [ 0..9 ]
            |> List.map
                   (fun checksum ->
                   { values with SwedishPersonalIdentityNumberValues.Checksum = checksum }
                   |> SwedishPersonalIdentityNumber.create)

        let numValid =
            results
            |> List.filter isValid
            |> List.length
        numValid =! 1
    testCase "FSharp should have no public constructor" <| fun () ->
        let typ = typeof<SwedishPersonalIdentityNumber>
        let numConstructors = typ.GetConstructors(BindingFlags.Public) |> Array.length
        numConstructors =! 0 ]
