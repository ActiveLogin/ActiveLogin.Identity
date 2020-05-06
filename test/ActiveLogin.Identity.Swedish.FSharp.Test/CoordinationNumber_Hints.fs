/// <remarks>
/// Tested with official test Personal Identity Numbers from Skatteverket:
/// https://skatteverket.entryscape.net/catalog/9/datasets/147
/// </remarks>
module ActiveLogin.Identity.Swedish.FSharp.Test.CoordinationNumber_Hints

open System
open Swensen.Unquote
open Expecto
open FsCheck
open ActiveLogin.Identity.Swedish.FSharp.Test.PinTestHelpers
open ActiveLogin.Identity.Swedish
open ActiveLogin.Identity.Swedish.Extensions

let getDateOfBirth (num: CoordinationNumber) =
    let date =
        match num.Month, num.RealDay with
        | (0, _) | (_, 0) -> None
        | (m, d) -> DateTime(num.Year, m, d) |> Some
    {| Date = date
       IsLeapDay = num.Month = 2 && num.Day = 29 |}

let isNotFromTheFuture (num: CoordinationNumber) =
    match num.Month, num.RealDay with
    | (0, _) | (_, 0) -> false
    | (m, d) -> DateTime(num.Year, m, d) < DateTime.UtcNow

[<Tests>]
let tests =
    testList "CoordinationNumber.hints" [
        testList "getAgeHint" [
            testProp "a person ages by years counting from their date of birth"
                <| fun (Gen.CoordNum.ValidNum num, Gen.Age (years, months, days)) ->
                    not (num.Month = 2 && num.Day = 29) ==>
                    lazy
                        let dateOfBirth = getDateOfBirth num
                        match dateOfBirth.Date with
                        | Some date ->
                            let checkDate =
                                date.Date
                                    .AddYears(years)
                                    .AddMonths(months)
                                    .AddDays(days)

                            num.GetAgeHint checkDate =! Nullable years
                        | None ->
                            num.GetAgeHint() =! Nullable()

            // Note: There are no examples of leap day coordination numbers in the test-data from Skatteverket, so we cannot run this test.
//            testProp "a person born on a leap day also ages by years counting from their date of birth"
//                <| fun (Gen.CoordNum.LeapDayCoordNum num, Gen.Age (years, months, days)) ->
//                    printfn "%A" num
//                    let dateOfBirth = getDateOfBirth num
//                    match dateOfBirth.Date with
//                    | Some date ->
//                        // Since there isn't a leap day every year we need to add 1 day to the checkdate
//                        let checkDate =
//                            date.Date
//                                .AddYears(years)
//                                .AddMonths(months)
//                                .AddDays(days + 1.)
//
//                        num.GetAgeHint checkDate =! Nullable years
//                    | None ->
//                        num.GetAgeHint() =! Nullable()

            testProp "cannot get age for date before person was born" <| fun (Gen.CoordNum.ValidNum num) ->
                let dateOfBirth = getDateOfBirth num
                match dateOfBirth.Date with
                | Some date ->
                    let checkOffset = rng.NextDouble() * 199. * 365.
                    let checkDate = date.Date.AddDays -checkOffset
                    toAction num.GetAgeHint checkDate
                    |> Expect.throwsWithMessage "The person is not yet born"
                | None ->
                    num.GetAgeHint() =! Nullable()

            testProp "getAgeHint uses DateTime.UtcNow as checkYear" <| fun (Gen.CoordNum.ValidNum num) ->
                isNotFromTheFuture num ==> lazy
                    let age1 = num.GetAgeHint DateTime.UtcNow
                    let age2 = num.GetAgeHint()
                    age1 =! age2
            ]

        testList "getDateOfBirthHint" [
            testProp "get date of birth hint extracts year, month and date from num" <| fun (Gen.CoordNum.ValidNum num) ->
                let result = num.GetDateOfBirthHint()
                if result.HasValue then
                    result.Value.Year =! num.Year
                    result.Value.Month =! num.Month
                    result.Value.Day =! num.RealDay
                else
                    // todo handle invalid date of birth-numbers
                    ()
            ]

        testList "getGenderHint" [
            testProp "even individualnumber indicates a woman, odd individualnumber a man" <| fun (Gen.CoordNum.ValidNum num) ->
                let numStr = num.To12DigitString()
                let individualNumber = numStr.[8..10] |> int
                match individualNumber with
                | Even -> num.GetGenderHint() =! Gender.Female
                | Odd -> num.GetGenderHint() =! Gender.Male
        ]
    ]
