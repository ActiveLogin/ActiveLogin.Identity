/// <remarks>
/// Tested with offical test Personal Identity Numbers from Skatteverket:
/// https://skatteverket.entryscape.net/catalog/9/datasets/147
/// </remarks>
module ActiveLogin.Identity.Swedish.FSharp.Test.SwedishPersonalIdentityNumber_create

open Swensen.Unquote
open Expecto
open Expecto.Flip
open FsCheck
open ActiveLogin.Identity.Swedish.FSharp
open System.Reflection
open ActiveLogin.Identity.Swedish.FSharp.TestData
open PinTestHelpers

let arbTypes = 
    [ typeof<Gen.ValidValuesGen> 
      typeof<Gen.InvalidYearGen>
      typeof<Gen.ValidYearGen>
      typeof<Gen.InvalidMonthGen>
      typeof<Gen.ValidMonthGen>
      typeof<Gen.WithInvalidDayGen>
      typeof<Gen.WithValidDayGen>
      typeof<Gen.InvalidBirthNumberGen> 
      typeof<Gen.ValidBirthNumberGen> ]

let config = 
    { FsCheckConfig.defaultConfig with arbitrary = arbTypes @ FsCheckConfig.defaultConfig.arbitrary }
let testProp name = testPropertyWithConfig config name
let ftestProp name = ftestPropertyWithConfig config name
let testPropWithMaxTest maxTest name = testPropertyWithConfig { config with maxTest = maxTest } name
let ftestPropWithMaxTest maxTest name = ftestPropertyWithConfig { config with maxTest = maxTest } name

[<Tests>]
let tests =
    testList "create" [
        testProp "valid values returns Ok Result" <| fun (Gen.ValidValues input) ->
            let result = SwedishPersonalIdentityNumber.create input
            Expect.isOk "should be Result.Ok" result 

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

        testProp "with all other values valid, only 1 of 10 checksums can be valid" <| 
            fun (Gen.ValidValues values) ->
                let withChecksum values checksum = { values with SwedishPersonalIdentityNumberValues.Checksum = checksum }
                let hasInvalidChecksum = function Error(InvalidChecksum _) -> true | _ -> false
                let numberOfInvalidPins =
                    [ 0..9 ]
                    |> List.map (withChecksum values >> SwedishPersonalIdentityNumber.create)
                    |> List.filter hasInvalidChecksum
                    |> List.length
                numberOfInvalidPins =! 9

        testProp "Possible coordination-number day" <| fun (Gen.ValidValues values) ->
            let coordinationDay = values.Day + 60
            let result = { values with Day = coordinationDay } |> SwedishPersonalIdentityNumber.create
            result =! Error(InvalidDay coordinationDay)

        testCase "FSharp should have no public constructor" <| fun () ->
            let typ = typeof<SwedishPersonalIdentityNumber>
            let numConstructors = typ.GetConstructors(BindingFlags.Public) |> Array.length
            numConstructors =! 0 ]
