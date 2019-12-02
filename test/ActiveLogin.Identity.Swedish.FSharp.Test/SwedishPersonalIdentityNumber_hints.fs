/// <remarks>
/// Tested with offical test Personal Identity Numbers from Skatteverket:
/// https://skatteverket.entryscape.net/catalog/9/datasets/147
/// </remarks>
module ActiveLogin.Identity.Swedish.FSharp.Test.SwedishPersonalIdentityNumber_hints

open Swensen.Unquote
open Expecto
open Expecto.Flip
open FsCheck
open System
open ActiveLogin.Identity.Swedish
open ActiveLogin.Identity.Swedish.Extensions
open ActiveLogin.Identity.Swedish.FSharp.Test.PinTestHelpers


let getDateOfBirth (pin: SwedishPersonalIdentityNumber) =
    {| Date = DateTime(pin.Year, pin.Month, pin.Day)
       IsLeapDay = pin.Month = 2 && pin.Day = 29 |}

let isNotFromTheFuture (pin: SwedishPersonalIdentityNumber) =
    DateTime(pin.Year, pin.Month, pin.Day) < DateTime.UtcNow

[<Tests>]
let tests =
    testList "SwedishPersonalIdentityNumber.hints" [
        testList "getAgeHint" [
            testProp "a person ages by years counting from their date of birth"
                <| fun (Gen.Pin.ValidPin pin, Gen.Age (years, months, days)) ->
                    not (pin.Month = 2 && pin.Day = 29) ==>
                    lazy
                        let dateOfBirth = getDateOfBirth pin
                        let checkDate =
                            dateOfBirth.Date
                                .AddYears(years)
                                .AddMonths(months)
                                .AddDays(days)

                        pin.GetAgeHint checkDate =! years

            testProp "a person born on a leap day also ages by years counting from their date of birth"
                <| fun (Gen.Pin.LeapDayPin pin, Gen.Age (years, months, days)) ->
                    let dateOfBirth = getDateOfBirth pin
                    // Since there isn't a leap day every year we need to add 1 day to the checkdate
                    let checkDate =
                        dateOfBirth.Date
                            .AddYears(years)
                            .AddMonths(months)
                            .AddDays(days + 1.)

                    pin.GetAgeHint checkDate =! years

            testProp "cannot get age for date before person was born" <| fun (Gen.Pin.ValidPin pin) ->
                let dateOfBirth = getDateOfBirth pin
                let checkOffset = rng.NextDouble() * 199. * 365.
                let checkDate = dateOfBirth.Date.AddDays -checkOffset
                toAction pin.GetAgeHint checkDate
                |> Expect.throwsWithMessage "The person is not yet born"

            testProp "getAgeHint uses DateTime.UtcNow as checkYear" <| fun (Gen.Pin.ValidPin pin) ->
                isNotFromTheFuture pin ==> lazy
                    let age1 = pin.GetAgeHint DateTime.UtcNow
                    let age2 = pin.GetAgeHint()
                    age1 =! age2 ]

        testList "getDateOfBirthHint" [
            testProp "get date of birth hint extracts year, month and date from pin" <| fun (Gen.Pin.ValidPin pin) ->
                let result = pin.GetDateOfBirthHint()

                result.Year =! pin.Year
                result.Month =! pin.Month
                result.Day =! pin.Day
        ]

        testList "getGenderHint" [
            testProp "even birthnumber indicates a female, odd birthnumber a male" <| fun (Gen.Pin.ValidPin pin) ->
                match pin.BirthNumber with
                | Even -> pin.GetGenderHint() =! Gender.Female
                | Odd -> pin.GetGenderHint() =! Gender.Male
        ]
    ]
