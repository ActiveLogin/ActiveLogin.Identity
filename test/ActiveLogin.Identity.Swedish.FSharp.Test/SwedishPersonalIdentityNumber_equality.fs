/// <remarks>
/// Tested with offical test Personal Identity Numbers from Skatteverket:
/// https://skatteverket.entryscape.net/catalog/9/datasets/147
/// </remarks>
module ActiveLogin.Identity.Swedish.FSharp.Test.SwedishPersonalIdentityNumber_equality

open Swensen.Unquote
open Expecto
open ActiveLogin.Identity.Swedish.FSharp
open FsCheck

let arbTypes = 
    [ typeof<Gen.TwoEqualPinsGen> 
      typeof<Gen.TwoPinsGen> ]


let config = 
    { FsCheckConfig.defaultConfig with arbitrary = arbTypes @ FsCheckConfig.defaultConfig.arbitrary }
let testProp name = testPropertyWithConfig config name
let ftestProp name = ftestPropertyWithConfig config name
let testPropWithMaxTest maxTest name = testPropertyWithConfig { config with maxTest = maxTest } name
let ftestPropWithMaxTest maxTest name = ftestPropertyWithConfig { config with maxTest = maxTest } name

[<Tests>]
let tests = testList "equality" [ 
    testProp "Identical pins are equal when using operator" <|
        fun (Gen.TwoEqualPins (pin1, pin2)) ->
            pin1 = pin2 =! true
            pin2 = pin1 =! true
    testProp "Identical pins are equal when using .Equals()" <|
        fun (Gen.TwoEqualPins (pin1, pin2)) ->
            pin1.Equals(pin2) =! true
            pin2.Equals(pin1) =! true
    testProp "Identical pins are equal when using .Equals() and one pin is object" <|
        fun (Gen.TwoEqualPins (pin1, pin2)) ->
            let pin2 = pin2 :> obj
            pin1.Equals(pin2) =! true
            pin2.Equals(pin1) =! true
    testProp "Different pins are not equal" <|
        fun (Gen.TwoPins (pin1, pin2)) ->
            pin1 <> pin2 ==> lazy 
            pin1 <> pin2 =! true
            pin2 <> pin1 =! true
    testProp [ typeof<TwoPins> ] "Different pins are not equal using .Equals()" <|
        fun (pin1: SwedishPersonalIdentityNumber, pin2: SwedishPersonalIdentityNumber) ->
            pin1 <> pin2 ==> lazy
            pin1.Equals(pin2) =! false
            pin2.Equals(pin1) =! false
    testProp [ typeof<ValidPin> ] "A pin is not equal to null using .Equals()" <|
        fun (pin:SwedishPersonalIdentityNumber) ->
            pin.Equals(null) =! false
    testProp [ typeof<ValidPin> ] "A pin is not equal to object null using .Equals()" <|
        fun (pin: SwedishPersonalIdentityNumber) ->
            let nullObject = null :> obj
            pin.Equals(nullObject) =! false ]
