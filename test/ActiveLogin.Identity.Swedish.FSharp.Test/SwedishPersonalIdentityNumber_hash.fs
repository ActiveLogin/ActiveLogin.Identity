/// <remarks>
/// Tested with offical test Personal Identity Numbers from Skatteverket:
/// https://skatteverket.entryscape.net/catalog/9/datasets/147
/// </remarks>
module ActiveLogin.Identity.Swedish.FSharp.Test.SwedishPersonalIdentityNumber_hash
open Expecto
open Arbitraries
open Swensen.Unquote
open ActiveLogin.Identity.Swedish.FSharp

let config = FsCheckConfig.defaultConfig

let addToConfig arbTypes = { config with arbitrary = arbTypes @ config.arbitrary }

let testProp arbTypes name =
    testPropertyWithConfig (addToConfig arbTypes) name
let fTestProp arbTypes name = 
    ftestPropertyWithConfig (addToConfig arbTypes) name
let pTestProp arbTypes name = 
    ptestPropertyWithConfig (addToConfig arbTypes) name

[<Tests>]
let tests = testList "hash" [ 
    testProp [ typeof<TwoEqualPins> ] "Identical pins have the same hash code" <|
        fun (pin1: SwedishPersonalIdentityNumber, pin2: SwedishPersonalIdentityNumber) ->
            hash pin1 =! hash pin2 ]
