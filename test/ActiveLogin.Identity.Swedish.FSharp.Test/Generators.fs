module ActiveLogin.Identity.Swedish.FSharp.Test.Gen

open FsCheck
open ActiveLogin.Identity.Swedish.FSharp
open ActiveLogin.Identity.Swedish.FSharp.TestData
open System
open PinTestHelpers

let chooseFromArray xs =
    gen { let! index = Gen.choose (0, (Array.length xs) - 1)
          return xs.[index] }

type Whitespace = Whitespace of string
type WhitespaceGen() =
    static member Gen() : Arbitrary<Whitespace> =
        Gen.sized(fun s -> Gen.choose(0,s) |> Gen.map (fun length -> String.replicate length " "))
        |> Gen.map Whitespace |> Arb.fromGen

type Max200 = Max200 of int
type Max200Gen() =
    static member Gen() : Arbitrary<Max200> =
        Gen.choose(-100, 200) |> Gen.map Max200 |> Arb.fromGen

let digits =
    let digit = Gen.choose(0,9) |> Gen.map (sprintf "%i")
    Gen.listOf digit |> Gen.map (String.concat "")
type Digits = Digits of string
type DigitsGen() =
    static member Gen() : Arbitrary<Digits> =
        digits |> Gen.map Digits |> Arb.fromGen

let stringToValues (pin : string) =
    { Year = pin.[0..3] |> int
      Month = pin.[4..5] |> int
      Day = pin.[6..7] |> int
      BirthNumber = pin.[8..10] |> int
      Checksum = pin.[11..11] |> int }

let valid12Digit = chooseFromArray SwedishPersonalIdentityNumberTestData.raw12DigitStrings
type Valid12Digit = Valid12Digit of string
type Valid12DigitGen() =
    static member Gen() : Arbitrary<Valid12Digit> =
        valid12Digit |> Gen.map Valid12Digit |> Arb.fromGen

type ValidValues = ValidValues of SwedishPersonalIdentityNumberValues

let validValues =
    valid12Digit |> Gen.map (stringToValues >> ValidValues)

type ValidValuesGen() =
    static member Gen() : Arbitrary<ValidValues> = validValues |> Arb.fromGen

let outsideRange min max =
    let low = Gen.choose (Int32.MinValue, min - 1)
    let high = Gen.choose (max + 1, Int32.MaxValue)
    Gen.oneof [ low; high ]

type InvalidYear = InvalidYear of int

type InvalidYearGen() =
    static member Gen() : Arbitrary<InvalidYear> =
        (1, 9999)
        ||> outsideRange
        |> Gen.map InvalidYear
        |> Arb.fromGen

let validYear = Gen.choose (1, 9999)
type ValidYear = ValidYear of int

type ValidYearGen() =
    static member Gen() : Arbitrary<ValidYear> =
        validYear
        |> Gen.map ValidYear
        |> Arb.fromGen

type InvalidMonth = InvalidMonth of int

type InvalidMonthGen() =
    static member Gen() : Arbitrary<InvalidMonth> =
        (1, 12)
        ||> outsideRange
        |> Gen.map InvalidMonth
        |> Arb.fromGen

type ValidMonth = ValidMonth of int

type ValidMonthGen() =
    static member Gen() : Arbitrary<ValidMonth> =
        (1, 12)
        |> Gen.choose
        |> Gen.map ValidMonth
        |> Arb.fromGen

type WithInvalidDay = WithInvalidDay of SwedishPersonalIdentityNumberValues

let withInvalidDay =
    gen {
        let! (ValidValues validValues) = validValues
        let daysInMonth = DateTime.DaysInMonth(validValues.Year, validValues.Month)
        let! invalidDay = outsideRange 1 daysInMonth
        return { validValues with Day = invalidDay } |> WithInvalidDay
    }

type WithInvalidDayGen() =
    static member Gen() : Arbitrary<WithInvalidDay> = withInvalidDay |> Arb.fromGen

type WithValidDay = WithValidDay of SwedishPersonalIdentityNumberValues

let withValidDay =
    gen {
        let! (ValidValues validValues) = validValues
        let daysInMonth = DateTime.DaysInMonth(validValues.Year, validValues.Month)
        let! validDay = Gen.choose (1, daysInMonth)
        return { validValues with Day = validDay } |> WithValidDay
    }

type WithValidDayGen() =
    static member Gen() : Arbitrary<WithValidDay> = withValidDay |> Arb.fromGen

type InvalidBirthNumber = InvalidBirthNumber of int

type InvalidBirthNumberGen() =
    static member Gen() : Arbitrary<InvalidBirthNumber> =
        (1, 999)
        ||> outsideRange
        |> Gen.map InvalidBirthNumber
        |> Arb.fromGen

type ValidBirthNumber = ValidBirthNumber of int

type ValidBirthNumberGen() =
    static member Gen() : Arbitrary<ValidBirthNumber> =
        (1, 999)
        |> Gen.choose
        |> Gen.map ValidBirthNumber
        |> Arb.fromGen

type TwoEqualPins = TwoEqualPins of SwedishPersonalIdentityNumber * SwedishPersonalIdentityNumber

type TwoEqualPinsGen() =
    static member Gen() : Arbitrary<TwoEqualPins> =
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

type TwoPins = TwoPins of SwedishPersonalIdentityNumber * SwedishPersonalIdentityNumber

type TwoPinsGen() =
    static member Gen() : Arbitrary<TwoPins> =
        gen {
            let pin1 = SwedishPersonalIdentityNumberTestData.getRandom()
            let pin2 = SwedishPersonalIdentityNumberTestData.getRandom()
            return (pin1, pin2) |> TwoPins
        }
        |> Arb.fromGen

type ValidPin = ValidPin of SwedishPersonalIdentityNumber

type ValidPinGen() =
    static member Gen() : Arbitrary<ValidPin> =
        validValues
        |> Gen.map (fun (ValidValues values) ->
               values
               |> SwedishPersonalIdentityNumber.createOrFail
               |> ValidPin)
        |> Arb.fromGen

type InvalidPinString = InvalidPinString of string

type InvalidPinStringGen() =
    static member Gen() : Arbitrary<InvalidPinString> =
            gen {
                let! valid = valid12Digit
                let withInvalidMonth =
                    gen {
                        let! month = Gen.choose(13,99) |> Gen.map (sprintf "%i")
                        return valid.[ 0..3 ] + month + valid.[ 6.. ]
                    }

                let withInvalidDay =
                    gen {
                        let! day = Gen.choose(32, 99) |> Gen.map (sprintf "%i")
                        return valid.[ 0..5 ] + day + valid.[ 8.. ]
                    }
                let withInvalidBirthNumber =
                    gen {
                        return valid.[ 0..7 ] + "000" + valid.[ 11.. ]
                    }
                let withInvalidChecksum =
                    let checksum = valid.[ 11.. ]
                    let invalid =
                        match checksum with
                        | "9" -> "0"
                        | x -> x |> int |> (+) 1 |> sprintf "%i"
                    gen { return valid.[ 0..10 ] + invalid }
                return! Gen.oneof [ withInvalidMonth; withInvalidDay; withInvalidBirthNumber; withInvalidChecksum ]
            } |> Gen.map InvalidPinString |> Arb.fromGen
