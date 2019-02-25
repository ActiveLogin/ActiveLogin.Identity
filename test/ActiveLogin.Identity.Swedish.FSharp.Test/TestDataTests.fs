module ActiveLogin.Identity.Swedish.FSharp.Test.TestDataTests
open Swensen.Unquote
open Expecto
open ActiveLogin.Identity.Swedish.FSharp
open ActiveLogin.Identity.Swedish.FSharp.TestData
open ActiveLogin.Identity.Swedish.FSharp.Test.PinTestHelpers
open Expecto.Flip
open FsCheck


[<Tests>]
let tests =
    testList "TestData" 
        [ test "raw12DigitStrings returns valid strings" {
            let pin = 
                SwedishPersonalIdentityNumberTestData.raw12DigitStrings 
                |> Array.head
                |> quickParse

            pin |> Expect.isSome "Should be a pin"
          } 
          test "isTestNumber returns true for a test number" {
            let pin = SwedishPersonalIdentityNumberTestData.getRandom()
            let result = pin |> SwedishPersonalIdentityNumber.isTestNumber
            result =! true
          }
          test "getRandomWithCount returns expected number of unique test numbers" {
              let pins = SwedishPersonalIdentityNumberTestData.getRandomWithCount 3 |> List.ofSeq
              pins |> List.distinct |> List.length =! 3
          } 
          test "getRandom returns random" {
              // with a non thread-safe implementation of the random generator in TestData this will actually fail when 
              // too many tests are running in parallel.
              let numUnique = 
                  seq { 1..5 }
                  |> Seq.map (fun _ -> SwedishPersonalIdentityNumberTestData.getRandom())
                  |> Seq.distinct
                  |> Seq.length 
              numUnique >! 1
          } ]