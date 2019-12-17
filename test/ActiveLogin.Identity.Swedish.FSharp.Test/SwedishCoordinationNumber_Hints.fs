/// <remarks>
/// Tested with official test Personal Identity Numbers from Skatteverket:
/// https://skatteverket.entryscape.net/catalog/9/datasets/147
/// </remarks>
module ActiveLogin.Identity.Swedish.FSharp.Test.SwedishCoordinationNumber_Hints

open Swensen.Unquote
open Expecto
open Expecto.Flip
open FsCheck
open System
open ActiveLogin.Identity.Swedish.FSharp.Test.PinTestHelpers
open ActiveLogin.Identity.Swedish
open ActiveLogin.Identity.Swedish.Extensions


[<Tests>]
let tests =
    testList "SwedishCoordinationNumber.hints" [

        testList "getGenderHint" [
            testProp "even birthnumber indicates a female, odd birthnumber a male" <| fun (Gen.CoordNum.ValidNum num) ->
                let numStr = num.To12DigitString()
                let individualNumber = numStr.[8..10] |> int
                match individualNumber with
                | Even -> num.GetGenderHint() =! Gender.Female
                | Odd -> num.GetGenderHint() =! Gender.Male
        ]
    ]
