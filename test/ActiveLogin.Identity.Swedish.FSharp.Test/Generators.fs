module ActiveLogin.Identity.Swedish.FSharp.Test.Gen

open FsCheck
open ActiveLogin.Identity.Swedish
open ActiveLogin.Identity.Swedish.TestData
open System

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
    type PinWithInvalidYear = PinWithInvalidYear of string
    type PinWithInvalidMonth = PinWithInvalidMonth of string
    type PinWithInvalidDay = PinWithInvalidDay of string
    type PinWithInvalidIndividualNumber = PinWithInvalidIndividualNumber of string
    type PinWithInvalidChecksum = PinWithInvalidChecksum of string
    type ValidPin = ValidPin of PersonalIdentityNumber
    type WithInvalidDay = WithInvalidDay of IdentityNumberValues
    type WithValidDay = WithValidDay of IdentityNumberValues
    type TwoEqualPins = TwoEqualPins of PersonalIdentityNumber * PersonalIdentityNumber
    type TwoPins = TwoPins of PersonalIdentityNumber * PersonalIdentityNumber
    type LeapDayPin = LeapDayPin of PersonalIdentityNumber

module CoordNum =
    let [<Literal>] DayOffset = 60
    type ValidNum = ValidNum of CoordinationNumber
    type Valid12Digit = Valid12Digit of string
    type NumWithInvalidYear = NumWithInvalidYear of string
    type NumWithInvalidMonth = NumWithInvalidMonth of string
    type NumWithInvalidDay = NumWithInvalidDay of string
    type NumWithInvalidIndividualNumber = NumWithInvalidIndividualNumber of string
    type NumWithInvalidChecksum = NumWithInvalidChecksum of string
    type TwoEqualCoordNums = TwoEqualCoordNums of CoordinationNumber * CoordinationNumber
    type ValidValues = ValidValues of IdentityNumberValues
    type TwoCoordNums = TwoCoordNums of CoordinationNumber * CoordinationNumber
    type WithInvalidCoordinationMonth = WithInvalidMonth of IdentityNumberValues
    type WithInvalidDay = WithInvalidDay of IdentityNumberValues
    type WithValidDay = WithValidDay of IdentityNumberValues
//    type LeapDayCoordNum = LeapDayCoordNum of CoordinationNumber // There aren't any leap day coordination numbers in the test data from Skatteverket, so we cannot generate this


module Gen =
    let chooseFromArray xs =
        gen { let! index = Gen.choose (0, (Array.length xs) - 1)
              return xs.[index] }

let stringToValues (pin: string) =
    ( pin.[0..3] |> int,
      pin.[4..5] |> int,
      pin.[6..7] |> int,
      pin.[8..10] |> int,
      pin.[11..11] |> int )

[<AutoOpen>]
module private Internal =

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

    let numStringWithInvalidYear (valid12Digit: Gen<string>) =
        gen {
            let! valid12Digit = valid12Digit
            return "0000" + valid12Digit.[4..]
        }

    let numStringWithInvalidIndividualNumber (valid12Digit: Gen<string>) =
        gen {
            let! valid12Digit = valid12Digit
            return valid12Digit.[0..7] + "000" + valid12Digit.[11..]
        }

    let numStringWithInvalidChecksum (valid12Digit: Gen<string>) =
        gen {
            let! valid12Digit = valid12Digit
            let checksum = valid12Digit.[11..]
            let invalid = checksum |> int |> fun i -> (i + 1) % 10 |> string
            return valid12Digit.[0..10] + invalid
        }

    module Pin =
        let valid12Digit = Gen.chooseFromArray PersonalIdentityNumberTestData.Raw12DigitStrings
        let valid12DigitGen() = valid12Digit |> Gen.map Pin.Valid12Digit |> Arb.fromGen
        let validDay year month =
            let daysInMonth = DateTime.DaysInMonth(year, month)
            Gen.choose (1, daysInMonth)

        let validValues = valid12Digit |> Gen.map (stringToValues >> Pin.ValidValues)
        let validValuesGen() = validValues |> Arb.fromGen

        let withInvalidDay =
            gen {
                let! (Pin.ValidValues (year, month, _, birthNumber, checksum)) = validValues
                let daysInMonth = DateTime.DaysInMonth(year, month)
                let! invalidDay = outsideRange 1 daysInMonth
                return (year, month, invalidDay, birthNumber, checksum) |> Pin.WithInvalidDay
            }

        let withInvalidDayGen() = withInvalidDay |> Arb.fromGen

        let withValidDay =
            gen {
                let! (Pin.ValidValues (year, month, _, birthNumber, checksum)) = validValues
                let! validDay = validDay year month
                return (year, month, validDay, birthNumber, checksum) |> Pin.WithValidDay
            }

        let withValidDayGen() = withValidDay |> Arb.fromGen

        let twoEqualPinsGen() =
            gen {
                let! (Pin.ValidValues values) = validValues
                let pin1 =
                    values
                    |> PersonalIdentityNumber

                let pin2 =
                    values
                    |> PersonalIdentityNumber

                return (pin1, pin2) |> Pin.TwoEqualPins
            }
            |> Arb.fromGen


        let twoPinsGen() =
            gen {
                let pin1 = PersonalIdentityNumberTestData.GetRandom()
                let pin2 = PersonalIdentityNumberTestData.GetRandom()
                return (pin1, pin2) |> Pin.TwoPins
            }
            |> Arb.fromGen


        let validPinGen() =
            gen { return PersonalIdentityNumberTestData.GetRandom() |> Pin.ValidPin }
            |> Arb.fromGen

        let pinStringWithInvalidYearGen() =
            numStringWithInvalidYear valid12Digit
            |> Gen.map Pin.PinWithInvalidYear
            |> Arb.fromGen

        let pinStringWithInvalidMonthGen() =
            gen {
                let! valid12Digit = valid12Digit
                let! month = Gen.choose (13, 99) |> Gen.map string
                return valid12Digit.[0..3] + month + valid12Digit.[6..]
            } |> Gen.map Pin.PinWithInvalidMonth |> Arb.fromGen

        let pinStringWithInvalidDayGen() =
            gen {
                let! valid12Digit = valid12Digit
                let year = valid12Digit.[0..3] |> int
                let month = valid12Digit.[4..5] |> int
                let daysInMonth = DateTime.DaysInMonth(year, month)
                let invalidDays = Array.append [| 0 |] [| daysInMonth + 1 .. 99 |]
                let! invalidDay =
                    Gen.chooseFromArray invalidDays
                    |> Gen.map (fun num -> num.ToString("00"))
                return valid12Digit.[0..5] + invalidDay + valid12Digit.[8..]
            } |> Gen.map Pin.PinWithInvalidDay |> Arb.fromGen

        let pinStringWithInvalidIndividualNumberGen() =
            numStringWithInvalidIndividualNumber valid12Digit
            |> Gen.map Pin.PinWithInvalidIndividualNumber
            |> Arb.fromGen

        let pinStringWithInvalidChecksumGen() =
            numStringWithInvalidChecksum valid12Digit
            |> Gen.map Pin.PinWithInvalidChecksum
            |> Arb.fromGen

        let leapDayPins =
            let isLeapDay (pin: PersonalIdentityNumber) =
                pin.Month = 2 && pin.Day = 29

            PersonalIdentityNumberTestData.AllPinsShuffled()
            |> Seq.filter isLeapDay
            |> Seq.toArray

        let leapDayPinGen() =
            leapDayPins
            |> Gen.chooseFromArray
            |> Gen.map Pin.LeapDayPin
            |> Arb.fromGen

    module CoordNum =

        let validCoordNumGen() =
            gen { return CoordinationNumberTestData.GetRandom() |> CoordNum.ValidNum }
            |> Arb.fromGen

        let valid12Digit = Gen.chooseFromArray CoordinationNumberTestData.Raw12DigitStrings
        let valid12DigitGen() = valid12Digit |> Gen.map CoordNum.Valid12Digit |> Arb.fromGen

        let validValues = valid12Digit |> Gen.map (stringToValues >> CoordNum.ValidValues)
        let validValuesGen() = validValues |> Arb.fromGen

        let numStringWithInvalidYearGen() =
            numStringWithInvalidYear valid12Digit
            |> Gen.map CoordNum.NumWithInvalidYear
            |> Arb.fromGen

        let numStringWithInvalidMonthGen() =
            gen {
                let! valid12Digit = valid12Digit
                let! month = Gen.choose (13, 99) |> Gen.map string
                return valid12Digit.[0..3] + month + valid12Digit.[6..]
            } |> Gen.map CoordNum.NumWithInvalidMonth |> Arb.fromGen

        let numStringWithInvalidDayGen() =
            gen {
                let! valid12Digit = valid12Digit
                // pseudo-code: if month = 0 then [60,91] is valid else [61-(daysInMonth+60)]
                let year = int valid12Digit.[0..3]
                let month = int valid12Digit.[4..5]
                let daysInMonth =
                    if month = 0 then 31 else DateTime.DaysInMonth(year, month)
                let tooLowDays = [| 0..(CoordNum.DayOffset - 1) |]
                let tooHighDays = [| daysInMonth + (CoordNum.DayOffset + 1) .. 99 |]
                let invalidDays = Array.append tooLowDays tooHighDays
                let! invalidDay =
                    Gen.chooseFromArray invalidDays
                    |> Gen.map (fun num -> num.ToString("00"))
                return valid12Digit.[0..5] + invalidDay + valid12Digit.[8..]
            } |> Gen.map CoordNum.NumWithInvalidDay |> Arb.fromGen

        let numStringWithInvalidIndividualNumberGen() =
            numStringWithInvalidIndividualNumber valid12Digit
            |> Gen.map CoordNum.NumWithInvalidIndividualNumber
            |> Arb.fromGen

        let numStringWithInvalidChecksumGen() =
            numStringWithInvalidChecksum valid12Digit
            |> Gen.map CoordNum.NumWithInvalidChecksum
            |> Arb.fromGen

        let twoEqualCoordNumsGen() =
            gen {
                let! (CoordNum.ValidValues values) = validValues
                let num1 =
                    values
                    |> CoordinationNumber

                let num2 =
                    values
                    |> CoordinationNumber

                return (num1, num2) |> CoordNum.TwoEqualCoordNums
            }
            |> Arb.fromGen

        let twoCoordNumsGen() =
            gen {
                let coordNum1 = CoordinationNumberTestData.GetRandom()
                let coordNum2 = CoordinationNumberTestData.GetRandom()
                return (coordNum1, coordNum2) |> CoordNum.TwoCoordNums
            }
            |> Arb.fromGen

        let maxDaysOfAnyMonth = 31
        let withInvalidDay =
            gen {
                let! (CoordNum.ValidValues (year, month, _, individualNumber, checksum)) = validValues
                let! invalidDay = outsideRange (CoordNum.DayOffset + 0) (CoordNum.DayOffset + maxDaysOfAnyMonth)
                return (year, month, invalidDay, individualNumber, checksum) |> CoordNum.WithInvalidDay
            }

        let withInvalidDayGen() = withInvalidDay |> Arb.fromGen

        let withValidCoordinationDay =
            gen {
                let! (CoordNum.ValidValues (year, month, _, individualNumber, checksum)) = validValues
                let daysInMonth = if month = 0 then maxDaysOfAnyMonth else DateTime.DaysInMonth(year, month)
                let! validDay = Gen.choose(CoordNum.DayOffset, (CoordNum.DayOffset + daysInMonth))
                return (year, month, validDay, individualNumber, checksum) |> CoordNum.WithValidDay
            }

        let withValidDayGen() = withValidCoordinationDay |> Arb.fromGen

        let withInvalidCoordinationMonth =
            gen {
                let! (CoordNum.ValidValues (year, _, day, individualNumber, checksum)) = validValues
                let! invalidMonth = outsideRange 0 12
                return (year, invalidMonth, day, individualNumber, checksum) |> CoordNum.WithInvalidMonth
            }

        let withInvalidCoordinationMonthGen() = withInvalidCoordinationMonth |> Arb.fromGen


type ValueGenerators() =
    static member EmptyString() = emptyStringGen()
    static member Digits() = digitsGen()
    static member Max200() = max200Gen()
    static member Valid12DigitPin() = Pin.valid12DigitGen()
    static member ValidPinValues() = Pin.validValuesGen()
    static member InvalidYear() = invalidYearGen()
    static member InvalidMonth() = invalidMonthGen()
    static member ValidYear() = validYearGen()
    static member PinStringWithInvalidYear() = Pin.pinStringWithInvalidYearGen()
    static member PinStringWithInvalidMonth() = Pin.pinStringWithInvalidMonthGen()
    static member PinStringWithInvalidDay() = Pin.pinStringWithInvalidDayGen()
    static member PinStringWithInvalidIndividualNumber() = Pin.pinStringWithInvalidIndividualNumberGen()
    static member PinStringWithInvalidChecksum() = Pin.pinStringWithInvalidChecksumGen()
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
    static member CoordNumStringWithInvalidYear() = CoordNum.numStringWithInvalidYearGen()
    static member CoordNumStringWithInvalidMonth() = CoordNum.numStringWithInvalidMonthGen()
    static member CoordNumStringWithInvalidDay() = CoordNum.numStringWithInvalidDayGen()
    static member CoordNumStringWithInvalidIndividualNumber() = CoordNum.numStringWithInvalidIndividualNumberGen()
    static member CoordNumStringWithInvalidChecksum() = CoordNum.numStringWithInvalidChecksumGen()
    static member TwoEqualCoordNums() = CoordNum.twoEqualCoordNumsGen()
    static member TwoCoordNums() = CoordNum.twoCoordNumsGen()
    static member Valid12DigitCoordNum() = CoordNum.valid12DigitGen()
    static member ValidCoordNumValues() = CoordNum.validValuesGen()
    static member CoordNumWithInvalidDay() = CoordNum.withInvalidDayGen()
    static member CoordNumWithValidDay() = CoordNum.withValidDayGen()
    static member CoordNumWithInvalidMonth() = CoordNum.withInvalidCoordinationMonthGen()
