module ActiveLogin.Identity.Swedish.FSharp.Test.Arbitraries
open Expecto
open Generators
open FsCheck
open ActiveLogin.Identity.Swedish.FSharp
open ActiveLogin.Identity.Swedish.FSharp.TestData
open System

let addToConfig config arbTypes = 
    { config with arbitrary = arbTypes @ config.arbitrary }

type InvalidYearGen() =
    static member Gen() : Arbitrary<SwedishPersonalIdentityNumberValues> = Arb.fromGen invalidYear

type InvalidMonthGen() = 
    static member Gen() : Arbitrary<SwedishPersonalIdentityNumberValues> = Arb.fromGen invalidMonth

type InvalidDayGen() =
    static member Gen() : Arbitrary<SwedishPersonalIdentityNumberValues> = Arb.fromGen invalidDay

type InvalidBirthNumberGen() =
    static member Gen() : Arbitrary<SwedishPersonalIdentityNumberValues> = Arb.fromGen invalidBirthNumber

type ValidValuesGen() =
    static member Gen() : Arbitrary<SwedishPersonalIdentityNumberValues> = 
        gen { return! validValues } 
        |> Arb.fromGen 

type TwoPins() =
    static member Gen() : Arbitrary<SwedishPersonalIdentityNumber * SwedishPersonalIdentityNumber> =
        gen {
            let pin1 = SwedishPersonalIdentityNumberTestData.getRandom()
            let pin2 = SwedishPersonalIdentityNumberTestData.getRandom()
            return (pin1, pin2)
        } |> Arb.fromGen

type Valid12Digit() =
    static member Gen() : Arbitrary<string * SwedishPersonalIdentityNumberValues> =
        random12Digit |> Arb.fromGen

type ValidPin() =
    static member Gen() : Arbitrary<SwedishPersonalIdentityNumber> =
        validPin |> Arb.fromGen

type TwoEqualPins() =
    static member Gen() : Arbitrary<SwedishPersonalIdentityNumber * SwedishPersonalIdentityNumber> =
        gen {
            let! pin = validPin
            return (pin, pin)
        } |> Arb.fromGen

type Valid10Digit() =
    static member Gen() : Arbitrary<string * SwedishPersonalIdentityNumberValues> =
        random10Digit |> Arb.fromGen

type Valid10DigitWithPlusDelimiter() =
    static member Gen() : Arbitrary<string * SwedishPersonalIdentityNumberValues> =
        gen {
            return random10DigitWithPlusDelimiter |> Seq.head
        } |> Arb.fromGen

type Valid10DigitWithHyphenDelimiter() =
    static member Gen() : Arbitrary<string * SwedishPersonalIdentityNumberValues> =
        gen {
            return random10DigitWithHyphenDelimiter |> Seq.head
        } |> Arb.fromGen

type Valid12DigitWithLeadingAndTrailingCharacters() =
    static member Gen() : Arbitrary<string * SwedishPersonalIdentityNumberValues> =
        gen {
            let! leading = noiseStringWeighted
            let! trailing = noiseStringWeighted
            let! (str, values) = random12Digit
            return (sprintf "%s%s%s" leading str trailing, values)
        } |> Arb.fromGen

type Valid10DigitStringWithAnyDelimiterExcludingPlus() =
    static member Gen() : Arbitrary<string * SwedishPersonalIdentityNumberValues> =
        gen {
            let (str, expected) = random10DigitWithHyphenDelimiter |> Seq.head
            let! delimiter = noiseDelimiterWeightedExcludingPlus
            let result = 
                match delimiter with
                | "+" -> str.[ 0..5 ] + str.[ 7..10 ] 
                | _ -> str.[ 0..5 ] + delimiter + str.[ 7..10 ] 
            return (result, expected)
        } |> Arb.fromGen

type Valid12DigitStringWithAnyDelimiter() =
    static member Gen() : Arbitrary<string * SwedishPersonalIdentityNumberValues> =
        gen {
            let! (str, expected) = random12Digit
            let! delimiter = noiseDelimiterWeighted
            let result = str.[ 0..7 ] + delimiter + str.[ 8..11 ]
            return (result, expected)
        } |> Arb.fromGen

// Inserts noise between all characters in pin
let mix noise pin =
    let toStringList str =
        [ for c in str -> [|c|] |> String ]
    let result = ResizeArray<string>()
    let first, rest = 
        match noise with
        | first :: rest -> first, rest
        | _ -> failwith "test setup error"
    result.Add(first) 
    pin 
    |> toStringList
    |> List.zip rest 
    |> List.iter (fun (fst,snd) -> 
        result.Add fst
        result.Add snd)

    result
    |> List.ofSeq
    |> String.concat ""

type Valid12DigitStringMixedWithCharacters() =
    static member Gen() : Arbitrary<string * SwedishPersonalIdentityNumberValues> =
        gen {
            let! (str, expected) = random12Digit
            let! randomNoise = Gen.listOfLength 13 noiseStringWeighted
            let withNoise = mix randomNoise str
            return (withNoise, expected)
        } |> Arb.fromGen

type Valid10DigitStringMixedWithCharacters() =
    static member Gen() : Arbitrary<string * SwedishPersonalIdentityNumberValues> =
        gen {
            let! (str, expected) = random10Digit
            let! randomNoise = Gen.listOfLength 12 noiseDelimiterWeightedExcludingPlus
            let withNoise = mix randomNoise str
            return (withNoise, expected)
        } |> Arb.fromGen