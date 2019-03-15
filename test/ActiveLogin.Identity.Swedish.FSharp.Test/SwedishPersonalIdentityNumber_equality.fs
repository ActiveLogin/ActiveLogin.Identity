/// <remarks>
/// Tested with offical test Personal Identity Numbers from Skatteverket:
/// https://skatteverket.entryscape.net/catalog/9/datasets/147
/// </remarks>
module ActiveLogin.Identity.Swedish.FSharp.Test.SwedishPersonalIdentityNumber_equality

open Swensen.Unquote
open Expecto
open ActiveLogin.Identity.Swedish.FSharp
open ActiveLogin.Identity.Swedish.FSharp.TestData
open Arbitraries
open FsCheck

let config = FsCheckConfig.defaultConfig

let testProp arbTypes name =
    testPropertyWithConfig (addToConfig config arbTypes) name
let ftestProp arbTypes name = 
    ftestPropertyWithConfig (addToConfig config arbTypes) name
let ptestProp arbTypes name = 
    ptestPropertyWithConfig (addToConfig config arbTypes) name

[<Tests>]
let tests = testList "equality" [ 
    testProp [ typeof<TwoEqualPins> ] "Identical pins are equal when using operator" <|
        fun (pin1: SwedishPersonalIdentityNumber, pin2: SwedishPersonalIdentityNumber) ->
            pin1 = pin2 =! true
            pin2 = pin1 =! true
    testProp [ typeof<TwoEqualPins> ] "Identical pins are equal when using .Equals()" <|
        fun (pin1: SwedishPersonalIdentityNumber, pin2: SwedishPersonalIdentityNumber) ->
            pin1.Equals(pin2) =! true
            pin2.Equals(pin1) =! true
    testProp [ typeof<TwoEqualPins> ] "Identical pins are equal when using .Equals() and one pin is object" <|
        fun (pin1: SwedishPersonalIdentityNumber, pin2: SwedishPersonalIdentityNumber) ->
            let pin2 = pin2 :> obj
            pin1.Equals(pin2) =! true
            pin2.Equals(pin1) =! true
    testProp [ typeof<TwoPins> ] "Different pins are not equal" <|
        fun (pin1: SwedishPersonalIdentityNumber, pin2: SwedishPersonalIdentityNumber) ->
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
