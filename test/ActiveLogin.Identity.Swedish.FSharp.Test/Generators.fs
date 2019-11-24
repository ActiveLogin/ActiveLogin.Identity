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
type InvalidYear = InvalidYear of int
type InvalidMonth = InvalidMonth of int
type ValidYear = ValidYear of int
type ValidMonth = ValidMonth of int
type InvalidBirthNumber = InvalidBirthNumber of int
type ValidBirthNumber = ValidBirthNumber of int
type Char100 = Char100 of char []
type Age = Age of Years: int * Months: int * Days: double
type IdentityNumberValues = (int * int * int * int * int)

module Pin =
    type Valid12Digit = Valid12Digit of string
    type ValidValues = ValidValues of IdentityNumberValues
    type InvalidPinString = InvalidPinString of string
    type ValidPin = ValidPin of SwedishPersonalIdentityNumber
    type WithInvalidDay = WithInvalidDay of IdentityNumberValues
    type WithValidDay = WithValidDay of IdentityNumberValues
    type TwoEqualPins = TwoEqualPins of SwedishPersonalIdentityNumber * SwedishPersonalIdentityNumber
    type TwoPins = TwoPins of SwedishPersonalIdentityNumber * SwedishPersonalIdentityNumber
    type LeapDayPin = LeapDayPin of SwedishPersonalIdentityNumber

module CoordNum =
    type ValidNum = ValidNum of SwedishCoordinationNumber
    type Valid12Digit = Valid12Digit of string
    type InvalidNumString = InvalidNumString of string
    type TwoEqualCoordNums = TwoEqualCoordNums of SwedishCoordinationNumber * SwedishCoordinationNumber
    type ValidValues = ValidValues of IdentityNumberValues
    type TwoCoordNums = TwoCoordNums of SwedishCoordinationNumber * SwedishCoordinationNumber
    type WithInvalidDay = WithInvalidDay of IdentityNumberValues
    type WithValidDay = WithValidDay of IdentityNumberValues
    type LeapDayCoordNum = LeapDayCoordNum of SwedishCoordinationNumber


let stringToValues (pin: string) =
    ( pin.[0..3] |> int,
      pin.[4..5] |> int,
      pin.[6..7] |> int,
      pin.[8..10] |> int,
      pin.[11..11] |> int )

module Generators =

    let max200Gen() = Gen.choose (-100, 200) |> Gen.map Max200 |> Arb.fromGen

    let emptyStringGen() =
        let emptyStringWithLength length = String.replicate length " "
        Gen.sized (fun s -> Gen.choose (0, s) |> Gen.map emptyStringWithLength)
        |> Gen.map EmptyString
        |> Arb.fromGen

    let digitsGen() =
        let createDigits (strs: string list) =
            System.String.Join("", strs)
            |> Digits
        Gen.choose (0, 9)
        |> Gen.map string
        |> Gen.listOf
        |> Gen.map createDigits
        |> Arb.fromGen

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
        (1, 12)
        |> Gen.choose

    let validMonthGen() =
        validMonth
        |> Gen.map ValidMonth
        |> Arb.fromGen

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

    let char100() =
        Gen.arrayOfLength 100 Arb.generate<char>
        |> Gen.map Char100
        |> Arb.fromGen

    let age() =
        gen {
            let! years = Gen.choose (0, 199)
            let! months = Gen.choose (0, 11)
            let! days = Gen.choose (0, 27) |> Gen.map float

            return Age(Years = years, Months = months, Days = days)
        } |> Arb.fromGen

    module Pin =
        let valid12Digit = chooseFromArray SwedishPersonalIdentityNumberTestData.raw12DigitStrings
        let valid12DigitGen() = valid12Digit |> Gen.map Pin.Valid12Digit |> Arb.fromGen
        let validDay year month =
            let daysInMonth = DateTime.DaysInMonth(year, month)
            Gen.choose (1, daysInMonth)

        let validValues = valid12Digit |> Gen.map (stringToValues >> Pin.ValidValues)
        let validValuesGen() = validValues |> Arb.fromGen

        let withInvalidDay =
            gen {
                let! (Pin.ValidValues (year, month, day, birthNumber, checksum)) = validValues
                let daysInMonth = DateTime.DaysInMonth(year, month)
                let! invalidDay = outsideRange 1 daysInMonth
                return (year, month, invalidDay, birthNumber, checksum) |> Pin.WithInvalidDay
            }

        let withInvalidDayGen() = withInvalidDay |> Arb.fromGen

        let withValidDay =
            gen {
                let! (Pin.ValidValues (year, month, day, birthNumber, checksum)) = validValues
                let! validDay = validDay year month
                return (year, month, validDay, birthNumber, checksum) |> Pin.WithValidDay
            }

        let withValidDayGen() = withValidDay |> Arb.fromGen

        let twoEqualPinsGen() =
            gen {
                let! (Pin.ValidValues values) = validValues
                let pin1 =
                    values
                    |> SwedishPersonalIdentityNumber.create

                let pin2 =
                    values
                    |> SwedishPersonalIdentityNumber.create

                return (pin1, pin2) |> Pin.TwoEqualPins
            }
            |> Arb.fromGen


        let twoPinsGen() =
            gen {
                let pin1 = SwedishPersonalIdentityNumberTestData.getRandom()
                let pin2 = SwedishPersonalIdentityNumberTestData.getRandom()
                return (pin1, pin2) |> Pin.TwoPins
            }
            |> Arb.fromGen


        let validPinGen() =
            gen { return SwedishPersonalIdentityNumberTestData.getRandom() |> Pin.ValidPin }
            |> Arb.fromGen

        let invalidPinStringGen() =
            gen {
                let! valid = valid12Digit
                let withInvalidYear =
                    gen {
                        return "0000" + valid.[4..]
                    }
                let withInvalidMonth =
                    gen {
                        let! month = Gen.choose (13, 99) |> Gen.map string
                        return valid.[0..3] + month + valid.[6..]
                    }
                let withInvalidDay =
                    gen {
                        let year = valid.[0..3] |> int
                        let month = valid.[4..5] |> int
                        let daysInMonth = DateTime.DaysInMonth(year, month)
                        let! day = Gen.choose (daysInMonth + 1, 99) |> Gen.map string
                        return valid.[0..5] + day + valid.[8..]
                    }
                let withInvalidBirthNumber =
                    gen {
                        return valid.[0..7] + "000" + valid.[11..]
                    }
                let withInvalidChecksum =
                    let checksum = valid.[11..]
                    let invalid = checksum |> int |> fun i -> (i + 1) % 10 |> string
                    gen { return valid.[0..10] + invalid }
                return! Gen.oneof [ withInvalidYear; withInvalidMonth; withInvalidDay; withInvalidBirthNumber; withInvalidChecksum ]
            } |> Gen.map Pin.InvalidPinString |> Arb.fromGen

        let leapDayPins =
            let isLeapDay (pin: SwedishPersonalIdentityNumber) =
                pin.Month = 2 && pin.Day = 29

            SwedishPersonalIdentityNumberTestData.allPinsShuffled()
            |> Seq.filter isLeapDay
            |> Seq.toArray

        let leapDayPinGen() =
            leapDayPins
            |> chooseFromArray
            |> Gen.map Pin.LeapDayPin
            |> Arb.fromGen

    module CoordNum =

        let validCoordNumGen() =
            gen { return SwedishCoordinationNumberTestData.getRandom() |> CoordNum.ValidNum }
            |> Arb.fromGen

        let valid12Digit = chooseFromArray SwedishCoordinationNumberTestData.raw12DigitStrings
        let valid12DigitGen() = valid12Digit |> Gen.map CoordNum.Valid12Digit |> Arb.fromGen

        let validValues = valid12Digit |> Gen.map (stringToValues >> CoordNum.ValidValues)
        let validValuesGen() = validValues |> Arb.fromGen

        let invalidCoordinationDay daysInMonth =
            gen {
                let tooLow = Gen.choose(0,60)
                let tooHigh = Gen.choose(daysInMonth + 61, 99)
                return! Gen.oneof [ tooLow; tooHigh ]
            }

        let invalidNumStringGen() =
            gen {
                let! valid12Digit = valid12Digit

                let withInvalidYear =
                    gen {
                        return "0000" + valid12Digit.[4..]
                    }
                let withInvalidMonth =
                    gen {
                        let! month = Gen.choose (13, 99) |> Gen.map string
                        return valid12Digit.[0..3] + month + valid12Digit.[6..]
                    }
                let withInvalidDay =
                    gen {
                        let year = valid12Digit.[0..3] |> int
                        let month = valid12Digit.[4..5] |> int
                        let daysInMonth = DateTime.DaysInMonth(year, month)
                        let day = invalidCoordinationDay daysInMonth
                        let! dayStr = day |> Gen.map (fun num -> num.ToString("00"))
                        return valid12Digit.[0..5] + dayStr + valid12Digit.[8..]
                    }
                let withInvalidBirthNumber =
                    gen {
                        return valid12Digit.[0..7] + "000" + valid12Digit.[11..]
                    }
                let withInvalidChecksum =
                    let checksum = valid12Digit.[11..]
                    let invalid = checksum |> int |> fun i -> (i + 1) % 10 |> string
                    gen { return valid12Digit.[0..10] + invalid }
                return! Gen.oneof [ withInvalidYear; withInvalidMonth; withInvalidDay; withInvalidBirthNumber; withInvalidChecksum ]
            } |> Gen.map CoordNum.InvalidNumString |> Arb.fromGen

        let twoEqualCoordNumsGen() =
            gen {
                let! (CoordNum.ValidValues values) = validValues
                let num1 =
                    values
                    |> SwedishCoordinationNumber.create

                let num2 =
                    values
                    |> SwedishCoordinationNumber.create

                return (num1, num2) |> CoordNum.TwoEqualCoordNums
            }
            |> Arb.fromGen

        let twoCoordNumsGen() =
            gen {
                let coordNum1 = SwedishCoordinationNumberTestData.getRandom()
                let coordNum2 = SwedishCoordinationNumberTestData.getRandom()
                return (coordNum1, coordNum2) |> CoordNum.TwoCoordNums
            }
            |> Arb.fromGen


        let withInvalidDay =
            gen {
                let! (CoordNum.ValidValues (year, month, day, birthNumber, checksum)) = validValues
                let daysInMonth = DateTime.DaysInMonth(year, month)
                let! invalidDay = invalidCoordinationDay daysInMonth
                return (year, month, invalidDay, birthNumber, checksum) |> CoordNum.WithInvalidDay
            }

        let withInvalidDayGen() = withInvalidDay |> Arb.fromGen

        let validCoordinationDay year month =
            Pin.validDay year month |> Gen.map (fun d -> d + 60)

        let withValidCoordinationDay =
            gen {
                let! (CoordNum.ValidValues (year, month, day, birthNumber, checksum)) = validValues
                let! validDay = validCoordinationDay year month
                return (year, month, validDay, birthNumber, checksum) |> CoordNum.WithValidDay
            }

        let withValidDayGen() = withValidCoordinationDay |> Arb.fromGen

        let leapDayCoordNums =
            let isLeapDay (num: SwedishCoordinationNumber) =
                num.Month = 2 && num.RealDay = 29

            SwedishCoordinationNumberTestData.allCoordNumsShuffled()
            |> Seq.filter isLeapDay
            |> Seq.toArray

        let leapDayCoordNumGen() =
            if leapDayCoordNums.Length < 1 then failwith "The test data does not contain any coordination numbers with leap days"
            leapDayCoordNums
            |> chooseFromArray
            |> Gen.map CoordNum.LeapDayCoordNum
            |> Arb.fromGen

open Generators

type ValueGenerators() =
    static member EmptyString() = emptyStringGen()
    static member Digits() = digitsGen()
    static member Max200() = max200Gen()
    static member Valid12DigitPin() = Pin.valid12DigitGen()
    static member ValidPinValues() = Pin.validValuesGen()
    static member InvalidYear() = invalidYearGen()
    static member InvalidMonth() = invalidMonthGen()
    static member ValidYear() = validYearGen()
    static member InvalidPinString() = Pin.invalidPinStringGen()
    static member ValidPin() = Pin.validPinGen()
    static member ValidMonth() = validMonthGen()
    static member PinWithInvalidDay() = Pin.withInvalidDayGen()
    static member PinWithValidDay() = Pin.withValidDayGen()
    static member PinInvalidBirthNumber() = invalidBirthNumberGen()
    static member ValidBirthNumber() = validBirthNumberGen()
    static member TwoEqualPins() = Pin.twoEqualPinsGen()
    static member TwoPins() = Pin.twoPinsGen()
    static member Char100() = char100()
    static member Age() = age()
    static member LeapDayPin() = Pin.leapDayPinGen()
    static member ValidCoordNum() = CoordNum.validCoordNumGen()
    static member InvalidCoordNumString() = CoordNum.invalidNumStringGen()
    static member TwoEqualCoordNums() = CoordNum.twoEqualCoordNumsGen()
    static member TwoCoordNums() = CoordNum.twoCoordNumsGen()
    static member Valid12DigitCoordNum() = CoordNum.valid12DigitGen()
    static member ValidCoordNumValues() = CoordNum.validValuesGen()
    static member CoordNumWithInvalidDay() = CoordNum.withInvalidDayGen()
    static member CoordNumWithValidDay() = CoordNum.withValidDayGen()
    static member LeapDayCoordNum() = CoordNum.leapDayCoordNumGen()
