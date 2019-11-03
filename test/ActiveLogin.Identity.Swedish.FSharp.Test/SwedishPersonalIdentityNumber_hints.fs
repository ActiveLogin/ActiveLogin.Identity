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
open ActiveLogin.Identity.Swedish


let getDateOfBirth (pin: SwedishPersonalIdentityNumber) =
    {| Date = DateTime(pin.Year.Value, pin.Month.Value, pin.Day.Value)
       IsLeapDay = pin.Month.Value = 2 && pin.Day.Value = 29 |}

let (|Even|Odd|) (num:BirthNumber) =
    match num.Value % 2 with
    | 0 -> Even
    | _ -> Odd

[<Tests>]
let tests =
    testList "SwedishPersonalIdentityNumber.hints" [
        testList "getAgeHint" [
            testProp "a person ages by years counting from their date of birth"
                <| fun (Gen.Pin.ValidPin pin, Gen.Age (years, months, days)) ->
                    not (pin.Month.Value = 2 && pin.Day.Value = 29) ==>
                    lazy
                        let dateOfBirth = getDateOfBirth pin
                        let checkDate =
                            dateOfBirth.Date
                                .AddYears(years)
                                .AddMonths(months)
                                .AddDays(days)

                        Hints.getAgeHintOnDate checkDate pin =! Some years

            testProp "a person born on a leap day also ages by years counting from their date of birth"
                <| fun (Gen.Pin.LeapDayPin pin, Gen.Age (years, months, days)) ->
                    let dateOfBirth = getDateOfBirth pin
                    // Since there isn't a leap day every year we need to add 1 day to the checkdate
                    let checkDate =
                        dateOfBirth.Date
                            .AddYears(years)
                            .AddMonths(months)
                            .AddDays(days + 1.)

                    Hints.getAgeHintOnDate checkDate pin =! Some years

            testProp "cannot get age for date before person was born" <| fun (Gen.Pin.ValidPin pin) ->
                let dateOfBirth = getDateOfBirth pin
                let checkOffset = rng.NextDouble() * 199. * 365.
                let checkDate = dateOfBirth.Date.AddDays -checkOffset
                let result = Hints.getAgeHintOnDate checkDate pin
                result |> Expect.isNone "age should be None"

            testProp "getAgeHint uses DateTime.UtcNow as checkYear" <| fun (Gen.Pin.ValidPin pin) ->
                let age1 = Hints.getAgeHintOnDate DateTime.UtcNow pin
                let age2 = Hints.getAgeHint pin
                age1 =! age2 ]

        testList "getDateOfBirthHint" [
            testProp "get date of birth hint extracts year, month and date from pin" <| fun (Gen.Pin.ValidPin pin) ->
                let result = Hints.getDateOfBirthHint pin

                result.Year =! pin.Year.Value
                result.Month =! pin.Month.Value
                result.Day =! pin.Day.Value
        ]

        testList "getGenderHint" [
            testProp "even birthnumber indicates a female, odd birthnumber a male" <| fun (Gen.Pin.ValidPin pin) ->
                match pin.BirthNumber with
                | Even -> Hints.getGenderHint pin =! Gender.Female
                | Odd -> Hints.getGenderHint pin =! Gender.Male
        ]
    ]
