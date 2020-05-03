/// <remarks>
/// Tested with official test Personal Identity Numbers from Skatteverket:
/// https://skatteverket.entryscape.net/catalog/9/datasets/147
/// </remarks>
module ActiveLogin.Identity.Swedish.FSharp.Test.PersonalIdentityNumber_hash
open Expecto
open Swensen.Unquote

[<Tests>]
let tests = testList "PersonalIdentityNumber.hash" [
    testProp "identical pins have the same hash code" <|
        fun (Gen.Pin.TwoEqualPins (pin1, pin2)) ->
            hash pin1 =! hash pin2 ]
