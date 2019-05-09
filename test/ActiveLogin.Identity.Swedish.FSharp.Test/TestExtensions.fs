[<AutoOpen>]
module ActiveLogin.Identity.Swedish.FSharp.TestExtensions
open Expecto
open Expecto.Flip
open Swensen.Unquote

module Expect =
    let equalPin (expected: SwedishPersonalIdentityNumberValues) (actual: Result<SwedishPersonalIdentityNumber,_>) =
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

module Year =
    let createOrFail = Year.create >> Result.OkValue
    let map f y = y |> Year.value |> f |> createOrFail
