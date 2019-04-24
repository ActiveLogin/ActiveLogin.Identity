module ActiveLogin.Identity.Swedish.FSharp.Test.PinTestHelpers
open ActiveLogin.Identity.Swedish.FSharp
open ActiveLogin.Identity.Swedish.FSharp.TestData
open Swensen.Unquote
open Expecto.Flip


let quickParseR (str:string) = 
    let values = 
        { Year = str.[ 0..3 ] |> int
          Month = str.[ 4..5 ] |> int 
          Day = str.[ 6..7 ] |> int
          BirthNumber = str.[ 8..10 ] |> int
          Checksum = str.[ 11..11 ] |> int }
    SwedishPersonalIdentityNumber.create values

let quickParse str = 
    match quickParseR str with 
    | Ok p -> p 
    | Error e -> e.ToString() |> failwithf "Test setup error %s"

let pinToValues (pin:SwedishPersonalIdentityNumber) =
    { Year = pin.Year |> Year.value 
      Month = pin.Month |> Month.value
      Day = pin.Day |> Day.value
      BirthNumber = pin.BirthNumber |> BirthNumber.value
      Checksum = pin.Checksum |> Checksum.value }

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

