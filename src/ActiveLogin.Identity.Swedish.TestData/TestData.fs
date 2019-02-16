namespace ActiveLogin.Identity.Swedish.FSharp.TestData
open ActiveLogin.Identity.Swedish.TestData.AllPins
open ActiveLogin.Identity.Swedish.FSharp
open System
open System.Collections.Generic

module SwedishPersonalIdentityTestNumbers =
    let private rng = Random()
    let private random _ = rng.Next()
    let private allWith12Digits = allPins |> Array.map fst
    let internal shuffled12Digits() = allWith12Digits |> Array.sortBy random

    let random12DigitString() = shuffled12Digits() |> Array.head 
    let random12DigitStrings(count) = shuffled12Digits() |> Array.take count :> IReadOnlyCollection<_>
    let all12DigitStringsSorted = allWith12Digits :> IReadOnlyCollection<_>
    let all12DigitStringsShuffled() = shuffled12Digits() :> IReadOnlyCollection<_>

    let private allWith10Digits = allPins |> Array.map snd
    let private shuffled10Digits() = allWith10Digits |> Array.sortBy random

    let random10DigitString() = shuffled10Digits() |> Array.head
    let random10DigitStrings(count) = shuffled10Digits() |> Array.take count :> IReadOnlyCollection<_>
    let all10DigitStringsSorted = allWith10Digits :> IReadOnlyCollection<_>
    let all10DigitStringsShuffled() = shuffled10Digits() :> IReadOnlyCollection<_>

    let internal parse str =
        match SwedishPersonalIdentityNumber.parse str with
        | Ok p -> p
        | Error _ -> failwith "broken test data" 

    let randomPin() = random12DigitString() |> parse 
    let randomPins(count) = shuffled12Digits() |> Array.take count |> Array.map parse :> IReadOnlyCollection<_>
    let allPinsSorted() = seq { for str in allWith12Digits do yield parse str }
    let allPinsShuffled() = seq { for str in shuffled12Digits() do yield parse str }

    let internal isTestNumberString str =
        allPins |> Array.map fst |> Array.contains str

    let isTestNumber pin =
        pin
        |> SwedishPersonalIdentityNumber.to12DigitString
        |> isTestNumberString
