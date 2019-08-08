[<AutoOpen>]
module ActiveLogin.Identity.Swedish.FSharp.Test.PinTestHelpers
open ActiveLogin.Identity.Swedish.FSharp
open ActiveLogin.Identity.Swedish.FSharp.TestData
open Expecto
open System
open System.Threading

let private arbTypes = [ typeof<Gen.PinGenerators> ]
let private config =
    { FsCheckConfig.defaultConfig with arbitrary = arbTypes @ FsCheckConfig.defaultConfig.arbitrary }
let testProp name = testPropertyWithConfig config name
let ftestProp name = ftestPropertyWithConfig config name
let testPropWithMaxTest maxTest name = testPropertyWithConfig { config with maxTest = maxTest } name
let ftestPropWithMaxTest maxTest name = ftestPropertyWithConfig { config with maxTest = maxTest } name

let tee f x = f x |> ignore; x

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


type Rng =
    { Next: int * int -> int
      NextDouble: unit -> float }
let rng =
    // this thread-safe implementation is required to handle running lots of invocations of getRandom in parallel
    let seedGenerator = Random()
    let localGenerator = new ThreadLocal<Random>(fun _ ->
        lock seedGenerator (fun _ ->
            let seed = seedGenerator.Next()
            Random()))
    { Next = fun (min, max) -> localGenerator.Value.Next(min, max)
      NextDouble = localGenerator.Value.NextDouble }
