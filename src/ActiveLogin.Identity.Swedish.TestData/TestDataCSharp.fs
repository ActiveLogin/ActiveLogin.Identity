namespace ActiveLogin.Identity.Swedish.TestData
open ActiveLogin.Identity.Swedish.FSharp
open ActiveLogin.Identity.Swedish.FSharp.TestData.SwedishPersonalIdentityNumberTestData
open ActiveLogin.Identity.Swedish

/// A class that provides easy access to the official test numbers for Swedish Personal Identity Number (Personnummer) 
/// from Skatteverket
[<CompiledName("SwedishPersonalIdentityNumberTestData")>]
type SwedishPersonalIdentityNumberTestDataCSharp() =
    static let toCSharpPin (pin: SwedishPersonalIdentityNumber) =
        let year = pin.Year |> Year.value
        let month = pin.Month |> Month.value
        let day = pin.Day |> Day.value
        let birthNumber = pin.BirthNumber |> BirthNumber.value
        let checksum = pin.Checksum |> Checksum.value
        SwedishPersonalIdentityNumberCSharp(year, month, day, birthNumber, checksum)

    /// All the testdata from Skatteverket presented as an array of 12 digit strings.
    static member Raw12DigitStrings = raw12DigitStrings
    /// A random test number
    static member GetRandom() = getRandom() |> toCSharpPin
    /// <summary>
    /// Returns a sequence of length specified by count, of unique random test numbers. If it is not important that the 
    /// sequence of numbers is unique it is more efficient to call getRandom() repeatedly
    /// </summary>
    /// <param name="count">The number of numbers to return</param>
    static member GetRandom(count) = 
        getRandomWithCount(count) 
        |> Seq.map toCSharpPin 
        |> List.ofSeq 
    /// A seqence of all test numbers ordered by date descending
    static member AllPinsByDateDesc() = allPinsByDateDesc() |> Seq.map toCSharpPin
    /// A sequence of all test numbers in random order
    static member AllPinsShuffled() = allPinsShuffled() |> Seq.map toCSharpPin

    /// <summary>
    /// Checks if a SwedishPersonalIdentityNumber is a test number
    /// </summary>
    /// <param name="pin">A SwedishPersonalIdentityNumber</param>
    static member IsTestNumber (pin:SwedishPersonalIdentityNumberCSharp) = 
        (pin.Year, pin.Month, pin.Day, pin.BirthNumber, pin.Checksum)
        |> isTestNumberTuple

open System.Runtime.CompilerServices

[<Extension>]
/// Checks if a SwedishPersonalIdentityNumber is a test number
type SwedishPersonalIdentityNumberCSharpTestDataExtensions() =
    [<Extension>]
    static member IsTestNumber(pin : SwedishPersonalIdentityNumberCSharp) = 
        SwedishPersonalIdentityNumberTestDataCSharp.IsTestNumber pin