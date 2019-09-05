module ActiveLogin.Identity.Swedish.FSharp.Test.Gen

open FsCheck
open ActiveLogin.Identity.Swedish.FSharp
open ActiveLogin.Identity.Swedish.FSharp.TestData
open System

let private chooseFromArray xs =
    gen { let! index = Gen.choose (0, (Array.length xs) - 1)
          return xs.[index] }

type EmptyString = EmptyString of string
type Digits = Digits of string
type Max200 = Max200 of int
type Valid12Digit = Valid12Digit of string
type ValidValues = ValidValues of SwedishPersonalIdentityNumberValues
type InvalidYear = InvalidYear of int
type InvalidMonth = InvalidMonth of int
type ValidYear = ValidYear of int
type InvalidPinString = InvalidPinString of string
type ValidPin = ValidPin of SwedishPersonalIdentityNumber
type ValidMonth = ValidMonth of int
type WithInvalidDay = WithInvalidDay of SwedishPersonalIdentityNumberValues
type WithValidDay = WithValidDay of SwedishPersonalIdentityNumberValues
type InvalidBirthNumber = InvalidBirthNumber of int
type ValidBirthNumber = ValidBirthNumber of int
type TwoEqualPins = TwoEqualPins of SwedishPersonalIdentityNumber * SwedishPersonalIdentityNumber
type TwoPins = TwoPins of SwedishPersonalIdentityNumber * SwedishPersonalIdentityNumber
type Char100 = Char100 of char[]
type Age = Age of Years: int * Months : int * Days: double
type LeapDayPin = LeapDayPin of SwedishPersonalIdentityNumber


let stringToValues (pin : string) =
    { Year = pin.[0..3] |> int
      Month = pin.[4..5] |> int
      Day = pin.[6..7] |> int
      BirthNumber = pin.[8..10] |> int
      Checksum = pin.[11..11] |> int }

module Generators =

    let max200Gen() = Gen.choose(-100, 200) |> Gen.map Max200 |> Arb.fromGen

    let emptyStringGen() =
        let emptyStringWithLength length = String.replicate length " "
        Gen.sized(fun s -> Gen.choose(0,s) |> Gen.map emptyStringWithLength)
        |> Gen.map EmptyString
        |> Arb.fromGen

    let digitsGen() =
        let createDigits (strs: string list) =
            System.String.Join("", strs)
            |> Digits
        Gen.choose(0,9)
        |> Gen.map string
        |> Gen.listOf
        |> Gen.map createDigits
        |> Arb.fromGen

    let valid12Digit = chooseFromArray SwedishPersonalIdentityNumberTestData.raw12DigitStrings
    let valid12DigitGen() = valid12Digit |> Gen.map Valid12Digit |> Arb.fromGen

    let validValues = valid12Digit |> Gen.map (stringToValues >> ValidValues)
    let validValuesGen() = validValues |> Arb.fromGen

    let outsideRange min max =
        let low = Gen.choose (Int32.MinValue, min - 1)
        let high = Gen.choose (max + 1, Int32.MaxValue)
        Gen.oneof [ low; high ]

    let invalidYearGen() =
        (1, 9999)
        ||> outsideRange
        |> Gen.map InvalidYear
        |> Arb.fromGen

    let validYear = Gen.choose (1, 9999)

    let validYearGen() =
        validYear
        |> Gen.map ValidYear
        |> Arb.fromGen

    let invalidMonthGen() =
        (1, 12)
        ||> outsideRange
        |> Gen.map InvalidMonth
        |> Arb.fromGen


    let private validMonth =
        (1,12)
        |> Gen.choose

    let validMonthGen() =
        validMonth
        |> Gen.map ValidMonth
        |> Arb.fromGen

    let withInvalidDay =
        gen {
            let! (ValidValues validValues) = validValues
            let daysInMonth = DateTime.DaysInMonth(validValues.Year, validValues.Month)
            let! invalidDay = outsideRange 1 daysInMonth
            return { validValues with Day = invalidDay } |> WithInvalidDay
        }

    let withInvalidDayGen() = withInvalidDay |> Arb.fromGen


    let private validDay year month =
        let daysInMonth = DateTime.DaysInMonth(year, month)
        Gen.choose (1, daysInMonth)

    let withValidDay =
        gen {
            let! (ValidValues validValues) = validValues
            let! validDay = validDay validValues.Year validValues.Month
            return { validValues with Day = validDay } |> WithValidDay
        }

    let withValidDayGen() = withValidDay |> Arb.fromGen


    let invalidBirthNumberGen() =
        (1, 999)
        ||> outsideRange
        |> Gen.map InvalidBirthNumber
        |> Arb.fromGen


    let validBirthNumberGen() =
        (1, 999)
        |> Gen.choose
        |> Gen.map ValidBirthNumber
        |> Arb.fromGen


    let twoEqualPinsGen() =
        gen {
            let! (ValidValues values) = validValues
            let pin1 =
                values
                |> SwedishPersonalIdentityNumber.createOrFail

            let pin2 =
                values
                |> SwedishPersonalIdentityNumber.createOrFail

            return (pin1, pin2) |> TwoEqualPins
        }
        |> Arb.fromGen


    let twoPinsGen() =
        gen {
            let pin1 = SwedishPersonalIdentityNumberTestData.getRandom()
            let pin2 = SwedishPersonalIdentityNumberTestData.getRandom()
            return (pin1, pin2) |> TwoPins
        }
        |> Arb.fromGen


    let validPinGen() =
        gen { return SwedishPersonalIdentityNumberTestData.getRandom() |> ValidPin }
        |> Arb.fromGen

    let invalidPinStringGen() =
        gen {
            let! valid = valid12Digit
            let withInvalidYear =
                gen {
                    return "0000" + valid.[ 4.. ]
                }
            let withInvalidMonth =
                gen {
                    let! month = Gen.choose(13,99) |> Gen.map string
                    return valid.[ 0..3 ] + month + valid.[ 6.. ]
                }
            let withInvalidDay =
                gen {
                    let year = valid.[ 0..3 ] |> int
                    let month = valid.[ 4..5 ] |> int
                    let daysInMonth = DateTime.DaysInMonth(year, month)
                    let! day = Gen.choose(daysInMonth + 1, 99) |> Gen.map string
                    return valid.[ 0..5 ] + day + valid.[ 8.. ]
                }
            let withInvalidBirthNumber =
                gen {
                    return valid.[ 0..7 ] + "000" + valid.[ 11.. ]
                }
            let withInvalidChecksum =
                let checksum = valid.[ 11.. ]
                let invalid = checksum |> int |> fun i -> (i + 1) % 10 |> string
                gen { return valid.[ 0..10 ] + invalid }
            return! Gen.oneof [ withInvalidYear; withInvalidMonth; withInvalidDay; withInvalidBirthNumber; withInvalidChecksum ]
        } |> Gen.map InvalidPinString |> Arb.fromGen

    let char100() =
        Gen.arrayOfLength 100 Arb.generate<char>
        |> Gen.map Char100
        |> Arb.fromGen

    let age() =
        gen {
            let! years = Gen.choose(0, 199)
            let! months = Gen.choose(0, 11)
            let! days = Gen.choose(0,27) |> Gen.map float

            return Age (Years = years, Months = months,Days = days)
        } |> Arb.fromGen

    let private leapDayPins =
        let isLeapDay (pin: SwedishPersonalIdentityNumber) =
            pin.Month.Value = 2 && pin.Day.Value = 29

        SwedishPersonalIdentityNumberTestData.allPinsShuffled()
        |> Seq.filter isLeapDay
        |> Seq.toArray

    let leapDayPinGen() =
        leapDayPins
        |> chooseFromArray
        |> Gen.map LeapDayPin
        |> Arb.fromGen


open Generators

type PinGenerators() =
    static member EmptyString() = emptyStringGen()
    static member Digits() = digitsGen()
    static member Max200() = max200Gen()
    static member Valid12Digit() = valid12DigitGen()
    static member ValidValues() = validValuesGen()
    static member InvalidYear() = invalidYearGen()
    static member InvalidMonth() = invalidMonthGen()
    static member ValidYear() = validYearGen()
    static member InvalidPinString() = invalidPinStringGen()
    static member ValidPin() = validPinGen()
    static member ValidMonth() = validMonthGen()
    static member WithInvalidDay() = withInvalidDayGen()
    static member WithValidDay() = withValidDayGen()
    static member InvalidBirthNumber() = invalidBirthNumberGen()
    static member ValidBirthNumber() = validBirthNumberGen()
    static member TwoEqualPins() = twoEqualPinsGen()
    static member TwoPins() = twoPinsGen()
    static member Char100() = char100()
    static member Age() = age()
    static member LeapDayPin() = leapDayPinGen()
