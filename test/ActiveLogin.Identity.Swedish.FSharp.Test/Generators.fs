module ActiveLogin.Identity.Swedish.FSharp.Test.Generators

open ActiveLogin.Identity.Swedish.FSharp
open FsCheck
open System
open ActiveLogin.Identity.Swedish.FSharp.TestData
open ActiveLogin.Identity.Swedish.FSharp.Test.PinTestHelpers

let chooseFromArray xs =
    gen { let! index = Gen.choose (0, (Array.length xs) - 1)
          return xs.[index] }
let valid12Digit = chooseFromArray SwedishPersonalIdentityNumberTestData.raw12DigitStrings

let private stringToValues (pin : string) =
    { Year = pin.[0..3] |> int
      Month = pin.[4..5] |> int
      Day = pin.[6..7] |> int
      BirthNumber = pin.[8..10] |> int
      Checksum = pin.[11..11] |> int }

let validValues =
    gen { let! str = valid12Digit
          return stringToValues str }
let random12Digit =
    gen { let! str = valid12Digit
          return (str, stringToValues str) }

let random10Digit =
    gen {
        let pin = SwedishPersonalIdentityNumberTestData.getRandom()
        return (pin |> SwedishPersonalIdentityNumber.to10DigitString, pinToValues pin)
    }

let private parseYear =
    DateTime.Today.Year
    |> Year.create
    |> function
    | Ok y -> y
    | Error e -> e |> failwith "Test setup error %A"

let random10DigitWithPlusDelimiter =
    seq {
        for pin in SwedishPersonalIdentityNumberTestData.allPinsShuffled() do
            let tenDigit = pin |> SwedishPersonalIdentityNumber.to10DigitStringInSpecificYear parseYear
            if tenDigit.Contains("+") then yield (tenDigit, pin |> pinToValues)
    }

let random10DigitWithHyphenDelimiter =
    seq {
        for pin in SwedishPersonalIdentityNumberTestData.allPinsShuffled() do
            let tenDigit = pin |> SwedishPersonalIdentityNumber.to10DigitStringInSpecificYear parseYear
            if tenDigit.Contains("+") |> not then yield (tenDigit, pin |> pinToValues)
    }

let invalidYear =
    gen {
        let! year = Gen.oneof [ (Gen.choose (Int32.MinValue, DateTime.MinValue.Year - 1))
                                (Gen.choose (DateTime.MaxValue.Year + 1, Int32.MaxValue)) ]
        let! values = validValues
        return { values with Year = year }
    }

let invalidMonth =
    gen {
        let! month = Gen.oneof [ (Gen.choose (Int32.MinValue, 0))
                                 (Gen.choose (13, Int32.MaxValue)) ]
        let! values = validValues
        return { values with Month = month }
    }

let invalidDay =
    gen {
        let! values = validValues
        let daysInMonth = DateTime.DaysInMonth(values.Year, values.Month)
        let! day = Gen.oneof [ Gen.choose (Int32.MinValue, 0)
                               Gen.choose (daysInMonth + 1, Int32.MaxValue) ]
        return { values with Day = day }
    }

let invalidBirthNumber =
    gen {
        let! birthNumber = Gen.oneof [ Gen.choose (Int32.MinValue, 0)
                                       Gen.choose (1000, Int32.MaxValue) ]
        let! values = validValues
        return { values with BirthNumber = birthNumber }
    }

let validPin = gen { return SwedishPersonalIdentityNumberTestData.getRandom() }

let private printableAsciiCharsExcludingPlus = [ 32..42 ] @ [ 44..47 ] @ [ 58..126 ] |> List.map char |> Array.ofList |> chooseFromArray

// There are some "noise" characters that I want to weigh heavier when generating test values
let private weightedIncludingPlus = [| ' '; '-'; '+'; ';'; '.'; '_'; |] |> chooseFromArray
let private weightedExcludingPlus = [| ' '; '-'; ';'; '.'; '_'; |] |> chooseFromArray

let private noiseCharWeighted =
    Gen.frequency [ (2, weightedIncludingPlus); (1, printableAsciiCharsExcludingPlus) ]

let noiseStringWeighted =
    gen {
        let! noiseChars = Gen.listOf noiseCharWeighted
        return noiseChars
               |> Array.ofList
               |> String
    }

let noiseCharWeightedExcludingPlus =
    Gen.frequency [ (2, weightedExcludingPlus); (1 ,printableAsciiCharsExcludingPlus) ]

let noiseDelimiterWeighted =
    gen {
        let! char = noiseCharWeighted
        return char
               |> Array.singleton
               |> String
    }

let noiseDelimiterWeightedExcludingPlus =
    gen {
        let! charsGen = Gen.listOf noiseCharWeightedExcludingPlus
        return charsGen
               |> Array.ofList
               |> String
    }
