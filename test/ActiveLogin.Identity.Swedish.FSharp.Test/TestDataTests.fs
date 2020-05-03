module ActiveLogin.Identity.Swedish.FSharp.Test.TestDataTests
open Swensen.Unquote
open Expecto
open ActiveLogin.Identity.Swedish
open ActiveLogin.Identity.Swedish.FSharp.Test.PinTestHelpers
open ActiveLogin.Identity.Swedish.TestData


[<Tests>]
let tests =
    testList "TestData"
        [ test "raw12DigitStrings returns valid strings" {
            PersonalIdentityNumberTestData.Raw12DigitStrings
            |> Array.head
            |> quickParse
            |> ignore

          }
          test "isTestNumber returns true for a test number" {
            let pin = PersonalIdentityNumberTestData.GetRandom()
            pin.IsTestNumber() =! true
          }
          test "getRandomWithCount returns expected number of unique test numbers" {
              let pins = PersonalIdentityNumberTestData.GetRandom 3 |> List.ofSeq
              pins |> List.distinct |> List.length =! 3
          }
          test "getRandom returns random" {
              // with a non thread-safe implementation of the random generator in TestData this will actually fail when
              // too many tests are running in parallel.
              let numUnique =
                  seq { 1..5 }
                  |> Seq.map (fun _ -> PersonalIdentityNumberTestData.GetRandom())
                  |> Seq.distinct
                  |> Seq.length
              numUnique >! 1
          } ]
