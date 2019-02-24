module ActiveLogin.Identity.Swedish.FSharp.Test.TestDataTests
open Swensen.Unquote
open Expecto
open ActiveLogin.Identity.Swedish.FSharp
open ActiveLogin.Identity.Swedish.FSharp.TestData
open ActiveLogin.Identity.Swedish.FSharp.Test.PinTestHelpers
open Expecto.Flip


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
          } ]
