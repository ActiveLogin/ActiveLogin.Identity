/// <remarks>
/// Tested with offical test Personal Identity Numbers from Skatteverket:
/// https://skatteverket.entryscape.net/catalog/9/datasets/147
/// </remarks>
module ActiveLogin.Identity.Swedish.FSharp.Test.SwedishPersonalIdentityNumber_hints

open Swensen.Unquote
open Expecto
open Expecto.Flip
open FsCheck
open ActiveLogin.Identity.Swedish.FSharp.SwedishPersonalIdentityNumber
open System
open ActiveLogin.Identity.Swedish.FSharp
open ActiveLogin.Identity.Swedish.FSharp.Test.PinTestHelpers
open ActiveLogin.Identity.Swedish.FSharp.TestData
open ActiveLogin.Identity.Swedish

let arbTypes =
    [ typeof<Gen.ValidPinGen> ]


let config =
    { FsCheckConfig.defaultConfig with arbitrary = arbTypes @ FsCheckConfig.defaultConfig.arbitrary }
let testProp name = testPropertyWithConfig config name
let ftestProp name = ftestPropertyWithConfig config name
let testPropWithMaxTest maxTest name = testPropertyWithConfig { config with maxTest = maxTest } name
let ftestPropWithMaxTest maxTest name = ftestPropertyWithConfig { config with maxTest = maxTest } name

let getDateOfBirth (pin: SwedishPersonalIdentityNumber) =
    {| Date = DateTime(pin.Year.Value, pin.Month.Value, pin.Day.Value)
       IsLeapDay = pin.Month.Value = 2 && pin.Day.Value = 29 |}

let (|Even|Odd|) (num:BirthNumber) =
    match num.Value % 2 with
    | 0 -> Even
    | _ -> Odd

[<Tests>]
let tests =
    testList "hints" [
        testList "getAgeHint" [
            testPropWithMaxTest 2000 "A person ages by years counting from their date of birth" <| fun (Gen.ValidPin pin) ->
                let dateOfBirth = getDateOfBirth pin
                let yearsSinceDOB = rng.Next(0,200)
                let checkDate =
                    let date = dateOfBirth.Date.AddYears(yearsSinceDOB)
                    if dateOfBirth.IsLeapDay then
                        date.AddDays(1.)
                    else
                        date
                let checkDate = if dateOfBirth.IsLeapDay then checkDate.AddDays(1.) else checkDate
                pin
                |> Hints.getAgeHintOnDate checkDate =! Some yearsSinceDOB

            testProp "Cannot get age for date before person was born" <| fun (Gen.ValidPin pin) ->
                let dateOfBirth = getDateOfBirth pin
                let checkOffset = rng.NextDouble() * 199. * 365.
                let checkDate = dateOfBirth.Date.AddDays -checkOffset
                let result = Hints.getAgeHintOnDate checkDate pin
                result |> Expect.isNone "age should be None"

            testProp "getAgeHint uses DateTime.Now as checkYear" <| fun (Gen.ValidPin pin) ->
                let age1 = Hints.getAgeHintOnDate DateTime.UtcNow pin
                let age2 = Hints.getAgeHint pin
                age1 =! age2 ]

        testList "getDateOfBirthHint" [
            testProp "get date of birth hint extracts year, month and date from pin" <| fun (Gen.ValidPin pin) ->
                let result = Hints.getDateOfBirthHint pin

                result.Year =! pin.Year.Value
                result.Month =! pin.Month.Value
                result.Day =! pin.Day.Value
        ]

        testList "getGenderHint" [
            testProp "even birthnumber indicates a female, odd birthnumber a male" <| fun (Gen.ValidPin pin) ->
                match pin.BirthNumber with
                | Even -> Hints.getGenderHint pin =! Gender.Female
                | Odd -> Hints.getGenderHint pin =! Gender.Male
        ]
    ]
