namespace ActiveLogin.Identity.Swedish.TestData
open ActiveLogin.Identity.Swedish.TestData.AllPins
open System
open System.Collections.Generic

type SwedishPersonalIdentityTestNumber() =
    
    static let shuffled() = 
        let rng = Random()
        allPins |> Array.sortBy (fun _ -> rng.Next())

    static member Random() = shuffled() |> Array.head
    static member Random(count) = shuffled() |> Array.take count |> seq
    static member AllInOrder = allPins :> IReadOnlyCollection<_>
    static member AllShuffled() = shuffled() :> IReadOnlyCollection<_>
