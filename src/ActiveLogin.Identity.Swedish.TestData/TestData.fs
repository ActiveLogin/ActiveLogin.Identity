namespace ActiveLogin.Identity.Swedish.FSharp.TestData
open ActiveLogin.Identity.Swedish.TestData.AllPins
open ActiveLogin.Identity.Swedish.FSharp
open System

module SwedishPersonalIdentityNumberTestData =
    let private rng = Random()
    let private random _ = rng.Next()
    let private all12Digits = allPins 
    let internal shuffled12Digits() = all12Digits |> Array.sortBy random

    let internal parse str =
        match SwedishPersonalIdentityNumber.parse str with
        | Ok p -> p
        | Error _ -> failwith "broken test data" 

    let allPinsSorted() = seq { for str in all12Digits do yield parse str }
    let allPinsShuffled() = seq { for str in shuffled12Digits() do yield parse str }
    let getRandom() = allPinsShuffled() |> Seq.head 
    let getRandomWithCount(count) = allPinsShuffled() |> Seq.take count

    let internal isTestNumberString str =
        allPins |> Array.contains str

    let isTestNumber pin =
        pin
        |> SwedishPersonalIdentityNumber.to12DigitString
        |> isTestNumberString
