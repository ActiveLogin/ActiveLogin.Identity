/// <remarks>
/// Tested with offical test Personal Identity Numbers from Skatteverket:
/// https://skatteverket.entryscape.net/catalog/9/datasets/147
/// </remarks>
module ActiveLogin.Identity.Swedish.FSharp.Test.SwedishPersonalIdentityNumber_hash
open Expecto
open Swensen.Unquote

let arbTypes = [ typeof<Gen.TwoEqualPinsGen> ]


let config = 
    { FsCheckConfig.defaultConfig with arbitrary = arbTypes @ FsCheckConfig.defaultConfig.arbitrary }
let testProp name = testPropertyWithConfig config name
let ftestProp name = ftestPropertyWithConfig config name
let testPropWithMaxTest maxTest name = testPropertyWithConfig { config with maxTest = maxTest } name
let ftestPropWithMaxTest maxTest name = ftestPropertyWithConfig { config with maxTest = maxTest } name

[<Tests>]
let tests = testList "hash" [ 
    testProp "Identical pins have the same hash code" <|
        fun (Gen.TwoEqualPins (pin1, pin2)) ->
            hash pin1 =! hash pin2 ]
