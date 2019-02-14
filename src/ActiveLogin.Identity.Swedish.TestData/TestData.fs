module ActiveLogin.Identity.Swedish.FSharp.TestData.SwedishPersonalIdentityNumber
open ActiveLogin.Identity.Swedish.FSharp.AllPins
open System

let random() =
    let rng = Random()
    allPins
    |> Array.sortBy (fun _ -> rng.Next())
    |> Array.head
