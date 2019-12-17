/// <remarks>
/// Tested with offical test Personal Identity Numbers from Skatteverket:
/// https://skatteverket.entryscape.net/catalog/9/datasets/147
/// </remarks>
module ActiveLogin.Identity.Swedish.FSharp.Test.SwedishCoordinationNumber_equality

open ActiveLogin.Identity.Swedish
open Swensen.Unquote
open Expecto
open FsCheck


[<Tests>]
let tests = testList "SwedishCoordinationNumber.equality" [
    testProp "identical numbers are equal when using operator" <|
        fun (Gen.CoordNum.TwoEqualCoordNums (num1, num2)) ->
            num1 = num2 =! true
            num2 = num1 =! true
            SwedishCoordinationNumber.op_Equality(num1, num2) =! true
            SwedishCoordinationNumber.op_Equality(num2, num1) =! true
    testProp "identical numbers are not unequal when using operator" <|
        fun (Gen.CoordNum.TwoEqualCoordNums (num1, num2)) ->
            num1 <> num2 =! false
            num2 <> num1 =! false
            SwedishCoordinationNumber.op_Inequality(num1, num2) =! false
            SwedishCoordinationNumber.op_Inequality(num2, num1) =! false
    testProp "identical numbers are equal when using .Equals()" <|
        fun (Gen.CoordNum.TwoEqualCoordNums (num1, num2)) ->
            num1.Equals(num2) =! true
            num2.Equals(num1) =! true
    testProp "identical numbers are equal when using .Equals() and one number is object" <|
        fun (Gen.CoordNum.TwoEqualCoordNums (num1, num2)) ->
            let num2 = num2 :> obj
            num1.Equals(num2) =! true
            num2.Equals(num1) =! true
    testProp "different numbers are not equal" <|
        fun (Gen.CoordNum.TwoCoordNums (num1, num2)) ->
            num1 <> num2 ==> lazy
            num1 <> num2 =! true
            num2 <> num1 =! true
    testProp "different numbers are not equal using .Equals()" <|
        fun (Gen.CoordNum.TwoCoordNums (num1, num2)) ->
            num1 <> num2 ==> lazy
            num1.Equals(num2) =! false
            num2.Equals(num1) =! false
    testProp "a num is not equal to null using .Equals()" <|
        fun (Gen.CoordNum.ValidNum num) ->
            num.Equals(null) =! false
    testProp "a num is not equal to object null using .Equals()" <|
        fun (Gen.CoordNum.ValidNum num) ->
            let nullObject = null :> obj
            num.Equals(nullObject) =! false ]
