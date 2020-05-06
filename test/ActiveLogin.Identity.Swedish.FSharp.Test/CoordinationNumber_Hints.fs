/// <remarks>
/// Tested with official test Personal Identity Numbers from Skatteverket:
/// https://skatteverket.entryscape.net/catalog/9/datasets/147
/// </remarks>
module ActiveLogin.Identity.Swedish.FSharp.Test.CoordinationNumber_Hints

open Swensen.Unquote
open Expecto
open FsCheck
open ActiveLogin.Identity.Swedish.FSharp.Test.PinTestHelpers
open ActiveLogin.Identity.Swedish
open ActiveLogin.Identity.Swedish.Extensions


[<Tests>]
let tests =
    testList "CoordinationNumber.hints" [

        testList "getGenderHint" [
            testProp "even birthnumber indicates a woman, odd birthnumber a man" <| fun (Gen.CoordNum.ValidNum num) ->
                let numStr = num.To12DigitString()
                let individualNumber = numStr.[8..10] |> int
                match individualNumber with
                | Even -> num.GetGenderHint() =! Gender.Female
                | Odd -> num.GetGenderHint() =! Gender.Male
            test "missing hints tests" { Expect.isFalse true "missing tests here" }
        ]
    ]
