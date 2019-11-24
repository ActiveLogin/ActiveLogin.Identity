/// <remarks>
/// Tested with offical test Personal Identity Numbers from Skatteverket:
/// https://skatteverket.entryscape.net/catalog/9/datasets/147
/// </remarks>
module ActiveLogin.Identity.Swedish.FSharp.Test.SwedishCoordinationNumber_hints

open Swensen.Unquote
open Expecto
open Expecto.Flip
open FsCheck
open ActiveLogin.Identity.Swedish.FSharp.SwedishCoordinationNumber
open System
open ActiveLogin.Identity.Swedish.FSharp
open ActiveLogin.Identity.Swedish.FSharp.Test.PinTestHelpers
open ActiveLogin.Identity.Swedish


let getDateOfBirth (num: SwedishCoordinationNumber) =
    {| Date = DateTime(num.Year.Value, num.Month.Value, num.RealDay)
       IsLeapDay = num.Month.Value = 2 && num.RealDay = 29 |}

let (|Even|Odd|) (num:BirthNumber) =
    match num.Value % 2 with
    | 0 -> Even
    | _ -> Odd

[<Tests>]
let tests =
    testList "SwedishCoordinationNumber.hints" [
        testList "getAgeHint" [
            testProp "a person ages by years counting from their date of birth"
                <| fun (Gen.CoordNum.ValidNum num, Gen.Age (years, months, days)) ->
                    not (num.Month.Value = 2 && num.RealDay = 29) ==>
                    lazy
                        let dateOfBirth = getDateOfBirth num
                        let checkDate =
                            dateOfBirth.Date
                                .AddYears(years)
                                .AddMonths(months)
                                .AddDays(days)

                        Hints.getAgeHintOnDate checkDate num =! Some years

// Right now we do not have leap days in the test data and cannot run this test
//            testProp "a person born on a leap day also ages by years counting from their date of birth"
//                <| fun (Gen.CoordNum.LeapDayCoordNum num, Gen.Age (years, months, days)) ->
//                    let dateOfBirth = getDateOfBirth num
//                    // Since there isn't a leap day every year we need to add 1 day to the checkdate
//                    let checkDate =
//                        dateOfBirth.Date
//                            .AddYears(years)
//                            .AddMonths(months)
//                            .AddDays(days + 1.)
//
//                    Hints.getAgeHintOnDate checkDate num =! Some years

            testProp "cannot get age for date before person was born" <| fun (Gen.CoordNum.ValidNum num) ->
                let dateOfBirth = getDateOfBirth num
                let checkOffset = rng.NextDouble() * 199. * 365.
                let checkDate = dateOfBirth.Date.AddDays -checkOffset
                let result = Hints.getAgeHintOnDate checkDate num
                result |> Expect.isNone "age should be None"

            testProp "getAgeHint uses DateTime.UtcNow as checkYear" <| fun (Gen.CoordNum.ValidNum num) ->
                let age1 = Hints.getAgeHintOnDate DateTime.UtcNow num
                let age2 = Hints.getAgeHint num
                age1 =! age2 ]

        testList "getDateOfBirthHint" [
            testProp "get date of birth hint extracts year, month and date from number" <| fun (Gen.CoordNum.ValidNum num) ->
                let result = Hints.getDateOfBirthHint num

                result.Year =! num.Year.Value
                result.Month =! num.Month.Value
                result.Day =! num.RealDay
        ]

        testList "getGenderHint" [
            testProp "even birthnumber indicates a female, odd birthnumber a male" <| fun (Gen.CoordNum.ValidNum num) ->
                match num.BirthNumber with
                | Even -> Hints.getGenderHint num =! Gender.Female
                | Odd -> Hints.getGenderHint num =! Gender.Male
        ]
    ]
