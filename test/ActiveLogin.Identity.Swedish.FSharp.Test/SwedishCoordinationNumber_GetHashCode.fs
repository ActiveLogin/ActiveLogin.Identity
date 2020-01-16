/// <remarks>
/// Tested with official test Personal Identity Numbers from Skatteverket:
/// https://skatteverket.entryscape.net/catalog/9/datasets/147
/// </remarks>
module ActiveLogin.Identity.Swedish.FSharp.Test.SwedishCoordinationNumber_hash
open Expecto
open Swensen.Unquote

[<Tests>]
let tests = testList "SwedishCoordinationNumber.hash" [
    testProp "identical numbers have the same hash code" <|
        fun (Gen.CoordNum.TwoEqualCoordNums (num1, num2)) ->
            hash num1 =! hash num2 ]
