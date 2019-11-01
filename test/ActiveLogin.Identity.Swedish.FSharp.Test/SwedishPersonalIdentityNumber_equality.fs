/// <remarks>
/// Tested with offical test Personal Identity Numbers from Skatteverket:
/// https://skatteverket.entryscape.net/catalog/9/datasets/147
/// </remarks>
module ActiveLogin.Identity.Swedish.FSharp.Test.SwedishPersonalIdentityNumber_equality

open Swensen.Unquote
open Expecto
open FsCheck


[<Tests>]
let tests = testList "equality" [
    testProp "identical pins are equal when using operator" <|
        fun (Gen.TwoEqualPins (pin1, pin2)) ->
            pin1 = pin2 =! true
            pin2 = pin1 =! true
    testProp "identical pins are equal when using .Equals()" <|
        fun (Gen.TwoEqualPins (pin1, pin2)) ->
            pin1.Equals(pin2) =! true
            pin2.Equals(pin1) =! true
    testProp "identical pins are equal when using .Equals() and one pin is object" <|
        fun (Gen.TwoEqualPins (pin1, pin2)) ->
            let pin2 = pin2 :> obj
            pin1.Equals(pin2) =! true
            pin2.Equals(pin1) =! true
    testProp "different pins are not equal" <|
        fun (Gen.TwoPins (pin1, pin2)) ->
            pin1 <> pin2 ==> lazy
            pin1 <> pin2 =! true
            pin2 <> pin1 =! true
    testProp "different pins are not equal using .Equals()" <|
        fun (Gen.TwoPins (pin1, pin2)) ->
            pin1 <> pin2 ==> lazy
            pin1.Equals(pin2) =! false
            pin2.Equals(pin1) =! false
    testProp "a pin is not equal to null using .Equals()" <|
        fun (Gen.ValidPin pin) ->
            pin.Equals(null) =! false
    testProp "a pin is not equal to object null using .Equals()" <|
        fun (Gen.ValidPin pin) ->
            let nullObject = null :> obj
            pin.Equals(nullObject) =! false ]
