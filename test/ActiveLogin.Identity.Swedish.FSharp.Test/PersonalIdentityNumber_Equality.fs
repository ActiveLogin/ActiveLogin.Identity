/// <remarks>
/// Tested with official test Personal Identity Numbers from Skatteverket:
/// https://skatteverket.entryscape.net/catalog/9/datasets/147
/// </remarks>
module ActiveLogin.Identity.Swedish.FSharp.Test.PersonalIdentityNumber_equality

open ActiveLogin.Identity.Swedish
open Swensen.Unquote
open Expecto
open FsCheck


[<Tests>]
let tests = testList "PersonalIdentityNumber.equality" [
    testProp "identical pins are equal when using operator" <|
        fun (Gen.Pin.TwoEqualPins (pin1, pin2)) ->
            pin1 = pin2 =! true
            pin2 = pin1 =! true
            PersonalIdentityNumber.op_Equality(pin1, pin2) =! true
            PersonalIdentityNumber.op_Equality(pin2, pin1) =! true
    testProp "identical pins are not unequal when using operator" <|
        fun (Gen.Pin.TwoEqualPins (pin1, pin2)) ->
            pin1 <> pin2 =! false
            pin2 <> pin1 =! false
            PersonalIdentityNumber.op_Inequality(pin1, pin2) =! false
            PersonalIdentityNumber.op_Inequality(pin2, pin1) =! false
    testProp "identical pins are equal when using .Equals()" <|
        fun (Gen.Pin.TwoEqualPins (pin1, pin2)) ->
            pin1.Equals(pin2) =! true
            pin2.Equals(pin1) =! true
    testProp "identical pins are equal when using .Equals() and one pin is object" <|
        fun (Gen.Pin.TwoEqualPins (pin1, pin2)) ->
            let pin2 = pin2 :> obj
            pin1.Equals(pin2) =! true
            pin2.Equals(pin1) =! true
    testProp "different pins are not equal" <|
        fun (Gen.Pin.TwoPins (pin1, pin2)) ->
            pin1 <> pin2 ==> lazy
            pin1 <> pin2 =! true
            pin2 <> pin1 =! true
    testProp "different pins are not equal using .Equals()" <|
        fun (Gen.Pin.TwoPins (pin1, pin2)) ->
            pin1 <> pin2 ==> lazy
            pin1.Equals(pin2) =! false
            pin2.Equals(pin1) =! false
    testProp "a pin is not equal to null using .Equals()" <|
        fun (Gen.Pin.ValidPin pin) ->
            pin.Equals(null) =! false
    testProp "a pin is not equal to object null using .Equals()" <|
        fun (Gen.Pin.ValidPin pin) ->
            let nullObject = null :> obj
            pin.Equals(nullObject) =! false ]
