[<AutoOpen>]
module ActiveLogin.Identity.Swedish.FSharp.TestExtensions
open Expecto
open Expecto.Flip
open Swensen.Unquote
open System

module Expect =
    let equalPin (expected: IdentityNumberValues) (actual: Result<SwedishPersonalIdentityNumber,_>) =
        actual |> Expect.isOk "should be ok"
        match actual with
        | Error _ -> failwith "test error"
        | Ok pin ->
            pin.Year |> Year.value =! expected.Year
            pin.Month |> Month.value =! expected.Month
            pin.Day |> Day.value =! expected.Day
            pin.BirthNumber |> BirthNumber.value =! expected.BirthNumber
            pin.Checksum |> Checksum.value =! expected.Checksum

module Result =
    let iter f res =
        match res with
        | Ok r -> f r
        | Error e -> ()

    let OkValue =
        function
        | Ok value -> value
        | Result.Error e ->
            e.ToString()
            |> sprintf "test setup error: %s"
            |> failwith

module SwedishPersonalIdentityNumber =
    let createOrFail = SwedishPersonalIdentityNumber.create >> Result.OkValue
    let parseOrFail = SwedishPersonalIdentityNumber.parse >> Result.OkValue

module SwedishCoordinationNumber =
    let createOrFail = SwedishCoordinationNumber.create >> Result.OkValue

module CoordinationDay =
    let createOrFail y m = CoordinationDay.create y m >> Result.OkValue

module Checksum =

    let createOrFail y m d b = Checksum.create y m d b >> Result.OkValue

    // copied from production code :(
    let getChecksum (year: Year) (month: Month) day (birth: BirthNumber) =
        let day' =
            match day with
            | Day d -> d.Value
            | CoordinationDay cd -> cd.Value
        let twoDigitYear = year.Value % 100
        let numberStr = sprintf "%02i%02i%02i%03i" twoDigitYear month.Value day' birth.Value
        let digits = numberStr |> Seq.map (fun s -> s.ToString() |> int)
        digits
        |> Seq.rev
        |> Seq.mapi (fun (i : int) (d : int) ->
               if i % 2 = 0 then d * 2
               else d)
        |> Seq.rev
        |> Seq.sumBy (fun (d : int) ->
               if d > 9 then d - 9
               else d)
        |> fun x -> (x * 9) % 10
        |> createOrFail year month day birth

module Year =
    let createOrFail = Year.create >> Result.OkValue
    let map f y = y |> Year.value |> f |> createOrFail

module Month =
    let createOrFail = Month.create >> Result.OkValue


    let getCheckSum (year) (month) day' (birth) =
        let twoDigitYear = year % 100
        let numberStr = sprintf "%02i%02i%02i%03i" twoDigitYear month day' birth
        let digits = numberStr |> Seq.map (fun s -> s.ToString() |> int)
        digits
        |> Seq.rev
        |> Seq.mapi (fun (i : int) (d : int) ->
               if i % 2 = 0 then d * 2
               else d)
        |> Seq.rev
        |> Seq.sumBy (fun (d : int) ->
               if d > 9 then d - 9
               else d)
        |> fun x -> (x * 9) % 10
