module ActiveLogin.Identity.Swedish.FSharp.Test.Arbitraries
open Expecto
open Generators
open FsCheck
open ActiveLogin.Identity.Swedish.FSharp
open ActiveLogin.Identity.Swedish.FSharp.TestData

let addToConfig config arbTypes = 
    { config with arbitrary = arbTypes @ config.arbitrary }

type InvalidYearGen() =
    static member Year() : Arbitrary<SwedishPersonalIdentityNumberValues> = Arb.fromGen invalidYear

type InvalidMonthGen() = 
    static member Month() : Arbitrary<SwedishPersonalIdentityNumberValues> = Arb.fromGen invalidMonth

type InvalidDayGen() =
    static member Day() : Arbitrary<SwedishPersonalIdentityNumberValues> = Arb.fromGen invalidDay

type InvalidBirthNumberGen() =
    static member BirthNumber() : Arbitrary<SwedishPersonalIdentityNumberValues> = Arb.fromGen invalidBirthNumber

type ValidValuesGen() =
    static member ValidValues() : Arbitrary<SwedishPersonalIdentityNumberValues> = 
        gen { return! validValues } 
        |> Arb.fromGen 

type TwoPins() =
    static member TwoPins() : Arbitrary<SwedishPersonalIdentityNumber * SwedishPersonalIdentityNumber> =
        gen {
            let pin1 = SwedishPersonalIdentityNumberTestData.getRandom()
            let pin2 = SwedishPersonalIdentityNumberTestData.getRandom()
            return (pin1, pin2)
        } |> Arb.fromGen

type Valid12Digit() =
    static member Valid12Digit() : Arbitrary<string * SwedishPersonalIdentityNumberValues> =
        random12Digit |> Arb.fromGen

type ValidPin() =
    static member ValidPin() : Arbitrary<SwedishPersonalIdentityNumber> =
        validPin |> Arb.fromGen

type TwoEqualPins() =
    static member TwoEqualPins() : Arbitrary<SwedishPersonalIdentityNumber * SwedishPersonalIdentityNumber> =
        gen {
            let! pin = validPin
            return (pin, pin)
        } |> Arb.fromGen

type Valid10Digit() =
    static member Valid10Digit : Arbitrary<string * SwedishPersonalIdentityNumberValues> =
        random10Digit |> Arb.fromGen

type Valid10DigitWithPlusDelimiter() =
    static member ValidWithPlusDelimiter : Arbitrary<string * SwedishPersonalIdentityNumberValues> =
        gen {
            return random10DigitWithPlusDelimiter |> Seq.head
        } |> Arb.fromGen

type Valid10DigitWithHyphenDelimiter() =
    static member ValidWithHyphenDelimiter : Arbitrary<string * SwedishPersonalIdentityNumberValues> =
        gen {
            return random10DigitWithHyphenDelimiter |> Seq.head
        } |> Arb.fromGen

type Valid12DigitWithLeadingAndTrailingCharacters() =
    static member Valid12DigitWithLeadingAndTrailingCharacters : Arbitrary<string * SwedishPersonalIdentityNumberValues> =
        gen {
            let! leading = printableAscii
            let! trailing = printableAscii
            let! (str, values) = random12Digit
            return (sprintf "%s%s%s" leading str trailing, values)
        } |> Arb.fromGen

type Valid10DigitStringWithAnyDelimiterExceptPlus() =
    static member Valid10DigitStringWithAnyDelimiterExceptPlus : Arbitrary<string * SwedishPersonalIdentityNumberValues> =
        gen {
            let (str, expected) = random10DigitWithHyphenDelimiter |> Seq.head
            let! delimiter = singlePrintableAsciiString
            let result = 
                match delimiter with
                | "+" -> str.[ 0..5 ] + str.[ 7..10 ] 
                | _ -> str.[ 0..5 ] + delimiter + str.[ 7..10 ] 
            return (result, expected)
        } |> Arb.fromGen

type Valid12DigitStringWithAnyDelimiter() =
    static member Valid12DigitStringWithAnyDelimiter() : Arbitrary<string * SwedishPersonalIdentityNumberValues> =
        gen {
            let! (str, expected) = random12Digit
            let! delimiter = singlePrintableAsciiString
            let result = str.[ 0..7 ] + delimiter + str.[ 8..11 ]
            return (result, expected)
        } |> Arb.fromGen

type Valid12DigitStringMixedWithCharacters =
    static member Valid12DigitStringMixedWithCharacters : Arbitrary<string * SwedishPersonalIdentityNumberValues> =
        gen {
            let! cs1 = printableAscii
            let! cs2 = printableAscii
            let! cs3 = printableAscii
            let! cs4 = printableAscii
            let! cs5 = printableAscii
            let! cs6 = printableAscii
            let! cs7 = printableAscii
            let! cs8 = printableAscii
            let! cs9 = printableAscii
            let! cs10 = printableAscii
            let! cs11 = printableAscii
            let! cs12 = printableAscii
            let! cs13 = printableAscii
            let! (str, expected) = random12Digit
            return (cs1 + str.[0..0] + cs2 + str.[1..1] + cs3 + str.[2..2] + cs4 + str.[3..3] + cs5 + str.[4..4] + cs6 + str.[5..5] + cs7 + str.[6..6] + cs8 + str.[7..7] + cs9 + str.[8..8] + cs10 + str.[9..9] + cs11 + str.[10..10] + cs12 + str.[11..11] + cs13, expected)
        } |> Arb.fromGen

type Valid10DigitStringMixedWithCharacters =
    static member Valid12DigitStringMixedWithCharacters : Arbitrary<string * SwedishPersonalIdentityNumberValues> =
        gen {
            let! cs1 = printableAsciiExceptPlus
            let! cs2 = printableAsciiExceptPlus
            let! cs3 = printableAsciiExceptPlus
            let! cs4 = printableAsciiExceptPlus
            let! cs5 = printableAsciiExceptPlus
            let! cs6 = printableAsciiExceptPlus
            let! cs7 = printableAsciiExceptPlus
            let! cs8 = printableAsciiExceptPlus
            let! cs9 = printableAsciiExceptPlus
            let! cs10 = printableAsciiExceptPlus
            let! cs11 = printableAsciiExceptPlus
            let! cs12 = printableAsciiExceptPlus
            let! (str, expected) = random10Digit
            return (cs1 + str.[0..0] + cs2 + str.[1..1] + cs3 + str.[2..2] + cs4 + str.[3..3] + cs5 + str.[4..4] + cs6 + str.[5..5] + cs7 + str.[6..6] + cs8 + str.[7..7] + cs9 + str.[8..8] + cs10 + str.[9..9] + cs11 + str.[10..10] + cs12, expected)
        } |> Arb.fromGen