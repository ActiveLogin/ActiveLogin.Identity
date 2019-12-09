namespace ActiveLogin.Identity.Swedish.Benchmark

open System
open BenchmarkDotNet.Attributes
open ActiveLogin.Identity.Swedish.TestData

type BenchmarkPinAccess() =
    let rng = Random()

    [<Params(100000)>]
    member val public N = 0 with get, set

    [<Benchmark>]
    member __.GetRandomPin() =
        for i = 0 to __.N do
            SwedishPersonalIdentityNumberTestData.GetRandom() |> ignore

    [<Benchmark>]
    member __.Get12DigitStringFromRandomPin() =
        for i = 0 to __.N do
            SwedishPersonalIdentityNumberTestData.GetRandom().To12DigitString()
            |> ignore

    [<Benchmark>]
    member __.GetRandom12DigitStringRaw() =
        for i = 0 to __.N do
            let index = rng.Next(0, Array.length SwedishPersonalIdentityNumberTestData.Raw12DigitStrings - 1)
            SwedishPersonalIdentityNumberTestData.Raw12DigitStrings.[index]
            |> ignore

    [<Benchmark>]
    member __.GetRandomPin4000Times() =
        for i = 0 to __.N/4000 do
            for j = 0 to 4000 do
                SwedishPersonalIdentityNumberTestData.GetRandom()
                |> ignore

    [<Benchmark>]
    member __.Get4000RandomPins() =
        for i = 0 to __.N/4000 do
            SwedishPersonalIdentityNumberTestData.GetRandom 4000
            |> List.ofSeq
            |> ignore

    [<Benchmark>]
    member __.GetAllRandomPins() =
        for i = 0 to __.N/40000 do
            SwedishPersonalIdentityNumberTestData.AllPinsShuffled()
            |> List.ofSeq
            |> ignore

