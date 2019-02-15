namespace ActiveLogin.Identity.Swedish.TestData
open ActiveLogin.Identity.Swedish.TestData.AllPins
open System

type SwedishPersonalIdentityTestNumber() =
    static let rng = Random()
    static let shuffled() =
        allPins
        |> Array.sortBy (fun _ -> rng.Next())
    static let random count = shuffled() |> Array.take count

    static member Random() = shuffled() |> Array.head
    static member Random(count) = random count
