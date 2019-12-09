/// <remarks>
/// Tested with offical test Personal Identity Numbers from Skatteverket:
/// https://skatteverket.entryscape.net/catalog/9/datasets/147
/// </remarks>
module ActiveLogin.Identity.Swedish.FSharp.Test.SwedishCoordinationNumber_Hints

open Swensen.Unquote
open Expecto
open Expecto.Flip
open FsCheck
open System
open ActiveLogin.Identity.Swedish.FSharp.Test.PinTestHelpers
open ActiveLogin.Identity.Swedish
open ActiveLogin.Identity.Swedish.Extensions

let getDateOfBirth (num: SwedishCoordinationNumber) =
    {| Date = DateTime(num.Year, num.Month, num.RealDay)
       IsLeapDay = num.Month = 2 && num.RealDay = 29 |}

let isNotFromTheFuture (num: SwedishCoordinationNumber) =
    DateTime(num.Year, num.Month, num.RealDay) < DateTime.UtcNow

[<Tests>]
let tests =
    testList "SwedishCoordinationNumber.hints" [
        testList "getAgeHint" [
            testProp "a person ages by years counting from their date of birth"
                <| fun (Gen.CoordNum.ValidNum num, Gen.Age (years, months, days)) ->
                    not (num.Month = 2 && num.RealDay = 29) ==>
                    lazy
                        let dateOfBirth = getDateOfBirth num
                        let checkDate =
                            dateOfBirth.Date
                                .AddYears(years)
                                .AddMonths(months)
                                .AddDays(days)

                        num.GetAgeHint checkDate =! years

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
                toAction num.GetAgeHint checkDate
                |> Expect.throwsWithMessage "The person is not yet born"

            testProp "getAgeHint uses DateTime.UtcNow as checkYear" <| fun (Gen.CoordNum.ValidNum num) ->
                isNotFromTheFuture num ==> lazy
                    let age1 = num.GetAgeHint DateTime.UtcNow
                    let age2 = num.GetAgeHint()
                    age1 =! age2 ]

        testList "getDateOfBirthHint" [
            testProp "get date of birth hint extracts year, month and date from number" <| fun (Gen.CoordNum.ValidNum num) ->
                let result = num.GetDateOfBirthHint ()

                result.Year =! num.Year
                result.Month =! num.Month
                result.Day =! num.RealDay
        ]

        testList "getGenderHint" [
            testProp "even birthnumber indicates a female, odd birthnumber a male" <| fun (Gen.CoordNum.ValidNum num) ->
                match num.BirthNumber with
                | Even -> num.GetGenderHint() =! Gender.Female
                | Odd -> num.GetGenderHint() =! Gender.Male
        ]
    ]
