[<AutoOpen>]
module ActiveLogin.Identity.Swedish.FSharp.Test.PinTestHelpers
open ActiveLogin.Identity.Swedish.FSharp
open ActiveLogin.Identity.Swedish.FSharp.TestData
open Expecto
open System
open System.Threading

let toAction f arg =
    fun _ -> f arg |> ignore

let toAction2 f arg1 arg2 =
    fun _ -> f arg1 arg2 |> ignore

let (|Even|Odd|) num =
    match num % 2 with
    | 0 -> Even
    | _ -> Odd


module Expect =
    let throwsWithType<'texn> f =
        Expect.throwsT<'texn>
            f
            "Should throw with expected type"

    let throwsWithMessages (msgs: string list) f =
        Expect.throwsC
            f
            (fun exn ->
                let failureMessage msg = sprintf "Should contain expected error message: %s" msg
                msgs
                |> List.iter
                    (fun msg ->
                        Expect.stringContains exn.Message msg (failureMessage msg))
                    )

    let throwsWithMessage (msg:string) f = throwsWithMessages [ msg ] f

    let doesNotThrowWithMessage (msg:string) f arg =
        let thrown =
            try
                f arg |> ignore
                None
            with e ->
                Some e

        match thrown with
            | None ->
                // Ok does not throw
                ()
            | Some exn ->
                if exn.Message.ToLower().Contains(msg.ToLower())
                then
                    failtestf "Should not throw exception with message: %s" msg

let private arbTypes = [ typeof<Gen.ValueGenerators> ]
let private config =
    { FsCheckConfig.defaultConfig with arbitrary = arbTypes @ FsCheckConfig.defaultConfig.arbitrary }
let testProp name = testPropertyWithConfig config name
let ftestProp name = ftestPropertyWithConfig config name
let testPropWithMaxTest maxTest name = testPropertyWithConfig { config with maxTest = maxTest } name
let ftestPropWithMaxTest maxTest name = ftestPropertyWithConfig { config with maxTest = maxTest } name

let tee f x = f x |> ignore; x

let quickParse (str:string) =
    let values =
        ( str.[ 0..3 ] |> int,
          str.[ 4..5 ] |> int,
          str.[ 6..7 ] |> int,
          str.[ 8..10 ] |> int,
          str.[ 11..11 ] |> int )
    SwedishPersonalIdentityNumber.create values

let pinToValues (pin:SwedishPersonalIdentityNumber) =
    ( pin.Year,
      pin.Month,
      pin.Day,
      pin.BirthNumber,
      pin.Checksum )


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


let getRandomFromArray arr =
    fun () ->
        let index = rng.Next(0, Array.length arr - 1)
        arr.[index]

let removeHyphen (str:string) =
    let isHyphen (c:char) = "-".Contains(c)
    String.filter (isHyphen >> not) str

let surroundEachChar (chars:char[]) (pin:string) =
    let rnd = getRandomFromArray chars
    let surroundWith c = [| rnd(); c; rnd() |]

    Seq.collect surroundWith pin
    |> Array.ofSeq
    |> System.String

let isDigit (c:char) = "0123456789".Contains(c)

